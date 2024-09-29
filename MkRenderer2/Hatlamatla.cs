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
		public static string vytvoritTableUnsorted(List<Zavod> races){
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
						//Console.WriteLine("no vybirac trate ? :(");
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
		public static string vytvoritTable(List<Zavod> races) {
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
						//Console.WriteLine("no vybirac trate ? :(");
					}
				}
				html.Append("</tr>");

				// Get all racers
				var racers = cup.SelectMany(r => r.zavodnici.Select(z => z.Item1.nick)).Distinct();

				// Prepare data for sorting
				List<Tuple<string, List<string>, string>> rows = new List<Tuple<string, List<string>, string>>();
				foreach(var racer in racers) {
					List<string> positions = new List<string>();
					foreach(var race in cup) {
						if(race.zavod != 99) {
							var racerPosition = race.zavodnici.FirstOrDefault(z => z.Item1.nick == racer)?.Item2;
							positions.Add(racerPosition.HasValue ? racerPosition.Value.ToString() : "-");
						}
					}
					var finalPosition = cup.FirstOrDefault(r => r.zavod == 99)?.zavodnici.FirstOrDefault(z => z.Item1.nick == racer)?.Item2;
					// Handle null finalPosition by using a placeholder or sorting logic
					string finalPositionStr = finalPosition.HasValue ? finalPosition.Value.ToString() : "N/A"; // Use "N/A" or similar as a placeholder
					rows.Add(new Tuple<string, List<string>, string>(racer, positions, finalPositionStr));
				}

				// Sort rows based on final position, handling "N/A" or similar placeholders appropriately
				var sortedRows = rows.OrderBy(r => r.Item3 == "N/A" ? int.MaxValue : int.Parse(r.Item3)).ToList();

				// Racer rows
				int row = 1;
				foreach(var sortedRow in sortedRows) {
					html.Append($"<tr class=\"tr{row % 2}\"><th>{sortedRow.Item1}</th>");
					foreach(var position in sortedRow.Item2) {
						html.Append($"<td>{position}</td>");
					}
					html.Append($"<td class=\"total2\">{sortedRow.Item3}</td></tr>");
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
	
		public static string vytvoritKartickyZavodniku(List<Zavodnik> zavodnici_, bool total) {
			List<Zavodnik> zavodnici = zavodnici_.OrderByDescending(z => z.pocetUjetychZavodu).ToList();
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
					foreach(var trat in zavodnik.vsechnyVybraneTrate) {
						vybraneTrateTemplate = vybraneTrateTemplate.Replace("{TRAT}", trat.nazev + " " + trat.kolikratVybrano + "x, {TRAT}");
					}
					vybraneTrateTemplate = vybraneTrateTemplate.Replace(", {TRAT}", "");
					karticka.Replace("{VYBRANE_TRATE}", vybraneTrateTemplate);
				}
				else {
					string vybraneTrateTemplate = "<div class=\"Trat\">Nejoblíbenější tratě:<br>{TRAT}</div>";
					var vybraneTrate = zavodnik.vsechnyVybraneTrate.OrderByDescending(t => t.kolikratVybrano).Take(5);
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

			int maxwidth = 90;
			finalIndex.Replace("BACKUPMAXWIDTH", $"{maxwidth}vw");
			finalIndex.Replace("BACKUPMAXHEIGHT", $"{maxwidth * height / width}vw");


			finalIndex.Replace("GRAFWIDTH", width.ToString());
			finalIndex.Replace("GRAFHEIGHT", height.ToString());

			finalIndex.Replace("{KARTICKY}", File.ReadAllText(directory + @"\karticky.html"));
			finalIndex.Replace("{TABULKY}", File.ReadAllText(directory + @"\table.html"));
			finalIndex.Replace("{NAZEV_ZAZNAMU}", dirname);
			//File.WriteAllText(directory + @"\index.php", finalIndex.ToString());
			return finalIndex.ToString();
		}




		public static void zpracovatOverallKartickyZavodniku(List<Zavodnik> zavodnici_total_) {
			List<Zavodnik> zavodnici_total = zavodnici_total_.OrderByDescending(z => z.pocetUjetychZavodu).ToList();

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

		public static void zpracovatRankingTrati(List<Zavodnik> zavodnici_total, Trate trate) { //zavodnici total contains how many times 1 person played a track, trate contains how many each times has played.
			StringBuilder template = new StringBuilder(File.ReadAllText(@".\out\trate_template.html"));
			string kartickaTemplate = File.ReadAllText(@".\out\trate_karticka.html");
			trate.Seradit();

			/*foreach(var z in zavodnici_total) {
				Console.WriteLine(z.nick);
				foreach(var t in z.vybraneTrate) {
					Console.WriteLine(t.nazev + " " + t.kolikratVybrano);
				}
				Console.WriteLine();
			}*/
			foreach(var t in trate.trate) {
				string nazevTrate = t.Key;
				int pocetHrani = t.Value;
				//find all zavodnici who choose this track, and sort them by how many times they played it. then take 5 zavodniks who played it the most
				var zavodniciNaTrati = zavodnici_total.Where(z => z.vlastniVybraneTrate.Any(tr => tr.nazev == nazevTrate)).OrderByDescending(z => z.vlastniVybraneTrate.First(tr => tr.nazev == nazevTrate).kolikratVybrano).Take(6);
				Console.WriteLine(nazevTrate + " " + pocetHrani);
				foreach(var z in zavodnici_total) {
					z.vlastniVybraneTrate.Where(tr => tr.nazev == nazevTrate).ToList().ForEach(tr => Console.WriteLine(z.nick + " " + tr.kolikratVybrano));
				}

				StringBuilder karticka = new StringBuilder(kartickaTemplate);
				karticka.Replace("NAZEV_TRATE", nazevTrate);
				karticka.Replace("POCET_HRANI0", pocetHrani.ToString() + "x");
			
				for(int i = 1; i <= 6; i++) {
					if(i < zavodniciNaTrati.Count() + 1) {
						karticka.Replace($"HRAC{i}", zavodniciNaTrati.ElementAt(i - 1).nick);
						karticka.Replace($"POCET_HRANI{i}", zavodniciNaTrati.ElementAt(i - 1).vlastniVybraneTrate.First(tr => tr.nazev == nazevTrate).kolikratVybrano.ToString() + "x");
					}
					else {
						//remove whole line that contains HRACi from the karticka
						karticka.Replace($"            <li>HRAC{i} &nbsp;<small><i>POCET_HRANI{i}</i></small></li>", "");
					}
				}
				template.Replace("KARTICKY", karticka.ToString());
			}
			template.Replace("KARTICKY", "");

			File.WriteAllText(@".\out\trate.php", template.ToString());
		}
	}
}
