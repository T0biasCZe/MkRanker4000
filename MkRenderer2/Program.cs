
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using static MkRenderer2.Render;
using static Zaznam;

namespace MkRenderer2 {
	public class Program {
		public static Dictionary<string, Color> nicknameColors = new Dictionary<string, Color> {
			{ "Stoupa", Color.Purple },
			{ "John Beak", Color.Cyan },
			{ "Tobik", Color.Lime},
			{ "lilibox", Color.Sienna },
			{ "BeNiSh", Color.Yellow },
			{ "Mates", Color.FromArgb(255, 223, 197, 254) },
			{ "Tom", Color.OrangeRed },
			{ "Cvrcek", Color.DarkGreen },
			{ "Boun", Color.DarkGray },
			{ "Swatty", Color.Salmon },
			{ "TosKo", Color.DeepSkyBlue },
		};
		static void Main(string[] args) {
			//check if file colourdefinition.csv exists, if yes , load it and overwrite nicknameColors
			if(!File.Exists("colourdefinition.csv")) {
				//create file with default values. Use the text names instead of HEX when possible
				StreamWriter sw = new StreamWriter("colourdefinition.csv");
				foreach(var pair in nicknameColors) {
					if(pair.Value.IsNamedColor) {
						sw.WriteLine(pair.Key + ";" + pair.Value.Name);
					}
					else {
						sw.WriteLine(pair.Key + ";" + pair.Value.ToHex());
					}
				}
				sw.Flush();
				sw.Close();
			}
			else {
				string[] lines = File.ReadAllLines("colourdefinition.csv");
				foreach(string line in lines) {
					string[] parts = line.Split(";");
					if(parts.Length != 2) {
						Console.WriteLine("Invalid line in colourdefinition.csv: " + line);
						continue;
					}
					if(Color.FromName(parts[1]).IsKnownColor) {
						nicknameColors[parts[0]] = Color.FromName(parts[1]);
					}
					else {
						nicknameColors[parts[0]] = ColorTranslator.FromHtml(parts[1]);
					}
				}
			}

			List<Zavodnik> zavodnici_total = new List<Zavodnik>();
			Trate finalTrate = new Trate();


			//for each file in folder .\in
			var files = Directory.GetFiles(".\\in");

			Stopwatch stw = new Stopwatch();
			stw.Start();

			//statistic how many unique players in each file, and maximal number of players that were playing at the same time
			List<int> pocetUnikatnichZavodniku = new List<int>();
			List<int> maximalniPocetZavodniku = new List<int>();
			List<int> minimalniPocetZavodniku = new List<int>();
			List<float> medianPocetZavodniku = new List<float>();
			List<int> ujetyPocetZavodu = new List<int>();


			List<string> datumPoctuZavodniku = new List<string>();

			foreach(string file in files) {
				if(file.EndsWith(".csv") == false) continue;
				Console.WriteLine("Processing file " + file);
				string filename = Path.GetFileNameWithoutExtension(file);


				List<Zaznam> zaznamy = loadCsv(file);

				Render.init();
				Render.zpracovatZaznamy(zaznamy, out List<Zavod> zavody, out List<Zavodnik> zavodnici, out int pocetUnikatnichZavodnikuOut, out int maximalniPocetZavodnikuOut, out int minimalniPocetZavodnikuOut, out float medianPocetZavodnikuOut, out int ujetyPocetZavoduOut);

				pocetUnikatnichZavodniku.Add(pocetUnikatnichZavodnikuOut);
				maximalniPocetZavodniku.Add(maximalniPocetZavodnikuOut);
				minimalniPocetZavodniku.Add(minimalniPocetZavodnikuOut);
				medianPocetZavodniku.Add(medianPocetZavodnikuOut);
				ujetyPocetZavodu.Add(ujetyPocetZavoduOut);
				datumPoctuZavodniku.Add(filename);


				finalTrate.PridatTrate(Render.trate);
				Render.trate = new Trate();


				//Bitmap bm = Render.renderGraphPozice(zaznamy);

				Directory.CreateDirectory($@".\out\{filename}\grafy");


				Render.BARVA_TEXTU = Color.Black;
				Render.trh = TextRenderingHint.AntiAlias;
				Bitmap bm = Render.renderGraphPozice(zavody, zavodnici, "", false, false);
				bm.SaveAsync(@$".\out\{filename}\grafy\graf_all.png");
				Render.BARVA_TEXTU = Color.White;
				Render.trh = TextRenderingHint.ClearTypeGridFit;
				bm = Render.renderGraphPozice(zavody, zavodnici, "", false, false);
				bm.SaveAsync($@".\out\{filename}\grafy\graf_all_dark.png");

				//Console.WriteLine("bitmapa zapsana do cele cesty " + Path.GetFullPath("output.png"));

				Render.vyrenderovatKarticky(zavody, zavodnici, $"./out/{filename}/grafy/");

				File.WriteAllText($@".\out\{filename}\table.html", Hatlamatla.vytvoritTable(zavody));
				File.WriteAllText(@$".\out\{filename}\karticky.html", Hatlamatla.vytvoritKartickyZavodniku(zavodnici, false));
				File.WriteAllText(@$".\out\{filename}\colortable.html", Hatlamatla.colorTable());

				//Hatlamatla.zpracovatFinalIndex();

				File.WriteAllText(@$".\out\{filename}\index.php", Hatlamatla.zpracovatFinalIndex(filename, bm.Width, bm.Height));

				/*
				Bitmap bitmapa = new Bitmap(200, 100);
				Graphics g = Graphics.FromImage(bitmapa);
				//with cleartype, White text works but black text is pixelated since the Cleartype "background" is black
				g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				g.DrawString("Hello Subpixel World!", new Font("Arial", 12), new SolidBrush(Color.Black), new PointF(4, 4));
				g.DrawString("Hello Subpixel World", new Font("Arial", 12), new SolidBrush(Color.White), new PointF(4, 20));

				//with antialias, both are antialiased but they are blury since they dont use subpixel rendering
				g.TextRenderingHint = TextRenderingHint.AntiAlias;
				g.DrawString("Hello Blury World", new Font("Arial", 12), new SolidBrush(Color.Black), new PointF(4, 36));
				g.DrawString("Hello Blury World", new Font("Arial", 12), new SolidBrush(Color.White), new PointF(4, 52));

				bitmapa.Save("AA test.png");*/

				//merge current zavodnici to total zavodnici, and recalculate median and average
				foreach(Zavodnik zavodnik in zavodnici) {
					Zavodnik zavodnik_total = zavodnici_total.Find(z => z.nick == zavodnik.nick);
					if(zavodnik_total == null) {
						zavodnik.RecalcPos();

						zavodnik.medianyVPrubehuCasu.Add(new Tuple<string, double>(filename, zavodnik.vazeneMedianPoradi));

						zavodnici_total.Add(zavodnik);
						zavodnik.pocetUjetychDnu = 1;
					} else {
						for(int i = 0; i < zavodnik_total.pocetDojetiNaIndexu.Count(); i++) {
							zavodnik_total.pocetDojetiNaIndexu[i] += zavodnik.pocetDojetiNaIndexu[i];
						}
						zavodnik_total.pocetUjetychZavodu += zavodnik.pocetUjetychZavodu;

						zavodnik_total.pocetUjetychDnu += 1;

						foreach(Trat t in zavodnik.vlastniVybraneTrate) {
							for(int i = 1; i <= t.kolikratVybrano; i++) {
								zavodnik_total.PridatTrat(t.nazev);
							}
						}
						foreach(Trat t in zavodnik.druheVybraneTrate) {
							for(int i = 1; i <= t.kolikratVybrano; i++) {
								zavodnik_total.PridatDruhouTrat(t.nazev);
							}
						}

						zavodnik_total.RecalcPos();

						zavodnik_total.medianyVPrubehuCasu.Add(new Tuple<string, double>(filename, zavodnik.vazeneMedianPoradi));
					}
				}

				/*save colour with jmena to barvy.txt with format "#RRGGBB;Nick"*/
				StreamWriter sw = new StreamWriter($".\\out\\{filename}\\barvy.txt");
				for(int i = 0; i < zavodnici.Count; i++) {
					if(zavodnici[i].isBot) continue;
					sw.WriteLine(zavodnici[i].barva.ToHex() + ";" + zavodnici[i].nick);
				}
				sw.Flush();
				sw.Close();


			}
			/*save colour with jmena to barvy.txt with format "#RRGGBB;Nick"*/
			StreamWriter sww = new StreamWriter($".\\out\\barvy.txt");
			for(int i = 0; i < zavodnici_total.Count; i++) {
				if(zavodnici_total[i].isBot) continue;
				sww.WriteLine(zavodnici_total[i].barva.ToHex() + ";" + zavodnici_total[i].nick);
			}
			sww.Flush();
			sww.Close();

			Render.vyrenderovatUcast(pocetUnikatnichZavodniku, maximalniPocetZavodniku, minimalniPocetZavodniku, medianPocetZavodniku, datumPoctuZavodniku, false, "");
			Render.vyrenderovatGrafPoctuUjetychZavodu(ujetyPocetZavodu, datumPoctuZavodniku);

			Hatlamatla.zpracovatOverallKartickyZavodniku(zavodnici_total);

			Hatlamatla.zpracovatRankingTrati(zavodnici_total, finalTrate);

			long celkovyCas = stw.ElapsedMilliseconds;

			//check for yes/no in the console with timeout of 5s
			Console.WriteLine("Do you want to overwrite existing jxl files? (yes/no)");
			Console.Beep(duration: 200, frequency: 1000);
			string yesno = Reader.ReadLine(5000);
			bool overwrite = yesno == "yes" ? true : false;

			BitmapExtensions.ToJpegXL(overwrite);

			stw.Stop();
			Console.WriteLine("Celkovy cas: " + celkovyCas + "ms, čas s konverzí Jpeg XL: " + stw.ElapsedMilliseconds);

			Console.WriteLine("Velikost PNG: " + BitmapExtensions.velikostPng());
			Console.WriteLine("Velikost JXL: " + BitmapExtensions.velikostJxl());
		}
		static Dictionary<string, string> stareNazvyNaNově = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
			{"TheStoupa", "Stoupa"},
			{"Cwrcekk", "Cvrcek"},
			{"T0biasCZe", "Tobik"},
			{"Benish", "BeNiSh"},
			{"Fernettak", "Fernetak"},
			{"ClaudiXd", "Claudi"},
			{"petgridus", "PetGriDus"},
		};
		private static List<Zaznam> loadCsv(string file) {
			//string[] lines = File.ReadAllLines("zaznamy.csv");
			string[] lines = File.ReadAllLines(file);
			List<Zaznam> zaznamy = new List<Zaznam>();
			foreach(string line in lines) {
				string[] parts = line.Split(";");
				if(parts[0] == "HEADER") continue;
				Zaznam zaznam = new Zaznam();
				zaznam.zprava = parts[0];
				zaznam.casZaznamu = DateTime.Parse(parts[1]);
				string zavodnik = parts[2];
				//if string "zavodnik" is in dictionary replace it with the new one
				if(stareNazvyNaNově.ContainsKey(zavodnik)) {
					zaznam.zavodnik = stareNazvyNaNově[zavodnik];
				}
				else zaznam.zavodnik = zavodnik;

				zaznam.cc = int.Parse(parts[3]);
				zaznam.mirrorOn = bool.Parse(parts[4]);
				zaznam.cup = int.Parse(parts[5]);
				zaznam.zavod = int.Parse(parts[6]);
				zaznam.poradi = int.Parse(parts[7]);
				zaznam.trat = FixTrackName(parts[8]);
				if(parts.Length > 9) {
					string vybiracTrate = parts[9];
					//if string "zavodnik" is in dictionary replace it with the new one
					if(stareNazvyNaNově.ContainsKey(vybiracTrate)) {
						zaznam.vybiracTrate = stareNazvyNaNově[vybiracTrate];
					}
					else zaznam.vybiracTrate = vybiracTrate;
					//Console.WriteLine("trat vybrana hračem " + zaznam.vybiracTrate);

					if(parts.Length > 10) {
						zaznam.isBot = bool.Parse(parts[10]);
					}

				}
				if(zaznam.isBot == false) {
					zaznam.isBot = isBot(zaznam.zavodnik);
				}
				//for now its easier to just ignore bots (future me problem)
				if(zaznam.isBot) continue;
				zaznamy.Add(zaznam);
			}
			return zaznamy;
		}
		static List<string> botsList = new List<string> { "Mario", "Metal Mario", "Gold Mario", "Luigi", "Peach", "Daisy", "Rosalina", "Tanooki Mario", "Cat Peach", "Yoshi", "Toad", "Koopa Troopa", "Shy Guy", "Lakitu", "Toadette", "King Boo", "Baby Mario", "Baby Luigi", "Baby Peach", "Baby Daisy", "Baby Rosalina", "Pink Gold Peach", "Wario", "Waluigi", "Donkey Kong", "Bowser", "Dry Bones", "Bowser Jr.", "Dry Bowser", "Lemmy", "Larry", "Wendy", "Ludwig", "Iggy", "Roy", "Morton", "Inkling Girl", "Inkling Boy", "Link", "Villager", "Isabelle", "Birdo", "Petey Piranha", "Wiggler", "Kamek", "Peachette", "Funky Kong", "Diddy Kong", "Pauline" };
		public static bool isBot(string nick) {
			return botsList.Contains(nick);
		}
		static Dictionary<string, string> MetricNames = new Dictionary<string, string>{
			{ "Neo Bowser City", "Neo Koopa City"},
			{"DK Summit", "DK Snowboard Cross"},
			{"Music Park", "Melody Motorway"},
			{"Toad Harbor", "Toad Harbour"},
			{"Rock Rock Mountain", "Alpine Pass" },
			{"Bone-Dry Dunes", "Bone Dry Dunes" },
			{"Piranha Plant Slide", "Piranha Plant Pipeway" },
			{"Cheese Land", "Cheat Land"},
			{"Boo Lake", "Bukake"},
			{"Moonview Highway", "Moonview Motorway"},
			{"Wario's Gold Mine", "Wario Gold Mine"},
		};
		public static string FixTrackName(string track) {
			if(MetricNames.ContainsKey(track)) {
				return MetricNames[track];
			}
			return track;
		}
	}

	class Reader {
		private static Thread inputThread;
		private static AutoResetEvent getInput, gotInput;
		private static string input;

		static Reader() {
			getInput = new AutoResetEvent(false);
			gotInput = new AutoResetEvent(false);
			inputThread = new Thread(reader);
			inputThread.IsBackground = true;
			inputThread.Start();
		}

		private static void reader() {
			while(true) {
				getInput.WaitOne();
				input = Console.ReadLine();
				gotInput.Set();
			}
		}

		// omit the parameter to read a line without a timeout
		public static string ReadLine(int timeOutMillisecs = Timeout.Infinite) {
			getInput.Set();
			bool success = gotInput.WaitOne(timeOutMillisecs);
			if(success)
				return input;
			else
				throw new TimeoutException("User did not provide input within the timelimit.");
		}
	}
}