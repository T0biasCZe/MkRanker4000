using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
	public string vybiracTrate = "";
	public bool isBot = false;
	public override string ToString() {
		return this.zprava + ";" + this.casZaznamu + ";" + this.zavodnik + ";" + this.cc + ";" + this.mirrorOn + ";" + this.cup + ";" + this.zavod + ";" + this.poradi + ";" + this.trat;
	}
}
public class Trat {
	public string nazev;
	public int kolikratVybrano;
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
	public List<Trat> vybraneTrate = new List<Trat>();
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
		Trat t = vybraneTrate.FirstOrDefault(x => x.nazev == nazevTrate);
		if(t != null) {
			t.kolikratVybrano++;
		}
		else {
			vybraneTrate.Add(new Trat() { nazev = nazevTrate, kolikratVybrano = 1 });
		}
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