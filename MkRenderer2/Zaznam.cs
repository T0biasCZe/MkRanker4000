using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum AnimalCrossingVerze {
	NeniAc,
	Jaro,
	Leto,
	Podzim,
	Zima,
	Nespecifikovano
}
public class Zaznam {
	public string zprava = "";
	public DateTime casZaznamu = DateTime.Now;
	public string zavodnik = "Bag";
	public int cc = 150;
	public bool mirrorOn = false;
	public int cup = 999;
	public int zavod = 999;
	public int poradi = 999;
	public string trat = "";
	public AnimalCrossingVerze acVerze = AnimalCrossingVerze.NeniAc;
	public string vybiracTrate = "";
	public bool vybranoRandom = false;
	public bool isBot = false;
	public override string ToString() {
		return this.zprava + ";" + this.casZaznamu + ";" + this.zavodnik + ";" + this.cc + ";" + this.mirrorOn + ";" + this.cup + ";" + this.zavod + ";" + this.poradi + ";" + this.trat;
	}
}
public class Trat {
	public string nazev;
	public AnimalCrossingVerze acVerze;
	public int kolikratVybrano;
}
public class Trate {
	public Dictionary<string, int> trate = new Dictionary<string, int>();
	public void PridatTrat(string nazevTrate) {
		if(trate.ContainsKey(nazevTrate)) {
			trate[nazevTrate]++;
		}
		else {
			trate.Add(nazevTrate, 1);
		}
	}
	public void PridatTrate(Trate trate) {
		foreach(var t in trate.trate) {
			if(this.trate.ContainsKey(t.Key)) {
				this.trate[t.Key] += t.Value;
			}
			else {
				this.trate.Add(t.Key, t.Value);
			}
		}
	}
	public void Seradit() {
		//sort the dictionary by value, bigger values first
		trate = trate.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
	}
	public string VypsatTrate() {
		string s = "";
		foreach(var t in trate) {
			s += t.Key + " " + t.Value + "\n";
		}
		return s;
	}
}

public class Zavodnik {
	public string nick = "";
	public Color barva;
	public int pocetUjetychZavodu;
	public float prumernePoradi;
	public float medianPoradi;
	public int[] pocetDojetiNaIndexu = new int[12];

	public double vazeneMedianPoradi;
	public List<Tuple<int, int>> poziceSWeight = new List<Tuple<int, int>>();
	public List<Tuple<string, double>> medianyVPrubehuCasu = new List<Tuple<string, double>>();

	public int pocetUjetychDnu;
	public bool isBot;
	public List<Trat> vlastniVybraneTrate = new List<Trat>();
	public List<Trat> druheVybraneTrate = new List<Trat>(); // kdyz vice lidi vybere stejnou trat, tak těm ostatnim kterym to nepadlo se to uloži zde
    public List<Trat> vsechnyVybraneTrate => vlastniVybraneTrate.Concat(druheVybraneTrate).ToList();
	public void RecalcPos() {
		//calculate average position of each zavodnik
		int celkem = 0;
		int pocet = 0;
		for(int i = 0; i < pocetDojetiNaIndexu.Length; i++) {
			celkem += pocetDojetiNaIndexu[i] * (i + 1);
			pocet += pocetDojetiNaIndexu[i];
		}
		prumernePoradi = (float)celkem / pocet;


		//calculate median position of each zavodnik
		celkem = 0;
		pocet = pocetDojetiNaIndexu.Sum();
		for(int i = 0; i < pocetDojetiNaIndexu.Length; i++) {
			celkem += pocetDojetiNaIndexu[i];
			if(celkem >= pocet / 2) {
				medianPoradi = i + 1;
				break;
			}
		}

		vazeneMedianPoradi = QuikMefs.CalculateWeightedMedian(poziceSWeight);
	}
	public void PridatTrat(string nazevTrate) {
		//if trat is already in list, increase the kolikratVybrano, else add it as new and set kolikratVybrano to 1
		Trat t = vlastniVybraneTrate.FirstOrDefault(x => x.nazev == nazevTrate);
		if(t != null) {
			t.kolikratVybrano++;
		}
		else {
			vlastniVybraneTrate.Add(new Trat() { nazev = nazevTrate, kolikratVybrano = 1 });
		}
	}
	public void PridatDruhouTrat(string nazevTrate) {
		//if trat is already in list, increase the kolikratVybrano, else add it as new and set kolikratVybrano to 1
		Trat t = druheVybraneTrate.FirstOrDefault(x => x.nazev == nazevTrate);
		if(t != null) {
			t.kolikratVybrano++;
		}
		else {
			druheVybraneTrate.Add(new Trat() { nazev = nazevTrate, kolikratVybrano = 1 });
		}
	}

	public bool IsWorthy(List<string> dates) {
		int pocetDnuZavodnik = this.pocetUjetychDnu;
		int pocetDnu = dates.Count;
		float ucast = (float)pocetDnuZavodnik / pocetDnu;

		bool racedInLast4Days = false;
		for(int i = this.medianyVPrubehuCasu.Count; i > (this.medianyVPrubehuCasu.Count - 5); i--) {
			if(i <= 0) break;
			string date = this.medianyVPrubehuCasu[i - 1].Item1;
			string currentDate = dates[dates.Count - 1];

			if(date == currentDate) {
				racedInLast4Days = true;
				break;
			}
		}
		if(ucast < 0.15 && !racedInLast4Days) {
			ConsoleColor oldColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("--------");
			Console.WriteLine("skipping " + this.nick + " because of low ucast");
			Console.WriteLine("--------");
			Console.ForegroundColor = oldColor;
			return false;
		}
		return true;
	}
}

public static class QuikMefs {
	public static double CalculateWeightedMedian(List<Tuple<int, int>> numbersWithWeights) {
		// Step 1: Sort the list by the numbers
		var sortedList = numbersWithWeights.OrderBy(x => x.Item1).ToList();

		// Step 2: Calculate the total weight
		int totalWeight = sortedList.Sum(x => x.Item2);

		// Step 3: Find the weighted median
		int cumulativeWeight = 0;
		foreach(var pair in sortedList) {
			cumulativeWeight += pair.Item2;
			if(cumulativeWeight >= totalWeight / 2.0) {
				return pair.Item1;
			}
		}

		// If the list is empty or there's an error, return 0 or throw an exception
		throw new InvalidOperationException("Cannot calculate the weighted median of an empty list.");
	}
}