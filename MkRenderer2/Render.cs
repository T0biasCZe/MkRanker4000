using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MkRenderer2.Render;

namespace MkRenderer2 {
	public static class Render {
		private const int OFFSET_ZLEVA = 10;
		private const int OFFSET_ZHORA = 10;
		private const int _VYSKA_JEDNE_JEDNOTKY = 20;
		private const int _SIRKA_JEDNE_JEDNOTKY = 40; //races will be 16 units wide horizontally
		private const int MAX_POCET_ZAVODNIKU = 12;
		public static Color BARVA_TEXTU = Color.White;
		public static Color BARVA_CAR = Color.DarkGray;
		private static List<Color> colorList;
		public static void init() {
			Type colorType = typeof(System.Drawing.Color);
			colorList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
				.Select(propInfo => Color.FromName(propInfo.Name))
				.ToList();
			colorList.Remove(Color.Transparent);
		}
		public class Zavod {
			public int cup; //cup number
			public int zavod; //number of race in this cup
			public string trat; //name of current track
			public string vybiracTrate = ""; //person who picked this track
			public List<Tuple<Zavodnik, int>> zavodnici = new List<Tuple<Zavodnik, int>>(); //pair of zavodnik and their position in this race
		}
		private static Dictionary<string, Color> nicknameColors = new Dictionary<string, Color> {
			{ "Stoupa", Color.Purple },
			{ "John Beak", Color.Cyan },
			{ "Tobik", Color.Lime},
			{ "lilibox", Color.Sienna },
			{ "Benish", Color.Yellow },
			{ "Mates", Color.FromArgb(255, 223, 197, 254) },
			{ "Tom", Color.OrangeRed },
			{ "Cvrcek", Color.DarkGreen },
			{ "Boun", Color.DarkGray },
			{ "Swatty", Color.Salmon },
		};
		public static void zpracovatZaznamy(List<Zaznam> zaznamy, out List<Zavod> zavody, out List<Zavodnik> zavodnici) {
			zavodnici = new List<Zavodnik>();
			zavody = new List<Zavod>();
			Random random = new Random();
			int cup = 0;
			int zavod = 0;

			foreach(Zaznam zaznam in zaznamy) {
				if(zaznam.poradi == 999) {
					Console.WriteLine("skipping zaznam because its misc log: " + zaznam);
					continue;
				}
				/*if(zaznam.zavod == 99) {
					Console.WriteLine("skipping zaznam because its final: " + zaznam);
					continue;
				}*/


				//parse all zaznams into zavodnici and zavody
				Zavodnik zavodnik = zavodnici.Find(z => z.nick == zaznam.zavodnik);
				if(zavodnik == null) {
					zavodnik = new Zavodnik();
					zavodnik.nick = zaznam.zavodnik;
					if(nicknameColors.ContainsKey(zavodnik.nick)) {
						zavodnik.barva = nicknameColors[zavodnik.nick];
					}
					else {
						//take random color from the list and check if it's not already taken
						Color barva = colorList[random.Next(colorList.Count)];
						while(zavodnici.Any(z => z.barva == barva)) {
							barva = colorList[random.Next(colorList.Count)];
						}
						zavodnik.barva = barva;

						string ansiCode = $"\x1b[38;2;{barva.R};{barva.G};{barva.B}m";

						Console.WriteLine($"{ansiCode}Zavodnik {zavodnik.nick} nema predurcenou barvu tak byl vybran {barva.ToKnownColor()} \x1b[0m"); // Reset color

						Console.ResetColor();
					}
					zavodnici.Add(zavodnik);
				}

				if(cup != zaznam.cup) {
					cup = zaznam.cup;
					zavod = 1;
				}
				if(zavod != zaznam.zavod) {
					zavod = zaznam.zavod;
				}

				Zavod zavod_ = zavody.Find(z => z.cup == cup && z.zavod == zavod);
				if(zavod_ == null) {
					zavod_ = new Zavod();
					zavod_.cup = cup;
					zavod_.zavod = zavod;
					zavod_.trat = zaznam.trat;
					zavod_.vybiracTrate = zaznam.vybiracTrate;
					if(zaznam.vybiracTrate.Length < 1) {
						Console.WriteLine("no vybirac ? :(");
					}
					zavody.Add(zavod_);
				}
				zavod_.zavodnici.Add(new Tuple<Zavodnik, int>(zavodnik, zaznam.poradi));
			}

			//writeline all the zavods and their zavodnici pairs for debugging
			foreach(Zavod zavod_ in zavody) {
				Console.WriteLine();
				Console.WriteLine("Cup " + zavod_.cup + " Zavod " + zavod_.zavod);
				foreach(Tuple<Zavodnik, int> zavodnik in zavod_.zavodnici) {
					Console.WriteLine(zavodnik.Item1.nick + " " + zavodnik.Item2);
				}
			}

			//calculate how many times each zavodnik finished race, but dont count zavod == 99
			foreach(Zavodnik zavodnik in zavodnici) {
				int pocetDojeti = 0;
				foreach(Zavod zavod_ in zavody) {
					if(zavod_.zavod != 99) {
						foreach(Tuple<Zavodnik, int> zavodnik_ in zavod_.zavodnici) {
							if(zavodnik_.Item1 == zavodnik) {
								pocetDojeti++;
								break;
							}
						}
					}
				}
				zavodnik.pocetUjetychZavodu = pocetDojeti;
			}

			//calculate how many times each zavodnik was at each position, and put it into array "pocetDojetiNaIndexu", where index is position and value is count
			foreach(Zavodnik zavodnik in zavodnici) {
				int[] pocetDojetiNaIndexu = new int[MAX_POCET_ZAVODNIKU];
				foreach(Zavod zavod_ in zavody) {
					foreach(Tuple<Zavodnik, int> zavodnik_ in zavod_.zavodnici) {
						if(zavodnik_.Item1 == zavodnik) {
							pocetDojetiNaIndexu[zavodnik_.Item2 - 1]++;
						}
					}
				}
				zavodnik.pocetDojetiNaIndexu = pocetDojetiNaIndexu;
			}

			foreach(Zavodnik zavodnik in zavodnici) {
				zavodnik.RecalcPos();
			}
		}


		public static TextRenderingHint trh;
		public static Bitmap renderGraphPozice(List<Zavod> zavody, List<Zavodnik> zavodnici, string filter, bool small, bool legenda) {
			int VYSKA_JEDNE_JEDNOTKY = small ? _VYSKA_JEDNE_JEDNOTKY/2 : _VYSKA_JEDNE_JEDNOTKY;
			int SIRKA_JEDNE_JEDNOTKY = small ? _SIRKA_JEDNE_JEDNOTKY/2 : _SIRKA_JEDNE_JEDNOTKY;

			Random random = new Random();

			int cup = 1;
			int zavod = 1;


			int pocetZavodu = zavody.Count;

			int sirkaBitmapy = pocetZavodu * SIRKA_JEDNE_JEDNOTKY + OFFSET_ZLEVA;
			if(legenda) sirkaBitmapy += 100;
			Bitmap bitmap = new Bitmap(sirkaBitmapy, MAX_POCET_ZAVODNIKU * VYSKA_JEDNE_JEDNOTKY +200);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.TextRenderingHint = trh;

			//vypsat jmena v jejich barve na prave strane
			if(legenda) {
				for(int i = 0; i < zavodnici.Count; i++) {
					graphics.DrawString(zavodnici[i].nick, new Font("Arial", 12), new SolidBrush(zavodnici[i].barva), new PointF(sirkaBitmapy - 100, i * 20 + OFFSET_ZHORA));
				}
			}

			for(int i = 0; i < MAX_POCET_ZAVODNIKU; i++) {
				for(int j = 0; j < VYSKA_JEDNE_JEDNOTKY; j++) {
					bitmap.SetPixel(OFFSET_ZLEVA, j + i * VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA, BARVA_CAR);
				}
				bitmap.SetPixel(OFFSET_ZLEVA + 1, i * VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA, BARVA_CAR);
			}
			for(int x = OFFSET_ZLEVA; x < bitmap.Width - 132; x += SIRKA_JEDNE_JEDNOTKY) {
				for(int x_ = 0; x_ < SIRKA_JEDNE_JEDNOTKY; x_++) {
					bitmap.SetPixel(x + x_, OFFSET_ZHORA + MAX_POCET_ZAVODNIKU * VYSKA_JEDNE_JEDNOTKY, BARVA_CAR);
				}
				bitmap.SetPixel(x, OFFSET_ZHORA + MAX_POCET_ZAVODNIKU * VYSKA_JEDNE_JEDNOTKY + 1, BARVA_CAR);
			}
			bitmap.SetPixel(bitmap.Width - 132, OFFSET_ZHORA + MAX_POCET_ZAVODNIKU * VYSKA_JEDNE_JEDNOTKY + 1, BARVA_CAR);


			System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
			drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
			Brush brush = new SolidBrush(BARVA_TEXTU);
			Font font = new Font("Arial", 11, FontStyle.Regular);
			for(int zavodIndex = 0; zavodIndex < zavody.Count; zavodIndex++) {
				int x = OFFSET_ZLEVA + zavodIndex * SIRKA_JEDNE_JEDNOTKY;
				Zavod zavod_ = zavody[zavodIndex];
				string trat = zavod_.zavod == 99 ? "Final" : zavod_.trat;
				graphics.DrawString(trat, font, brush, x - 10, OFFSET_ZHORA + MAX_POCET_ZAVODNIKU * VYSKA_JEDNE_JEDNOTKY + 5, drawFormat);
				for(int zavodnikIndex = 0; zavodnikIndex < zavod_.zavodnici.Count; zavodnikIndex++) {

					Tuple<Zavodnik, int> zavodnik = zavod_.zavodnici[zavodnikIndex];
					if(filter.Trim().Length != 0 && zavodnik.Item1.nick != filter) continue;

					int y = OFFSET_ZHORA + zavodnik.Item2 * VYSKA_JEDNE_JEDNOTKY;
					//Color barva = zavod_.zavod == 99 ? Color.Gold : zavodnik.Item1.barva;
					Color barva = zavodnik.Item1.barva;
					graphics.DrawEllipse(new Pen(barva), x - 3, y - 3 , 6, 6);
					if(zavod_.zavod == 99) {
						graphics.FillEllipse(new SolidBrush(barva), x - 4, y - 4 , 8, 8);
					}

					//draw line to the position of this zavodnik in the next zavod if he is there
					if(zavodIndex + 1 < zavody.Count) {
						Zavod nextZavod = zavody[zavodIndex + 1];
						Tuple<Zavodnik, int> nextZavodnik = nextZavod.zavodnici.Find(z => z.Item1 == zavodnik.Item1);
						if(nextZavodnik != null) {
							int y_ = OFFSET_ZHORA + nextZavodnik.Item2 * VYSKA_JEDNE_JEDNOTKY;
							graphics.DrawLine(new Pen(zavodnik.Item1.barva), x, y, x + SIRKA_JEDNE_JEDNOTKY, y_);
						}
					}
				}
			}

			return bitmap;
		}

		public static void renderGraphDistribuce(List<Zavodnik> zavodnici, bool dark, string directory) {
			Random random = new Random();

			foreach(Zavodnik zavodnik in zavodnici) {
				Bitmap bitmapa = new Bitmap(300, 200);
				Graphics graphics = Graphics.FromImage(bitmapa);
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.TextRenderingHint = trh;

				int[] ints = zavodnik.pocetDojetiNaIndexu;
				int max = ints.Max();
				//draw the axis
				graphics.DrawLine(new Pen(BARVA_CAR), 10, 10, 10, 190);
				graphics.DrawLine(new Pen(BARVA_CAR), 10, 190, 290, 190);

				//draw value at top of the axis
				graphics.DrawString(max.ToString(), new Font("Arial", 8), new SolidBrush(BARVA_TEXTU), 4, 5);

				//draw values on horizontal axis
				for(int i = 0; i < ints.Length; i++) {
					graphics.DrawString((i + 1).ToString(), new Font("Arial", 8), new SolidBrush(BARVA_TEXTU), 10 + i * 20, 188);
				}

				//draw the graph using graphics.drawline
				for(int i = 0; i < ints.Length - 1; i++) {
					graphics.DrawLine(new Pen(zavodnik.barva), 10 + i * 20, 190 - (int)(ints[i] * 180.0 / max), 10 + (i + 1) * 20, 190 - (int)(ints[i + 1] * 180.0 / max));
					graphics.DrawEllipse(new Pen(zavodnik.barva), 10 + i * 20 - 2, 190 - (int)(ints[i] * 180.0 / max) - 2, 4, 4);
				}
				graphics.DrawEllipse(new Pen(zavodnik.barva), 10 + 11 * 20 - 2, 190 - (int)(ints[11] * 180.0 / max) - 2, 4, 4);

				//save the bitmap to "./out/grafy/zavodnik/distribuce.png"
				//bitmapa.Save("./out/grafy/" + zavodnik.nick + (dark ? "/distribuce_dark.png" : "/distribuce.png"));
				string filepath = directory + zavodnik.nick + (dark ? "/distribuce_dark.png" : "/distribuce.png");
				Console.WriteLine("saving graph distribuce to " + filepath);
				bitmapa.Save(directory + zavodnik.nick + (dark ? "/distribuce_dark.png" : "/distribuce.png"));
			}
		}
		public static void vyrenderovatKarticky(List<Zavod> zavody, List<Zavodnik> zavodnici, string dirname) {
			foreach(Zavodnik z in zavodnici) {
				/*string path = "./out/grafy/" + z.nick;
				if(!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}*/
				Directory.CreateDirectory(dirname + z.nick);
				BARVA_TEXTU = Color.Black;
				trh = TextRenderingHint.AntiAlias;
				Bitmap bm = renderGraphPozice(zavody, zavodnici, z.nick, true, false);
				bm.Save(dirname + z.nick + "/prubeh.png");
				BARVA_TEXTU = Color.White;

				trh = TextRenderingHint.ClearTypeGridFit;
				bm = renderGraphPozice(zavody, zavodnici, z.nick, true, false);
				bm.Save(dirname + z.nick + "/prubeh_dark.png");
			}

			renderGraphDistribuce(zavodnici, true, dirname);
			BARVA_CAR = Color.DarkGray;
			BARVA_TEXTU = Color.Black;
			trh = TextRenderingHint.AntiAlias;
			renderGraphDistribuce(zavodnici, false, dirname);
		}
	}

	public static class HexColorExtensions {
		public static string ToHex(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
	}
}
