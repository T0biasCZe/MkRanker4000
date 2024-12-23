namespace MkRanker3000 {
	partial class OCR {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
			radioButton1 = new RadioButton();
			radioButton2 = new RadioButton();
			dataGridView1 = new DataGridView();
			label1 = new Label();
			textBox1 = new TextBox();
			trackBar1 = new TrackBar();
			trackBar2 = new TrackBar();
			trackBar4 = new TrackBar();
			trackBar3 = new TrackBar();
			label2 = new Label();
			label3 = new Label();
			toolTip1 = new ToolTip(components);
			label4 = new Label();
			label5 = new Label();
			button1 = new Button();
			button2 = new Button();
			trackBar5 = new TrackBar();
			label6 = new Label();
			label_cpuWarn = new Label();
			checkBox_skipValidation = new CheckBox();
			position_image = new DataGridViewImageColumn();
			position = new DataGridViewTextBoxColumn();
			player_image = new DataGridViewImageColumn();
			player = new DataGridViewTextBoxColumn();
			zavodnik_combobox = new DataGridViewTextBoxColumn();
			isBotColumn = new DataGridViewCheckBoxColumn();
			((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
			((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
			((System.ComponentModel.ISupportInitialize)trackBar2).BeginInit();
			((System.ComponentModel.ISupportInitialize)trackBar4).BeginInit();
			((System.ComponentModel.ISupportInitialize)trackBar3).BeginInit();
			((System.ComponentModel.ISupportInitialize)trackBar5).BeginInit();
			SuspendLayout();
			// 
			// radioButton1
			// 
			radioButton1.AutoSize = true;
			radioButton1.Checked = true;
			radioButton1.Location = new Point(24, 16);
			radioButton1.Name = "radioButton1";
			radioButton1.Size = new Size(109, 19);
			radioButton1.TabIndex = 0;
			radioButton1.TabStop = true;
			radioButton1.Text = "Normalni závod";
			radioButton1.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			radioButton2.AutoSize = true;
			radioButton2.Location = new Point(288, 16);
			radioButton2.Name = "radioButton2";
			radioButton2.Size = new Size(108, 19);
			radioButton2.TabIndex = 1;
			radioButton2.Text = "Finální výsledky";
			radioButton2.UseVisualStyleBackColor = true;
			// 
			// dataGridView1
			// 
			dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridView1.Columns.AddRange(new DataGridViewColumn[] { position_image, position, player_image, player, zavodnik_combobox, isBotColumn });
			dataGridView1.Location = new Point(16, 104);
			dataGridView1.Name = "dataGridView1";
			dataGridView1.RowHeadersWidth = 20;
			dataGridView1.Size = new Size(552, 392);
			dataGridView1.TabIndex = 2;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(16, 440);
			label1.Name = "label1";
			label1.Size = new Size(0, 15);
			label1.TabIndex = 3;
			// 
			// textBox1
			// 
			textBox1.Location = new Point(520, 16);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(48, 23);
			textBox1.TabIndex = 4;
			textBox1.Text = "ces";
			// 
			// trackBar1
			// 
			trackBar1.Location = new Point(16, 64);
			trackBar1.Maximum = 100;
			trackBar1.Name = "trackBar1";
			trackBar1.Size = new Size(104, 45);
			trackBar1.TabIndex = 5;
			trackBar1.TickFrequency = 10;
			trackBar1.Value = 35;
			trackBar1.Scroll += trackBar_Scroll;
			trackBar1.MouseHover += trackBar_Scroll;
			// 
			// trackBar2
			// 
			trackBar2.Location = new Point(120, 64);
			trackBar2.Maximum = 255;
			trackBar2.Name = "trackBar2";
			trackBar2.Size = new Size(104, 45);
			trackBar2.TabIndex = 6;
			trackBar2.TickFrequency = 26;
			trackBar2.Value = 25;
			trackBar2.Scroll += trackBar_Scroll;
			trackBar2.MouseHover += trackBar_Scroll;
			// 
			// trackBar4
			// 
			trackBar4.Location = new Point(384, 64);
			trackBar4.Maximum = 255;
			trackBar4.Name = "trackBar4";
			trackBar4.Size = new Size(104, 45);
			trackBar4.TabIndex = 8;
			trackBar4.TickFrequency = 26;
			trackBar4.Value = 25;
			trackBar4.Scroll += trackBar_Scroll;
			trackBar4.MouseHover += trackBar_Scroll;
			// 
			// trackBar3
			// 
			trackBar3.Location = new Point(280, 64);
			trackBar3.Maximum = 100;
			trackBar3.Name = "trackBar3";
			trackBar3.Size = new Size(104, 45);
			trackBar3.TabIndex = 7;
			trackBar3.TickFrequency = 10;
			trackBar3.Value = 35;
			trackBar3.Scroll += trackBar_Scroll;
			trackBar3.MouseHover += trackBar_Scroll;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(24, 48);
			label2.Name = "label2";
			label2.Size = new Size(51, 15);
			label2.TabIndex = 9;
			label2.Text = "Kontrast";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(128, 48);
			label3.Name = "label3";
			label3.Size = new Size(45, 15);
			label3.TabIndex = 10;
			label3.Text = "Min jas";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(392, 48);
			label4.Name = "label4";
			label4.Size = new Size(45, 15);
			label4.TabIndex = 12;
			label4.Text = "Min jas";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new Point(288, 48);
			label5.Name = "label5";
			label5.Size = new Size(51, 15);
			label5.TabIndex = 11;
			label5.Text = "Kontrast";
			// 
			// button1
			// 
			button1.Font = new Font("Wingdings", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
			button1.Location = new Point(488, 16);
			button1.Name = "button1";
			button1.Size = new Size(24, 32);
			button1.TabIndex = 13;
			button1.Text = "<";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// button2
			// 
			button2.Location = new Point(496, 504);
			button2.Name = "button2";
			button2.Size = new Size(75, 23);
			button2.TabIndex = 14;
			button2.Text = "Submit";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// trackBar5
			// 
			trackBar5.Location = new Point(488, 64);
			trackBar5.Maximum = 255;
			trackBar5.Name = "trackBar5";
			trackBar5.Size = new Size(80, 45);
			trackBar5.TabIndex = 15;
			trackBar5.TickFrequency = 25;
			trackBar5.Value = 80;
			trackBar5.Scroll += trackBar_Scroll;
			trackBar5.MouseHover += trackBar_Scroll;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new Point(496, 48);
			label6.Name = "label6";
			label6.Size = new Size(55, 15);
			label6.TabIndex = 16;
			label6.Text = "Cislo clip";
			// 
			// label_cpuWarn
			// 
			label_cpuWarn.AutoSize = true;
			label_cpuWarn.BackColor = Color.Transparent;
			label_cpuWarn.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point);
			label_cpuWarn.ForeColor = Color.Red;
			label_cpuWarn.Location = new Point(16, 496);
			label_cpuWarn.Name = "label_cpuWarn";
			label_cpuWarn.Size = new Size(255, 37);
			label_cpuWarn.TabIndex = 17;
			label_cpuWarn.Text = "RUNNING ON CPU";
			// 
			// checkBox_skipValidation
			// 
			checkBox_skipValidation.AutoSize = true;
			checkBox_skipValidation.Location = new Point(390, 507);
			checkBox_skipValidation.Name = "checkBox_skipValidation";
			checkBox_skipValidation.Size = new Size(102, 19);
			checkBox_skipValidation.TabIndex = 18;
			checkBox_skipValidation.Text = "skip validation";
			checkBox_skipValidation.UseVisualStyleBackColor = true;
			// 
			// position_image
			// 
			position_image.HeaderText = "Pozice";
			position_image.Name = "position_image";
			position_image.ReadOnly = true;
			position_image.Width = 50;
			// 
			// position
			// 
			position.HeaderText = "Pozice";
			position.Name = "position";
			position.Width = 50;
			// 
			// player_image
			// 
			player_image.HeaderText = "závodník";
			player_image.Name = "player_image";
			player_image.ReadOnly = true;
			player_image.Width = 180;
			// 
			// player
			// 
			player.HeaderText = "závodník auto";
			player.Name = "player";
			player.Visible = false;
			// 
			// zavodnik_combobox
			// 
			zavodnik_combobox.HeaderText = "Zavodnik match";
			zavodnik_combobox.Name = "zavodnik_combobox";
			zavodnik_combobox.Resizable = DataGridViewTriState.True;
			zavodnik_combobox.SortMode = DataGridViewColumnSortMode.NotSortable;
			zavodnik_combobox.Width = 110;
			// 
			// isBotColumn
			// 
			isBotColumn.HeaderText = "bot";
			isBotColumn.Name = "isBotColumn";
			isBotColumn.Width = 40;
			// 
			// OCR
			// 
			AllowDrop = true;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(577, 538);
			Controls.Add(checkBox_skipValidation);
			Controls.Add(label_cpuWarn);
			Controls.Add(label6);
			Controls.Add(dataGridView1);
			Controls.Add(trackBar5);
			Controls.Add(button2);
			Controls.Add(button1);
			Controls.Add(label4);
			Controls.Add(label5);
			Controls.Add(label3);
			Controls.Add(label2);
			Controls.Add(trackBar4);
			Controls.Add(trackBar3);
			Controls.Add(trackBar2);
			Controls.Add(trackBar1);
			Controls.Add(textBox1);
			Controls.Add(label1);
			Controls.Add(radioButton2);
			Controls.Add(radioButton1);
			Name = "OCR";
			Text = "OCR";
			DragDrop += OCR_DragDrop;
			DragEnter += OCR_DragEnter;
			((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
			((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
			((System.ComponentModel.ISupportInitialize)trackBar2).EndInit();
			((System.ComponentModel.ISupportInitialize)trackBar4).EndInit();
			((System.ComponentModel.ISupportInitialize)trackBar3).EndInit();
			((System.ComponentModel.ISupportInitialize)trackBar5).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private RadioButton radioButton1;
		private RadioButton radioButton2;
		private DataGridView dataGridView1;
		private Label label1;
		private TextBox textBox1;
		private TrackBar trackBar1;
		private TrackBar trackBar2;
		private TrackBar trackBar4;
		private TrackBar trackBar3;
		private Label label2;
		private Label label3;
		private ToolTip toolTip1;
		private Label label4;
		private Label label5;
		private Button button1;
		private DataGridViewComboBoxColumn zavodnik_fix;
		private Button button2;
		private TrackBar trackBar5;
		private Label label6;
		private Label label_cpuWarn;
		private CheckBox checkBox_skipValidation;
		private DataGridViewImageColumn position_image;
		private DataGridViewTextBoxColumn position;
		private DataGridViewImageColumn player_image;
		private DataGridViewTextBoxColumn player;
		private DataGridViewTextBoxColumn zavodnik_combobox;
		private DataGridViewCheckBoxColumn isBotColumn;
	}
}