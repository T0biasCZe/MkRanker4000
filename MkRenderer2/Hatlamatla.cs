using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static MkRenderer2.Render;

namespace MkRenderer2 {
	public static class Hatlamatla {
		public static string vytvoritTable(List<Zavod> races){
			StringBuilder html = new StringBuilder();
			var cups = races.GroupBy(r => r.cup);

			foreach(var cup in cups) {
				html.Append($"<h2>{cup.Key}. Cup</h2>");
				html.Append("<table class=\"table11\">");

				// Header
				html.Append("<tr class=\"tr1\"><td rowspan=\"2\" class=\"total2\"></td>");
				for(int i = 1; i <= cup.Count(); i++) {
					if(cup.ElementAt(i - 1).zavod == 99) continue;
					html.Append($"<th>{i}</th>");
				}
				html.Append("<th rowspan=\"2\">F</th></tr>");

				// Race names
				html.Append("<tr class=\"tr0\">");
				foreach(var race in cup) {
					if(race.zavod == 99) continue;
					if(race.vybiracTrate.Length > 0) {
						html.Append($"<th>{race.trat}<br>{race.vybiracTrate}</th>");
					}
					else {
						html.Append($"<th>{race.trat}</th>");
						Console.WriteLine("no vybirac trate ? :(");
					}
				}
				html.Append("</tr>");

				// Get all racers
				var racers = cup.SelectMany(r => r.zavodnici.Select(z => z.Item1.nick)).Distinct();

				// Racer rows
				int row = 1;
				foreach(var racer in racers) {
					html.Append($"<tr class=\"tr{row % 2}\"><th>{racer}</th>");
					foreach(var race in cup) {
						if(race.zavod != 99) {
							var racerPosition = race.zavodnici.FirstOrDefault(z => z.Item1.nick == racer)?.Item2;
							html.Append($"<td>{racerPosition}</td>");
						}
					}
					var finalPosition = cup.FirstOrDefault(r => r.zavod == 99)?.zavodnici.FirstOrDefault(z => z.Item1.nick == racer)?.Item2;
					html.Append($"<td class=\"total2\">{finalPosition}</td></tr>");
					row++;
				}

				html.Append("</table>");
			}
			return html.ToString();
		}
		public static string colorTable() {
			Type colorType = typeof(System.Drawing.Color);
			var colorList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
				.Select(propInfo => Color.FromName(propInfo.Name))
				.ToList();
			colorList.Remove(Color.Transparent);

			StringBuilder html = new StringBuilder();
			//generate table with all the colours, with first row being the colour names and second row being the RGB hex value. the background of the cell is the colour itself
			html.Append("All valid predefined colors in System.Drawing.Color (except Transparent)");
			html.Append("<table>");
			foreach( var color in colorList) {
				html.Append($"<tr style=\"background-color:{ColorTranslator.ToHtml(color)}\"><td>{color.Name}</td><td>{ColorTranslator.ToHtml(color)}</td></tr>");
			}
			html.Append("</table>");
			return html.ToString();
		}
	
		public static string vytvoritKartickyZavodniku(List<Zavodnik> zavodnici, bool total) {
			string templatePath = total ? @".\out\karticka_total.html" : @".\out\karticka.html";
			string template = File.ReadAllText(templatePath);

			StringBuilder karticky = new StringBuilder();
			//karticky.AppendLine("<link rel=\"stylesheet\"  href=\"./css/style.css\">");
			foreach(var zavodnik in zavodnici) {
				if(zavodnik.isBot) continue;
				StringBuilder karticka = new StringBuilder(template);
				if(zavodnik.nick == "lilibox") {
					karticka.Replace("{MEDIAN_POZICE}", "{MEDIAN_POZICE}\n<p>Počet VPN vrstev: <?php echo(rand (1,20)); ?></p> ");
				}
				karticka.Replace("{ZAVODNIK}", zavodnik.nick);
				karticka.Replace("{POCET_ZAVODU}", zavodnik.pocetUjetychZavodu.ToString());
				karticka.Replace("{PRUMERNA_POZICE}", zavodnik.prumernePoradi.ToString("0.0"));
				karticka.Replace("{MEDIAN_POZICE}", zavodnik.medianPoradi.ToString());
				karticka.Replace("{VAZENY_MEDIAN_POZICE}", zavodnik.vazeneMedianPoradi.ToString());
				karticka.Replace("{POCET_DNU}", zavodnik.pocetUjetychDnu.ToString());
				if(!total) {
					string vybraneTrateTemplate = "<div class=\"Trat\">Vybrané tratě:<br>{TRAT}</div>";
					foreach(var trat in zavodnik.vybraneTrate) {
						vybraneTrateTemplate = vybraneTrateTemplate.Replace("{TRAT}", trat.nazev + " " + trat.kolikratVybrano + "x, {TRAT}");
					}
					vybraneTrateTemplate = vybraneTrateTemplate.Replace(", {TRAT}", "");
					karticka.Replace("{VYBRANE_TRATE}", vybraneTrateTemplate);
				}
				else {
					string vybraneTrateTemplate = "<div class=\"Trat\">Nejoblíbenější tratě:<br>{TRAT}</div>";
					var vybraneTrate = zavodnik.vybraneTrate.OrderByDescending(t => t.kolikratVybrano).Take(5);
					foreach(var trat in vybraneTrate) {
						vybraneTrateTemplate = vybraneTrateTemplate.Replace("{TRAT}", trat.nazev + " " + trat.kolikratVybrano + "x<br>{TRAT}");
					}
					vybraneTrateTemplate = vybraneTrateTemplate.Replace("<br>{TRAT}", "");
					karticka.Replace("{VYBRANE_TRATE}", vybraneTrateTemplate);
				}

				karticky.Append(karticka.ToString());
			}
			return karticky.ToString();
		}

		public static string zpracovatFinalIndex(string dirname, int width, int height) {
			string directory = @".\out\" + dirname + @"\";

			StringBuilder finalIndex = new StringBuilder(File.ReadAllText(@".\out\index_template.html"));

			
			finalIndex.Replace("GRAFWIDTH", width.ToString());
			finalIndex.Replace("GRAFHEIGHT", height.ToString());

			finalIndex.Replace("{KARTICKY}", File.ReadAllText(directory + @"\karticky.html"));
			finalIndex.Replace("{TABULKY}", File.ReadAllText(directory + @"\table.html"));
			finalIndex.Replace("{NAZEV_ZAZNAMU}", dirname);
			//File.WriteAllText(directory + @"\index.php", finalIndex.ToString());
			return finalIndex.ToString();
		}




		public static void zpracovatOverallKartickyZavodniku(List<Zavodnik> zavodnici_total) {
			string karticky = vytvoritKartickyZavodniku(zavodnici_total, true);
			StringBuilder karticky_total = new StringBuilder(File.ReadAllText(@".\out\zavodnici_total.html"));

			//render distribution graph and save it to ./out/racer/nick/distribution.png using Render.renderGraphDistribuce()
			BARVA_CAR = Color.DarkGray;
			BARVA_TEXTU = Color.White;
			trh = TextRenderingHint.AntiAlias;

			foreach(Zavodnik zavodnik in zavodnici_total) {
				if(!Directory.Exists($@".\out\racisti\{zavodnik.nick}")) {
					Directory.CreateDirectory($@".\out\racisti\{zavodnik.nick}");
				}
			}

			renderGraphDistribuce(zavodnici_total, true, "./out/racisti/");

			BARVA_CAR = Color.DarkGray;
			BARVA_TEXTU = Color.Black;
			trh = TextRenderingHint.AntiAlias;
			renderGraphDistribuce(zavodnici_total, false, "./out/racisti/");

			karticky_total.Replace("{KARTICKY}", karticky);

			renderDlouhodobyGraf(zavodnici_total).Save("./out/dlouhodobygraf.png");

			File.WriteAllText($@".\out\racisti\index.php", karticky_total.ToString());
		}
	}
}
