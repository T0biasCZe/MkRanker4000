
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using static MkRenderer2.Render;
using static Zaznam;

namespace MkRenderer2 {
	internal class Program {
		static void Main(string[] args) {
			List<Zavodnik> zavodnici_total = new List<Zavodnik>();


			//for each file in folder .\in
			var files = Directory.GetFiles(".\\in");

			foreach(string file in files) {
				Console.WriteLine("Processing file " + file);
				string filename = Path.GetFileNameWithoutExtension(file);


				List<Zaznam> zaznamy = loadCsv(file);

				Render.init();
				Render.zpracovatZaznamy(zaznamy, out List<Zavod> zavody, out List<Zavodnik> zavodnici);
				//Bitmap bm = Render.renderGraphPozice(zaznamy);

				Directory.CreateDirectory($@".\out\{filename}\grafy");


				Render.BARVA_TEXTU = Color.Black;
				Render.trh = TextRenderingHint.AntiAlias;
				Bitmap bm = Render.renderGraphPozice(zavody, zavodnici, "", false, false);
				bm.Save(@$".\out\{filename}\grafy\graf_all.png");
				Render.BARVA_TEXTU = Color.White;
				Render.trh = TextRenderingHint.ClearTypeGridFit;
				bm = Render.renderGraphPozice(zavody, zavodnici, "", false, false);
				bm.Save($@".\out\{filename}\grafy\graf_all_dark.png");

				//Console.WriteLine("bitmapa zapsana do cele cesty " + Path.GetFullPath("output.png"));

				Render.vyrenderovatKarticky(zavody, zavodnici, $"./out/{filename}/grafy/");

				File.WriteAllText($@".\out\{filename}\table.html", Hatlamatla.vytvoritTable(zavody));
				File.WriteAllText(@$".\out\{filename}\karticky.html", Hatlamatla.vytvoritKartickyZavodniku(zavodnici, false));
				File.WriteAllText(@$".\out\{filename}\colortable.html", Hatlamatla.colorTable());

				//Hatlamatla.zpracovatFinalIndex();

				File.WriteAllText(@$".\out\{filename}\index.php", Hatlamatla.zpracovatFinalIndex(filename));

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
						zavodnici_total.Add(zavodnik);
						zavodnik.pocetUjetychDnu = 1;
					} else {
						for(int i = 0; i < zavodnik_total.pocetDojetiNaIndexu.Count(); i++) {
							zavodnik_total.pocetDojetiNaIndexu[i] += zavodnik.pocetDojetiNaIndexu[i];
						}
						zavodnik_total.pocetUjetychZavodu += zavodnik.pocetUjetychZavodu;

						zavodnik_total.RecalcPos();
						zavodnik_total.pocetUjetychDnu += 1;
					}
				}

				/*save colour with jmena to barvy.txt with format "#RRGGBB;Nick"*/
				StreamWriter sw = new StreamWriter($".\\out\\{filename}\\barvy.txt");
				for(int i = 0; i < zavodnici.Count; i++) {
					sw.WriteLine(zavodnici[i].barva.ToHex() + ";" + zavodnici[i].nick);
				}
				sw.Flush();
				sw.Close();

			}
			Hatlamatla.zpracovatOverallKartickyZavodniku(zavodnici_total);

		}
		private static List<Zaznam> loadCsv(string file) {
			//string[] lines = File.ReadAllLines("zaznamy.csv");
			string[] lines = File.ReadAllLines(file);
			List<Zaznam> zaznamy = new List<Zaznam>();
			foreach(string line in lines) {
				string[] parts = line.Split(";");
				Zaznam zaznam = new Zaznam();
				zaznam.zprava = parts[0];
				zaznam.casZaznamu = DateTime.Parse(parts[1]);
				zaznam.zavodnik = parts[2];
				zaznam.cc = int.Parse(parts[3]);
				zaznam.mirrorOn = bool.Parse(parts[4]);
				zaznam.cup = int.Parse(parts[5]);
				zaznam.zavod = int.Parse(parts[6]);
				zaznam.poradi = int.Parse(parts[7]);
				zaznam.trat = parts[8];
				if(parts.Length > 9) {
					zaznam.vybiracTrate = parts[9];
					Console.WriteLine("trat vybrana hračem " + zaznam.vybiracTrate);
				}
				zaznamy.Add(zaznam);
			}
			return zaznamy;
		}
	}
}