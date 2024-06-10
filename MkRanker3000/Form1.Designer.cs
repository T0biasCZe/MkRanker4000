namespace MKRanker3000 {
	partial class Form1 {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			listBox_e = new ListBox();
			numericUpDown_pozice = new NumericUpDown();
			button_submitZaznam = new Button();
			button_submitRace = new Button();
			button1_finishCup = new Button();
			button_newCup = new Button();
			label_zavodCount = new Label();
			label_final = new Label();
			label_cupCount = new Label();
			numericUpDown_cc = new NumericUpDown();
			checkBox_mirror = new CheckBox();
			comboBox_trat = new ComboBox();
			textBox_pridatZavodnika = new TextBox();
			comboBox_vybiracTrate = new ComboBox();
			button1 = new Button();
			checkBox_isBot = new CheckBox();
			listBox1 = new ListBox();
			((System.ComponentModel.ISupportInitialize)numericUpDown_pozice).BeginInit();
			((System.ComponentModel.ISupportInitialize)numericUpDown_cc).BeginInit();
			SuspendLayout();
			// 
			// listBox_e
			// 
			listBox_e.FormattingEnabled = true;
			listBox_e.ItemHeight = 15;
			listBox_e.Location = new Point(376, 16);
			listBox_e.Name = "listBox_e";
			listBox_e.Size = new Size(528, 499);
			listBox_e.TabIndex = 0;
			// 
			// numericUpDown_pozice
			// 
			numericUpDown_pozice.Location = new Point(24, 40);
			numericUpDown_pozice.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
			numericUpDown_pozice.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			numericUpDown_pozice.Name = "numericUpDown_pozice";
			numericUpDown_pozice.Size = new Size(160, 23);
			numericUpDown_pozice.TabIndex = 2;
			numericUpDown_pozice.Value = new decimal(new int[] { 1, 0, 0, 0 });
			// 
			// button_submitZaznam
			// 
			button_submitZaznam.Location = new Point(48, 144);
			button_submitZaznam.Name = "button_submitZaznam";
			button_submitZaznam.Size = new Size(120, 23);
			button_submitZaznam.TabIndex = 4;
			button_submitZaznam.Text = "přidat zaznam";
			button_submitZaznam.UseVisualStyleBackColor = true;
			button_submitZaznam.Click += button_submitZaznam_Click;
			// 
			// button_submitRace
			// 
			button_submitRace.Location = new Point(48, 176);
			button_submitRace.Name = "button_submitRace";
			button_submitRace.Size = new Size(120, 23);
			button_submitRace.TabIndex = 5;
			button_submitRace.Text = "dokoncit zavod";
			button_submitRace.UseVisualStyleBackColor = true;
			button_submitRace.Click += button_submitRace_Click;
			// 
			// button1_finishCup
			// 
			button1_finishCup.Location = new Point(48, 208);
			button1_finishCup.Name = "button1_finishCup";
			button1_finishCup.Size = new Size(120, 23);
			button1_finishCup.TabIndex = 6;
			button1_finishCup.Text = "dokoncit cup";
			button1_finishCup.UseVisualStyleBackColor = true;
			button1_finishCup.Click += button1_finishCup_Click;
			// 
			// button_newCup
			// 
			button_newCup.Location = new Point(48, 312);
			button_newCup.Name = "button_newCup";
			button_newCup.Size = new Size(120, 23);
			button_newCup.TabIndex = 7;
			button_newCup.Text = "novy cup";
			button_newCup.UseVisualStyleBackColor = true;
			button_newCup.Click += button_newCup_Click;
			// 
			// label_zavodCount
			// 
			label_zavodCount.AutoSize = true;
			label_zavodCount.Location = new Point(16, 8);
			label_zavodCount.Name = "label_zavodCount";
			label_zavodCount.Size = new Size(85, 15);
			label_zavodCount.TabIndex = 8;
			label_zavodCount.Text = "zavod cislo: ##";
			// 
			// label_final
			// 
			label_final.AutoSize = true;
			label_final.Enabled = false;
			label_final.ForeColor = Color.Red;
			label_final.Location = new Point(208, 8);
			label_final.Name = "label_final";
			label_final.Size = new Size(147, 15);
			label_final.TabIndex = 9;
			label_final.Text = "zadejte finalni pozice cupu";
			// 
			// label_cupCount
			// 
			label_cupCount.AutoSize = true;
			label_cupCount.Location = new Point(112, 8);
			label_cupCount.Name = "label_cupCount";
			label_cupCount.Size = new Size(74, 15);
			label_cupCount.TabIndex = 10;
			label_cupCount.Text = "cup cislo: ##";
			// 
			// numericUpDown_cc
			// 
			numericUpDown_cc.Increment = new decimal(new int[] { 50, 0, 0, 0 });
			numericUpDown_cc.Location = new Point(48, 280);
			numericUpDown_cc.Maximum = new decimal(new int[] { 200, 0, 0, 0 });
			numericUpDown_cc.Minimum = new decimal(new int[] { 50, 0, 0, 0 });
			numericUpDown_cc.Name = "numericUpDown_cc";
			numericUpDown_cc.Size = new Size(120, 23);
			numericUpDown_cc.TabIndex = 11;
			numericUpDown_cc.Value = new decimal(new int[] { 150, 0, 0, 0 });
			// 
			// checkBox_mirror
			// 
			checkBox_mirror.AutoSize = true;
			checkBox_mirror.Location = new Point(48, 256);
			checkBox_mirror.Name = "checkBox_mirror";
			checkBox_mirror.Size = new Size(98, 19);
			checkBox_mirror.TabIndex = 12;
			checkBox_mirror.Text = "mirror zapnut";
			checkBox_mirror.UseVisualStyleBackColor = true;
			// 
			// comboBox_trat
			// 
			comboBox_trat.AutoCompleteMode = AutoCompleteMode.Suggest;
			comboBox_trat.AutoCompleteSource = AutoCompleteSource.ListItems;
			comboBox_trat.FormattingEnabled = true;
			comboBox_trat.Items.AddRange(new object[] { "Amsterdam Drift", "Animal Crossing", "Athens Dash", "Baby Park", "Bangkok Rush", "Berlin Byways", "Big Blue", "Bone-Dry Dunes", "Boo Lake", "Bowser Castle 3 (SNES)", "Bowser's Castle (Wii U)", "Cheep Cheep Beach", "Cheese Land", "Choco Mountain", "City Tracks", "Cloudtop Cruise", "Coconut Mall", "Daisy Circuit", "Daisy Cruiser", "DK Jungle", "DK Mountain", "DK Summit", "Dolphin Shoals", "Donut Plains 3", "Dragon Driftway", "Dry Dry Desert", "Electrodrome", "Excitebike Arena", "Grumble Volcano", "Hyrule Circuit", "Ice Ice Outpost", "Kalimari Desert", "Koopa Cape", "London Loop", "Los Angeles Laps", "Madrid Drive", "Maple Treeway", "Mario Circuit (DS)", "Mario Circuit (GBA)", "Mario Circuit (Wii U)", "Mario Circuit 3", "Mario Kart Stadium", "Merry Mountain", "Moo Moo Meadows", "Moonview Highway", "Mount Wario", "Mushroom Gorge", "Music Park", "Mute City", "Neo Bowser City", "New York Minute", "Ninja Hideaway", "Paris Promenade", "Peach Gardens", "Piranha Plant Cove", "Piranha Plant Slide", "Rainbow Road (3DS)", "Rainbow Road (N64)", "Rainbow Road (SNES)", "Rainbow Road (Wii U)", "Rainbow Road (Wii)", "Ribbon Road", "Riverside Park", "Rock Rock Mountain", "Rome Avanti", "Rosalina's Ice World", "Royal Raceway", "Sherbet Land (GCN)", "Shroom Ridge", "Shy Guy Falls", "Singapore Speedway", "Sky Garden", "Sky-High Sundae", "Snow Land", "Squeaky Clean Sprint", "Sunset Wilds", "Sunshine Airport", "Super Bell Subway", "Sweet Sweet Canyon", "Sydney Sprint", "Thwomp Ruins", "Tick-Tock Clock", "Toad Circuit", "Toad Harbor", "Toad's Turnpike", "Tokyo Blur", "Twisted Mansion", "Vancouver Velocity", "Waluigi Pinball", "Waluigi Stadium (GCN)", "Wario Stadium (DS)", "Wario's Gold Mine", "Water Park", "Wild Woods", "Yoshi Circuit", "Yoshi Valley", "Yoshi's Island" });
			comboBox_trat.Location = new Point(24, 80);
			comboBox_trat.Name = "comboBox_trat";
			comboBox_trat.Size = new Size(160, 23);
			comboBox_trat.TabIndex = 13;
			// 
			// textBox_pridatZavodnika
			// 
			textBox_pridatZavodnika.Location = new Point(248, 385);
			textBox_pridatZavodnika.Name = "textBox_pridatZavodnika";
			textBox_pridatZavodnika.PlaceholderText = "Přidat závodnika";
			textBox_pridatZavodnika.Size = new Size(112, 23);
			textBox_pridatZavodnika.TabIndex = 15;
			textBox_pridatZavodnika.KeyPress += textBox_pridatZavodnika_KeyPress;
			// 
			// comboBox_vybiracTrate
			// 
			comboBox_vybiracTrate.FormattingEnabled = true;
			comboBox_vybiracTrate.Location = new Point(24, 112);
			comboBox_vybiracTrate.Name = "comboBox_vybiracTrate";
			comboBox_vybiracTrate.Size = new Size(160, 23);
			comboBox_vybiracTrate.TabIndex = 16;
			// 
			// button1
			// 
			button1.Font = new Font("Comic Sans MS", 11.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
			button1.Location = new Point(48, 376);
			button1.Name = "button1";
			button1.Size = new Size(120, 48);
			button1.TabIndex = 17;
			button1.Text = "OCR MAGIC";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// checkBox_isBot
			// 
			checkBox_isBot.AutoSize = true;
			checkBox_isBot.Location = new Point(200, 388);
			checkBox_isBot.Name = "checkBox_isBot";
			checkBox_isBot.Size = new Size(44, 19);
			checkBox_isBot.TabIndex = 18;
			checkBox_isBot.Text = "bot";
			checkBox_isBot.UseVisualStyleBackColor = true;
			// 
			// listBox1
			// 
			listBox1.FormattingEnabled = true;
			listBox1.ItemHeight = 15;
			listBox1.Location = new Point(200, 32);
			listBox1.Name = "listBox1";
			listBox1.Size = new Size(160, 349);
			listBox1.TabIndex = 14;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(919, 529);
			Controls.Add(checkBox_isBot);
			Controls.Add(button1);
			Controls.Add(comboBox_vybiracTrate);
			Controls.Add(textBox_pridatZavodnika);
			Controls.Add(listBox1);
			Controls.Add(comboBox_trat);
			Controls.Add(checkBox_mirror);
			Controls.Add(numericUpDown_cc);
			Controls.Add(label_cupCount);
			Controls.Add(label_final);
			Controls.Add(label_zavodCount);
			Controls.Add(button_newCup);
			Controls.Add(button1_finishCup);
			Controls.Add(button_submitRace);
			Controls.Add(button_submitZaznam);
			Controls.Add(numericUpDown_pozice);
			Controls.Add(listBox_e);
			Name = "Form1";
			Text = "Form1";
			Resize += Form1_Resize;
			((System.ComponentModel.ISupportInitialize)numericUpDown_pozice).EndInit();
			((System.ComponentModel.ISupportInitialize)numericUpDown_cc).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ListBox listBox_e;
		private NumericUpDown numericUpDown_pozice;
		private Button button_submitZaznam;
		private Button button_submitRace;
		private Button button1_finishCup;
		private Button button_newCup;
		private Label label_zavodCount;
		private Label label_final;
		private Label label_cupCount;
		private NumericUpDown numericUpDown_cc;
		private CheckBox checkBox_mirror;
		private ComboBox comboBox_trat;
		private TextBox textBox_pridatZavodnika;
		private ComboBox comboBox_vybiracTrate;
		private Button button1;
		private CheckBox checkBox_isBot;
		private ListBox listBox1;
	}
}
