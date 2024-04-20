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
    using System.Security.Principal;
    using Microsoft.VisualBasic.ApplicationServices;
    using System.Threading;
    using TimerSys;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;
    using Newtonsoft.Json;
    using System.Reflection.Emit;
    using System.Net;
    using System.Globalization;

    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer timer1 = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer timer2 = new System.Windows.Forms.Timer();
        private DateTime timer1EndTime;
        private DateTime timer2EndTime;
        private string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./data/config.ini");
        private NameValueCollection appSettings;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        public static Icon ExtractIconFromExe(string file, int number, bool largeIcon)
        {
            IntPtr[] large = new IntPtr[1];
            IntPtr[] small = new IntPtr[1];
            ExtractIconEx(file, number, large, small, 1);
            return Icon.FromHandle(largeIcon ? large[0] : small[0]);
        }

        public static string systemDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
        public static string shell32Path = Path.Combine(systemDirectory, "shell32.dll");
        public static string appDirectory = Path.GetDirectoryName(Application.ExecutablePath);

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(new string[] { "Odtwórz dŸwiêk", "Uruchom polecenie", "Wy³¹cz monitor", "Uœpienie", "Wylogowanie", "Restart", "Wy³¹czenie komputera", "Hibernacja", "Blokada u¿ytkownika" });
            comboBox2.Items.AddRange(new string[] { "Odtwórz dŸwiêk", "Uruchom polecenie", "Wy³¹cz monitor", "Uœpienie", "Wylogowanie", "Restart", "Wy³¹czenie komputera", "Hibernacja", "Blokada u¿ytkownika" });
            timer1.Interval = 1000;
            timer1.Tick += Timer1_Tick;
            timer2.Interval = 1000;
            timer2.Tick += Timer2_Tick;
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button4.Click += button4_Click;
            //button5.Click += button5_Click;
            linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            LoadConfigFile();
            notifyIcon1.Icon = SystemIcons.Application;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            int icon1 = 112;

            if (File.Exists(Path.Combine(appDirectory, "icons", "clock.ico")))
            {
                notifyIcon1.Icon = new Icon(Path.Combine(appDirectory, "icons", "clock.ico"));
            }
            else
            {
                notifyIcon1.Icon = ExtractIconFromExe(shell32Path, icon1, true);
            }

            Version osVersion = Environment.OSVersion.Version;
            if (osVersion.Major < 10)
            {
                tabPage1.Text = "Aktywator bezczynnoœci";
                tabPage2.Text = "Aktywator czasowy";
                tabPage3.Text = "Ustawienia";
                tabPage4.Text = "Autorzy";
                tabPage5.Text = "O programie...";
            }

            textBoxInfo.ScrollBars = ScrollBars.Vertical;

            string dataLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./data/psd.dat");
            if (!File.Exists(dataLocation))
            {
                Form4 form4 = new Form4();
                form4.ShowDialog();
            }

            ShowLabelBasedOnVersion();

        }

        public void ShowLabelBasedOnVersion()
        {
            using (var client = new HttpClient())
            {
                string repoUrl = "https://api.github.com/repos/Mattronix7200/TimerSysApp/tags";
                string versionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./data/version.txt");
                client.DefaultRequestHeaders.Add("User-Agent", "C# App");
                var response = client.GetAsync(repoUrl).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                var tags = JsonConvert.DeserializeObject<List<GitHubTag>>(content);

                var versionTags = tags.Where(t => Regex.IsMatch(t.Name, @"release-\d+\.\d+\.\d+")).ToList();
                var latestVersion = versionTags.Select(t => new Version(t.Name.Replace("release-", ""))).OrderByDescending(v => v).FirstOrDefault();

                var currentVersion = new Version(File.ReadAllText(versionFilePath));

                if (latestVersion > currentVersion)
                {
                    label20.Enabled = true;
                    label20.Visible = true;
                }
                else
                {
                    label23.Enabled = true;
                    label23.Visible = true;
                }
            }
        }

        public class GitHubTag
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }



        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["protect"] = checkBox6.Checked ? "1" : "0";
            SaveConfigFile();
            if (checkBox6.Checked)
            {
                tabControl1.TabPages["tabPage1"].Enabled = false;
                tabControl1.TabPages["tabPage2"].Enabled = false;
                tabControl1.TabPages["tabPage3"].Enabled = false;
                button6.Enabled = true;
                label19.Visible = true;
                label19.Enabled = true;
                pictureBox6.Enabled = true;
                pictureBox6.Visible = true;
                // Zablokuj tabControl1, tabControl2, tabControl3
            }
            else
            {
                tabControl1.TabPages["tabPage1"].Enabled = true;
                tabControl1.TabPages["tabPage2"].Enabled = true;
                tabControl1.TabPages["tabPage3"].Enabled = true;
                button6.Enabled = false;
                label19.Visible = false;
                label19.Enabled = false;
                pictureBox6.Enabled = false;
                pictureBox6.Visible = false;
                // Odblokuj tabControl1, tabControl2, tabControl3
            }

        }

        private Form2 form2 = null;
        private Form3 form3 = null;

        private void button6_Click(object sender, EventArgs e)
        {
            if (form2 == null || form2.IsDisposed)
            {
                form2 = new Form2();
                form2.ShowDialog();
            }
            else
            {
                form2.BringToFront();
            }
        }

        private void label19_Click(object sender, EventArgs e)
        {
            if (form3 == null || form3.IsDisposed)
            {
                form3 = new Form3();
                form3.MainForm = this;
                form3.Show();
            }
            else
            {
                form3.BringToFront();
            }
        }


        private void tabPage1_Click(object sender, EventArgs e)
        {
            ShowPasswordForm();
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {
            ShowPasswordForm();
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
            ShowPasswordForm();
        }

        private void ShowPasswordForm()
        {
            if (checkBox6.Checked)
            {
                Form3 form3 = new Form3();
                form3.ShowDialog();
            }
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

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "powerPlanID");
                    writer.WriteAttributeString("value", "0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "powerPlanSetID");
                    writer.WriteAttributeString("value", null);
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "protect");
                    writer.WriteAttributeString("value", "0");
                    writer.WriteEndElement();

                    writer.WriteStartElement("add");
                    writer.WriteAttributeString("key", "userCommandBehaviour");
                    writer.WriteAttributeString("value", "0");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }
            int maxAttempts = 3;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    // Wczytaj plik konfiguracyjny
                    var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
                    var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
                    var dict = config.AppSettings.Settings.AllKeys.ToDictionary(key => key, key => config.AppSettings.Settings[key].Value);

                    appSettings = new NameValueCollection();
                    foreach (var kvp in dict)
                    {
                        appSettings.Add(kvp.Key, kvp.Value);
                    }

                    // Sprawdz czy uruchomione w tle

                    if (checkBox2.Checked)
                    {
                        timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(appSettings["timer1ActivateBy"]));
                        timer1.Start();

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
                    //DateTime settingsDate = DateTime.Parse(appSettings["timer2DateActivateBy"]);
                    DateTime settingsDate = DateTime.ParseExact(appSettings["timer2DateActivateBy"], "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    DateTime currentDate = DateTime.Now;
                    dateTimePicker1.MinDate = settingsDate < currentDate ? settingsDate : currentDate;
                    dateTimePicker1.Value = settingsDate;
                    dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;

                    // opcje- ustawienia
                    checkBox1.Checked = appSettings["runWindowsAuto"] == "1";
                    checkBox2.Checked = appSettings["autoStart"] == "1";
                    checkBox3.Checked = appSettings["noMinimalize"] == "1";
                    checkBox4.Checked = appSettings["RunHost"] == "1";
                    checkBox5.Checked = appSettings["powerPlanSetID"] != "";
                    checkBox6.Checked = appSettings["protect"] == "1";
                    checkBox7.Checked = appSettings["userCommandBehaviour"] == "1";
                    break;
                }
                catch (IOException)
                {
                    if (attempt < maxAttempts - 1)
                    {
                        Thread.Sleep(3500);
                        continue;
                    }
                    else
                    {
                        // nothing to do...

                    }
                }
            }
        }


        private void SaveConfigFile()
        {
            int maxAttempts = 3;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
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
                    break; // Jeœli plik zosta³ pomyœlnie zapisany, wyjdŸ z pêtli
                }
                catch (IOException)
                {
                    if (attempt < maxAttempts - 1)
                    {
                        Thread.Sleep(3500);
                        continue;
                    }
                    else
                    {
                        // nothing to do...
                    }
                }
            }
        }

        // funkcja do zaimportowania planu zasilania oraz kontroli czy jest aktywny w systemie

        private System.Threading.Timer powerPlanMonitor;
        private string importedPowerPlanGuid;

        private string ImportAndSetActivePowerPlan()
        {
            var cmd = new Process { StartInfo = { FileName = "powercfg" } };
            var inputPath = Path.Combine(Environment.CurrentDirectory, "./data/plan.pow");
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            var guidString = Guid.NewGuid().ToString("D");

            cmd.StartInfo.Arguments = $"-import \"{inputPath}\" {guidString}";
            cmd.Start();

            cmd.StartInfo.Arguments = $"/setactive {guidString}";
            cmd.Start();

            SavePowerPlanImportedGuidToConfigFile(guidString);

            return guidString;
        }

        private void StartPowerPlanMonitor()
        {
            powerPlanMonitor = new System.Threading.Timer(CheckPowerPlan, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
        }

        private void StopPowerPlanMonitor()
        {
            powerPlanMonitor?.Dispose();
            powerPlanMonitor = null;
        }

        private void CheckPowerPlan(object state)
        {
            if (!checkBox5.Checked)
            {
                return;
            }

            var cmd = new Process { StartInfo = { FileName = "powercfg" } };
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = "/GETACTIVESCHEME";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            var output = cmd.StandardOutput.ReadToEnd();
            var outputArray = output.Split(' ');

            if (Environment.OSVersion.Version.Major < 10)
            {
                // Windows 7 and older
                if (outputArray.Length > 3)
                {
                    var activePlanGuid = outputArray[4].Trim();
                    if (activePlanGuid != importedPowerPlanGuid)
                    {
                        cmd.StartInfo.Arguments = $"/setactive {importedPowerPlanGuid}";
                        cmd.Start();
                    }
                }
            }
            else
            {
                // Windows 10 and newer
                if (outputArray.Length > 2)
                {
                    var activePlanGuid = outputArray[3].Trim();
                    if (activePlanGuid != importedPowerPlanGuid)
                    {
                        cmd.StartInfo.Arguments = $"/setactive {importedPowerPlanGuid}";
                        cmd.Start();
                    }
                }
            }
        }


        //private void CheckPowerPlan(object state)
        //{
        //    if (!checkBox5.Checked)
        //    {
        //        return;
        //    }

        //    var cmd = new Process { StartInfo = { FileName = "powercfg" } };
        //    cmd.StartInfo.CreateNoWindow = true;
        //    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //    cmd.StartInfo.Arguments = "/GETACTIVESCHEME";
        //    cmd.StartInfo.RedirectStandardOutput = true;
        //    cmd.StartInfo.UseShellExecute = false;
        //    cmd.Start();

        //    var output = cmd.StandardOutput.ReadToEnd();

        //    var outputArray = output.Split(' ');
        //    if (outputArray.Length > 2)
        //    {
        //        var activePlanGuid = outputArray[3].Trim();
        //        if (activePlanGuid != importedPowerPlanGuid)
        //        {
        //            cmd.StartInfo.Arguments = $"/setactive {importedPowerPlanGuid}";
        //            cmd.Start();
        //        }
        //    }
        //    else
        //    {
        //        // Handler
        //    }
        //}

        //private string GetCurrentPowerPlanGuid()
        //{
        //    var cmd = new Process { StartInfo = { FileName = "powercfg" } };
        //    cmd.StartInfo.CreateNoWindow = true;
        //    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //    cmd.StartInfo.Arguments = "/GETACTIVESCHEME";
        //    cmd.StartInfo.RedirectStandardOutput = true;
        //    cmd.StartInfo.UseShellExecute = false;
        //    cmd.Start();

        //    var output = cmd.StandardOutput.ReadToEnd();
        //    var outputArray = output.Split(' ');

        //    if (outputArray.Length > 2)
        //    {
        //        var activePlanGuid = outputArray[3].Trim();
        //        return activePlanGuid;
        //    }
        //    else
        //    {
        //        return null; // throw an exception
        //    }
        //}

        private string GetCurrentPowerPlanGuid()
        {
            var cmd = new Process { StartInfo = { FileName = "powercfg" } };
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = "/GETACTIVESCHEME";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            var output = cmd.StandardOutput.ReadToEnd();
            var outputArray = output.Split(' ');

            if (Environment.OSVersion.Version.Major < 10)
            {
                // Windows 7 and older
                if (outputArray.Length > 3)
                {
                    var activePlanGuid = outputArray[4].Trim();
                    return activePlanGuid;
                }
            }
            else
            {
                // Windows 10 and newer
                if (outputArray.Length > 2)
                {
                    var activePlanGuid = outputArray[3].Trim();
                    return activePlanGuid;
                }
            }

            return null; // throw an exception
        }



        private void SavePowerPlanGuidToConfigFile(string guid)
        {
            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            if (config.AppSettings.Settings["powerPlanID"] == null)
            {
                config.AppSettings.Settings.Add("powerPlanID", guid);
            }
            else
            {
                config.AppSettings.Settings["powerPlanID"].Value = guid;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }


        private void SavePowerPlanImportedGuidToConfigFile(string guid)
        {
            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            if (config.AppSettings.Settings["powerPlanSetID"] == null)
            {
                config.AppSettings.Settings.Add("powerPlanSetID", guid);
            }
            else
            {
                config.AppSettings.Settings["powerPlanSetID"].Value = guid;
            }

            config.Save(ConfigurationSaveMode.Modified);
        }


        private void SavePowerPlanRestoreGuidToConfigFile()
        {
            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            config.AppSettings.Settings["powerPlanSetID"].Value = null;
            config.Save(ConfigurationSaveMode.Modified);
        }

        private void RemovePowerPlan()
        {
            string powerPlanGuid = appSettings["powerPlanSetID"];
            if (File.Exists(@"./data/w7restorePlan.ps1"))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-ExecutionPolicy Bypass -File ./data/w7restorePlan.ps1",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var process = Process.Start(psi);
                process.WaitForExit();
            }
            SavePowerPlanRestoreGuidToConfigFile();
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

            if (idleTime < 1000)
            {
                timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(textBox4.Text));
            }

            var remainingTime = timer1EndTime - DateTime.Now;
            if (remainingTime.TotalSeconds <= 0)
            {
                timer1.Stop();
                ExecuteCommand(comboBox1.SelectedItem.ToString());
                textBox5.Text = "Wykonujê wybran¹ akcjê...";
                tabPage2.Enabled = true;
            }
            else
            {
                textBox5.Text = "Pozosta³y czas: " + remainingTime.ToString(@"hh\:mm\:ss");
            }
        }

        public void StartTimer1()
        {
            timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(textBox4.Text));
            timer1.Start();
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
                    textBox9.Text = "Pozosta³y czas: " + remainingTime.ToString(@"%s") + " sekund";
                }
                else if (remainingTime.TotalMinutes < 60)
                {
                    textBox9.Text = "Pozosta³y czas: " + remainingTime.ToString(@"%m") + " minut";
                }
                else if (remainingTime.TotalHours < 24)
                {
                    textBox9.Text = "Pozosta³y czas: " + remainingTime.ToString(@"%h") + " godzin";
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
                textBox5.Text = "Pozosta³y czas: " + remainingTime.ToString(@"hh\:mm\:ss");
            }
        }


        private void ExecuteCommand(string command)
        {
            //var userCommand = textBoxUser.Text;
            string userCommand = WebUtility.HtmlDecode(appSettings["userSet"]);

            switch (command)
            {
                case "Odtwórz dŸwiêk":
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer();
                    string systemDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System);
                    string defaultSound = Path.Combine(systemDirectory, "..", "Media", "tada.wav");
                    string soundFile = Path.Combine(appDirectory, "sound", "sound.wav");

                    if (!File.Exists(soundFile))
                    {
                        player.SoundLocation = defaultSound;
                        player.Load();
                        player.Play();
                    }
                    else
                    {
                        player.SoundLocation = soundFile;
                        player.Load();
                        player.Play();
                    }
                    break;
                case "Uruchom polecenie":
                    var psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c " + userCommand,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    if (appSettings["userCommandBehaviour"] == "1")
                    {
                        var process = Process.Start(psi);
                        process.WaitForExit();
                    }
                    else
                    {
                        Process.Start(psi);
                    }
                    break;
                case "Wy³¹cz monitor":
                    if (File.Exists(@"./data/monitor.bat"))
                    {
                        Process proc = new Process();
                        proc.StartInfo.FileName = @"./data/monitor.bat";
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.Start();
                    }
                    break;
                case "Uœpienie":
                    Process.Start("Rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0");
                    break;
                case "Wylogowanie":
                    Process.Start("shutdown", "/l");
                    break;
                case "Restart":
                    Process.Start("shutdown", "/r /t 0");
                    break;
                case "Wy³¹czenie komputera":
                    Process.Start("shutdown", "/s /t 0");
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
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startupFolder, "Wy³¹cznik czasowy.lnk");

            if (enabled)
            {
                if (!File.Exists(shortcutPath))
                {
                    IWshRuntimeLibrary.WshShell wsh = new IWshRuntimeLibrary.WshShell();
                    IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(shortcutPath) as IWshRuntimeLibrary.IWshShortcut;
                    shortcut.TargetPath = Application.ExecutablePath;
                    shortcut.Arguments = "--minimized";
                    shortcut.Save();
                }
            }
            else
            {
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }
            }
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            appSettings["defaultAction1"] = comboBox1.SelectedIndex.ToString();
            SaveConfigFile();

            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() == "Wy³¹cz monitor")
            {
                label13.Visible = true;
            }
            else
            {
                label13.Visible = false;
            }

            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() == "Odtwórz dŸwiêk")
            {
                label15.Visible = true;
            }
            else
            {
                label15.Visible = false;
            }
            if (comboBox1.SelectedItem != null && comboBox1.SelectedItem.ToString() == "Uruchom polecenie")
            {
                label17.Visible = true;
            }
            else
            {
                label17.Visible = false;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            appSettings["defaultAction2"] = comboBox2.SelectedIndex.ToString();
            SaveConfigFile();

            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() == "Wy³¹cz monitor")
            {
                label16.Visible = true;
            }
            else
            {
                label16.Visible = false;
            }

            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() == "Odtwórz dŸwiêk")
            {
                label14.Visible = true;
            }
            else
            {
                label14.Visible = false;
            }
            if (comboBox2.SelectedItem != null && comboBox2.SelectedItem.ToString() == "Uruchom polecenie")
            {
                label18.Visible = true;
            }
            else
            {
                label18.Visible = false;
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //static bool IsAdministrator()
            //{
            //    WindowsIdentity identity = WindowsIdentity.GetCurrent();
            //    WindowsPrincipal principal = new WindowsPrincipal(identity);
            //    return principal.IsInRole(WindowsBuiltInRole.Administrator);
            //}

            //if (!IsAdministrator())
            //{
            //    checkBox1.Enabled = false;
            //    checkBox1.Text = "Automatycznie uruchamiaj program (wymagane uprawnienia administratora)";
            //}

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
            menu.Items[0].Image = File.Exists(Path.Combine(appDirectory, "icons", "close.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "close.ico")) : null;
            menu.Items[3].Image = File.Exists(Path.Combine(appDirectory, "icons", "next.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "next.ico")) : null;
            menu.Items[4].Image = File.Exists(Path.Combine(appDirectory, "icons", "next.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "next.ico")) : null;
            menu.Items[1].Image = File.Exists(Path.Combine(appDirectory, "icons", "stop.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "stop.ico")) : null;
            menu.Items[2].Image = File.Exists(Path.Combine(appDirectory, "icons", "stop.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "stop.ico")) : null;
            menu.Items[1].Enabled = !timer1.Enabled;
            notifyIcon1.ContextMenuStrip = menu;

            if (checkBox6.Checked)
            {
                tabControl1.TabPages["tabPage1"].Enabled = false;
                tabControl1.TabPages["tabPage2"].Enabled = false;
                tabControl1.TabPages["tabPage3"].Enabled = false;
                button6.Enabled = false;
                label19.Visible = true;
                label19.Enabled = true;
                pictureBox6.Enabled = true;
                pictureBox6.Visible = true;
            }
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

            TimeSpan maxTime = new TimeSpan(24, 0, 0);
            TimeSpan inputTime;

            if (TimeSpan.TryParse(textBox4.Text, out inputTime) && inputTime > maxTime)
            {
                textBox5.Text = "Maksymalny okres odliczanego czasu w tym oknie wynosi 1 dzieñ.";
                timer1.Stop();
                textBox4.Text = "24:00:00";
            }

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
            appSettings["timer2DateActivateBy"] = dateTimePicker1.Value.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
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
                textBox9.Text = "Wybra³eœ datê z przesz³oœci, wybierz tê z przysz³oœci.";
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

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (appSettings["powerPlanSetID"] == appSettings["powerPlanID"])
            {
                //fix the issue with adding plan while program is runned after rerun 
            }

            if (checkBox5.Checked)
            {
                // Store the GUID of the current power plan
                var currentPowerPlanGuid = GetCurrentPowerPlanGuid();
                appSettings["powerPlanID"] = currentPowerPlanGuid;
                SavePowerPlanGuidToConfigFile(currentPowerPlanGuid);  // Save the current power plan GUID to the config file

                // Check if the power plan already exists before importing and setting it
                if (appSettings["powerPlanSetID"] == "")
                {
                    // Import and set the new power plan, and start the monitor
                    importedPowerPlanGuid = ImportAndSetActivePowerPlan();
                    StartPowerPlanMonitor();
                }
            }
            else
            {
                StopPowerPlanMonitor();

                var cmd = new Process { StartInfo = { FileName = "powercfg" } };
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmd.StartInfo.Arguments = $"/setactive {appSettings["powerPlanID"]}";
                cmd.Start();

                RemovePowerPlan();
                checkBox5.Enabled = false;
                checkBox5.Text = "Uruchom ponownie aby zastosowaæ zmiany";
            }
        }


        //private void checkBox5_CheckedChanged(object sender, EventArgs e)
        //{
        //    checkBox5.Checked = appSettings["powerPlanID"] == null;

        //    if (checkBox5.Checked)
        //    {
        //        // Store the GUID of the current power plan
        //        var currentPowerPlanGuid = GetCurrentPowerPlanGuid();
        //        appSettings["powerPlanID"] = currentPowerPlanGuid;
        //        SavePowerPlanGuidToConfigFile(currentPowerPlanGuid);  // Save the current power plan GUID to the config file

        //        // Import and set the new power plan, and start the monitor
        //        importedPowerPlanGuid = ImportAndSetActivePowerPlan();
        //        StartPowerPlanMonitor();
        //    }
        //    else
        //    {
        //        StopPowerPlanMonitor();

        //        var cmd = new Process { StartInfo = { FileName = "powercfg" } };
        //        cmd.StartInfo.CreateNoWindow = true;
        //        cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //        cmd.StartInfo.Arguments = $"/setactive {appSettings["powerPlanID"]}";
        //        cmd.Start();

        //        RemovePowerPlan();
        //    }
        //}


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBox3.Checked)
            {
                e.Cancel = true;
                this.Hide();
                notifyIcon1.Visible = true;
            }

            if (checkBox6.Checked)
            {
                tabControl1.TabPages["tabPage1"].Enabled = false;
                tabControl1.TabPages["tabPage2"].Enabled = false;
                tabControl1.TabPages["tabPage3"].Enabled = false;
                button6.Enabled = false;
                label19.Visible = true;
                label19.Enabled = true;
                pictureBox6.Enabled = true;
                pictureBox6.Visible = true;
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


        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./data/config.ini");
            var psi2 = new ProcessStartInfo
            {
                FileName = configFilePath,
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

        private void label13_Click_1(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "https://github.com/Mattronix7200/TimerSysApp/",
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click_1(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), @"data\fixSettings.ps1");
            if (File.Exists(scriptPath))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                var osVersion = Environment.OSVersion.Version;
                if (osVersion.Major < 10)
                {
                    psi.WorkingDirectory = Path.GetDirectoryName(scriptPath);
                    psi.Arguments = $"-ExecutionPolicy Bypass -File {Path.GetFileName(scriptPath)}";
                }
                else
                {
                    psi.Arguments = $"-ExecutionPolicy Bypass -File {scriptPath}";
                }

                var process = Process.Start(psi);
                process.WaitForExit();
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["userCommandBehaviour"] = checkBox7.Checked ? "1" : "0";
            SaveConfigFile();
        }
    }
}
