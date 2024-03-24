namespace Timer2
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using static System.Windows.Forms.DataFormats;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;
    using TextBox = TextBox;
    using System.IO;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Xml;
    using Microsoft.Win32;

    public partial class Form1 : Form
    {

        private Timer timer1 = new Timer();
        private Timer timer2 = new Timer();
        private DateTime timer1EndTime;
        private DateTime timer2EndTime;
        private string configFilePath = "config.ini";
        private NameValueCollection appSettings;


        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(new string[] { "Uruchom polecenie", "Wy³¹cz monitor", "Uœpienie", "Wylogowanie", "Restart", "Wy³¹czenie komputera", "Hibernacja", "Blokada u¿ytkownika" });
            comboBox2.Items.AddRange(new string[] { "Uruchom polecenie", "Wy³¹cz monitor", "Uœpienie", "Wylogowanie", "Restart", "Wy³¹czenie komputera", "Hibernacja", "Blokada u¿ytkownika" });
            timer1.Interval = 1000; // co sekundê
            timer1.Tick += Timer1_Tick;
            timer2.Interval = 1000; // co sekundê
            timer2.Tick += Timer2_Tick;
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button4.Click += button4_Click;
            button5.Click += button5_Click;
            linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            LoadConfigFile();
            notifyIcon1.Icon = SystemIcons.Application;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = new Icon("./icons/clock.ico");
            textBoxInfo.ScrollBars = ScrollBars.Vertical;
        }

        private void LoadConfigFile()
        {
            if (!File.Exists(configFilePath))
            {
                // Utwórz plik konfiguracyjny, jeœli nie istnieje
                using (XmlWriter writer = XmlWriter.Create(configFilePath))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("configuration");
                    writer.WriteStartElement("appSettings");

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "defaultAction1");
                    writer.WriteAttributeString("value", "0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "defaultAction2");
                    writer.WriteAttributeString("value", "0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "timer1ActivateBy");
                    writer.WriteAttributeString("value", "00:05:00");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "timer1EnabledAuto");
                    writer.WriteAttributeString("value", "0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "timer2HourActivateBy");
                    writer.WriteAttributeString("value", "10:00");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "timer2DateActivateBy");
                    writer.WriteAttributeString("value", "01.01.2024");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "userSet");
                    writer.WriteAttributeString("value", "mspaint.exe");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "runWindowsAuto");
                    writer.WriteAttributeString("value", "0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "autoStart");
                    writer.WriteAttributeString("value", "0");

                    writer.WriteEndElement();
                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "noMinimalize");
                    writer.WriteAttributeString("value", "0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "RunHost");
                    writer.WriteAttributeString("value", "0");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }

            // Wczytaj plik konfiguracyjny
            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            var dict = config.AppSettings.Settings.AllKeys.ToDictionary(key => key, key => config.AppSettings.Settings[key].Value);

            appSettings = new NameValueCollection();
            foreach (var kvp in dict)
            {
                appSettings.Add(kvp.Key, kvp.Value);
            }

            // Ustaw domyœlne wartoœci na podstawie wczytanych ustawieñ
            comboBox1.SelectedIndex = int.Parse(appSettings["defaultAction1"]);
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndex = int.Parse(appSettings["defaultAction2"]);
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;

            textBox4.Text = appSettings["timer1ActivateBy"];
            textBox4.TextChanged += textBox4_TextChanged;
            textBox7.Text = appSettings["timer2HourActivateBy"];
            textBox7.TextChanged += textBox7_TextChanged;
            textBoxUser.Text = appSettings["userSet"];
            textBoxUser.TextChanged += textBoxUser_TextChanged;

            //data- ustawienia 
            DateTime settingsDate = DateTime.Parse(appSettings["timer2DateActivateBy"]);
            DateTime currentDate = DateTime.Now;
            dateTimePicker1.MinDate = settingsDate < currentDate ? settingsDate : currentDate;
            dateTimePicker1.Value = settingsDate;
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;

            // opcje- ustawienia
            checkBox1.Checked = appSettings["runWindowsAuto"] == "1";
            checkBox2.Checked = appSettings["autoStart"] == "1";
            checkBox3.Checked = appSettings["noMinimalize"] == "1";
            checkBox4.Checked = appSettings["RunHost"] == "1";

        }


        private void SaveConfigFile()
        {
            // Zapisz zmiany do pliku konfiguracyjnego
            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            foreach (var key in appSettings.AllKeys)
            {
                if (config.AppSettings.Settings[key] == null)
                {
                    config.AppSettings.Settings.Add(key, appSettings[key]);
                }
                else
                {
                    config.AppSettings.Settings[key].Value = appSettings[key];
                }
            }

            config.Save(ConfigurationSaveMode.Modified);
        }

        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public uint dwTime;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = LASTINPUTINFO.SizeOf;
            GetLastInputInfo(ref lastInputInfo);

            long idleTime = Environment.TickCount - lastInputInfo.dwTime;

            if (idleTime < 1000) // Mniej ni¿ sekunda bezczynnoœci
            {
                timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(textBox4.Text));
            }

            var remainingTime = timer1EndTime - DateTime.Now;
            if (remainingTime.TotalSeconds <= 0)
            {
                timer1.Stop();
                ExecuteCommand(comboBox1.SelectedItem.ToString());
                textBox5.Text = "00:00:00";
                tabPage2.Enabled = true;
            }
            else
            {
                textBox5.Text = "Pozosta³y czas: " + remainingTime.ToString(@"hh\:mm\:ss");
            }
        }


        private void Timer2_Tick(object sender, EventArgs e)
        {
            var remainingTime = timer2EndTime - DateTime.Now;
            if (remainingTime.TotalSeconds <= 0)
            {
                timer2.Stop();
                ExecuteCommand(comboBox2.SelectedItem.ToString());
                textBox9.Text = "00:00";
                tabPage1.Enabled = true;
            }
            else
            {
                if (remainingTime.TotalSeconds < 60)
                {
                    textBox9.Text = "Pozosta³y czas: " + remainingTime.ToString(@"ss") + " sekund";
                }
                else if (remainingTime.TotalMinutes < 60)
                {
                    textBox9.Text = "Pozosta³y czas: " + remainingTime.ToString(@"mm") + " minut";
                }
                else if (remainingTime.TotalHours < 24)
                {
                    textBox9.Text = "Pozosta³y czas: " + remainingTime.ToString(@"hh") + " godzin";
                }
                else
                {
                    textBox9.Text = "Pozosta³y czas: " + remainingTime.ToString(@"%d") + " dni";
                }
            }
        }

        private void Timer1_Tick_Reset(object sender, EventArgs e)
        {
            tabPage2.Enabled = false;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = LASTINPUTINFO.SizeOf;
            GetLastInputInfo(ref lastInputInfo);

            long idleTime = Environment.TickCount - lastInputInfo.dwTime;

            if (idleTime < 1000)
            {
                timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(textBox4.Text));
            }

            var remainingTime = timer1EndTime - DateTime.Now;
            if (remainingTime.TotalSeconds <= 0)
            {
                ExecuteCommand(comboBox1.SelectedItem.ToString());
                timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(textBox4.Text));
            }
            else
            {
                textBox5.Text = remainingTime.ToString(@"hh\:mm\:ss");
            }
        }


        private void ExecuteCommand(string command)
        {
            var userCommand = textBoxUser.Text;

            switch (command)
            {
                case "Uruchom polecenie":
                    Process.Start(userCommand);
                    break;
                case "Wy³¹cz monitor":
                    Process.Start("nircmd.exe", "monitor off");
                    break;
                case "Uœpienie":
                    Process.Start("nircmd.exe", "standby");
                    break;
                case "Wylogowanie":
                    Process.Start("nircmd.exe", "exitwin logoff");
                    break;
                case "Restart":
                    Process.Start("nircmd.exe", "exitwin reboot");
                    break;
                case "Wy³¹czenie komputera":
                    Process.Start("nircmd.exe", "exitwin poweroff");
                    break;
                case "Hibernacja":
                    Process.Start("shutdown", "/h");
                    break;
                case "Blokada u¿ytkownika":
                    Process.Start("Rundll32.exe", "user32.dll,LockWorkStation");
                    break;
            }

        }



        private void SetAutoStart(bool enabled)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (enabled)
                {
                    key.SetValue(Application.ProductName, Application.ExecutablePath);
                }
                else
                {
                    key.DeleteValue(Application.ProductName, false);
                }
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            appSettings["defaultAction1"] = comboBox1.SelectedIndex.ToString();
            SaveConfigFile();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add(new ToolStripMenuItem("Zamknij program", null, (s, ev) =>
            {
                Application.ExitThread();
            }));
            menu.Items.Add(new ToolStripMenuItem("Zatrzymaj odliczanie (bezczynnoœæ)", null, (s, ev) =>
            {
                timer1.Stop();
            }));
            menu.Items.Add(new ToolStripMenuItem("Zatrzymaj odliczanie (czasowe)", null, (s, ev) =>
            {
                timer2.Stop();
            }));
            menu.Items.Add(new ToolStripMenuItem("Wznów odliczanie (bezczynnoœæ)", null, (s, ev) =>
            {
                timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(textBox4.Text));
                timer1.Start();
                tabPage2.Enabled = false;
            }));
            menu.Items.Add(new ToolStripMenuItem("Wznów odliczanie (czasowe)", null, (s, ev) =>
            {
                DateTime selectedDate = dateTimePicker1.Value.Date;
                TimeSpan selectedTime = TimeSpan.Parse(textBox7.Text);
                DateTime fullSelectedDateTime = selectedDate + selectedTime;

                TimeSpan timeToWait = fullSelectedDateTime - DateTime.Now;
                if (timeToWait.TotalSeconds > 0)
                {
                    timer2EndTime = DateTime.Now.Add(timeToWait);
                    timer2.Start();
                    tabPage1.Enabled = false;
                }
            }));
            menu.Items[0].Image = Image.FromFile("./icons/close.ico");
            menu.Items[3].Image = Image.FromFile("./icons/next.ico");
            menu.Items[4].Image = Image.FromFile("./icons/next.ico");
            menu.Items[1].Image = Image.FromFile("./icons/stop.ico");
            menu.Items[2].Image = Image.FromFile("./icons/stop.ico");
            menu.Items[1].Enabled = !timer1.Enabled;
            notifyIcon1.ContextMenuStrip = menu;
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            appSettings["timer1ActivateBy"] = textBox4.Text;
            SaveConfigFile();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            appSettings["timer2HourActivateBy"] = textBox7.Text;
            SaveConfigFile();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            appSettings["timer2DateActivateBy"] = dateTimePicker1.Value.ToString("dd.MM.yyyy");
            SaveConfigFile();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(textBox4.Text));
            timer1.Start();
            tabPage2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            tabPage2.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DateTime selectedDate = dateTimePicker1.Value.Date;
            TimeSpan selectedTime = TimeSpan.Parse(textBox7.Text);
            DateTime fullSelectedDateTime = selectedDate + selectedTime;

            TimeSpan timeToWait = fullSelectedDateTime - DateTime.Now;
            if (timeToWait.TotalSeconds <= 0)
            {
                textBox9.Text = "Wpisana data jest nieprawid³owa.";
            }
            else
            {
                timer2EndTime = DateTime.Now.Add(timeToWait);
                timer2.Start();
                tabPage1.Enabled = false;
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            timer2.Stop();
            tabPage1.Enabled = true;
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://windowsbase.pl",
                UseShellExecute = true
            };
            Process.Start(psi);
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["runWindowsAuto"] = checkBox1.Checked ? "1" : "0";
            SetAutoStart(checkBox1.Checked);
            SaveConfigFile();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["autoStart"] = checkBox2.Checked ? "1" : "0";
            if (checkBox2.Checked)
            {
                timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(textBox4.Text));
                timer1.Start();
            }
            SaveConfigFile();
        }


        private NotifyIcon notifyIcon1 = new NotifyIcon();

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["noMinimalize"] = checkBox3.Checked ? "1" : "0";
            this.MinimizeBox = !checkBox3.Checked;
            if (checkBox3.Checked)
            {
                this.FormClosing += Form1_FormClosing;
                this.Resize += Form1_Resize;
                notifyIcon1.DoubleClick += NotifyIcon1_DoubleClick;
            }
            else
            {
                this.FormClosing -= Form1_FormClosing;
                this.Resize -= Form1_Resize;
                notifyIcon1.DoubleClick -= NotifyIcon1_DoubleClick;
            }
            SaveConfigFile();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBox3.Checked)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && checkBox3.Checked)
            {
                this.Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void NotifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["RunHost"] = checkBox4.Checked ? "1" : "0";
            if (checkBox4.Checked)
            {
                timer1.Tick -= Timer1_Tick;
                timer1.Tick += Timer1_Tick_Reset;
            }
            else
            {
                timer1.Tick -= Timer1_Tick_Reset;
                timer1.Tick += Timer1_Tick;
            }
            SaveConfigFile();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            appSettings["defaultAction2"] = comboBox2.SelectedIndex.ToString();
            SaveConfigFile();
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            appSettings["userSet"] = textBoxUser.Text;
            SaveConfigFile();
        }

        //puste obiekty mo¿liwe do zape³nienia

        private void textBox12_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }


        private void label12_Click(object sender, EventArgs e)
        {

        }


        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            var psi2 = new ProcessStartInfo
            {
                FileName = "config.ini",
                UseShellExecute = true
            };
            Process.Start(psi2);
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_2(object sender, EventArgs e)
        {
            
        }
    }
}
