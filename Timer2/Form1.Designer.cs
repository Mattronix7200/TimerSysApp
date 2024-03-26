using System.Diagnostics;

namespace Timer2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            tabPage2 = new TabPage();
            label18 = new Label();
            label14 = new Label();
            label16 = new Label();
            label12 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            textBox9 = new TextBox();
            textBox7 = new TextBox();
            button4 = new Button();
            dateTimePicker1 = new DateTimePicker();
            button3 = new Button();
            comboBox2 = new ComboBox();
            tabPage1 = new TabPage();
            label15 = new Label();
            label17 = new Label();
            label13 = new Label();
            label11 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            textBox5 = new TextBox();
            textBox4 = new TextBox();
            button2 = new Button();
            button1 = new Button();
            comboBox1 = new ComboBox();
            tabControl1 = new TabControl();
            tabPage3 = new TabPage();
            groupBox2 = new GroupBox();
            label1 = new Label();
            textBoxUser = new TextBox();
            groupBox1 = new GroupBox();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox3 = new CheckBox();
            checkBox4 = new CheckBox();
            button5 = new Button();
            tabPage4 = new TabPage();
            label10 = new Label();
            label9 = new Label();
            pictureBox4 = new PictureBox();
            pictureBox3 = new PictureBox();
            linkLabel1 = new LinkLabel();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            label8 = new Label();
            tabPage5 = new TabPage();
            textBoxInfo = new TextBox();
            pictureBox5 = new PictureBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            tabPage2.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage3.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            SuspendLayout();
            // 
            // tabPage2
            // 
            tabPage2.BackColor = SystemColors.ButtonHighlight;
            tabPage2.Controls.Add(label18);
            tabPage2.Controls.Add(label14);
            tabPage2.Controls.Add(label16);
            tabPage2.Controls.Add(label12);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(textBox9);
            tabPage2.Controls.Add(textBox7);
            tabPage2.Controls.Add(button4);
            tabPage2.Controls.Add(dateTimePicker1);
            tabPage2.Controls.Add(button3);
            tabPage2.Controls.Add(comboBox2);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(666, 353);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "🗓️Aktywator czasowy";
            // 
            // label18
            // 
            label18.BackColor = SystemColors.Info;
            label18.Cursor = Cursors.Help;
            label18.ForeColor = Color.FromArgb(192, 64, 0);
            label18.Location = new Point(380, 41);
            label18.Name = "label18";
            label18.Padding = new Padding(10);
            label18.Size = new Size(268, 160);
            label18.TabIndex = 23;
            label18.Text = "WSKAZÓWKA\r\n\r\nPo wybraniu tej opcji zostanie wykonane polecenie, które zdefiniujesz w ustawieniach, np: uruchomienie programu lub skryptu.\r\n";
            // 
            // label14
            // 
            label14.BackColor = SystemColors.Info;
            label14.Cursor = Cursors.Help;
            label14.ForeColor = Color.FromArgb(192, 64, 0);
            label14.Location = new Point(380, 41);
            label14.Name = "label14";
            label14.Padding = new Padding(10);
            label14.Size = new Size(268, 160);
            label14.TabIndex = 21;
            label14.Text = "WSKAZÓWKA\r\n\r\nAby zmienić dźwięk na własny, podmień go w katalogu \"sound\" w folderze aplikacji na plik \".wav\"";
            // 
            // label16
            // 
            label16.BackColor = SystemColors.Info;
            label16.Cursor = Cursors.Help;
            label16.ForeColor = Color.FromArgb(192, 64, 0);
            label16.Location = new Point(380, 41);
            label16.Name = "label16";
            label16.Padding = new Padding(10);
            label16.Size = new Size(268, 160);
            label16.TabIndex = 22;
            label16.Text = "WSKAZÓWKA\r\n\r\nUwaga! Wybranie tej opcji  w czasie krótszym niż 15 sekund lub/i wykonanie jej razem z włączoną opcją - Resetuj licznik (patrz Ustawienia), może spowodować blokadę ekranu.";
            // 
            // label12
            // 
            label12.BackColor = SystemColors.Control;
            label12.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 238);
            label12.Location = new Point(21, 267);
            label12.Name = "label12";
            label12.Padding = new Padding(10);
            label12.Size = new Size(627, 57);
            label12.TabIndex = 19;
            label12.Text = "Tutaj możesz zdefiniować oraz aktywować określone działanie w konkretnym czasie. Aby to zadziałało, należy wybrać datę oraz podać godznę. Bezczynność nie będzie monitorowana.";
            label12.TextAlign = ContentAlignment.MiddleCenter;
            label12.Click += label12_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = Color.DimGray;
            label7.Location = new Point(106, 141);
            label7.Name = "label7";
            label7.Size = new Size(209, 17);
            label7.TabIndex = 20;
            label7.Text = "Podaj godzinę w formacie HH:MM";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label6.Location = new Point(18, 84);
            label6.Name = "label6";
            label6.Size = new Size(231, 17);
            label6.TabIndex = 19;
            label6.Text = "Kiedy mam wykonać wybraną akcję?";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label5.Location = new Point(19, 18);
            label5.Name = "label5";
            label5.Size = new Size(193, 17);
            label5.TabIndex = 18;
            label5.Text = "Zdecyduj, co chciałbyś zrobić?";
            label5.Click += label5_Click;
            // 
            // textBox9
            // 
            textBox9.BackColor = SystemColors.ButtonHighlight;
            textBox9.BorderStyle = BorderStyle.None;
            textBox9.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 238);
            textBox9.ForeColor = SystemColors.MenuHighlight;
            textBox9.Location = new Point(19, 225);
            textBox9.Multiline = true;
            textBox9.Name = "textBox9";
            textBox9.ReadOnly = true;
            textBox9.Size = new Size(629, 63);
            textBox9.TabIndex = 15;
            textBox9.TextAlign = HorizontalAlignment.Center;
            textBox9.TextChanged += textBox9_TextChanged;
            // 
            // textBox7
            // 
            textBox7.BorderStyle = BorderStyle.FixedSingle;
            textBox7.Location = new Point(20, 139);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(79, 25);
            textBox7.TabIndex = 14;
            textBox7.TextChanged += textBox7_TextChanged;
            // 
            // button4
            // 
            button4.Location = new Point(136, 182);
            button4.Name = "button4";
            button4.Size = new Size(99, 26);
            button4.TabIndex = 15;
            button4.Text = "Zatrzymaj";
            button4.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(20, 108);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(295, 25);
            dateTimePicker1.TabIndex = 15;
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;
            // 
            // button3
            // 
            button3.Location = new Point(20, 182);
            button3.Name = "button3";
            button3.Size = new Size(99, 26);
            button3.TabIndex = 14;
            button3.Text = "Uruchom";
            button3.UseVisualStyleBackColor = true;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(20, 41);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(343, 25);
            comboBox2.TabIndex = 11;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.ButtonHighlight;
            tabPage1.Controls.Add(label15);
            tabPage1.Controls.Add(label17);
            tabPage1.Controls.Add(label13);
            tabPage1.Controls.Add(label11);
            tabPage1.Controls.Add(label4);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(textBox5);
            tabPage1.Controls.Add(textBox4);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(comboBox1);
            tabPage1.Location = new Point(4, 26);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(666, 351);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "⏰Aktywator bezczynności";
            // 
            // label15
            // 
            label15.BackColor = SystemColors.Info;
            label15.Cursor = Cursors.Help;
            label15.ForeColor = Color.FromArgb(192, 64, 0);
            label15.Location = new Point(379, 41);
            label15.Name = "label15";
            label15.Padding = new Padding(10);
            label15.Size = new Size(268, 160);
            label15.TabIndex = 20;
            label15.Text = "WSKAZÓWKA\r\n\r\nAby zmienić dźwięk na własny, podmień go w katalogu \"sound\" w folderze aplikacji na plik \".wav\"";
            // 
            // label17
            // 
            label17.BackColor = SystemColors.Info;
            label17.Cursor = Cursors.Help;
            label17.ForeColor = Color.FromArgb(192, 64, 0);
            label17.Location = new Point(380, 41);
            label17.Name = "label17";
            label17.Padding = new Padding(10);
            label17.Size = new Size(268, 160);
            label17.TabIndex = 22;
            label17.Text = "WSKAZÓWKA\r\n\r\nPo wybraniu tej opcji zostanie wykonane polecenie, które zdefiniujesz w ustawieniach, np: uruchomienie programu lub skryptu.";
            // 
            // label13
            // 
            label13.BackColor = SystemColors.Info;
            label13.Cursor = Cursors.Help;
            label13.ForeColor = Color.FromArgb(192, 64, 0);
            label13.Location = new Point(379, 41);
            label13.Name = "label13";
            label13.Padding = new Padding(10);
            label13.Size = new Size(268, 160);
            label13.TabIndex = 19;
            label13.Text = "WSKAZÓWKA\r\n\r\nUwaga! Wybranie tej opcji  w czasie krótszym niż 15 sekund lub/i wykonanie jej razem z włączoną opcją - Resetuj licznik (patrz Ustawienia), może spowodować blokadę ekranu.";
            label13.Click += label13_Click_1;
            // 
            // label11
            // 
            label11.BackColor = SystemColors.Control;
            label11.Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point, 238);
            label11.Location = new Point(20, 265);
            label11.Name = "label11";
            label11.Padding = new Padding(10);
            label11.Size = new Size(627, 57);
            label11.TabIndex = 18;
            label11.Text = "Za pomocą powyższych opcji możesz zdefiniować wykonanie akcji po upływie określonej ilośći czasu bezczynności systemu (braku aktywności użytkownika)";
            label11.TextAlign = ContentAlignment.MiddleCenter;
            label11.Click += label11_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.DimGray;
            label4.Location = new Point(20, 142);
            label4.Name = "label4";
            label4.Size = new Size(204, 17);
            label4.TabIndex = 17;
            label4.Text = "Podaj czas w formacie HH:MM:SS";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label3.Location = new Point(20, 85);
            label3.Name = "label3";
            label3.Size = new Size(231, 17);
            label3.TabIndex = 16;
            label3.Text = "Za ile mam wykonać wybraną akcję?";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label2.Location = new Point(20, 17);
            label2.Name = "label2";
            label2.Size = new Size(193, 17);
            label2.TabIndex = 15;
            label2.Text = "Zdecyduj, co chciałbyś zrobić?";
            label2.Click += label2_Click;
            // 
            // textBox5
            // 
            textBox5.BackColor = SystemColors.ButtonHighlight;
            textBox5.BorderStyle = BorderStyle.None;
            textBox5.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 238);
            textBox5.ForeColor = SystemColors.MenuHighlight;
            textBox5.Location = new Point(20, 221);
            textBox5.Multiline = true;
            textBox5.Name = "textBox5";
            textBox5.ReadOnly = true;
            textBox5.Size = new Size(627, 56);
            textBox5.TabIndex = 14;
            textBox5.TextAlign = HorizontalAlignment.Center;
            textBox5.TextChanged += textBox5_TextChanged;
            // 
            // textBox4
            // 
            textBox4.BorderStyle = BorderStyle.FixedSingle;
            textBox4.Location = new Point(20, 109);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(343, 25);
            textBox4.TabIndex = 12;
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // button2
            // 
            button2.Location = new Point(138, 168);
            button2.Name = "button2";
            button2.Size = new Size(99, 26);
            button2.TabIndex = 13;
            button2.Text = "Zatrzymaj";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(20, 168);
            button1.Name = "button1";
            button1.Size = new Size(99, 26);
            button1.TabIndex = 2;
            button1.Text = "Uruchom";
            button1.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(20, 41);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(343, 25);
            comboBox1.TabIndex = 0;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Location = new Point(28, 143);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(674, 381);
            tabControl1.TabIndex = 8;
            // 
            // tabPage3
            // 
            tabPage3.BackColor = SystemColors.ButtonHighlight;
            tabPage3.Controls.Add(groupBox2);
            tabPage3.Controls.Add(groupBox1);
            tabPage3.Controls.Add(button5);
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(666, 351);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "🛠️Ustawienia";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(textBoxUser);
            groupBox2.Location = new Point(20, 191);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(628, 93);
            groupBox2.TabIndex = 8;
            groupBox2.TabStop = false;
            groupBox2.Text = "Zdefiniowane akcje";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 62);
            label1.Name = "label1";
            label1.Size = new Size(478, 17);
            label1.TabIndex = 8;
            label1.Text = "Powyżej wpisz polecenie, które ma zadziałać z działaniem - \"Uruchom polecenie\"";
            label1.Click += label1_Click_1;
            // 
            // textBoxUser
            // 
            textBoxUser.BorderStyle = BorderStyle.FixedSingle;
            textBoxUser.Location = new Point(17, 24);
            textBoxUser.Name = "textBoxUser";
            textBoxUser.Size = new Size(592, 25);
            textBoxUser.TabIndex = 7;
            textBoxUser.TextChanged += textBoxUser_TextChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBox1);
            groupBox1.Controls.Add(checkBox2);
            groupBox1.Controls.Add(checkBox3);
            groupBox1.Controls.Add(checkBox4);
            groupBox1.Location = new Point(20, 14);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(628, 171);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Ogólne";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(6, 24);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(394, 21);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Automatycznie uruchamiaj program wraz z systemem Windows";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(6, 51);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(584, 21);
            checkBox2.TabIndex = 1;
            checkBox2.Text = "Automatycznie uruchamiaj odliczanie po uruchomieniu programu (tylko aktywator bezczynności)";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(6, 78);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(349, 21);
            checkBox3.TabIndex = 3;
            checkBox3.Text = "Minimalizuj do paska zadań, zamiast zamykać program";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Location = new Point(6, 105);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(439, 21);
            checkBox4.TabIndex = 2;
            checkBox4.Text = "Włącz automatyczne resetowanie licznika (tylko akywator bezczynności)";
            checkBox4.UseVisualStyleBackColor = true;
            checkBox4.CheckedChanged += checkBox4_CheckedChanged;
            // 
            // button5
            // 
            button5.Location = new Point(20, 301);
            button5.Name = "button5";
            button5.Size = new Size(628, 28);
            button5.TabIndex = 5;
            button5.Text = "Edytuj plik konfiguracyjny (zaawansowane)";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // tabPage4
            // 
            tabPage4.BackColor = SystemColors.ButtonHighlight;
            tabPage4.Controls.Add(label10);
            tabPage4.Controls.Add(label9);
            tabPage4.Controls.Add(pictureBox4);
            tabPage4.Controls.Add(pictureBox3);
            tabPage4.Controls.Add(linkLabel1);
            tabPage4.Controls.Add(pictureBox2);
            tabPage4.Controls.Add(pictureBox1);
            tabPage4.Controls.Add(label8);
            tabPage4.Location = new Point(4, 26);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(666, 351);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "ℹ️ Autorzy";
            // 
            // label10
            // 
            label10.BackColor = Color.Transparent;
            label10.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 238);
            label10.Location = new Point(120, 244);
            label10.Name = "label10";
            label10.Size = new Size(527, 72);
            label10.TabIndex = 16;
            label10.Text = resources.GetString("label10.Text");
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.Transparent;
            label9.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 238);
            label9.Location = new Point(120, 118);
            label9.Name = "label9";
            label9.Size = new Size(314, 60);
            label9.TabIndex = 15;
            label9.Text = "Program wykorzystuje następujące technologie: \r\n\r\n.NET Framework w wersji 8.0 firmy Microsoft Corporation, \r\nZestaw ikon Flaticon- https://www.flaticon.com/";
            label9.Click += label9_Click;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.InitialImage = (Image)resources.GetObject("pictureBox4.InitialImage");
            pictureBox4.Location = new Point(118, 231);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(530, 1);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 13;
            pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.InitialImage = (Image)resources.GetObject("pictureBox3.InitialImage");
            pictureBox3.Location = new Point(118, 100);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(530, 1);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 12;
            pictureBox3.TabStop = false;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 238);
            linkLabel1.Location = new Point(196, 60);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(131, 15);
            linkLabel1.TabIndex = 10;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://windowsbase.pl";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.InitialImage = (Image)resources.GetObject("pictureBox2.InitialImage");
            pictureBox2.Location = new Point(25, 15);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(64, 64);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 11;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(14, 244);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(88, 31);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 10;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // label8
            // 
            label8.BackColor = Color.Transparent;
            label8.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 238);
            label8.Location = new Point(118, 15);
            label8.Name = "label8";
            label8.Size = new Size(530, 70);
            label8.TabIndex = 14;
            label8.Text = "Program został zaprojektowany i stworzony przez Jakub Michalski dla strony WindowsBASE.pl. \r\nCopyright 2024.\r\n\r\nStrona autora: ";
            // 
            // tabPage5
            // 
            tabPage5.BackColor = SystemColors.Window;
            tabPage5.Controls.Add(textBoxInfo);
            tabPage5.Location = new Point(4, 26);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(666, 351);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "💿 O programie...";
            // 
            // textBoxInfo
            // 
            textBoxInfo.BackColor = SystemColors.Window;
            textBoxInfo.BorderStyle = BorderStyle.None;
            textBoxInfo.Font = new Font("Ebrima", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 238);
            textBoxInfo.Location = new Point(20, 16);
            textBoxInfo.Multiline = true;
            textBoxInfo.Name = "textBoxInfo";
            textBoxInfo.ReadOnly = true;
            textBoxInfo.Size = new Size(628, 316);
            textBoxInfo.TabIndex = 18;
            textBoxInfo.Text = resources.GetString("textBoxInfo.Text");
            textBoxInfo.TextChanged += textBox1_TextChanged_2;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = (Image)resources.GetObject("pictureBox5.Image");
            pictureBox5.InitialImage = (Image)resources.GetObject("pictureBox5.InitialImage");
            pictureBox5.Location = new Point(82, 24);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(562, 97);
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.TabIndex = 16;
            pictureBox5.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            contextMenuStrip1.Opening += contextMenuStrip1_Opening;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(732, 552);
            Controls.Add(pictureBox5);
            Controls.Add(tabControl1);
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "Wyłącznik czasowy - v.1.0.2";
            Load += Form1_Load;
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private TabPage tabPage2;
        private TextBox textBox9;
        private TextBox textBox7;
        private Button button4;
        private DateTimePicker dateTimePicker1;
        private Button button3;
        private ComboBox comboBox2;
        private TabPage tabPage1;
        private TextBox textBox5;
        private TextBox textBox4;
        private Button button2;
        private Button button1;
        private ComboBox comboBox1;
        private TabControl tabControl1;
        private TabPage tabPage3;
        private TabPage tabPage4;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private LinkLabel linkLabel1;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private Label label2;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label7;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label11;
        private PictureBox pictureBox5;
        private Label label12;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private Button button5;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private TextBox textBoxUser;
        private ContextMenuStrip contextMenuStrip1;
        private TabPage tabPage5;
        private TextBox textBoxInfo;
        private Label label13;
        private Label label15;
        private Label label14;
        private Label label16;
        private Label label17;
        private Label label18;
    }
}

