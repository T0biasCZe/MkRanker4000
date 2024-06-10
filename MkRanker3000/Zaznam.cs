using System;
using System.Collections.Generic;
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
		return this.zprava + ";" + this.casZaznamu + ";" + this.zavodnik + ";" + this.cc + ";" + this.mirrorOn + ";" + this.cup + ";" + this.zavod + ";" + this.poradi + ";" + this.trat + ";" + this.vybiracTrate + ";" + this.isBot;
	}
}
public class Zavodnik {
	public string nick = "";
	public Color barva;
	public int pocetUjetychZavodu;
	public float prumernePoradi;
	public float medianPoradi;
	public int[] pocetDojetiNaIndexu = new int[12];

	public int pocetUjetychDnu = 1;
}