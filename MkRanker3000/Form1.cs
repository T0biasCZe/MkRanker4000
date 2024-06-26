using MkRanker3000;
using System.Text;

namespace MKRanker3000 {
	public partial class Form1 : Form {
		public static List<string> zavodnici = new List<string> { "TheStoupa", "T0biasCZe", "Tom", "Jane", "Cwrcekk", "Boun", "BeNiSh", "Claudi", "lilibox", "Mates", "John Beak", "NUL", "Honzik" };
		public static List<Zaznam> zaznamy = new List<Zaznam>();
		public Form1() {
			InitializeComponent();
			foreach(string zavodnik in zavodnici) {
				//comboBox_zavodnik.Items.Add(zavodnik);
				listBox1.Items.Add(zavodnik);
				comboBox_vybiracTrate.Items.Add(zavodnik);
			}
			if(File.Exists("zaznamy.csv")) {
				loadCsv();
			}

		}
		int actualniCup = 0;
		int actualniZavod = 1;
		int actualniCC = 150;
		bool actualniMirror = false;

		bool overrideZavodnik = false;
		string zavodnikOverride = "";

		private void button_submitZaznam_Click(object sender, EventArgs e) {
			Zaznam zaznam = new Zaznam();
			//zaznam.zavodnik = comboBox_zavodnik.Text;

			if(overrideZavodnik) {
				zaznam.zavodnik = zavodnikOverride;
				overrideZavodnik = false;
			}
			else zaznam.zavodnik = listBox1.SelectedItem.ToString();

			zaznam.cup = actualniCup;
			zaznam.zavod = actualniZavod;
			zaznam.poradi = (int)numericUpDown_pozice.Value;
			zaznam.trat = comboBox_trat.Text;
			zaznam.vybiracTrate = comboBox_vybiracTrate.Text;
			zaznam.casZaznamu = DateTime.Now;
			zaznam.isBot = checkBox_isBot.Checked;

			zaznamy.Add(zaznam);
			listBox_e.Items.Add(zaznam);
			listBox_e.SelectedIndex = listBox_e.Items.Count - 1;

			writeCsv();
		}
		private void button_submitRace_Click(object sender, EventArgs e) {
			actualniZavod++;
			label_zavodCount.Text = "zavod cislo: " + actualniZavod;
			Zaznam zaznam = new Zaznam();
			zaznam.zprava = "Zadavani zavodu " + actualniZavod + "v cupu " + actualniCup + "bylo ukonceno.";
			zaznam.casZaznamu = DateTime.Now;
			zaznamy.Add(zaznam);
			listBox_e.Items.Add(zaznam);
			listBox_e.SelectedIndex = listBox_e.Items.Count - 1;

			writeCsv();
		}
		private bool zadavaniFinalnichVysledkuCupu = true;
		private void button1_finishCup_Click(object sender, EventArgs e) {
			label_final.Enabled = true;
			actualniZavod = 99;
			zadavaniFinalnichVysledkuCupu = true;
			Zaznam zaznam = new Zaznam();
			zaznam.zprava = "Cup " + actualniCup + "byl dojen, probiha zadavani finalnich vysledku.";
			zaznam.casZaznamu = DateTime.Now;
			zaznamy.Add(zaznam);
			listBox_e.Items.Add(zaznam);

			writeCsv();
		}

		private void button_newCup_Click(object sender, EventArgs e) {
			if(!zadavaniFinalnichVysledkuCupu) {
				MessageBox.Show(Text = "Nelze zahajit novy cup, pokud neni ukonceno zadavani finalnich vysledku predchoziho cupu.", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			label_final.Enabled = false;
			zadavaniFinalnichVysledkuCupu = false;
			actualniCup++;
			actualniZavod = 1;
			Zaznam zaznam = new Zaznam();
			zaznam.zprava = "Byl zahajen novy cup " + actualniCup + ".";
			zaznam.casZaznamu = DateTime.Now;
			zaznamy.Add(zaznam);
			listBox_e.Items.Add(zaznam);
			actualniCC = (int)numericUpDown_cc.Value;
			actualniMirror = checkBox_mirror.Checked;

			label_zavodCount.Text = "zavod cislo: " + actualniZavod;
			label_cupCount.Text = "cup cislo: " + actualniCup;

			writeCsv();
		}
		private void writeCsv() {
			StringBuilder csv = new StringBuilder();
			foreach(Zaznam zaznam in zaznamy) {
				//csv.AppendLine(zaznam.zprava + ";" + zaznam.casZaznamu + ";" + zaznam.zavodnik + ";" + zaznam.cc + ";" + zaznam.mirrorOn + ";" + zaznam.cup + ";" + zaznam.zavod + ";" + zaznam.poradi + ";" + zaznam.trat + ";");
				csv.AppendLine(zaznam.ToString());
			}
			File.WriteAllText("zaznamy.csv", csv.ToString());
		}

		private void loadCsv() {
			string[] lines = File.ReadAllLines("zaznamy.csv");
			foreach(string line in lines) {
				string[] parts = line.Split(";");
				Zaznam zaznam = new Zaznam();
				zaznam.zprava = parts[0];
				zaznam.casZaznamu = DateTime.Parse(parts[1]);

				//I am a fuckin idiot, I forgot this line
				zaznam.zavodnik = parts[2];

				zaznam.cc = int.Parse(parts[3]);
				zaznam.mirrorOn = bool.Parse(parts[4]);
				zaznam.cup = int.Parse(parts[5]);
				zaznam.zavod = int.Parse(parts[6]);
				zaznam.poradi = int.Parse(parts[7]);
				zaznam.trat = parts[8];
				if(parts.Length > 9) {
					zaznam.vybiracTrate = parts[9];
					if( parts.Length > 10) {
						zaznam.isBot = bool.Parse(parts[10]);
					}

				}
				zaznamy.Add(zaznam);
				listBox_e.Items.Add(zaznam);
			}
		}

		private void textBox_pridatZavodnika_KeyPress(object sender, KeyPressEventArgs e) {
			//check if enter was pressed
			if(e.KeyChar == (char)Keys.Enter) {
				//comboBox_zavodnik.Items.Add(textBox_pridatZavodnika.Text);
				listBox1.Items.Add(textBox_pridatZavodnika.Text);
				zavodnici.Add(textBox_pridatZavodnika.Text);
				textBox_pridatZavodnika.Text = "";
			}
		}

		private void Form1_Resize(object sender, EventArgs e) {
			listBox_e.Height = this.Height - 48 - 8;
		}

		private void button1_Click(object sender, EventArgs e) {
			OCR ocr = new OCR();
			ocr.playersList = zavodnici;
			ocr.ShowDialog();

			if(ocr.success) {

				var data = ocr.output;
				foreach(var zaznam in data) {
					try {
						checkBox_isBot.Checked = zaznam.isBot;
						checkBox_isBot.Refresh();
						zavodnikOverride = zaznam.zavodnik;
						overrideZavodnik = true;
						numericUpDown_pozice.Value = zaznam.pozice;

						Thread.Sleep(16);

						button_submitZaznam.PerformClick();
					}
					catch(Exception ex) {
						MessageBox.Show(ex.ToString(), "OKUREK", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			else {
				System.Media.SystemSounds.Beep.Play();
			}
		}

		private void label_zavodCount_Click(object sender, EventArgs e) {
			actualniZavod = ShowNumberDialog(1, 99);
			label_zavodCount.Text = "zavod cislo: " + actualniZavod;
		}
		private void label_cupCount_Click(object sender, EventArgs e) {
			actualniCup = ShowNumberDialog(1, 99);
			label_cupCount.Text = "cup cislo: " + actualniCup;
		}
		private int ShowNumberDialog(int min, int max) {
			Form numForm = new Form();
			numForm.Text = "Zadej cislo";
			numForm.Size = new Size(200, 100);
			NumericUpDown num = new NumericUpDown();
			num.Minimum = min;
			num.Maximum = max;
			numForm.Controls.Add(num);
			num.Dock = DockStyle.Fill;
			numForm.ShowDialog();
			return (int)num.Value;
		}
	}
}
