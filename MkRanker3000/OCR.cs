using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TrackBar = System.Windows.Forms.TrackBar;

namespace MkRanker3000 {
	public partial class OCR : Form {
		public OCR() {
			InitializeComponent();
			position_image.ImageLayout = DataGridViewImageCellLayout.Stretch;
			player_image.ImageLayout = DataGridViewImageCellLayout.Stretch;
			UpdateComboboxesInDGV();

			if(IsNvidiaAvailable()) {
				label_cpuWarn.Visible = false;
			}
		}

		public List<string> playersList = new List<string>();
		List<string> botsList = new List<string> { "Mario", "Metal Mario", "Gold Mario", "Luigi", "Peach", "Daisy", "Rosalina", "Tanooki Mario", "Cat Peach", "Yoshi", "Toad", "Koopa Troopa", "Shy Guy", "Lakitu", "Toadette", "King Boo", "Baby Mario", "Baby Luigi", "Baby Peach", "Baby Daisy", "Baby Rosalina", "Pink Gold Peach", "Wario", "Waluigi", "Donkey Kong", "Bowser", "Dry Bones", "Bowser Jr.", "Dry Bowser", "Lemmy", "Larry", "Wendy", "Ludwig", "Iggy", "Roy", "Morton", "Inkling Girl", "Inkling Boy", "Link", "Villager", "Isabelle", "Birdo", "Petey Piranha", "Wiggler", "Kamek", "Peachette", "Funky Kong", "Diddy Kong", "Pauline" };
		public void UpdateComboboxesInDGV() {
			//update all the combo boxes in datagridview to have all the zavodniks in datagridview
			//zavodnik_combobox.Items.Clear();
			//foreach(string player in playersList) zavodnik_combobox.Items.Add(player);
			//foreach(string player in botsList) zavodnik_combobox.Items.Add(player);


		}

		private void OCR_DragEnter(object sender, DragEventArgs e) {
			//if bitmap file like jpg or ´png is dragged into the form then allow the drop
			if(e.Data.GetDataPresent(DataFormats.FileDrop) && ((string[])e.Data.GetData(DataFormats.FileDrop))[0].EndsWith(".jpg") || ((string[])e.Data.GetData(DataFormats.FileDrop))[0].EndsWith(".png")) {
				e.Effect = DragDropEffects.Copy;
			}
		}

		private void OCR_DragDrop(object sender, DragEventArgs e) {
			string filepath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
			Bitmap bitmap = new Bitmap(filepath);
			dataGridView1.Rows.Clear();
			ZpracovatZaznamy(bitmap, filepath);
		}


		private float ZACATEK_COLUMN_1, KONEC_COLUMN_1, ZACATEK_COLUMN_2, KONEC_COLUMN_2, ZACATEK_PRVNIHO_RADKU, VYSKA_RADKU, RADEK_MARGIN_TOP, RADEK_MARGIN_BOTTOM, RADEK1_MARGIN_TOP, RADEK1_MARGIN_BOTTOM;
		public void SetData(bool final) {
			if(final) {
				ZACATEK_COLUMN_1 = 0.04f;
				KONEC_COLUMN_1 = 0.08f;
				//the column with name
				ZACATEK_COLUMN_2 = 0.115f;
				KONEC_COLUMN_2 = 0.276f;

				ZACATEK_PRVNIHO_RADKU = 0.185f;
				VYSKA_RADKU = 0.058f;
				RADEK_MARGIN_TOP = 0.008f;
				RADEK_MARGIN_BOTTOM = 0.008f;

				//number
				RADEK1_MARGIN_TOP = 0.005f;
				RADEK1_MARGIN_BOTTOM = 0.005f;
			}
			else {
				ZACATEK_COLUMN_1 = 0.435f;
				KONEC_COLUMN_1 = 0.484f;
				//the column with name
				ZACATEK_COLUMN_2 = 0.53f;
				KONEC_COLUMN_2 = 0.7f;

				ZACATEK_PRVNIHO_RADKU = 0.072f;
				VYSKA_RADKU = 0.072f;
				RADEK_MARGIN_TOP = 0.01f;
				RADEK_MARGIN_BOTTOM = 0.01f;

				//number
				RADEK1_MARGIN_TOP = 0.005f;
				RADEK1_MARGIN_BOTTOM = 0.015f;
			}
		}
		public enum confidence {
			LOW = 2,
			MEDIUM = 1,
			HIGH = 0,
		}
		List<string> potentiallyBadRecognisions = new List<string> { "Tom", "Wario", "Mario", "Mates", "Mates1500"};
		List<string> surelyBadRecognisions = new List<string> { "N/A", "NUL", ""};
		private void ZpracovatZaznamy(Bitmap bitmap, string filepath) {
			SetData(radioButton2.Checked);

			int brightness = -1;
			int contrast = -1;
			if(radioButton2.Checked) {
				brightness = trackBar4.Value;
				contrast = trackBar3.Value;
			}
			else {
				brightness = trackBar2.Value;
				contrast = trackBar1.Value;
			}

			//MessageBox.Show("Processing image");
			var output = new List<VysledekLite>();
			label1.Text = "Adjusting contrast...";
			label1.Refresh();
			try {
				ApplyContrast((float)contrast, bitmap);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
			label1.Text = "Clipping brightess...";
			label1.Refresh();
			try {
				ClipDark((float)brightness, bitmap);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
			label1.Text = "Inverting colah...";
			label1.Refresh();
			try {
				Invert(bitmap);
			}
			catch(Exception ex) {
				MessageBox.Show(ex.ToString());
			}
			label1.Text = "saving brightness contrast...";
			label1.Refresh();
			bitmap.Save(Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath) + "_contrast" + Path.GetExtension(filepath)));
			try {

				label1.Text = "Loading OCR...";
				label1.Refresh();
				var engine = new TesseractEngine(@"./tessdata", textBox1.Text, EngineMode.Default);

				label1.Text = "Loading image...";
				label1.Refresh();
				var btp = new BitmapToPixConverter();
				var pix = btp.Convert(bitmap);

				Bitmap mask = new Bitmap(bitmap.Width, bitmap.Height);
				Graphics g = Graphics.FromImage(mask);

				for(int i = 0; i < 12; i++) {

					label1.Text = "Processing " + (i + 1) + ". row...";
					label1.Refresh();
					Rectangle cell_column1 = new Rectangle(
						(int)(ZACATEK_COLUMN_1 * bitmap.Width),
						(int)(((ZACATEK_PRVNIHO_RADKU + VYSKA_RADKU * i) + RADEK1_MARGIN_TOP) * bitmap.Height),
						(int)((KONEC_COLUMN_1 - ZACATEK_COLUMN_1) * bitmap.Width),
						(int)((VYSKA_RADKU - RADEK1_MARGIN_TOP - RADEK1_MARGIN_BOTTOM) * bitmap.Height)
					);
					g.DrawRectangle(new Pen(Color.Red, 2), cell_column1);

					Bitmap cislo = bitmap.Clone(cell_column1, PixelFormat.DontCare);
					//save as i.png next to exe
					cislo.Save(i + ".png");


					Rectangle cell_column2 = new Rectangle(
						(int)(ZACATEK_COLUMN_2 * bitmap.Width),
						(int)((ZACATEK_PRVNIHO_RADKU + VYSKA_RADKU * i + RADEK_MARGIN_TOP) * bitmap.Height),
						(int)((KONEC_COLUMN_2 - ZACATEK_COLUMN_2) * bitmap.Width),
						(int)((VYSKA_RADKU - RADEK_MARGIN_TOP - RADEK_MARGIN_BOTTOM) * bitmap.Height)
					);
					g.DrawRectangle(new Pen(Color.Green, 2), cell_column2);

					Bitmap zavodnik = bitmap.Clone(cell_column2, PixelFormat.DontCare);

					//get colour of pixel 2 pixels from right in the middle of the cell and save it to Color c
					int xxx = cell_column2.X + cell_column2.Width - 6;
					int yyy = cell_column2.Y + cell_column2.Height / 2;
					Color c = bitmap.GetPixel(xxx, yyy);
					g.DrawEllipse(new Pen(Color.Blue, 2), new Rectangle(xxx - 2, yyy - 2, 4, 4));
					//check if colour is close to 4800ff if yes then invert the cell
					if(c.R < 0x50 && c.G < 0x02 && c.B > 0xee) {
						Console.WriteLine("inverting colour " + c);
						try {
							Invert(cislo);
						}
						catch(Exception ex) {
							MessageBox.Show(ex.ToString());
						}
					}
					else Console.WriteLine("not inverting colour " + c);

					int position = i + 1;
					confidence position_confidence = confidence.HIGH;
					if(radioButton2.Checked) {
						try {
							position = GetPozici(cislo, radioButton2.Checked);
							position_confidence = confidence.HIGH;
						}
						catch(Exception ex) {
							position_confidence = confidence.LOW;
							MessageBox.Show(ex.ToString());
						}
					}
					string racer = "N/A";
					confidence racer_confidence = confidence.HIGH;

					try {
						engine.SetVariable("classify_bln_numeric_mode", "0");
						engine.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJLMNOPQRSTUV123456789 ");
						using(var page = engine.Process(pix, ToRect(cell_column2), PageSegMode.SingleLine)) {
							racer = page.GetText().Trim();
							Console.WriteLine("Racer: " + racer);
							Console.WriteLine("Confidence: " + page.GetMeanConfidence());
							if(page.GetMeanConfidence() < 0.4f) racer_confidence = confidence.LOW;
							else if(page.GetMeanConfidence() < 0.7f) racer_confidence = confidence.MEDIUM;
						}
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
					}
					/*
					MicrosoftOcrScan(bitmap, cell_column1, out position);
					MicrosoftOcrScan(bitmap, cell_column2, out racer);
					*/
					string closest = FindClosest(racer, 5, out bool isBot);
					dataGridView1.Rows.Add(cislo, position.ToString(), zavodnik, racer, closest, isBot);

					//if confidence is low paint the cell inside datagridview red, if medium paint it yellow
					if(position_confidence == confidence.LOW) {
						dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.Red;
					}
					else if(position_confidence == confidence.MEDIUM) {
						dataGridView1.Rows[i].Cells[1].Style.BackColor = Color.Yellow;
					}
					if(racer_confidence == confidence.LOW) {
						dataGridView1.Rows[i].Cells[3].Style.BackColor = Color.Red;
					}
					else if(racer_confidence == confidence.MEDIUM) {
						dataGridView1.Rows[i].Cells[3].Style.BackColor = Color.Yellow;
					}

					//highlight the name row if the name is in the list of potentially bad recognisions
					if(potentiallyBadRecognisions.Contains(closest)) {
						dataGridView1.Rows[i].Cells[4].Style.BackColor = Color.Yellow;
					}
					if(surelyBadRecognisions.Contains(racer) || surelyBadRecognisions.Contains(closest)) {
						dataGridView1.Rows[i].Cells[4].Style.BackColor = Color.Red;
					}

				}
				g.Dispose();
				label1.Text = "Saving mask...";
				mask.Save(Path.Combine(Path.GetDirectoryName(filepath), Path.GetFileNameWithoutExtension(filepath) + "_mask" + Path.GetExtension(filepath)));

			}
			catch(Exception ex) {
				MessageBox.Show(ex.ToString());

			}
			label1.Text = "Done";
			//return output;
		}
		public Rectangle ToRectangle(Rect rect) {
			return new Rectangle(rect.X1, rect.Y1, rect.Width, rect.Height);
		}
		public Rect ToRect(Rectangle rectangle) {
			return new Rect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
		private unsafe void ApplyContrast(double contrast, Bitmap bmp) {
			byte[] contrast_lookup = new byte[256];
			double newValue = 0;
			double c = (100.0 + contrast) / 100.0;

			c *= c;

			for(int i = 0; i < 256; i++) {
				newValue = (double)i;
				newValue /= 255.0;
				newValue -= 0.5;
				newValue *= c;
				newValue += 0.5;
				newValue *= 255;

				if(newValue < 0)
					newValue = 0;
				if(newValue > 255)
					newValue = 255;
				contrast_lookup[i] = (byte)newValue;
			}

			var bitmapdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
				System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			int PixelSize = 4;

			for(int y = 0; y < bitmapdata.Height; y++) {
				byte* destPixels = (byte*)bitmapdata.Scan0 + (y * bitmapdata.Stride);
				for(int x = 0; x < bitmapdata.Width; x++) {
					destPixels[x * PixelSize] = contrast_lookup[destPixels[x * PixelSize]]; // B
					destPixels[x * PixelSize + 1] = contrast_lookup[destPixels[x * PixelSize + 1]]; // G
					destPixels[x * PixelSize + 2] = contrast_lookup[destPixels[x * PixelSize + 2]]; // R
																									//destPixels[x * PixelSize + 3] = contrast_lookup[destPixels[x * PixelSize + 3]]; //A
				}
			}
			bmp.UnlockBits(bitmapdata);
		}
		private unsafe void ClipDark(double minBright, Bitmap bmp) {

			var bitmapdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
				System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			int PixelSize = 4;

			for(int y = 0; y < bitmapdata.Height; y++) {
				byte* destPixels = (byte*)bitmapdata.Scan0 + (y * bitmapdata.Stride);
				for(int x = 0; x < bitmapdata.Width; x++) {

					if(destPixels[x * PixelSize] < minBright && destPixels[x * PixelSize + 1] < minBright && destPixels[x * PixelSize + 2] < minBright) {
						destPixels[x * PixelSize] = 0;
						destPixels[x * PixelSize + 1] = 0;
						destPixels[x * PixelSize + 2] = 0;
					}
				}
			}
			bmp.UnlockBits(bitmapdata);
		}
		private unsafe void ClipMonochrome(double minBright, Bitmap bmp) {

			var bitmapdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
				System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			int PixelSize = 4;

			for(int y = 0; y < bitmapdata.Height; y++) {
				byte* destPixels = (byte*)bitmapdata.Scan0 + (y * bitmapdata.Stride);
				for(int x = 0; x < bitmapdata.Width; x++) {

					if(destPixels[x * PixelSize] < minBright && destPixels[x * PixelSize + 1] < minBright && destPixels[x * PixelSize + 2] < minBright) {
						destPixels[x * PixelSize] = 0;
						destPixels[x * PixelSize + 1] = 0;
						destPixels[x * PixelSize + 2] = 0;
					}
					else {
						destPixels[x * PixelSize] = 255;
						destPixels[x * PixelSize + 1] = 255;
						destPixels[x * PixelSize + 2] = 255;
					}
				}
			}
			bmp.UnlockBits(bitmapdata);
		}

		private unsafe void Invert(Bitmap bmp) {
			var bitmapdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
	System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			int PixelSize = 4;

			for(int y = 0; y < bitmapdata.Height; y++) {
				byte* destPixels = (byte*)bitmapdata.Scan0 + (y * bitmapdata.Stride);
				for(int x = 0; x < bitmapdata.Width; x++) {
					destPixels[x * PixelSize] = (byte)(255 - destPixels[x * PixelSize]);
					destPixels[x * PixelSize + 1] = (byte)(255 - destPixels[x * PixelSize + 1]);
					destPixels[x * PixelSize + 2] = (byte)(255 - destPixels[x * PixelSize + 2]);
				}
			}
			bmp.UnlockBits(bitmapdata);
		}

		private string FindClosest(string name, int tolerance, out bool isBot) {
			//find closest name in the list
			int min = int.MaxValue;
			string closest = "";
			isBot = false;
			foreach(string player in playersList) {
				int dist = LevenshteinDistance(name, player);
				if(dist < min) {
					min = dist;
					closest = player;
				}
			}
			foreach(string bot in botsList) {
				int dist = LevenshteinDistance(name, bot);
				if(dist < min) {
					min = dist;
					closest = bot;
					isBot = true;
				}
			}
			if(min < tolerance) { return closest; }
			else { return "NUL"; }

		}
		private int LevenshteinDistance(string s, string t) {
			int n = s.Length;
			int m = t.Length;
			int[,] d = new int[n + 1, m + 1];

			if(n == 0) {
				return m;
			}

			if(m == 0) {
				return n;
			}

			for(int i = 0; i <= n; d[i, 0] = i++) {
			}

			for(int j = 0; j <= m; d[0, j] = j++) {
			}

			for(int i = 1; i <= n; i++) {
				for(int j = 1; j <= m; j++) {
					int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
					d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
				}
			}
			return d[n, m];
		}/*
		public void MicrosoftOcrScan(Bitmap bitmap, Rectangle rect, out string text) {
			text = "";

			var croppedBitmap = bitmap.Clone(rect, PixelFormat.DontCare);
			//save bitmap to %temp% folder
			croppedBitmap.Save(Path.Combine(Path.GetTempPath(), "cropped.png"));
			//load bitmap from %temp% folder to softwarebitmap
			SoftwareBitmap softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, (int)croppedBitmap.Width, (int)croppedBitmap.Height, BitmapAlphaMode.Premultiplied);

			
		}*/
		public List<Bitmap> cisla = new List<Bitmap>();
		public List<Bitmap> cislafinal = new List<Bitmap>();
		public bool cisla_nactena = false;
		public unsafe int GetPozici(Bitmap bitmapa, bool final) {
			//if(!cisla_nactena) {
			cisla.Clear();
			cislafinal.Clear();
			for(int i = 1; i <= 12; i++) {
				Bitmap bmp = new Bitmap(".\\nums\\" + i + ".png");
				ClipMonochrome(trackBar5.Value, bmp);
				cisla.Add(bmp);

				Bitmap bmp2 = new Bitmap(".\\numsalt\\" + i + ".png");
				ClipMonochrome(trackBar5.Value, bmp2);
				cislafinal.Add(bmp2);

			}
			cisla_nactena = true;
			//}
			ClipMonochrome(trackBar5.Value, bitmapa);
			int closestNum = -1;
			int matchedPixels = -1;
			List<Bitmap> d;
			if(final) {
				d = cislafinal;
			}
			else {
				d = cisla;
			}
			for(int i = 1; i <= d.Count; i++) {
				var cislo = d[i - 1];
				BitmapData data1 = cislo.LockBits(new Rectangle(0, 0, cislo.Width, cislo.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				BitmapData data2 = bitmapa.LockBits(new Rectangle(0, 0, bitmapa.Width, bitmapa.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

				int matchCount = 0;

				unsafe {
					byte* ptr1 = (byte*)data1.Scan0;
					byte* ptr2 = (byte*)data2.Scan0;

					for(int y = 0; y < cislo.Height; y++) {
						for(int x = 0; x < cislo.Width; x++) {
							// Compare pixel values (ARGB)
							if(*ptr1 == *ptr2 && *(ptr1 + 1) == *(ptr2 + 1) && *(ptr1 + 2) == *(ptr2 + 2) && *(ptr1 + 3) == *(ptr2 + 3)) {
								matchCount++;
							}

							// Move to next pixel
							ptr1 += 4;
							ptr2 += 4;
						}

						// Move to next row
						ptr1 += data1.Stride - cislo.Width * 4;
						ptr2 += data2.Stride - bitmapa.Width * 4;
					}
				}

				cislo.UnlockBits(data1);
				bitmapa.UnlockBits(data2);

				Console.WriteLine($"Bitmap number {i} has {matchedPixels} matched pixels");

				// Update the closest match
				if(matchCount > matchedPixels) {
					matchedPixels = matchCount;
					closestNum = i;
					Console.WriteLine($"Bitmap number {i} is closest");
				}
			}
			return closestNum;
		}

		private void trackBar_Scroll(object sender, EventArgs e) {
			var trackbar = (TrackBar)sender;
			//set tooltip of this trackbar to value of this trackbar
			if(trackbar != null) {
				toolTip1.SetToolTip(trackbar, trackbar.Value.ToString());
			}

		}

		private void button1_Click(object sender, EventArgs e) {
			System.Media.SystemSounds.Question.Play();
			var result = MessageBox.Show("Chcete stáhnout Tesseract model pro rozpoznávání textu?\nTahle operace zabere zhruba 10 MiB dat", "Stáhnout model", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if(result == DialogResult.Yes) {
				if(File.Exists("./tessdata/ces.traineddata")) {
					System.Media.SystemSounds.Asterisk.Play();
					MessageBox.Show("Data jsou již potřeba, není potřeba stáhnout", "JIŽ STAHNUTO", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				try {
					if(!Directory.Exists("./tessdata/")) {
						Directory.CreateDirectory("./tessdata/");
					}
					string model = "https://github.com/tesseract-ocr/tessdata_best/raw/main/ces.traineddata";
					using(var client = new WebClient()) {
						client.DownloadFile(model, "./tessdata/ces.traineddata");
					}
					System.Media.SystemSounds.Exclamation.Play();
					MessageBox.Show("Model bys uspěšně stažen", "STAHOVANI DOKONČENO", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch(Exception ex) {
					System.Media.SystemSounds.Asterisk.Play();
					MessageBox.Show("Error occured:\n" + ex.ToString(), "OKUREK 🥒", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		public List<VysledekLite> output = new List<VysledekLite>();
		public bool success = false;
		private void button2_Click(object sender, EventArgs e) {
			if(!validateInput() && !checkBox_skipValidation.Checked){
				return;
			}

			output.Clear();
			foreach(DataGridViewRow row in dataGridView1.Rows) {
				if(!row.IsNewRow) {
					VysledekLite vl = new VysledekLite();
					string pozice = row.Cells[1].Value.ToString(); // first column
					int.TryParse(pozice, out int poziceInt);
																 // fourth column
					string fourthColumnValue = row.Cells[4].Value.ToString();
					// fifth column
					var checkboxCell = row.Cells[5] as DataGridViewCheckBoxCell;
					bool isBot = (bool)checkboxCell.Value;
					
					vl.isBot = isBot;
					vl.zavodnik = fourthColumnValue;
					vl.pozice = poziceInt;

					output.Add(vl);
				}
			}
			success = true;
		}
		public bool validateInput() {
			//check if there are no empty cells in the position column
			foreach(DataGridViewRow row in dataGridView1.Rows) {
				if(!row.IsNewRow) {
					string pozice = row.Cells[1].Value.ToString();
					if(string.IsNullOrEmpty(pozice)) {
						SystemSounds.Asterisk.Play();
						MessageBox.Show("Pozice v řádku " + (row.Index + 1) + " je prázdná", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
			}
			//check if the numbers in the position column arent larger than the index of the row
			foreach(DataGridViewRow row in dataGridView1.Rows) {
				if(!row.IsNewRow) {
					string pozice = row.Cells[1].Value.ToString();
					int.TryParse(pozice, out int poziceInt);
					if(poziceInt > row.Index + 1) {
						SystemSounds.Asterisk.Play();
						MessageBox.Show("Pozice v řádku " + (row.Index + 1) + " je větší než index řádku", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
				}
			}
			//check if player isnt listed twice in the zavodnik column
			List<string> zavodnici = new List<string>();
			foreach(DataGridViewRow row in dataGridView1.Rows) {
				if(!row.IsNewRow) {
					string zavodnik = row.Cells[4].Value.ToString();
					if(zavodnici.Contains(zavodnik)) {
						SystemSounds.Asterisk.Play();
						MessageBox.Show("Závodník " + zavodnik + " je uveden vícekrát", "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return false;
					}
					zavodnici.Add(zavodnik);
				}
			}
			//check if there is column named NULL or N/A, and if there is, ask user if he wants to continue
			foreach(DataGridViewRow row in dataGridView1.Rows) {
				if(!row.IsNewRow) {
					string zavodnik = row.Cells[4].Value.ToString();
					if(zavodnik == "N/A" || zavodnik == "NUL") {
						SystemSounds.Question.Play();
						var result = MessageBox.Show("Závodník " + zavodnik + " je uveden v seznamu, chcete pokračovat?", "Varování", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
						if(result == DialogResult.No) {
							return false;
						}
					}
				}
			}
			return true;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
		public struct DISPLAY_DEVICE {
			public int cb;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string DeviceName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceString;
			public int StateFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceID;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string DeviceKey;
		}

		[DllImport("user32.dll", CharSet = CharSet.Ansi)]
		public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

		public static bool IsNvidiaAvailable() {
			DISPLAY_DEVICE d = new DISPLAY_DEVICE();
			d.cb = Marshal.SizeOf(d);
			uint devNum = 0;

			while(EnumDisplayDevices(null, devNum, ref d, 0)) {
				if(d.DeviceString.ToLower().Contains("nvidia")) {
					return true;
				}
				devNum++;
			}

			return false;
		}
	}
	public class VysledekLite {
		public int pozice;
		public string zavodnik;
		public bool isBot;
	}
}
