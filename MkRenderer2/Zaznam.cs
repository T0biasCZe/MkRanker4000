﻿using System;
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
	public override string ToString() {
		return this.zprava + ";" + this.casZaznamu + ";" + this.zavodnik + ";" + this.cc + ";" + this.mirrorOn + ";" + this.cup + ";" + this.zavod + ";" + this.poradi + ";" + this.trat;
	}
}
public class Zavodnik {
	public string nick = "";
	public Color barva;
	public int pocetUjetychZavodu;
	public float prumernePoradi;
	public float medianPoradi;
	public int[] pocetDojetiNaIndexu = new int[12];
	public int pocetUjetychDnu;
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
	}
}
