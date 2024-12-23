using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			public AnimalCrossingVerze acVerze; //animal crossing version
			public string vybiracTrate = ""; //person who picked this track
			public bool vybranoRandom = false;
			public List<Tuple<Zavodnik, int>> zavodnici = new List<Tuple<Zavodnik, int>>(); //pair of zavodnik and their position in this race
			public int pocetZavodniku = 0;
			public int pocetBotu = 0;
		}
		public static Trate trate = new Trate();
		public static void zpracovatZaznamy(List<Zaznam> zaznamy, out List<Zavod> zavody, out List<Zavodnik> zavodnici, out int pocetUnikatnichZavodnikuOut, out int maximalniPocetZavodnikuOut, out int minimalniPocetZavodnikuOut, out float medianPocetZavodnikuOut, out int ujetyPocetZavoduOut) {
			zavodnici = new List<Zavodnik>();
			zavody = new List<Zavod>();
			Random random = new Random();
			int cup = 0;
			int zavod = 0;

			foreach(Zaznam zaznam in zaznamy) {
				if(zaznam.poradi == 999) {
					//Console.WriteLine("skipping zaznam because its misc log: " + zaznam);
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
					zavodnik.isBot = zaznam.isBot;
					if(Program.nicknameColors.ContainsKey(zavodnik.nick)) {
						zavodnik.barva = Program.nicknameColors[zavodnik.nick];
					}
					else {
						Console.WriteLine($"Zavodnik {zavodnik.nick} nema predurcenou barvu");
						//take random color from the list and check if it's not already taken
						Color barva = colorList[random.Next(colorList.Count)];

						string ansiCode = "";
						string ansiCodeConflicting = "";
						//check if any other colour already in dictionary is too similar to this one using Utils.CompareRGB
						while(Program.nicknameColors.Any(z => Utils.CompareRGB(z.Value, barva) < 80)) {
							ansiCode = $"\x1b[38;2;{barva.R};{barva.G};{barva.B}m";
							Color conflictingBarva = Program.nicknameColors.First(z => Utils.CompareRGB(z.Value, barva) < 80).Value;
							ansiCodeConflicting = $"\x1b[38;2;{conflictingBarva.R};{conflictingBarva.G};{conflictingBarva.B}m";
							Console.WriteLine($"Barva {ansiCode} {barva.ToKnownColor()}\x1b[0m je moc podobna jako barva {ansiCodeConflicting}{conflictingBarva.ToKnownColor()} \x1b[0m"); // Reset color

							barva = colorList[random.Next(colorList.Count)];
						}

						zavodnik.barva = barva;

						ansiCode = $"\x1b[38;2;{barva.R};{barva.G};{barva.B}m";

						Console.WriteLine($"{ansiCode}Zavodnik {zavodnik.nick} nema predurcenou barvu tak byl vybran {barva.ToKnownColor()} \x1b[0m"); // Reset color

						Console.ResetColor();

						//add the color to the dictionary so it's not random next time
						//Program.nicknameColors.Add(zavodnik.nick, barva);
					}

					zavodnici.Add(zavodnik);
				}
				if(zaznam.vybiracTrate.Length > 1) {
					if(zaznam.zavod == 99) goto escape;
					string[] vybiraciTrate = zaznam.vybiracTrate.Split(',');
					/*foreach(string vybiracTrate in vybiraciTrate) {
						if(vybiracTrate.Trim().ToLower().Equals(zaznam.zavodnik.Trim().ToLower())) {
							
							zavodnik.PridatTrat(zaznam.trat);
							Console.WriteLine("vybirac trate je zavodnik, pridavam trat " + zaznam.trat + " zavodnikovi " + zaznam.zavodnik);
						}
					}*/
					for(int i = 0; i < vybiraciTrate.Length; i++) {
						if(vybiraciTrate[i].Trim().ToLower().Equals(zaznam.zavodnik.Trim().ToLower())) {
							if(i == 0) {
								zavodnik.PridatTrat(zaznam.trat);
								Console.WriteLine("vybirac trate je zavodnik, pridavam trat " + zaznam.trat + " zavodnikovi " + zaznam.zavodnik);
							}
							else {
								zavodnik.PridatDruhouTrat(zaznam.trat);
								Console.WriteLine("vybirac trate je zavodnik, pridavam druhou trat " + zaznam.trat + " zavodnikovi " + zaznam.zavodnik);
							}
						}
					}

				}
			escape:;
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
					zavod_.acVerze = zaznam.acVerze;
					zavod_.vybiracTrate = zaznam.vybiracTrate;
					zavod_.vybranoRandom = zaznam.vybranoRandom;
					if(zaznam.vybiracTrate.Length < 1) {
						//Console.WriteLine("no vybirac ? :(");
					}
					zavody.Add(zavod_);
				}
				zavod_.zavodnici.Add(new Tuple<Zavodnik, int>(zavodnik, zaznam.poradi));

				if(zaznam.isBot == true) {
					zavod_.pocetBotu++;
				}
				else {
					zavod_.pocetZavodniku++;
				}
			}

			//writeline all the zavods and their zavodnici pairs for debugging
			/*foreach(Zavod zavod_ in zavody) {
				Console.WriteLine();
				Console.WriteLine("Cup " + zavod_.cup + " Zavod " + zavod_.zavod);
				foreach(Tuple<Zavodnik, int> zavodnik in zavod_.zavodnici) {
					Console.WriteLine(zavodnik.Item1.nick + " " + zavodnik.Item2);
				}
			}*/
			ConsoleColor oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			foreach(Zavod zavod_ in zavody) {
				if(zavod_.zavod == 99) continue; //skip final results of each cup
				trate.PridatTrat(zavod_.trat);
			}
			trate.Seradit();
			Console.WriteLine("Trate:");
			Console.WriteLine(trate.VypsatTrate());
			Console.ForegroundColor = oldColor;
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
					if(zavod_.zavod == 99) continue; //skip final results of each cup
					foreach(Tuple<Zavodnik, int> zavodnik_ in zavod_.zavodnici) {
						if(zavodnik_.Item1 == zavodnik) {
							pocetDojetiNaIndexu[zavodnik_.Item2 - 1]++;
						}
					}
				}
				zavodnik.pocetDojetiNaIndexu = pocetDojetiNaIndexu;
			}
			foreach(Zavod zavod_ in zavody) {
				//take position of each zavodnik in this zavod and weight in percentage which is how many real racers were there compared to bots. lets assume there are always 12 racers, so bot count is 12 - pocetZavodniku
				foreach(Tuple<Zavodnik, int> zavodnik in zavod_.zavodnici) {
					int weight = 100 * zavod_.pocetZavodniku / 12;
					zavodnik.Item1.poziceSWeight.Add(new Tuple<int, int>(zavodnik.Item2, weight));
				}
			}

			foreach(Zavodnik zavodnik in zavodnici) {
				zavodnik.RecalcPos();
			}



			//process pocet unikatnich zavodniku and maximalni pocet zavodniku naraz
			pocetUnikatnichZavodnikuOut = 0;
			maximalniPocetZavodnikuOut = 0;
			minimalniPocetZavodnikuOut = 999;
			medianPocetZavodnikuOut = 0;
			foreach(Zavod zavod_ in zavody) {
				if(zavod_.zavod == 99) continue; //skip final results of each cup
				if(zavod_.pocetZavodniku > maximalniPocetZavodnikuOut) {
					maximalniPocetZavodnikuOut = zavod_.pocetZavodniku;
				}
				if(zavod_.pocetZavodniku < minimalniPocetZavodnikuOut) {
					minimalniPocetZavodnikuOut = zavod_.pocetZavodniku;
				}
			}
			foreach(Zavodnik zavodnik in zavodnici) {
				if(zavodnik.pocetUjetychZavodu > 0) {
					pocetUnikatnichZavodnikuOut++;
				}
			}
			//calculate median pocet zavodniku
			List<int> pocetZavodnikuList = new List<int>();
			foreach(Zavod zavod_ in zavody) {
				if(zavod_.zavod == 99) continue; //skip final results of each cup
				pocetZavodnikuList.Add(zavod_.pocetZavodniku);
			}
			pocetZavodnikuList.Sort();
			if(pocetZavodnikuList.Count % 2 == 0) {
				medianPocetZavodnikuOut = (pocetZavodnikuList[pocetZavodnikuList.Count / 2] + pocetZavodnikuList[pocetZavodnikuList.Count / 2 - 1]) / 2;
			}
			else {
				medianPocetZavodnikuOut = pocetZavodnikuList[pocetZavodnikuList.Count / 2];
			}


			ujetyPocetZavoduOut = 0;
			foreach(Zavod zavod_ in zavody) {
				if(zavod_.zavod < 99) {
					ujetyPocetZavoduOut++;
				}
			}
		}


		public static TextRenderingHint trh;
		public static Bitmap renderGraphPozice(List<Zavod> zavody, List<Zavodnik> zavodnici, string filter, bool small, bool legenda) {
			int VYSKA_JEDNE_JEDNOTKY = small ? _VYSKA_JEDNE_JEDNOTKY / 2 : _VYSKA_JEDNE_JEDNOTKY;
			int SIRKA_JEDNE_JEDNOTKY = small ? _SIRKA_JEDNE_JEDNOTKY / 2 : _SIRKA_JEDNE_JEDNOTKY;

			Random random = new Random();

			int cup = 1;
			int zavod = 1;


			int pocetZavodu = zavody.Count;

			int sirkaBitmapy = pocetZavodu * SIRKA_JEDNE_JEDNOTKY + OFFSET_ZLEVA;
			if(legenda) sirkaBitmapy += 100;
			Bitmap bitmap = new Bitmap(sirkaBitmapy, MAX_POCET_ZAVODNIKU * VYSKA_JEDNE_JEDNOTKY + 200);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.TextRenderingHint = trh;

			//vypsat jmena v jejich barve na prave strane
			if(legenda) {
				for(int i = 0, j = 0; i < zavodnici.Count; i++, j++) {
					if(zavodnici[i].isBot) {
						j--;
						continue;
					}
					graphics.DrawString(zavodnici[i].nick, new Font("Arial", 12), new SolidBrush(zavodnici[i].barva), new PointF(sirkaBitmapy - 100, j * 20 + OFFSET_ZHORA));
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
			int lastPixel = bitmap.Width - 132;
			if(lastPixel > 0)
				bitmap.SetPixel(lastPixel, OFFSET_ZHORA + MAX_POCET_ZAVODNIKU * VYSKA_JEDNE_JEDNOTKY + 1, BARVA_CAR);


			System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
			drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
			Brush brush = new SolidBrush(BARVA_TEXTU);
			Font font = new Font("Arial", 11, FontStyle.Regular);
			Font fontAlt = new Font("Arial", 11, FontStyle.Underline);
			for(int zavodIndex = 0; zavodIndex < zavody.Count; zavodIndex++) {
				int x = OFFSET_ZLEVA + zavodIndex * SIRKA_JEDNE_JEDNOTKY;
				Zavod zavod_ = zavody[zavodIndex];
				string trat = zavod_.zavod == 99 ? "Final" : zavod_.trat;
				var font_ = zavod_.zavod == 99 ? fontAlt : font;
				graphics.DrawString(trat, font_, brush, x - 10, OFFSET_ZHORA + MAX_POCET_ZAVODNIKU * VYSKA_JEDNE_JEDNOTKY + 5, drawFormat);
				for(int zavodnikIndex = 0; zavodnikIndex < zavod_.zavodnici.Count; zavodnikIndex++) {
					if(zavod_.zavodnici[zavodnikIndex].Item1.isBot) continue;

					Tuple<Zavodnik, int> zavodnik = zavod_.zavodnici[zavodnikIndex];
					if(filter.Trim().Length != 0 && zavodnik.Item1.nick != filter) continue;

					int y = OFFSET_ZHORA + zavodnik.Item2 * VYSKA_JEDNE_JEDNOTKY;
					//Color barva = zavod_.zavod == 99 ? Color.Gold : zavodnik.Item1.barva;
					Color barva = zavodnik.Item1.barva;
					graphics.DrawEllipse(new Pen(barva), x - 3, y - 3, 6, 6);
					if(zavod_.zavod == 99) {
						graphics.FillEllipse(new SolidBrush(barva), x - 4, y - 4, 8, 8);
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
		public static Bitmap renderDlouhodobyGraf(List<Zavodnik> zavodnici) {
			List<string> dates = new List<string>();
			foreach(Zavodnik zavodnik in zavodnici) {
				foreach(Tuple<string, double> median in zavodnik.medianyVPrubehuCasu) {
					if(!dates.Contains(median.Item1)) {
						dates.Add(median.Item1);
					}
				}
			}
			dates.Sort();
			int pocetDni = dates.Count;

			int vyskaBitmapy = 12 * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA + 80;
			int sirkaBitmapy = pocetDni * _SIRKA_JEDNE_JEDNOTKY + OFFSET_ZLEVA;

			Bitmap bitmap = new Bitmap(sirkaBitmapy, vyskaBitmapy);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.TextRenderingHint = trh;
			StringFormat strignFormat = new StringFormat();
			strignFormat.FormatFlags = StringFormatFlags.DirectionVertical;



			//draw the axis
			graphics.DrawLine(new Pen(BARVA_CAR), OFFSET_ZLEVA, OFFSET_ZHORA, OFFSET_ZLEVA, 12 * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA);
			graphics.DrawLine(new Pen(BARVA_CAR), OFFSET_ZLEVA, 12 * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA, pocetDni * _SIRKA_JEDNE_JEDNOTKY + OFFSET_ZLEVA, 12 * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA);

			//draw value at top of the axis
			graphics.DrawString("1", new Font("Arial", 8), new SolidBrush(BARVA_TEXTU), 0, 5 + OFFSET_ZHORA);

			//draw names diagonally on the horizontal axis

			for(int i = 0; i < pocetDni; i++) {
				graphics.DrawString(dates[i], new Font("Arial", 8), new SolidBrush(BARVA_TEXTU), i * _SIRKA_JEDNE_JEDNOTKY + OFFSET_ZLEVA - 6, 12 * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA, strignFormat);
			}
			int previousPoint = -99;
			int previousId = -99;
			List<Tuple<string, int>> alreadyRenderedValues = new List<Tuple<string, int>>();

			for(int i = 1; i <= 12; i++) {
				int y = i * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA;
				graphics.DrawLine(new Pen(BARVA_CAR), OFFSET_ZLEVA, y, OFFSET_ZLEVA + 1, y);
			}

			foreach(Zavodnik zavodnik in zavodnici) {
				if(!zavodnik.IsWorthy(dates)) continue;
				if(zavodnik.isBot) continue;
				for(int i = 0; i < zavodnik.medianyVPrubehuCasu.Count; i++) {
					int id = dates.IndexOf(zavodnik.medianyVPrubehuCasu[i].Item1);
					if(id == -1) continue;

					int currentValue = (int)zavodnik.medianyVPrubehuCasu[i].Item2;
					//check list if there is entry for this value at this date
					if(!alreadyRenderedValues.Contains(new Tuple<string, int>(dates[id], currentValue))) {
						graphics.FillEllipse(new SolidBrush(zavodnik.barva), id * _SIRKA_JEDNE_JEDNOTKY - 4 + OFFSET_ZLEVA, currentValue * _VYSKA_JEDNE_JEDNOTKY - 4 + OFFSET_ZHORA, 9, 9);
					}




					graphics.DrawEllipse(new Pen(zavodnik.barva, 2), id * _SIRKA_JEDNE_JEDNOTKY - 3 + OFFSET_ZLEVA, currentValue * _VYSKA_JEDNE_JEDNOTKY - 3 + OFFSET_ZHORA, 7, 7);

					//check if this racer had the same value at previous date, and at the same time, some other racer has the same value today and also at the previous date. if yes, draw only dotted line instead of full line (so it dithers between the two racer's colours)


					if(previousId + 1 == id) {
						bool notDrawn = true;

						//foreach zavodnik, check if some zavodnik today has the same value as this, and at the same time, has same value yesterday as this zavodnik, and at the same time, this zavodnik has the same value today as yesterday
						if(alreadyRenderedValues.Contains(new Tuple<string, int>(dates[id], currentValue))) {
							foreach(Zavodnik zavodnik_ in zavodnici) {
								if(zavodnik_ == zavodnik) continue;
								for(int j = 0; j < zavodnik_.medianyVPrubehuCasu.Count; j++) {
									if(dates.IndexOf(zavodnik_.medianyVPrubehuCasu[j].Item1) == id && zavodnik_.medianyVPrubehuCasu[j].Item2 == currentValue) {
										for(int k = 0; k < zavodnik_.medianyVPrubehuCasu.Count; k++) {
											if(dates.IndexOf(zavodnik_.medianyVPrubehuCasu[k].Item1) == id - 1 && zavodnik_.medianyVPrubehuCasu[k].Item2 == previousPoint) {
												for(int l = 0; l < zavodnik.medianyVPrubehuCasu.Count; l++) {
													if(dates.IndexOf(zavodnik.medianyVPrubehuCasu[l].Item1) == id - 1 && zavodnik.medianyVPrubehuCasu[l].Item2 == previousPoint) {
														var oldSmoothMode = graphics.SmoothingMode;
														graphics.SmoothingMode = SmoothingMode.None;

														graphics.DrawLine(new Pen(zavodnik.barva, 1) { DashStyle = DashStyle.Dash }, (id - 1) * _SIRKA_JEDNE_JEDNOTKY + OFFSET_ZLEVA, previousPoint * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA, id * _SIRKA_JEDNE_JEDNOTKY + OFFSET_ZLEVA, currentValue * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA);
														graphics.SmoothingMode = oldSmoothMode;
														Console.WriteLine("dotted line drawn");
														notDrawn = false;
													}
												}
											}
										}
									}
								}
							}
						}
						if(notDrawn) {
							graphics.DrawLine(new Pen(zavodnik.barva),
								(id - 1) * _SIRKA_JEDNE_JEDNOTKY + OFFSET_ZLEVA, previousPoint * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA,
								id * _SIRKA_JEDNE_JEDNOTKY + OFFSET_ZLEVA, currentValue * _VYSKA_JEDNE_JEDNOTKY + OFFSET_ZHORA
							);
						}
					}
					previousPoint = currentValue;
					previousId = id;

					//add this value with date to the list of already rendered values
					alreadyRenderedValues.Add(new Tuple<string, int>(dates[id], currentValue));
				}


				previousPoint = -99;
				previousId = -99;




			CONTINUEFOREACH:;
			}

			return bitmap;
		}

		public static void renderGraphDistribuce(List<Zavodnik> zavodnici, bool dark, string directory) {
			Random random = new Random();

			foreach(Zavodnik zavodnik in zavodnici) {
				if(zavodnik.isBot) continue;
				Console.WriteLine("rendering graph distribuce for " + zavodnik.nick + " (dark: " + dark);
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
					int x1 = 10 + i * 20;
					int y1 = 190 - (int)(ints[i] * 180.0 / max);
					int x2 = 10 + (i + 1) * 20;
					int y2 = 190 - (int)(ints[i + 1] * 180.0 / max);
					try {
						//graphics.DrawLine(new Pen(zavodnik.barva), 10 + i * 20, 190 - (int)(ints[i] * 180.0 / max), 10 + (i + 1) * 20, 190 - (int)(ints[i + 1] * 180.0 / max));
						graphics.DrawLine(new Pen(zavodnik.barva), x1, y1, x2, y2);
					}
					catch(Exception ex) {
						ConsoleColor oldColor = Console.ForegroundColor;
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(String.Concat(Enumerable.Repeat("-", 6)));
						Console.WriteLine($"error drawing line on line {ex.GetLine()}: {ex.Message} i: {i} max: {max} x1: {x1} y1: {y1} x2: {x2} y2: {y2}");
						Console.WriteLine(String.Concat(Enumerable.Repeat("-", 6)));
					}
					int x = 10 + i * 20 - 2;
					int y = 190 - (int)(ints[i] * 180.0 / max) - 2;
					try {
						graphics.DrawEllipse(new Pen(zavodnik.barva), 10 + i * 20 - 2, 190 - (int)(ints[i] * 180.0 / max) - 2, 4, 4);
					}
					catch(Exception ex) {
						ConsoleColor oldColor = Console.ForegroundColor;
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(String.Concat(Enumerable.Repeat("-", 6)));
						Console.WriteLine($"error drawing ellipse on line {ex.GetLine()}: {ex.Message} i: {i} max: {max} x: {x} y: {y}");
						Console.WriteLine(String.Concat(Enumerable.Repeat("-", 6)));
					}
				}
				graphics.DrawEllipse(new Pen(zavodnik.barva), 10 + 11 * 20 - 2, 190 - (int)(ints[11] * 180.0 / max) - 2, 4, 4);

				//save the bitmap to "./out/grafy/zavodnik/distribuce.png"
				//bitmapa.Save("./out/grafy/" + zavodnik.nick + (dark ? "/distribuce_dark.png" : "/distribuce.png"));
				string filepath = directory + zavodnik.nick + (dark ? "/distribuce_dark.png" : "/distribuce.png");
				Console.WriteLine("saving graph distribuce to " + filepath);
				bitmapa.SaveAsync(directory + zavodnik.nick + (dark ? "/distribuce_dark.png" : "/distribuce.png"));
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
				bm.SaveAsync(dirname + z.nick + "/prubeh.png");
				bm = renderGraphPozice(zavody, zavodnici, z.nick, false, false);
				bm.SaveAsync(dirname + z.nick + "/prubeh_fullsize.png");
				BARVA_TEXTU = Color.White;

				trh = TextRenderingHint.ClearTypeGridFit;
				bm = renderGraphPozice(zavody, zavodnici, z.nick, true, false);
				bm.SaveAsync(dirname + z.nick + "/prubeh_dark.png");

				bm = renderGraphPozice(zavody, zavodnici, z.nick, false, false);
				bm.SaveAsync(dirname + z.nick + "/prubeh_dark_fullsize.png");
			}

			renderGraphDistribuce(zavodnici, true, dirname);
			BARVA_CAR = Color.DarkGray;
			BARVA_TEXTU = Color.Black;
			trh = TextRenderingHint.AntiAlias;
			renderGraphDistribuce(zavodnici, false, dirname);
		}

		public static void vyrenderovatUcast(List<int> pocetUnikatnichZavodniku, List<int> maximalniPocetZavodniku, List<int> minimalniPocetZavodniku, List<float> medianPocetZavodniku, List<string> datumPoctuZavodniku, bool pouzePrvni, string outfile) {
			if(pocetUnikatnichZavodniku.Count != maximalniPocetZavodniku.Count) {
				Console.WriteLine("PROSÍM POZOR. pocetUnikatnichZavodniku a maximalniPocetZavodniku nesedí");
			}

			Color barvaUnikat = Color.Blue;
			Color barvaMax = Color.Red;
			Color barvaMedian = Color.Green;
			Color barvaMin = Color.Gold;

			int delka = Math.Max(pocetUnikatnichZavodniku.Count, maximalniPocetZavodniku.Count);

			int padding = 10;
			int extraMistoProText = 40;
			//int maxPocetPolicekVertikalne = 16; //budu doufat že v 1 den nebude vic jak 20 unikátních závodníků
			int maxPocetPolicekVertikalne = pocetUnikatnichZavodniku.Max();
			int sirkaJedneJednotky = 20;
			int vyskaJedneJednotky = 8;
			int vertikalniCarkuCo = 1;
			if(pouzePrvni) {
				vyskaJedneJednotky = 3;
				vertikalniCarkuCo = 4;
			}
			int sirkaBitmapy = (delka - 1) * sirkaJedneJednotky + padding * 2;
			int vyskaBitmapy = maxPocetPolicekVertikalne * vyskaJedneJednotky + padding * 2;


			Bitmap bitmap = new Bitmap(sirkaBitmapy, vyskaBitmapy + extraMistoProText);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.TextRenderingHint = trh;

			Pen pen = new Pen(BARVA_CAR);
			Brush brush = new SolidBrush(BARVA_TEXTU);

			//draw the horizontal and vertical axis, with dot every sirkaJedneJednotky and vyskaJedneJednotky pixels
			for(int i = 0; i < delka; i++) {
				int x = padding + i * sirkaJedneJednotky;
				int y = vyskaBitmapy - padding;
				graphics.DrawLine(pen, x, y - 2, x, y + 2);
			}
			for(int i = 0; i <= maxPocetPolicekVertikalne; i += vertikalniCarkuCo) {
				int x = padding;
				int y = vyskaBitmapy - padding - i * vyskaJedneJednotky;
				graphics.DrawLine(pen, x - 2, y, x + 2, y);
			}
			int x_ = padding;
			int y_ = vyskaBitmapy - padding - maxPocetPolicekVertikalne * vyskaJedneJednotky;
			graphics.DrawLine(pen, x_ -2, y_, x_ + 2, y_);


			graphics.DrawLine(pen, padding, padding, padding, vyskaBitmapy - padding);
			graphics.DrawLine(pen, padding, vyskaBitmapy - padding, sirkaBitmapy - padding, vyskaBitmapy - padding);

			for(int i = 1; i < delka; i++) {
				int pocetUnikatnichZavodniku_ = pocetUnikatnichZavodniku[i];
				int predchoziPocetUnikatnichZavodniku = pocetUnikatnichZavodniku[i - 1];
				graphics.DrawLine(new Pen(barvaUnikat), padding + (i - 1) * sirkaJedneJednotky, vyskaBitmapy - padding - predchoziPocetUnikatnichZavodniku * vyskaJedneJednotky, padding + i * sirkaJedneJednotky, vyskaBitmapy - padding - pocetUnikatnichZavodniku_ * vyskaJedneJednotky);

				if(!pouzePrvni) {
					int maximalniPocetZavodniku_ = maximalniPocetZavodniku[i];
					int minimalniPocetZavodniku_ = minimalniPocetZavodniku[i];
					float medianPocetZavodniku_ = medianPocetZavodniku[i];

					int predchoziMaximalniPocetZavodniku = maximalniPocetZavodniku[i - 1];
					int predchoziMinimalniPocetZavodniku = minimalniPocetZavodniku[i - 1];
					float predchoziMedianPocetZavodniku = medianPocetZavodniku[i - 1];

				
					graphics.DrawLine(new Pen(barvaMax), padding + (i - 1) * sirkaJedneJednotky, vyskaBitmapy - padding - predchoziMaximalniPocetZavodniku * vyskaJedneJednotky, padding + i * sirkaJedneJednotky, vyskaBitmapy - padding - maximalniPocetZavodniku_ * vyskaJedneJednotky);
					graphics.DrawLine(new Pen(barvaMin), padding + (i - 1) * sirkaJedneJednotky, vyskaBitmapy - padding - predchoziMinimalniPocetZavodniku * vyskaJedneJednotky, padding + i * sirkaJedneJednotky, vyskaBitmapy - padding - minimalniPocetZavodniku_ * vyskaJedneJednotky);

					//check if median and previous median are same as maximalni and previous maximalni, if yes, draw the line dashed
					if(predchoziMedianPocetZavodniku == predchoziMaximalniPocetZavodniku && medianPocetZavodniku_ == maximalniPocetZavodniku_) {
						graphics.DrawLine(new Pen(barvaMedian) { DashStyle = DashStyle.Dot }, padding + (i - 1) * sirkaJedneJednotky, vyskaBitmapy - padding - (int)(predchoziMedianPocetZavodniku * vyskaJedneJednotky), padding + i * sirkaJedneJednotky, vyskaBitmapy - padding - (int)(medianPocetZavodniku_ * vyskaJedneJednotky));
					}
					else {
						graphics.DrawLine(new Pen(barvaMedian), padding + (i - 1) * sirkaJedneJednotky, vyskaBitmapy - padding - (int)(predchoziMedianPocetZavodniku * vyskaJedneJednotky), padding + i * sirkaJedneJednotky, vyskaBitmapy - padding - (int)(medianPocetZavodniku_ * vyskaJedneJednotky));
					}
				}
			}

			for(int i = 0; i < delka; i++) {
				graphics.DrawString(datumPoctuZavodniku[i].Substring(5), new Font("Consolas", 8), brush, padding + i * sirkaJedneJednotky - 5, vyskaBitmapy - padding + 5, new StringFormat(StringFormatFlags.DirectionVertical));
			}
			graphics.DrawString(maxPocetPolicekVertikalne.ToString(), new Font("Consolas", 7), brush, -2, padding / 2, new StringFormat());
			graphics.DrawString("0", new Font("Consolas", 7), brush, 0, maxPocetPolicekVertikalne * vyskaJedneJednotky + padding / 2, new StringFormat());
			graphics.DrawString("12", new Font("Consolas", 7), brush, 0, (maxPocetPolicekVertikalne - 12) * vyskaJedneJednotky + padding / 2, new StringFormat());

			string path = "";
			string path_dark = "";
			if(outfile == "") {
				path = "./out/ucast.png";
				path_dark = "./out/ucast_dark.png";
			}
			else {
				path = outfile;
				path_dark = outfile.Replace(".png", "_dark.png");
			}

			bitmap.Save(path);

			brush = new SolidBrush(Color.White);

			for(int i = 0; i < delka; i++) {
				graphics.DrawString(datumPoctuZavodniku[i].Substring(5), new Font("Consolas", 8), brush, padding + i * sirkaJedneJednotky - 5, vyskaBitmapy - padding + 5, new StringFormat(StringFormatFlags.DirectionVertical));
			}
			graphics.DrawString(maxPocetPolicekVertikalne.ToString(), new Font("Consolas", 7), brush, -2, padding / 2, new StringFormat());
			graphics.DrawString("0", new Font("Consolas", 7), brush, 0, maxPocetPolicekVertikalne * vyskaJedneJednotky + padding / 2, new StringFormat());
			graphics.DrawString("12", new Font("Consolas", 7), brush, 0, (maxPocetPolicekVertikalne - 12) * vyskaJedneJednotky + padding / 2, new StringFormat());
			bitmap.Save(path_dark);
		}

		public static void vyrenderovatGrafPoctuUjetychZavodu(List<int> ujetyPocetZavodu, List<string> datumPoctuZavodniku) {
			vyrenderovatUcast(ujetyPocetZavodu, new List<int>(), new List<int>(), new List<float>(), datumPoctuZavodniku, true, "./out/pocetujetychzavodu.png");
		}
	}

	public static class HexColorExtensions {
		public static string ToHex(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
	}
	public static class BitmapExtensions {
		public static List<string> allSavedBitmaps = new List<string>();
		public static void ToJpegXL(bool overwrite) {
			int numberOfBitmaps = allSavedBitmaps.Count;
			for(int i = 0; i < numberOfBitmaps; i++) {
				string path = allSavedBitmaps[i];
				string outPath = path.Replace(".png", ".jxl");
				if(File.Exists(outPath) && !overwrite) {
					Console.WriteLine("skipping " + i + ". " + Path.GetFileName(path));
				}
				else {
					Console.WriteLine("converting " + i + ". " + path);
					string command = $"cjxl \"{path}\" \"{outPath}\" --keep_invisible 0 --patches=1 -q 100 -e 9  --allow_expert_options --brotli_effort=11";
					Process process = new Process();
					process.StartInfo.FileName = "cmd.exe";
					process.StartInfo.Arguments = $"/c {command}";
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.RedirectStandardError = true;
					process.Start();
					string output = process.StandardOutput.ReadToEnd();
					string error = process.StandardError.ReadToEnd();
					process.WaitForExit();
					long pngSize = new FileInfo(path).Length;
					long jxlSize = new FileInfo(outPath).Length;
					Console.WriteLine("png size: " + pngSize + " jxl size: " + jxlSize + " saved: " + (pngSize - jxlSize) + " bytes");
				}

				Console.WriteLine("Processing " + i + ". out of " + numberOfBitmaps + "bitmaps. Press ESC to abort conversion to JXL");
				if(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) {
					Console.WriteLine("Aborting conversion to JXL");
					break;
				}
			}
		}
		public static long velikostPng() {
			long size = 0;
			foreach(string path in allSavedBitmaps) {
				FileInfo fi = new FileInfo(path);
				size += fi.Length;
			}
			return size;
		}
		public static long velikostJxl() {
			long size = 0;
			foreach(string path in allSavedBitmaps) {
				FileInfo fi = new FileInfo(path.Replace(".png", ".jxl"));
				size += fi.Length;
			}
			return size;

		}
		public static void SaveAsync(this Bitmap bitmap, string path) {
			Task.Run(() => bitmap.Save(path));
			allSavedBitmaps.Add(path);
		}
	}
	public static class ExceptionExtensions {
		public static int GetLine(this Exception ex) {
			var st = new StackTrace(ex, true);
			var frame = st.GetFrame(0);
			return frame.GetFileLineNumber();
		}
	}
}
