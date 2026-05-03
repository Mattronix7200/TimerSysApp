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
    using System.IO.Compression;
    using System.Collections.Specialized;
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
        private DateTime _timer2LocalTarget;
        private int _lastSecond1 = -1;
        private int _lastSecond2 = -1;
        private string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "config.json");
        private NameValueCollection appSettings = new NameValueCollection();
        private System.Windows.Forms.Timer timer3 = new System.Windows.Forms.Timer();
        private int _lastFiredScheduleMinute = -1;
        private DateTime _lastFiredDate = DateTime.MinValue;
        private System.Windows.Forms.Timer _timerSnooze = new System.Windows.Forms.Timer { Interval = 500 };
        private DateTime _snoozeEndTime;
        private System.Windows.Forms.Label _lblScheduleStatus;
        private System.Windows.Forms.Label _lblSnoozeStatus;
        private System.Windows.Forms.Button _btnActSettings1;
        private System.Windows.Forms.Button _btnActSettings2;
        private System.Windows.Forms.Button _btnActSettings3;
        private System.Windows.Forms.ToolTip _actionTip = new System.Windows.Forms.ToolTip();
        private System.Windows.Forms.Timer _timerStatusUI = new System.Windows.Forms.Timer { Interval = 1000 };
        private ToolStripMenuItem _menuScheduleToggle;
        private List<TaskEntry> _taskList = new();
        private int _taskListFilter = 0;
        private TaskEntry? _currentlyFiringTask = null;

        private static readonly string[] _scheduleLabels = {
            "Codziennie", "Co dwa dni", "Co trzy dni", "Co cztery dni", "Co pięć dni",
            "Co sześć dni", "Co tydzień", "W każdy weekend (Pt.-Nd.)",
            "W każdy poniedziałek", "W każdy wtorek", "W każdą środę", "W każdy czwartek",
            "W każdy piątek", "W każdą sobotę", "W każdą niedzielę", "Od poniedziałku do piątku",
            "Powtarzaj co..."
        };
        private const int SCHEDULE_INTERVAL_IDX = 16;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint ExtractIconEx(string szFileName, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


        [System.Runtime.InteropServices.ComImport]
        [System.Runtime.InteropServices.Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLinkCoClass { }

        [System.Runtime.InteropServices.ComImport]
        [System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
        [System.Runtime.InteropServices.Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLinkW
        {
            void GetPath([Out, System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] System.Text.StringBuilder pszFile, int cch, IntPtr pfd, uint fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] System.Text.StringBuilder pszName, int cch);
            void SetDescription([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] System.Text.StringBuilder pszDir, int cch);
            void SetWorkingDirectory([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] System.Text.StringBuilder pszArgs, int cch);
            void SetArguments([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] System.Text.StringBuilder pszIconPath, int cch, out int piIcon);
            void SetIconLocation([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
            void Resolve(IntPtr hwnd, uint fFlags);
            void SetPath([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszFile);
        }

        [System.Runtime.InteropServices.ComImport]
        [System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
        [System.Runtime.InteropServices.Guid("0000010b-0000-0000-C000-000000000046")]
        private interface IPersistFile
        {
            void GetClassID(out Guid pClassID);
            [System.Runtime.InteropServices.PreserveSig] int IsDirty();
            void Load([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
            void Save([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszFileName, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool fRemember);
            void SaveCompleted([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string pszFileName);
            void GetCurFile([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] out string ppszFileName);
        }

        private static void CreateShortcutNative(string shortcutPath, string targetPath, string arguments)
        {
            var link = (IShellLinkW)new ShellLinkCoClass();
            link.SetPath(targetPath);
            link.SetArguments(arguments ?? "");
            link.SetWorkingDirectory(Path.GetDirectoryName(targetPath) ?? "");
            ((IPersistFile)link).Save(shortcutPath, false);
        }

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
            string[] comboActions = new string[] {
                "Odtwórz dźwięk",
                "Uruchom polecenie",
                "Wyłącz monitor",
                "Uśpienie",
                "Wylogowanie",
                "Restart",
                "Wyłączenie komputera",
                "Hibernacja",
                "Blokada użytkownika",
                "Oczyść pliki tymczasowe",
                "Zapisz notatkę na pulpicie",
                "Otwórz stronę WWW",
                "Opróżnij kosz",
                "Wykonaj zrzut ekranu",
            };
            comboBox1.Items.AddRange(comboActions);
            comboBox2.Items.AddRange(comboActions);
            comboBox3.Items.AddRange(comboActions);
            timer1.Interval = 100;
            timer1.Tick += Timer1_Tick;
            timer2.Interval = 100;
            timer2.Tick += Timer2_Tick;
            timer3.Interval = 1000;
            timer3.Tick += Timer3_Tick;


            checkBox8.CheckedChanged += checkBox8_CheckedChanged;
            checkBox9.CheckedChanged += checkBox9_CheckedChanged;
            checkBox10.CheckedChanged += checkBox10_CheckedChanged;
            checkBox11.CheckedChanged += checkBox11_CheckedChanged;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            EnsureDataFiles();
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
                tabPage1.Text = "Aktywator bezczynności";
                tabPage2.Text = "Aktywator czasowy";
                tabPage3.Text = "Ustawienia";
                tabPage4.Text = "Autorzy";
            }

            textBoxInfo.ScrollBars = ScrollBars.Vertical;

            string dataLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./data/psd.dat");
            bool isFirstLaunch = !File.Exists(dataLocation);
            if (isFirstLaunch)
            {

                Form4 form4Welcome = new Form4();
                form4Welcome.ShowDialog();

                checkBox1.Checked = true;
                checkBox3.Checked = true;
                SaveConfigFile();
            }


        }

        public void ShowLabelBasedOnVersion()
        {
            Task.Run(async () =>
            {
                try
                {
                    string repoUrl = "https://api.github.com/repos/Mattronix7200/TimerSysApp/tags";
                    string versionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "./data/version.txt");
                    using var client = new HttpClient();
                    client.Timeout = TimeSpan.FromSeconds(10);
                    client.DefaultRequestHeaders.Add("User-Agent", "C# App");
                    var response = await client.GetAsync(repoUrl).ConfigureAwait(false);
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var tags = JsonConvert.DeserializeObject<List<GitHubTag>>(responseContent);


                    if (tags == null || !tags.Any())
                    {
                        BeginInvoke(new Action(() => { label23.Enabled = true; label23.Visible = true; }));
                        return;
                    }

                    var versionTags = tags.Where(t => Regex.IsMatch(t.Name, @"release-\d+\.\d+\.\d+")).ToList();
                    var latestVersion = versionTags.Select(t => new Version(t.Name.Replace("release-", ""))).OrderByDescending(v => v).FirstOrDefault();


                    string currentVersionStr = "0.0.0";
                    if (File.Exists(versionFilePath))
                        currentVersionStr = File.ReadAllText(versionFilePath).Trim();
                    var currentVersion = Version.TryParse(currentVersionStr, out var cv) ? cv : new Version("0.0.0");

                    bool isNewer = latestVersion != null && latestVersion > currentVersion;

                    BeginInvoke(new Action(() =>
                    {
                        label20.Enabled = isNewer;
                        label20.Visible = isNewer;
                        label23.Enabled = !isNewer;
                        label23.Visible = !isNewer;
                    }));
                }
                catch
                {

                    try { BeginInvoke(new Action(() => { label23.Visible = true; label23.Enabled = true; })); }
                    catch { }
                }
            });
        }

        public class GitHubTag
        {
            [JsonProperty("name")]
            public string Name { get; set; }
        }


        public void UnlockScreen()
        {
            tabControl1.TabPages["tabPage1"].Enabled = true;
            tabControl1.TabPages["tabPage2"].Enabled = true;
            tabControl1.TabPages["tabPage3"].Enabled = true;
            button6.Enabled = false;
            label19.Visible = false;
            label19.Enabled = false;
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

            }
            else
            {
                tabControl1.TabPages["tabPage1"].Enabled = true;
                tabControl1.TabPages["tabPage2"].Enabled = true;
                tabControl1.TabPages["tabPage3"].Enabled = true;
                button6.Enabled = false;
                label19.Visible = false;
                label19.Enabled = false;

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
                form3.MainForm = this;
                form3.ShowDialog();
            }
        }

        private Dictionary<string, string> GetDefaultSettings() => new Dictionary<string, string>
        {
            ["defaultAction1"] = "0",
            ["defaultAction2"] = "0",
            ["defaultAction3"] = "0",
            ["timer1ActivateBy"] = "00:05:00",
            ["timer1EnabledAuto"] = "0",
            ["timer2HourActivateBy"] = "10:00",
            ["timer2DateActivateBy"] = DateTime.Now.ToString("dd.MM.yyyy"),
            ["timer3HourActivateBy"] = "10:00",
            ["timer3CheckedDays"] = "",
            ["timer3Enabled"] = "0",
            ["timer3LastFired"] = "",
            ["runWindowsAuto"] = "0",
            ["autoStart"] = "0",
            ["noMinimalize"] = "0",
            ["RunHost"] = "0",
            ["powerPlanSetID"] = "",
            ["powerPlanID"] = "",
            ["protect"] = "0",
            ["userSet"] = "msedge.exe",
            ["userCommandBehaviour"] = "0",
            ["timezoneCheck"] = "1",
            ["noTray"] = "0",
            ["noTimerShow"] = "0",
            ["darkModeEnabled"] = "0",
            ["soundFile"] = "",
            ["act1_url"] = "https://google.com",
            ["act2_url"] = "https://google.com",
            ["act3_url"] = "https://google.com",
            ["act1_cmd"] = "",
            ["act2_cmd"] = "",
            ["act3_cmd"] = "",
            ["act1_note"] = "",
            ["act2_note"] = "",
            ["act3_note"] = "",

            ["cleanTemp"] = "1",
            ["cleanWinTemp"] = "1",
            ["cleanDownloads"] = "0",
            ["cleanWU"] = "1",
            ["cleanDriverCache"] = "0",
            ["cleanThumbs"] = "1",
            ["cleanPrefetch"] = "0",
            ["cleanRecycleBin"] = "0",
            ["cleanEventLog"] = "0",
            ["cleanCustomFolders"] = "",
            ["noBalloon"] = "0",
            ["taskList"] = "[]",
            ["autoStartQueue"] = "1",
            ["timer3UserPaused"] = "0",

            ["ss_format"]   = "PNG",
            ["ss_count"]    = "1",
            ["ss_delay"]    = "0",
            ["ss_interval"] = "1000",
            ["ss_compress"] = "0",
            ["ss_folder"]   = "",
        };

        private void LoadConfigFile()
        {
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            Directory.CreateDirectory(dataDir);
            configFilePath = Path.Combine(dataDir, "config.json");

            var defaults = GetDefaultSettings();
            var loaded = new Dictionary<string, string>();

            if (File.Exists(configFilePath))
            {
                try { loaded = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(configFilePath)) ?? loaded; }
                catch {  }
            }


            if (!loaded.Any())
            {
                string oldIni = Path.Combine(dataDir, "config.ini");
                if (File.Exists(oldIni)) {  }
            }

            appSettings = new NameValueCollection();
            foreach (var kv in defaults)
                appSettings[kv.Key] = loaded.TryGetValue(kv.Key, out var v) ? v : kv.Value;
            foreach (var kv in loaded)
                if (appSettings[kv.Key] == null) appSettings[kv.Key] = kv.Value;


            if (!File.Exists(configFilePath)) SaveConfigFile();


            if (appSettings["autoStart"] == "1")
            {
                timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(appSettings["timer1ActivateBy"]));
                timer1.Start();
            }


            comboBox1.SelectedIndex = int.TryParse(appSettings["defaultAction1"], out int a1) ? a1 : 0;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndex = int.TryParse(appSettings["defaultAction2"], out int a2) ? a2 : 0;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            comboBox3.SelectedIndex = int.TryParse(appSettings["defaultAction3"], out int a3) ? a3 : 0;
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;

            textBox4.Text = appSettings["timer1ActivateBy"];
            textBox4.TextChanged += textBox4_TextChanged;
            textBox7.Text = appSettings["timer2HourActivateBy"];
            textBox7.TextChanged += textBox7_TextChanged;
            textBox1.Text = appSettings["timer3HourActivateBy"];
            textBoxUser.Text = appSettings["userSet"];
            textBoxUser.TextChanged += textBoxUser_TextChanged;


            if (!string.IsNullOrEmpty(appSettings["timer3CheckedDays"]))
            {
                foreach (var part in appSettings["timer3CheckedDays"].Split(','))
                    if (int.TryParse(part.Trim(), out int idx) && idx >= 0 && idx < checkedListBox1.Items.Count)
                        checkedListBox1.SetItemChecked(idx, true);
            }


            if (!DateTime.TryParseExact(appSettings["timer2DateActivateBy"], "dd.MM.yyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime settingsDate))
                settingsDate = DateTime.Now;
            DateTime currentDate = DateTime.Now;
            dateTimePicker1.MinDate = settingsDate < currentDate ? settingsDate : currentDate;
            dateTimePicker1.Value = settingsDate;
            dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;


            checkBox1.Checked = appSettings["runWindowsAuto"] == "1";
            checkBox2.Checked = appSettings["autoStart"] == "1";
            checkBox3.Checked = appSettings["noMinimalize"] == "1";
            checkBox4.Checked = appSettings["RunHost"] == "1";
            checkBox5.Checked = appSettings["powerPlanSetID"] != "";
            checkBox6.Checked = appSettings["protect"] == "1";
            checkBox7.Checked = appSettings["userCommandBehaviour"] == "1";
            checkBox8.Checked = appSettings["timezoneCheck"] == "1";
            checkBox9.Checked = appSettings["noTray"] == "1";
            checkBox10.Checked = appSettings["noTimerShow"] == "1";
            checkBox11.Checked = appSettings["darkModeEnabled"] == "1";
            checkBox12.Checked = appSettings["autoStartQueue"] != "0";


            try { _taskList = JsonConvert.DeserializeObject<List<TaskEntry>>(appSettings["taskList"] ?? "[]") ?? new(); }
            catch { _taskList = new(); }


            if (appSettings["timer3Enabled"] == "1" && !string.IsNullOrEmpty(appSettings["timer3CheckedDays"]))
                timer3.Start();


            if (appSettings["autoStartQueue"] != "0")
            {
                if (!timer3.Enabled && appSettings["timer3UserPaused"] != "1" && _taskList.Any(t => t.ActivatorType == 3 && t.Enabled))
                    timer3.Start();
                if (!timer2.Enabled && _taskList.Any(t => t.ActivatorType == 2 && t.Enabled && t.EndTimeUtc > DateTime.UtcNow))
                    timer2.Start();
            }

        }


        private void SaveConfigFile()
        {
            try
            {
                var dict = new Dictionary<string, string>();
                foreach (string k in appSettings.AllKeys)
                    dict[k] = appSettings[k] ?? "";

                dict["taskList"] = JsonConvert.SerializeObject(_taskList ?? new List<TaskEntry>());
                Directory.CreateDirectory(Path.GetDirectoryName(configFilePath));
                File.WriteAllText(configFilePath, JsonConvert.SerializeObject(dict, Formatting.Indented));
            }
            catch {  }
        }


        private System.Threading.Timer powerPlanMonitor;
        private string importedPowerPlanGuid;

        private string ImportAndSetActivePowerPlan()
        {
            var cmd = new Process { StartInfo = { FileName = "powercfg" } };
            var inputPath = GetDataFilePath("plan.pow");
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

                if (outputArray.Length > 3)
                {
                    var activePlanGuid = outputArray[4].Trim();
                    return activePlanGuid;
                }
            }
            else
            {

                if (outputArray.Length > 2)
                {
                    var activePlanGuid = outputArray[3].Trim();
                    return activePlanGuid;
                }
            }

            return null;
        }


        private void SavePowerPlanGuidToConfigFile(string guid)
        {
            appSettings["powerPlanID"] = guid;
            SaveConfigFile();
        }

        private void SavePowerPlanImportedGuidToConfigFile(string guid)
        {
            appSettings["powerPlanSetID"] = guid;
            SaveConfigFile();
        }

        private void SavePowerPlanRestoreGuidToConfigFile()
        {
            appSettings["powerPlanSetID"] = "";
            SaveConfigFile();
        }

        private void RemovePowerPlan()
        {
            string scriptPath = GetDataFilePath("w7restorePlan.ps1");
            if (File.Exists(scriptPath))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(scriptPath)
                };
                Process.Start(psi)?.WaitForExit();
            }
            SavePowerPlanRestoreGuidToConfigFile();
        }


        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("winmm.dll", CharSet = CharSet.Unicode)]
        private static extern int mciSendString(string command, System.Text.StringBuilder returnValue, int returnLength, IntPtr winHandle);

        private static readonly string[] _soundExtensions = { "*.wav", "*.mp3", "*.aac", "*.ogg", "*.wma", "*.flac" };

        private void PlaySoundFile(string filePath)
        {

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                string soundsDir = Path.Combine(appDirectory, "sounds");
                if (Directory.Exists(soundsDir))
                {
                    foreach (var ext in _soundExtensions)
                    {
                        var found = Directory.GetFiles(soundsDir, ext).FirstOrDefault();
                        if (found != null) { filePath = found; break; }
                    }
                }

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    try
                    {
                        string soundsPath = Path.Combine(appDirectory, "sounds", "sound.wma");
                        if (!File.Exists(soundsPath))
                        {
                            var asm = System.Reflection.Assembly.GetExecutingAssembly();
                            using var resStream = asm.GetManifestResourceStream("TimerSys.data.sound.wma");
                            if (resStream != null)
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(soundsPath)!);
                                using var fs = File.Create(soundsPath);
                                resStream.CopyTo(fs);
                            }
                        }
                        if (File.Exists(soundsPath)) filePath = soundsPath;
                    }
                    catch { }
                }
            }

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                string mediaDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "..", "Media");
                foreach (var fb in new[] { "tada.wav", "Alarm01.wav", "chimes.wav", "ding.wav" })
                {
                    string fp = Path.Combine(mediaDir, fb);
                    if (File.Exists(fp)) { filePath = fp; break; }
                }
            }
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return;

            string extLow = Path.GetExtension(filePath).ToLowerInvariant();
            if (extLow == ".wav")
            {
                try { new System.Media.SoundPlayer(filePath).Play(); } catch { }
            }
            else
            {

                try
                {
                    mciSendString("close mci_sound", null, 0, IntPtr.Zero);
                    mciSendString($"open \"{filePath}\" alias mci_sound", null, 0, IntPtr.Zero);
                    mciSendString("play mci_sound", null, 0, IntPtr.Zero);
                }
                catch { }
            }
        }

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
            long idleMs = (long)(Environment.TickCount64 - lastInputInfo.dwTime);

            bool anyFired = false;
            var tasks = _taskList.Where(t => t.ActivatorType == 1 && t.Enabled).ToList();
            foreach (var t in tasks)
            {

                string timeoutStr = string.IsNullOrEmpty(t.IdleTimeout) ? textBox4.Text : t.IdleTimeout;
                if (!TimeSpan.TryParse(timeoutStr, out var threshold)) continue;
                if (idleMs >= (long)threshold.TotalMilliseconds)
                {
                    ExecuteCommand(t.Action, 1);
                    t.Enabled = false;
                    anyFired = true;
                }
            }
            if (anyFired) { SaveConfigFile(); RefreshTaskGrid(); }


            if (!_taskList.Any(t => t.ActivatorType == 1 && t.Enabled))
            {
                timer1.Stop();
                _lastSecond1 = -1;
                if (anyFired && !checkBox10.Checked) textBox5.Text = "Akcja wykonana.";
                return;
            }


            if (!checkBox10.Checked)
            {
                var pending = _taskList
                    .Where(t => t.ActivatorType == 1 && t.Enabled && TimeSpan.TryParse(
                        string.IsNullOrEmpty(t.IdleTimeout) ? textBox4.Text : t.IdleTimeout, out _))
                    .Select(t => TimeSpan.Parse(string.IsNullOrEmpty(t.IdleTimeout) ? textBox4.Text : t.IdleTimeout))
                    .OrderBy(ts => ts)
                    .FirstOrDefault();
                if (pending != default)
                {
                    var remaining = pending - TimeSpan.FromMilliseconds(idleMs);
                    int sec1 = (int)remaining.TotalSeconds;
                    if (sec1 != _lastSecond1)
                    {
                        _lastSecond1 = sec1;
                        textBox5.Text = remaining.TotalSeconds > 0
                            ? "Pozostały czas: " + remaining.ToString(@"hh\:mm\:ss")
                            : "Gotowe...";
                    }
                }
            }
        }

        public void StartTimer1()
        {
            timer1EndTime = DateTime.Now.Add(TimeSpan.Parse(textBox4.Text));
            timer1.Start();
        }


        private void Timer2_Tick(object sender, EventArgs e)
        {
            var now = DateTime.UtcNow;
            bool anyFired = false;


            var stopers = _taskList.Where(t => t.ActivatorType == 2 && t.Enabled && t.EndTimeUtc != default).ToList();
            foreach (var t in stopers)
            {
                if (t.EndTimeUtc <= now)
                {
                    ExecuteCommand(t.Action, 2);
                    t.Enabled = false;
                    anyFired = true;
                }
            }
            if (anyFired) { SaveConfigFile(); RefreshTaskGrid(); }


            var remaining = _taskList.Where(t => t.ActivatorType == 2 && t.Enabled && t.EndTimeUtc > now).ToList();
            if (!remaining.Any())
            {
                timer2.Stop();
                _lastSecond2 = -1;
                if (!anyFired) textBox9.Text = "00:00:00";
                return;
            }


            var nearest = remaining.OrderBy(t => t.EndTimeUtc).First();
            var rem = nearest.EndTimeUtc - now;
            int sec2 = (int)rem.TotalSeconds;
            if (sec2 != _lastSecond2)
            {
                _lastSecond2 = sec2;
                if (!checkBox10.Checked)
                    textBox9.Text = "Następna: " + rem.ToString(@"hh\:mm\:ss");
            }
        }

        private void Timer1_Tick_Reset(object sender, EventArgs e)
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = LASTINPUTINFO.SizeOf;
            GetLastInputInfo(ref lastInputInfo);

            long idleTime = (long)(Environment.TickCount64 - lastInputInfo.dwTime);

            if (idleTime < 1000 && TimeSpan.TryParse(textBox4.Text, out var ts4r))
                timer1EndTime = DateTime.Now.Add(ts4r);

            var remainingTime = timer1EndTime - DateTime.Now;
            if (remainingTime.TotalSeconds <= 0)
            {

                var tasks = _taskList.Where(t => t.ActivatorType == 1 && t.Enabled).ToList();
                foreach (var t in tasks)
                    ExecuteCommand(t.Action, 1);
                if (!tasks.Any() && comboBox1.SelectedItem != null)
                    ExecuteCommand(comboBox1.SelectedItem.ToString(), 1);

                if (TimeSpan.TryParse(textBox4.Text, out var ts4Reset))
                    timer1EndTime = DateTime.Now.Add(ts4Reset);
            }
            else if (!checkBox10.Checked)
            {
                textBox5.Text = "Pozostały czas: " + remainingTime.ToString(@"hh\:mm\:ss");
            }
        }


        private void ExecuteCommand(string command, int activator = 0)
        {

            string _extraCtx = command switch
            {
                "Odtwórz dźwięk"            => Path.GetFileName(appSettings["soundFile"] ?? ""),
                "Otwórz stronę internetową" => appSettings[$"act{activator}_url"] ?? "",
                "Uruchom polecenie"         => appSettings[$"act{activator}_cmd"] ?? appSettings["userSet"] ?? "",
                "Oczyść pliki tymczasowe"   => BuildCleanupSummary(),
                _                           => ""
            };
            ShowActivationBalloon(command, activator, _extraCtx);


            string actKey = activator > 0 ? $"act{activator}" : null;
            string globalCmd = System.Net.WebUtility.HtmlDecode(appSettings["userSet"] ?? "");
            string userCommand = (actKey != null && !string.IsNullOrEmpty(appSettings[$"{actKey}_cmd"]))
                ? appSettings[$"{actKey}_cmd"]
                : globalCmd;

            switch (command)
            {
                case "Odtwórz dźwięk":

                    PlaySoundFile((appSettings["soundFile"] ?? "").Trim());
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
                        process?.WaitForExit();
                    }
                    else
                    {
                        Process.Start(psi);
                    }
                    break;
                case "Wyłącz monitor":

                    var _monHandle = this.Handle;
                    Task.Run(() => {
                        System.Threading.Thread.Sleep(300);
                        SendMessage(_monHandle, 0x0112, new IntPtr(0xF170), new IntPtr(2));
                    });
                    break;
                case "Uśpienie":
                    Process.Start("Rundll32.exe", "powrprof.dll,SetSuspendState 0,1,0");
                    break;
                case "Wylogowanie":
                    Process.Start("shutdown", "/l");
                    break;
                case "Restart":
                    Process.Start("shutdown", "/r /t 0");
                    break;
                case "Wyłączenie komputera":
                    Process.Start("shutdown", "/s /t 0");
                    break;
                case "Hibernacja":
                    Process.Start("shutdown", "/h");
                    break;
                case "Blokada użytkownika":
                    Process.Start("Rundll32.exe", "user32.dll,LockWorkStation");
                    break;

                case "Oczyść pliki tymczasowe":

                    var cleanSettings = new System.Collections.Generic.Dictionary<string, string>();
                    foreach (string ck in new[] { "cleanTemp","cleanWinTemp","cleanDownloads","cleanWU",
                                                   "cleanDriverCache","cleanThumbs","cleanPrefetch",
                                                   "cleanRecycleBin","cleanEventLog","cleanCustomFolders" })
                        cleanSettings[ck] = appSettings[ck] ?? "0";

                    Task.Run(() =>
                    {
                        int deleted = 0;


                        void SafeDeleteDir(string dir)
                        {
                            if (!Directory.Exists(dir)) return;
                            try
                            {
                                foreach (var f in Directory.EnumerateFiles(dir))
                                    try { File.Delete(f); deleted++; } catch { }
                            }
                            catch { }
                            try
                            {
                                foreach (var sub in Directory.EnumerateDirectories(dir))
                                    try { Directory.Delete(sub, true); deleted++; } catch { }
                            }
                            catch { }
                        }

                        void SafeDeleteFiles(string dir, string pattern)
                        {
                            if (!Directory.Exists(dir)) return;
                            try
                            {
                                foreach (var f in Directory.EnumerateFiles(dir, pattern))
                                    try { File.Delete(f); deleted++; } catch { }
                            }
                            catch { }
                        }

                        void SafeDeleteRecursive(string dir)
                        {
                            if (!Directory.Exists(dir)) return;
                            try
                            {
                                foreach (var f in Directory.EnumerateFiles(dir))
                                    try { File.Delete(f); deleted++; } catch { }
                            }
                            catch { }
                            try
                            {
                                foreach (var sub in Directory.EnumerateDirectories(dir))
                                    try { SafeDeleteRecursive(sub); } catch { }
                            }
                            catch { }
                        }

                        try
                        {
                            if (cleanSettings["cleanTemp"] != "0")
                                SafeDeleteDir(Path.GetTempPath());

                            if (cleanSettings["cleanWinTemp"] != "0")
                                SafeDeleteDir(Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"));

                            if (cleanSettings["cleanDownloads"] == "1")
                                SafeDeleteFiles(Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"), "*");

                            if (cleanSettings["cleanWU"] == "1")
                                SafeDeleteRecursive(Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                                    "SoftwareDistribution", "Download"));

                            if (cleanSettings["cleanDriverCache"] == "1")
                                SafeDeleteDir(Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                                    "System32", "DriverStore", "Temp"));

                            if (cleanSettings["cleanThumbs"] == "1")
                                SafeDeleteFiles(Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                    "Microsoft", "Windows", "Explorer"), "thumbcache_*.db");

                            if (cleanSettings["cleanPrefetch"] == "1")
                                SafeDeleteFiles(Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch"), "*.pf");

                            if (cleanSettings["cleanRecycleBin"] == "1")
                                try
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = "powershell.exe",
                                        Arguments = "-NoProfile -Command \"Clear-RecycleBin -Force -ErrorAction SilentlyContinue\"",
                                        CreateNoWindow = true,
                                        UseShellExecute = false
                                    });
                                }
                                catch { }

                            if (cleanSettings["cleanEventLog"] == "1")
                                try
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = "wevtutil.exe",
                                        Arguments = "cl System",
                                        CreateNoWindow = true,
                                        UseShellExecute = false
                                    })?.WaitForExit();
                                }
                                catch { }
                        }
                        catch { }


                        if (!string.IsNullOrEmpty(cleanSettings["cleanCustomFolders"]))
                            foreach (var customDir in cleanSettings["cleanCustomFolders"]
                                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                                .Where(d => !string.IsNullOrWhiteSpace(d)))
                                SafeDeleteDir(customDir);

                        BeginInvoke(new Action(() =>
                            MessageBox.Show(
                                $"Czyszczenie zakończone.\nUsunięto {deleted} plików/folderów tymczasowych.",
                                "Wyłącznik czasowy – Czyszczenie", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                    });
                    break;

                case "Zapisz notatkę na pulpicie":
                    try
                    {
                        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        string noteFile = Path.Combine(desktop, $"Notatka_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                        string noteContent = actKey != null ? (appSettings[$"{actKey}_note"] ?? "") : "";
                        string content = $"Wyłącznik czasowy – wpis wygenerowany automatycznie\r\n" +
                                         $"Data/godzina: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\r\n" +
                                         $"Komputer: {Environment.MachineName} | Użytkownik: {Environment.UserName}\r\n";
                        if (!string.IsNullOrEmpty(noteContent))
                            content += $"\r\n--- Treść notatki ---\r\n{noteContent}\r\n";
                        File.WriteAllText(noteFile, content, System.Text.Encoding.UTF8);
                    }
                    catch { }
                    break;

                case "Otwórz stronę WWW":
                    try
                    {
                        string urlVal = actKey != null ? (appSettings[$"{actKey}_url"] ?? "") : "";
                        if (string.IsNullOrEmpty(urlVal)) urlVal = userCommand.Trim();
                        if (!urlVal.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                            !urlVal.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                            urlVal = "https://" + urlVal;
                        if (!string.IsNullOrEmpty(urlVal))
                            Process.Start(new ProcessStartInfo { FileName = urlVal, UseShellExecute = true });
                    }
                    catch { }
                    break;

                case "Opróżnij kosz":
                    try
                    {
                        var psiRb = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments = "-NoProfile -Command \"Clear-RecycleBin -Force -ErrorAction SilentlyContinue\"",
                            CreateNoWindow = true,
                            UseShellExecute = false
                        };
                        Process.Start(psiRb);
                    }
                    catch { }
                    break;

                case "Wykonaj zrzut ekranu":
                    Task.Run(() => TakeScreenshots());
                    break;
            }

        }

        private void SetAutoStart(bool enabled)
        {
            string startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = Path.Combine(startupFolder, "Wyłącznik czasowy.lnk");

            if (enabled)
            {
                if (!File.Exists(shortcutPath))
                    CreateShortcutNative(shortcutPath, Application.ExecutablePath, "--minimized");
            }
            else
            {
                if (File.Exists(shortcutPath))
                {
                    File.Delete(shortcutPath);
                }
            }
        }


        private void StopSound()
        {
            try
            {
                mciSendString("stop mci_sound", null, 0, IntPtr.Zero);
                mciSendString("close mci_sound", null, 0, IntPtr.Zero);
            }
            catch { }

            try { new System.Media.SoundPlayer().Stop(); } catch { }
        }


        private void ShowActivationBalloon(string actionName, int activator = 0, string extraContext = "")
        {
            if (appSettings["noBalloon"] == "1") return;
            try
            {
                string now   = DateTime.Now.ToString("HH:mm");
                string today = DateTime.Now.ToString("dd.MM.yyyy");

                string body = activator switch
                {
                    1 => $"Akcja \"{actionName}\" uruchomiona o {now} ({today}) po wykryciu bezczynności" +
                         $" przez {(TimeSpan.TryParse(appSettings["timer1ActivateBy"], out var ts1) ? (int)ts1.TotalMinutes : 0)} min.",
                    2 => $"Akcja \"{actionName}\" uruchomiona o {now} ({today}) po upłynięciu ustawionego czasu.",
                    3 => $"Akcja \"{actionName}\" uruchomiona o {now} ({today}) zgodnie z harmonogramem ({GetScheduleDescription()}).",
                    _ => $"Akcja \"{actionName}\" wykonana o {now} ({today})."
                };

                if (!string.IsNullOrEmpty(extraContext))
                    body += $" Dotyczy: {extraContext}";


                if (InvokeRequired)
                    BeginInvoke(new Action(() => ShowCustomToast(body, actionName, activator)));
                else
                    ShowCustomToast(body, actionName, activator);
            }
            catch { }
        }


        private void TakeScreenshots()
        {
            try
            {

                string fmt     = appSettings["ss_format"] ?? "PNG";
                int count      = int.TryParse(appSettings["ss_count"],    out int sc) ? Math.Max(1, sc) : 1;
                int delayMs    = int.TryParse(appSettings["ss_delay"],    out int sd) ? sd : 0;
                int intervalMs = int.TryParse(appSettings["ss_interval"], out int si) ? si : 1000;
                bool compress  = appSettings["ss_compress"] == "1";
                string folder  = appSettings["ss_folder"] ?? "";
                if (string.IsNullOrWhiteSpace(folder))
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                Directory.CreateDirectory(folder);

                System.Drawing.Imaging.ImageFormat imgFmt = fmt.ToUpperInvariant() switch
                {
                    "JPG" or "JPEG" => System.Drawing.Imaging.ImageFormat.Jpeg,
                    "BMP"           => System.Drawing.Imaging.ImageFormat.Bmp,
                    "GIF"           => System.Drawing.Imaging.ImageFormat.Gif,
                    _               => System.Drawing.Imaging.ImageFormat.Png
                };
                string ext = fmt.ToLowerInvariant();
                if (ext == "jpeg") ext = "jpg";

                if (delayMs > 0) System.Threading.Thread.Sleep(delayMs);

                var bounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
                var files  = new System.Collections.Generic.List<string>();

                for (int i = 0; i < count; i++)
                {
                    if (i > 0) System.Threading.Thread.Sleep(intervalMs);
                    using var bmp = new Bitmap(bounds.Width, bounds.Height);
                    using var g   = Graphics.FromImage(bmp);
                    g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    string ts   = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                    string path = Path.Combine(folder, $"screenshot_{ts}.{ext}");
                    bmp.Save(path, imgFmt);
                    files.Add(path);
                }

                if (compress && files.Count > 0)
                {
                    string zipPath = Path.Combine(folder, $"screenshots_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
                    using var zip = new ZipArchive(File.Create(zipPath), ZipArchiveMode.Create);
                    foreach (var f in files)
                    { zip.CreateEntryFromFile(f, Path.GetFileName(f)); File.Delete(f); }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    BeginInvoke(new Action(() =>
                        MessageBox.Show($"Błąd zrzutu ekranu:\n{ex.Message}", "Błąd",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
                catch { }
            }
        }


        private void ShowCustomToast(string message, string actionName = "", int activator = 0)
        {
            bool dark      = appSettings["darkModeEnabled"] == "1";
            Color bgColor  = dark ? Color.FromArgb(45, 45, 48)    : Color.FromArgb(245, 245, 248);
            Color fgColor  = dark ? Color.FromArgb(230, 230, 230)  : Color.FromArgb(30, 30, 30);
            Color subColor = dark ? Color.FromArgb(160, 160, 160)  : Color.FromArgb(100, 100, 100);
            Color barColor = activator switch
            {
                1 => Color.FromArgb(0, 150, 136),
                2 => Color.FromArgb(33, 150, 243),
                3 => Color.FromArgb(156, 39, 176),
                _ => Color.FromArgb(0, 122, 204)
            };
            string icon  = activator switch { 1 => "💤", 2 => "⏱", 3 => "📅", _ => "🔔" };
            string title = string.IsNullOrEmpty(actionName) ? "Akcja aktywowana" : $"Akcja: {actionName}";

            int toastW = 420, toastH = 140;
            var wa = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);

            var toast = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition   = FormStartPosition.Manual,
                TopMost         = true,
                ShowInTaskbar   = false,
                ClientSize      = new Size(toastW, toastH),
                BackColor       = bgColor,
                Opacity         = 0
            };

            var bar = new System.Windows.Forms.Label
            { BackColor = barColor, Location = new Point(0, 0), Size = new Size(5, toastH) };
            toast.Controls.Add(bar);

            var lblIcon = new System.Windows.Forms.Label
            {
                Text = icon, Font = new Font("Segoe UI Emoji", 18f),
                ForeColor = barColor, BackColor = Color.Transparent,
                Location = new Point(12, 14), Size = new Size(36, 36), AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            toast.Controls.Add(lblIcon);

            var lblTitle = new System.Windows.Forms.Label
            {
                Text = title, Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = fgColor, BackColor = Color.Transparent,
                Location = new Point(54, 10), Size = new Size(toastW - 54 - 28, 20), AutoSize = false
            };
            toast.Controls.Add(lblTitle);

            var lblMsg = new System.Windows.Forms.Label
            {
                Text = message, Font = new Font("Segoe UI", 8.5f),
                ForeColor = subColor, BackColor = Color.Transparent,
                Location = new Point(54, 34), Size = new Size(toastW - 60, 52), AutoSize = false
            };
            toast.Controls.Add(lblMsg);

            var btnX = new System.Windows.Forms.Button
            {
                Text = "✕", FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8f),
                ForeColor = subColor, BackColor = Color.Transparent,
                Location = new Point(toastW - 26, 4), Size = new Size(22, 22), TabStop = false
            };
            btnX.FlatAppearance.BorderSize = 0;
            btnX.Click += (s2, ev2) => { try { toast.Close(); } catch { } };
            toast.Controls.Add(btnX);

            var lblCountdown = new System.Windows.Forms.Label
            {
                Text = "Zostanie zamknięte za 15 sek.",
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = subColor, BackColor = Color.Transparent,
                Location = new Point(54, 90), Size = new Size(toastW - 80, 16), AutoSize = false
            };
            toast.Controls.Add(lblCountdown);

            var progressBg = new System.Windows.Forms.Panel
            {
                BackColor = dark ? Color.FromArgb(70, 70, 73) : Color.FromArgb(215, 215, 218),
                Location  = new Point(0, toastH - 4), Size = new Size(toastW, 4)
            };
            toast.Controls.Add(progressBg);

            var progressFg = new System.Windows.Forms.Panel
            {
                BackColor = barColor,
                Location  = new Point(0, toastH - 4), Size = new Size(toastW, 4)
            };
            toast.Controls.Add(progressFg);

            int finalX = wa.Right - toastW - 12;
            int finalY = wa.Bottom - toastH - 12;
            toast.Location = new Point(finalX, wa.Bottom + 5);

            int totalMs = 15000, remainMs = totalMs;
            bool paused = false;

            void OnEnter(object? s2, EventArgs ev2) { paused = true; }
            void OnLeave(object? s2, EventArgs ev2)
            {
                if (!toast.IsDisposed)
                    paused = toast.Bounds.Contains(Cursor.Position);
            }

            foreach (Control c in toast.Controls)
            { c.MouseEnter += OnEnter; c.MouseLeave += OnLeave; }
            toast.MouseEnter += OnEnter;
            toast.MouseLeave += OnLeave;

            toast.Show();


            int slideSteps = 12, slideStep = 0;
            double totalSlide = wa.Bottom + 5 - finalY;
            var slideTimer = new System.Windows.Forms.Timer { Interval = 16 };
            slideTimer.Tick += (s2, ev2) =>
            {
                if (slideStep >= slideSteps || toast.IsDisposed) { slideTimer.Stop(); slideTimer.Dispose(); return; }
                slideStep++;
                double t    = (double)slideStep / slideSteps;
                double ease = 1 - Math.Pow(1 - t, 3);
                int newY    = (int)(wa.Bottom + 5 - totalSlide * ease);
                toast.Location = new Point(finalX, newY);
                toast.Opacity  = 0.96 * ease;
            };
            slideTimer.Start();


            var ct = new System.Windows.Forms.Timer { Interval = 100 };
            ct.Tick += (s2, ev2) =>
            {
                if (toast.IsDisposed) { ct.Stop(); ct.Dispose(); return; }
                if (!paused) remainMs -= 100;
                if (remainMs <= 0)
                {
                    ct.Stop(); ct.Dispose();
                    try { if (!toast.IsDisposed) toast.Close(); } catch { }
                    return;
                }
                int secs = (remainMs + 999) / 1000;
                lblCountdown.Text = $"Zostanie zamknięte za {secs} sek.";
                progressFg.Width  = Math.Max(0, (int)(toastW * (double)remainMs / totalMs));
            };
            ct.Start();
        }


        private string GetScheduleDescription()
        {

            if (_currentlyFiringTask != null && !string.IsNullOrEmpty(_currentlyFiringTask.RepeatIntervalType))
            {
                string unit = _currentlyFiringTask.RepeatIntervalType switch
                { "h" => "godzin", "d" => "dni", _ => "minut" };
                return $"co {_currentlyFiringTask.RepeatIntervalValue} {unit}";
            }

            var days = (appSettings["timer3CheckedDays"] ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => int.TryParse(p.Trim(), out int i) && i >= 0 && i < _scheduleLabels.Length
                    ? _scheduleLabels[i] : null)
                .Where(s => s != null);
            string time = appSettings["timer3HourActivateBy"] ?? "?";
            return string.Join(", ", days) + " o godz. " + time;
        }


        private string BuildCleanupSummary()
        {
            var map = new (string Key, string Label)[]
            {
                ("cleanTemp",        "%TEMP%"),
                ("cleanWinTemp",     "Windows\\Temp"),
                ("cleanDownloads",   "Pobrane"),
                ("cleanWU",          "Windows Update"),
                ("cleanDriverCache", "Sterowniki"),
                ("cleanThumbs",      "Miniatury"),
                ("cleanPrefetch",    "Prefetch"),
                ("cleanRecycleBin",  "Kosz"),
                ("cleanEventLog",    "Logi"),
            };
            var enabled = map.Where(x => appSettings[x.Key] != "0").Select(x => x.Label);
            var result = string.Join(", ", enabled);
            return result.Length > 0 ? result : "brak wybranych opcji";
        }


        private void UpdateComboHints(System.Windows.Forms.ComboBox combo,
            System.Windows.Forms.Label tipLabel, System.Windows.Forms.Button btnSettings)
        {
            int idx = combo.SelectedIndex;
            string desc = (idx >= 0 && idx < _actionDescriptions.Length)
                ? _actionDescriptions[idx]
                : "Wybierz akcję z listy, aby zobaczyć jej opis.";

            if (tipLabel != null)
                tipLabel.Text = "WSKAZÓWKA\r\n\r\n" + desc;

            if (btnSettings != null)
            {
                btnSettings.Enabled = ActionHasParams(idx);
                _actionTip.SetToolTip(btnSettings, desc);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            appSettings["defaultAction1"] = comboBox1.SelectedIndex.ToString();
            SaveConfigFile();
            UpdateComboHints(comboBox1, tip01, _btnActSettings1);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            appSettings["defaultAction2"] = comboBox2.SelectedIndex.ToString();
            SaveConfigFile();
            UpdateComboHints(comboBox2, tip02, _btnActSettings2);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            appSettings["defaultAction3"] = comboBox3.SelectedIndex.ToString();
            SaveConfigFile();
            UpdateComboHints(comboBox3, tip03, _btnActSettings3);
        }


        private static readonly string[] _actionDescriptions = {
            "Odtwarza plik dźwiękowy. Możesz wskazać własny plik (WAV, MP3, AAC, OGG).\nJeśli nic nie wybrano – program użyje pliku z folderu 'sounds/' lub systemowego dźwięku.",
            "Uruchamia polecenie lub program. Możesz ustawić oddzielne polecenie\ndla każdego aktywatora (niezależnie od globalnego ustawienia).",
            "Wyłącza ekran monitora bez usypiania komputera.",
            "Przełącza komputer w tryb uśpienia.",
            "Wylogowuje bieżącego użytkownika z systemu Windows.",
            "Restartuje komputer natychmiast.",
            "Wyłącza komputer natychmiast.",
            "Hibernuje komputer – stan RAM zapisywany na dysk, następnie wyłączenie. Opcja zadziała tylko jeśli hibernacja jest włączona w systemie.",
            "Blokuje ekran użytkownika. Ekran blokady może wymagać wprowadzenia hasła aby zalogować się do systemu.",
            "Usuwa całą zawartość niepotrzebnych plików tymczasowych.",
            "Tworzy plik tekstowy na pulpicie z datą, godziną i opcjonalną treścią notatki.",
            "Otwiera wskazany adres URL w domyślnej przeglądarce.",
            "Opróżnia Kosz bez potwierdzenia.",
            "Wykonuje zrzut ekranu i zapisuje go w wybranym folderze.\nMożna ustawić format (PNG/JPG/BMP/GIF), liczbę zrzutów, opóźnienie, interwał i kompresję ZIP.",
        };

        private static bool ActionHasParams(int idx) => idx == 0 || idx == 1 || idx == 9 || idx == 10 || idx == 11 || idx == 13;

        private void ShowActionSettingsDialog(System.Windows.Forms.ComboBox combo, int activatorNum)
        {
            int idx = combo.SelectedIndex;
            string desc = (idx >= 0 && idx < _actionDescriptions.Length) ? _actionDescriptions[idx] : "";


            if (idx == 0) { ShowSoundSettingsDialog(); return; }

            if (idx == 9) { ShowCleanupSettingsDialog(); return; }

            if (idx == 13) { ShowScreenshotSettingsDialog(); return; }

            string actKey = $"act{activatorNum}";
            string paramLabel = idx == 1 ? "Polecenie lub ścieżka do pliku/skryptu:" :
                                idx == 10 ? "Treść notatki (opcjonalnie – zapisana w pliku TXT):" :
                                idx == 11 ? "Adres URL do otwarcia (np. https://www.example.com):" : "";
            string configKey = idx == 1 ? $"{actKey}_cmd" :
                               idx == 10 ? $"{actKey}_note" :
                               idx == 11 ? $"{actKey}_url" : "";

            bool isNote = idx == 10;
            bool hasParams = ActionHasParams(idx);

            var dlg = new Form
            {
                Text = $"Ustawienia akcji – {(combo.SelectedItem?.ToString() ?? "")}",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = this.BackColor
            };

            var lblDesc = new System.Windows.Forms.Label
            { Text = desc, Location = new Point(12, 10), Size = new Size(456, 50), AutoSize = false };
            dlg.Controls.Add(lblDesc);

            if (!hasParams)
            {
                dlg.ClientSize = new Size(480, 140);
                var btnOk0 = new System.Windows.Forms.Button
                { Text = "Zamknij", DialogResult = DialogResult.OK, Location = new Point(185, 98), Size = new Size(90, 28) };
                dlg.Controls.Add(btnOk0); dlg.AcceptButton = btnOk0;
                if (checkBox11.Checked) ApplyThemeToControls(dlg.Controls, Color.FromArgb(30, 30, 30), Color.FromArgb(220, 220, 220), Color.FromArgb(55, 55, 55), true);
                dlg.ShowDialog(this); return;
            }

            var lblParam = new System.Windows.Forms.Label { Text = paramLabel, Location = new Point(12, 68), AutoSize = true };
            dlg.Controls.Add(lblParam);

            int txtW = idx == 1 ? 364 : 456;
            var txtValue = new TextBox
            {
                Text = configKey.Length > 0 ? (appSettings[configKey] ?? "") : "",
                Location = new Point(12, 88),
                Size = new Size(txtW, isNote ? 80 : 25),
                Multiline = isNote,
                ScrollBars = isNote ? ScrollBars.Vertical : ScrollBars.None
            };
            dlg.Controls.Add(txtValue);

            if (idx == 1)
            {
                var btnBrw = new System.Windows.Forms.Button
                { Text = "📂", Location = new Point(txtValue.Right + 4, 88), Size = new Size(30, 25), FlatStyle = FlatStyle.Flat };
                btnBrw.Click += (s, ev) =>
                {
                    using var ofd = new OpenFileDialog { Filter = "Programy i skrypty|*.exe;*.bat;*.cmd;*.ps1;*.vbs|Wszystkie pliki|*.*", Title = "Wybierz plik do uruchomienia" };
                    if (ofd.ShowDialog() == DialogResult.OK) txtValue.Text = ofd.FileName;
                };
                dlg.Controls.Add(btnBrw);
            }

            int btnY = isNote ? (88 + 80 + 12) : 124;
            dlg.ClientSize = new Size(480, btnY + 48);

            var btnSave = new System.Windows.Forms.Button { Text = "Zapisz", Location = new Point(12, btnY), Size = new Size(90, 28), FlatStyle = FlatStyle.Flat };
            var btnCancel2 = new System.Windows.Forms.Button { Text = "Anuluj", Location = new Point(108, btnY), Size = new Size(90, 28), FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };
            btnSave.Click += (s, ev) => { appSettings[configKey] = txtValue.Text.Trim(); SaveConfigFile(); dlg.DialogResult = DialogResult.OK; dlg.Close(); };
            dlg.Controls.Add(btnSave); dlg.Controls.Add(btnCancel2);
            dlg.AcceptButton = btnSave; dlg.CancelButton = btnCancel2;
            if (checkBox11.Checked) ApplyThemeToControls(dlg.Controls, Color.FromArgb(30, 30, 30), Color.FromArgb(220, 220, 220), Color.FromArgb(55, 55, 55), true);
            dlg.ShowDialog(this);
        }


        private void ShowScreenshotSettingsDialog()
        {

            string fmtKey = "ss_format";
            string cntKey = "ss_count";
            string dlyKey = "ss_delay";
            string intKey = "ss_interval";
            string zipKey = "ss_compress";
            string dirKey = "ss_folder";

            var dlg = new Form
            {
                Text = "Ustawienia – Wykonaj zrzut ekranu",
                ClientSize = new Size(420, 290),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false, MinimizeBox = false,
                BackColor = this.BackColor
            };

            int row = 14;
            void AddRow(string lbl, Control ctrl)
            {
                var lbCtrl = new System.Windows.Forms.Label { Text = lbl, Location = new Point(12, row + 3), AutoSize = true };
                dlg.Controls.Add(lbCtrl);
                ctrl.Location = new Point(150, row);
                ctrl.Size = new Size(248, 23);
                dlg.Controls.Add(ctrl);
                row += 32;
            }

            var cboFmt = new System.Windows.Forms.ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cboFmt.Items.AddRange(new object[] { "PNG", "JPG", "BMP", "GIF" });
            string curFmt = appSettings[fmtKey] ?? "PNG";
            cboFmt.SelectedIndex = Math.Max(0, cboFmt.Items.IndexOf(curFmt));
            AddRow("Format pliku:", cboFmt);

            var nudCount = new System.Windows.Forms.NumericUpDown
            { Minimum = 1, Maximum = 99, Value = int.TryParse(appSettings[cntKey], out int cv) ? Math.Max(1, cv) : 1 };
            AddRow("Liczba zrzutów:", nudCount);

            var nudDelay = new System.Windows.Forms.NumericUpDown
            { Minimum = 0, Maximum = 60000, Increment = 500, Value = int.TryParse(appSettings[dlyKey], out int dv) ? dv : 0 };
            AddRow("Opóźnienie (ms):", nudDelay);

            var nudInterval = new System.Windows.Forms.NumericUpDown
            { Minimum = 100, Maximum = 60000, Increment = 500, Value = int.TryParse(appSettings[intKey], out int iv) ? iv : 1000 };
            AddRow("Interwał (ms):", nudInterval);

            var chkZip = new System.Windows.Forms.CheckBox
            { Text = "Spakuj do pliku ZIP", Checked = appSettings[zipKey] == "1", Location = new Point(150, row), AutoSize = true };
            dlg.Controls.Add(chkZip);
            row += 32;

            var txtFolder = new System.Windows.Forms.TextBox { Text = appSettings[dirKey] ?? "" };
            txtFolder.Location = new Point(150, row);
            txtFolder.Size = new Size(215, 23);
            var lbFolder = new System.Windows.Forms.Label { Text = "Folder docelowy:", Location = new Point(12, row + 3), AutoSize = true };
            dlg.Controls.Add(lbFolder);
            dlg.Controls.Add(txtFolder);
            var btnBrowse = new System.Windows.Forms.Button
            { Text = "…", Location = new Point(368, row), Size = new Size(28, 23), FlatStyle = FlatStyle.Flat };
            dlg.Controls.Add(btnBrowse);
            btnBrowse.Click += (s2, ev2) =>
            {
                using var fbd = new FolderBrowserDialog { SelectedPath = txtFolder.Text };
                if (fbd.ShowDialog() == DialogResult.OK) txtFolder.Text = fbd.SelectedPath;
            };
            row += 36;

            var btnOk2 = new System.Windows.Forms.Button
            { Text = "Zapisz", Location = new Point(150, row), Size = new Size(88, 28), FlatStyle = FlatStyle.Flat };
            var btnCancel3 = new System.Windows.Forms.Button
            { Text = "Anuluj", Location = new Point(244, row), Size = new Size(88, 28), FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };
            btnOk2.Click += (s2, ev2) =>
            {
                appSettings[fmtKey] = cboFmt.Text;
                appSettings[cntKey] = ((int)nudCount.Value).ToString();
                appSettings[dlyKey] = ((int)nudDelay.Value).ToString();
                appSettings[intKey] = ((int)nudInterval.Value).ToString();
                appSettings[zipKey] = chkZip.Checked ? "1" : "0";
                appSettings[dirKey] = txtFolder.Text.Trim();
                SaveConfigFile();
                dlg.DialogResult = DialogResult.OK;
                dlg.Close();
            };
            dlg.Controls.Add(btnOk2);
            dlg.Controls.Add(btnCancel3);
            dlg.AcceptButton = btnOk2;
            dlg.CancelButton = btnCancel3;
            dlg.ClientSize = new Size(420, row + 48);

            if (checkBox11.Checked)
                ApplyThemeToControls(dlg.Controls, Color.FromArgb(30, 30, 30), Color.FromArgb(220, 220, 220), Color.FromArgb(55, 55, 55), true);
            dlg.ShowDialog(this);
        }


        private (string type, int value) ShowRepeatIntervalDialog()
        {
            bool dark    = appSettings["darkModeEnabled"] == "1";
            string sType = appSettings["timer3_repeat_type"]  ?? "m";
            int    sVal  = int.TryParse(appSettings["timer3_repeat_value"], out int sv) ? sv : 30;

            var dlg = new Form
            {
                Text            = "Powtarzaj co...",
                ClientSize      = new Size(320, 160),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition   = FormStartPosition.CenterParent,
                MaximizeBox     = false, MinimizeBox = false,
                BackColor       = this.BackColor
            };

            dlg.Controls.Add(new System.Windows.Forms.Label
            { Text = "Powtarzaj akcje co:", Location = new Point(12, 14), AutoSize = true });

            var nud = new System.Windows.Forms.NumericUpDown
            { Minimum = 1, Maximum = 99999, Value = Math.Max(1, sVal),
              Location = new Point(12, 40), Size = new Size(80, 23) };
            dlg.Controls.Add(nud);

            var rbMin  = new System.Windows.Forms.RadioButton { Text = "minut",  Location = new Point(102, 41), AutoSize = true, Checked = sType == "m" };
            var rbHour = new System.Windows.Forms.RadioButton { Text = "godzin", Location = new Point(172, 41), AutoSize = true, Checked = sType == "h" };
            var rbDay  = new System.Windows.Forms.RadioButton { Text = "dni",    Location = new Point(242, 41), AutoSize = true, Checked = sType == "d" };
            dlg.Controls.Add(rbMin); dlg.Controls.Add(rbHour); dlg.Controls.Add(rbDay);


            if (!rbMin.Checked && !rbHour.Checked && !rbDay.Checked) rbMin.Checked = true;

            string resultType = ""; int resultValue = 0;
            var btnOk = new System.Windows.Forms.Button
            { Text = "Zapisz", Location = new Point(70, 110), Size = new Size(80, 28), FlatStyle = FlatStyle.Flat };
            var btnCancel = new System.Windows.Forms.Button
            { Text = "Anuluj", Location = new Point(162, 110), Size = new Size(80, 28),
              FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };
            btnOk.Click += (s2, ev2) =>
            {
                resultType  = rbMin.Checked ? "m" : rbHour.Checked ? "h" : "d";
                resultValue = (int)nud.Value;
                dlg.DialogResult = DialogResult.OK;
                dlg.Close();
            };
            dlg.Controls.Add(btnOk);
            dlg.Controls.Add(btnCancel);
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;

            if (dark) ApplyThemeToControls(dlg.Controls,
                Color.FromArgb(30, 30, 30), Color.FromArgb(220, 220, 220), Color.FromArgb(55, 55, 55), true);
            dlg.ShowDialog(this);
            return (resultType, resultValue);
        }

        private void ShowSoundSettingsDialog()
        {
            var dlg = new Form
            {
                Text = "Plik dźwiękowy – wspólny dla wszystkich aktywatorów",
                ClientSize = new Size(480, 185),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = this.BackColor
            };
            var lblInfo = new System.Windows.Forms.Label
            {
                Text = "Wybierz plik dźwiękowy odtwarzany przez WSZYSTKIE aktywatory.\nJeśli pole jest puste – użyty zostanie plik z folderu 'sounds/' lub systemowy.",
                Location = new Point(12, 10),
                Size = new Size(456, 40),
                AutoSize = false
            };
            dlg.Controls.Add(lblInfo);
            var lblPath = new System.Windows.Forms.Label { Text = "Ścieżka do pliku (WAV, MP3, AAC, OGG, WMA, FLAC):", Location = new Point(12, 58), AutoSize = true };
            dlg.Controls.Add(lblPath);
            var txtFile = new TextBox { Text = appSettings["soundFile"] ?? "", Location = new Point(12, 76), Size = new Size(396, 25) };
            dlg.Controls.Add(txtFile);
            var btnBrw = new System.Windows.Forms.Button { Text = "📂", Location = new Point(414, 76), Size = new Size(30, 25), FlatStyle = FlatStyle.Flat };
            btnBrw.Click += (s, ev) =>
            {
                using var ofd = new OpenFileDialog { Filter = "Pliki dźwiękowe|*.wav;*.mp3;*.aac;*.ogg;*.wma;*.flac|Wszystkie pliki|*.*", Title = "Wybierz plik dźwiękowy" };
                if (ofd.ShowDialog() == DialogResult.OK) txtFile.Text = ofd.FileName;
            };
            dlg.Controls.Add(btnBrw);
            var btnPlay = new System.Windows.Forms.Button { Text = "▶ Test", Location = new Point(12, 110), Size = new Size(80, 26), FlatStyle = FlatStyle.Flat };
            btnPlay.Click += (s, ev) => PlaySoundFile(txtFile.Text.Trim());
            dlg.Controls.Add(btnPlay);
            var btnStop = new System.Windows.Forms.Button { Text = "⏹ Stop", Location = new Point(98, 110), Size = new Size(80, 26), FlatStyle = FlatStyle.Flat };
            btnStop.Click += (s, ev) => StopSound();
            dlg.Controls.Add(btnStop);
            var btnSave = new System.Windows.Forms.Button { Text = "Zapisz", Location = new Point(12, 146), Size = new Size(90, 28), FlatStyle = FlatStyle.Flat };
            var btnCancel = new System.Windows.Forms.Button { Text = "Anuluj", Location = new Point(108, 146), Size = new Size(90, 28), FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };
            btnSave.Click += (s, ev) => { appSettings["soundFile"] = txtFile.Text.Trim(); SaveConfigFile(); StopSound(); dlg.DialogResult = DialogResult.OK; dlg.Close(); };
            dlg.Controls.Add(btnSave); dlg.Controls.Add(btnCancel);
            dlg.AcceptButton = btnSave; dlg.CancelButton = btnCancel;
            if (checkBox11.Checked) ApplyThemeToControls(dlg.Controls, Color.FromArgb(30, 30, 30), Color.FromArgb(220, 220, 220), Color.FromArgb(55, 55, 55), true);
            dlg.ShowDialog(this);
        }


        private static bool IsAdmin()
        {
            using var id = System.Security.Principal.WindowsIdentity.GetCurrent();
            return new System.Security.Principal.WindowsPrincipal(id)
                .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        private void ShowCleanupSettingsDialog()
        {
            var options = new (string Key, string Label, string Tip, bool NeedsAdmin)[]
            {
                ("cleanTemp",        "Pliki tymczasowe użytkownika (%TEMP%)",               "Folder %TEMP% – tymczasowe pliki programów i systemu",                      false),
                ("cleanWinTemp",     "Pliki tymczasowe systemu Windows (C:\\Windows\\Temp)", "C:\\Windows\\Temp – tymczasowe pliki systemu operacyjnego",                  true),
                ("cleanDownloads",   "Folder Pobrane (Downloads) – pliki użytkownika",       "Usuwa wszystkie pliki z folderu Pobrane (nie foldery!)",                     false),
                ("cleanWU",          "Cache Windows Update (SoftwareDistribution\\Download)","Pobrane, ale niezainstalowane aktualizacje Windows",                         true),
                ("cleanDriverCache", "Cache sterowników (DriverStore\\Temp)",                "Tymczasowe pliki sterowników urządzeń",                                      true),
                ("cleanThumbs",      "Miniatury obrazków (thumbcache_*.db)",                 "Baza miniatur Eksploratora – bezpiecznie usuwalna",                          false),
                ("cleanPrefetch",    "Pliki Prefetch (*.pf)",                                "Pliki przyspieszające uruchamianie programów; system odtworzy je automatycznie", false),
                ("cleanRecycleBin",  "Kosz systemowy (wszystkich dysków)",                   "Opróżnia Kosz – odpowiednik Shift+Del",                                     false),
                ("cleanEventLog",    "Dzienniki zdarzeń Windows (Event Log)",               "Czyści logi systemowe – przydatne do odzyskania miejsca",                   true),
            };

            bool adminNow = IsAdmin();

            var dlg = new Form
            {
                Text = "Czyszczenie plików – wybierz co usuwać",
                ClientSize = new Size(520, options.Length * 28 + 210),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = this.BackColor
            };
            var tip = new System.Windows.Forms.ToolTip();
            var checkboxes = new List<System.Windows.Forms.CheckBox>();
            int y = 10;
            foreach (var opt in options)
            {
                bool disabled = opt.NeedsAdmin && !adminNow;
                var cb = new System.Windows.Forms.CheckBox
                {
                    Text = opt.Label + (disabled ? "  [⚠ wymaga administratora]" : ""),
                    Checked = !disabled && appSettings[opt.Key] != "0",
                    Enabled = !disabled,
                    Location = new Point(12, y),
                    AutoSize = true,
                    Tag = opt.Key,
                    ForeColor = disabled ? Color.Gray : SystemColors.ControlText
                };
                tip.SetToolTip(cb, disabled
                    ? opt.Tip + "\n⚠ Ta opcja wymaga uprawnień administratora."
                    : opt.Tip);
                dlg.Controls.Add(cb);
                checkboxes.Add(cb);
                y += 28;
            }


            y += 6;
            dlg.Controls.Add(new System.Windows.Forms.Label
            {
                Text = "Dodatkowe foldery do wyczyszczenia:",
                Location = new Point(12, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            });
            y += 22;
            var customFolderList = new System.Windows.Forms.ListBox
            {
                Location = new Point(12, y),
                Size = new Size(392, 70),
                BorderStyle = BorderStyle.FixedSingle
            };

            var savedCustom = (appSettings["cleanCustomFolders"] ?? "")
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !string.IsNullOrWhiteSpace(p));
            foreach (var p in savedCustom) customFolderList.Items.Add(p);
            dlg.Controls.Add(customFolderList);

            var btnAddFolder = new System.Windows.Forms.Button
            {
                Text = "+ Dodaj",
                Location = new Point(412, y),
                Size = new Size(96, 32),
                FlatStyle = FlatStyle.Flat
            };
            btnAddFolder.Click += (s, ev) =>
            {
                using var fbd = new FolderBrowserDialog { Description = "Wybierz folder do wyczyszczenia", UseDescriptionForTitle = true };
                if (fbd.ShowDialog(dlg) == DialogResult.OK && !customFolderList.Items.Contains(fbd.SelectedPath))
                    customFolderList.Items.Add(fbd.SelectedPath);
            };
            dlg.Controls.Add(btnAddFolder);

            var btnRemoveFolder = new System.Windows.Forms.Button
            {
                Text = "✕ Usuń",
                Location = new Point(412, y + 36),
                Size = new Size(96, 32),
                FlatStyle = FlatStyle.Flat
            };
            btnRemoveFolder.Click += (s, ev) =>
            {
                if (customFolderList.SelectedIndex >= 0)
                    customFolderList.Items.RemoveAt(customFolderList.SelectedIndex);
            };
            dlg.Controls.Add(btnRemoveFolder);
            y += 78;
            var btnSave = new System.Windows.Forms.Button { Text = "Zapisz", Location = new Point(12, y + 8), Size = new Size(90, 28), FlatStyle = FlatStyle.Flat };
            var btnCancel = new System.Windows.Forms.Button { Text = "Anuluj", Location = new Point(108, y + 8), Size = new Size(90, 28), FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };
            btnSave.Click += (s, ev) =>
            {
                foreach (var cb in checkboxes)
                    appSettings[(string)cb.Tag] = cb.Checked ? "1" : "0";

                appSettings["cleanCustomFolders"] = string.Join(";", customFolderList.Items.Cast<string>());
                SaveConfigFile();
                dlg.DialogResult = DialogResult.OK; dlg.Close();
            };


            var btnAdmin = new System.Windows.Forms.Button
            {
                Text = adminNow ? "✓ Uruchomiony jako administrator" : "🛡 Uruchom jako administrator",
                Enabled = !adminNow,
                Location = new Point(210, y + 8),
                Size = new Size(288, 28),
                FlatStyle = FlatStyle.Flat
            };
            tip.SetToolTip(btnAdmin, adminNow
                ? "Program już działa z uprawnieniami administratora."
                : "Zapisuje ustawienia i restartuje program z uprawnieniami administratora (wymagany monit UAC).");
            btnAdmin.Click += (s, ev) =>
            {

                foreach (var cb in checkboxes)
                    appSettings[(string)cb.Tag] = cb.Checked ? "1" : "0";
                SaveConfigFile();

                try
                {
                    int pid  = Environment.ProcessId;
                    string exe = Application.ExecutablePath.Replace("'", "''");
                    string cmdArgs = $"/c taskkill /F /PID {pid} & timeout /t 2 /nobreak > nul & powershell -WindowStyle Hidden -NonInteractive -Command \"Start-Process -FilePath '{exe}' -Verb RunAs\"";
                    Process.Start(new ProcessStartInfo
                    {
                        FileName        = "cmd.exe",
                        Arguments       = cmdArgs,
                        UseShellExecute = false,
                        CreateNoWindow  = true,
                        WindowStyle     = ProcessWindowStyle.Hidden
                    });
                    Environment.Exit(0);
                }
                catch {  }
            };

            dlg.Controls.Add(btnSave);
            dlg.Controls.Add(btnCancel);
            dlg.Controls.Add(btnAdmin);
            dlg.AcceptButton = btnSave;
            dlg.CancelButton = btnCancel;
            if (checkBox11.Checked) ApplyThemeToControls(dlg.Controls, Color.FromArgb(30, 30, 30), Color.FromArgb(220, 220, 220), Color.FromArgb(55, 55, 55), true);
            dlg.ShowDialog(this);
        }


        private void ShowBackupRestoreDialog()
        {
            bool dark = appSettings["darkModeEnabled"] == "1";
            var dlg = new Form
            {
                Text = "Kopia zapasowa / Przywróć ustawienia",
                ClientSize = new Size(440, 195),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false, MinimizeBox = false,
                BackColor = this.BackColor
            };

            dlg.Controls.Add(new System.Windows.Forms.Label
            {
                Text = "Utwórz lub przywróć kopię zapasową ustawień programu\n(konfiguracja, hasło ochronne). Przy przywracaniu program zostanie zamknięty.",
                Location = new Point(12, 12), Size = new Size(416, 36), AutoSize = false
            });

            var btnBackup = new System.Windows.Forms.Button
            {
                Text = "💾 Utwórz kopię zapasową...",
                Location = new Point(12, 58), Size = new Size(200, 36), FlatStyle = FlatStyle.Flat
            };
            btnBackup.Click += (s, ev) =>
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "Kopia zapasowa Wyłącznik czasowy|*.dat",
                    FileName = $"Wyłącznik czasowy_backup_{DateTime.Now:yyyyMMdd_HHmm}.dat",
                    Title = "Zapisz kopię zapasową"
                };
                if (sfd.ShowDialog() != DialogResult.OK) return;
                try
                {
                    string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                    using var zip = new ZipArchive(File.Create(sfd.FileName), ZipArchiveMode.Create);
                    foreach (var fn in new[] { "config.json", "psd.dat" })
                    {
                        string src = Path.Combine(dataDir, fn);
                        if (File.Exists(src)) zip.CreateEntryFromFile(src, fn);
                    }
                    MessageBox.Show("Kopia zapasowa zapisana pomyślnie.", "Sukces",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd tworzenia kopii:\n{ex.Message}", "Błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            dlg.Controls.Add(btnBackup);

            var btnRestore = new System.Windows.Forms.Button
            {
                Text = "📂 Przywróć z kopii...",
                Location = new Point(224, 58), Size = new Size(200, 36), FlatStyle = FlatStyle.Flat
            };
            btnRestore.Click += (s, ev) =>
            {
                using var ofd = new OpenFileDialog
                {
                    Filter = "Kopia zapasowa Wyłącznik czasowy|*.dat|Wszystkie pliki|*.*",
                    Title = "Wybierz plik kopii zapasowej"
                };
                if (ofd.ShowDialog() != DialogResult.OK) return;
                if (MessageBox.Show("To zastąpi bieżącą konfigurację i zamknie program.\nKontynuować?",
                    "Potwierdzenie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
                try
                {
                    string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                    using var zip = new ZipArchive(File.OpenRead(ofd.FileName), ZipArchiveMode.Read);
                    foreach (var entry in zip.Entries)
                    {
                        string dest = Path.Combine(dataDir, entry.FullName);
                        if (!string.IsNullOrEmpty(Path.GetFileName(dest)))
                            entry.ExtractToFile(dest, true);
                    }
                    MessageBox.Show("Przywracanie zakończone. Program zostanie zamknięty i uruchomiony ponownie.",
                        "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    int pid2 = Environment.ProcessId;
                    string exePath = Application.ExecutablePath;
                    Process.Start(new ProcessStartInfo
                    {
                        FileName        = "cmd.exe",
                        Arguments       = $"/c taskkill /F /PID {pid2} & timeout /t 2 /nobreak > nul & start \"\" \"{exePath}\"",
                        UseShellExecute = false,
                        CreateNoWindow  = true,
                        WindowStyle     = ProcessWindowStyle.Hidden
                    });
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd przywracania:\n{ex.Message}", "Błąd",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            dlg.Controls.Add(btnRestore);

            var btnClose = new System.Windows.Forms.Button
            {
                Text = "Zamknij", Location = new Point(175, 150), Size = new Size(90, 28),
                FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel
            };
            dlg.Controls.Add(btnClose);
            dlg.CancelButton = btnClose;

            if (dark) ApplyThemeToControls(dlg.Controls,
                Color.FromArgb(30, 30, 30), Color.FromArgb(220, 220, 220), Color.FromArgb(55, 55, 55), true);
            dlg.ShowDialog(this);
        }


        private void ShowHelpDialog()
        {
            bool dark = appSettings["darkModeEnabled"] == "1";
            Color bg = dark ? Color.FromArgb(25, 25, 28) : Color.FromArgb(250, 250, 252);
            Color fg = dark ? Color.FromArgb(210, 210, 210) : Color.FromArgb(40, 40, 40);

            var dlg = new Form
            {
                Text = "Dokumentacja Systemowa – Wyłącznik Czasowy",
                ClientSize = new Size(750, 600),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = bg
            };

            var rtb = new RichTextBox
            {
                Location = new Point(10, 10),
                Size = new Size(730, 530),
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = bg,
                ForeColor = fg,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Font = new Font("Segoe UI", 10f),
                // Ustawiamy marginesy w sposób bezpieczny dla .NET
                SelectionIndent = 15,
                SelectionRightIndent = 15
            };
            dlg.Controls.Add(rtb);

            // --- Metody formatowania tekstu ---
            void AddHeader(string text, Color color)
            {
                rtb.SelectionStart = rtb.TextLength;
                rtb.SelectionFont = new Font("Segoe UI", 15f, FontStyle.Bold);
                rtb.SelectionColor = color;
                rtb.AppendText(text + "\n");
            }

            void AddSubHeader(string text)
            {
                rtb.SelectionStart = rtb.TextLength;
                rtb.SelectionFont = new Font("Segoe UI", 11f, FontStyle.Bold);
                rtb.SelectionColor = dark ? Color.WhiteSmoke : Color.FromArgb(60, 60, 60);
                rtb.AppendText(text + "\n");
            }

            void AddParagraph(string text)
            {
                rtb.SelectionStart = rtb.TextLength;
                rtb.SelectionFont = new Font("Segoe UI", 10f, FontStyle.Regular);
                rtb.SelectionColor = fg;
                rtb.AppendText(text + "\n");
            }

            void AddBullet(string text)
            {
                rtb.SelectionStart = rtb.TextLength;
                rtb.SelectionFont = new Font("Segoe UI", 10f, FontStyle.Regular);
                rtb.SelectionColor = dark ? Color.FromArgb(170, 170, 170) : Color.FromArgb(100, 100, 100);
                rtb.AppendText(" • " + text + "\n");
            }

            void AddSpace() { rtb.AppendText("\n"); }

            // --- KOLORYSTYKA SEKCJI ---
            Color accent = Color.FromArgb(0, 120, 215); // Windows Blue
            Color success = Color.FromArgb(40, 167, 69);
            Color warning = Color.FromArgb(255, 133, 27);

            // --- TREŚĆ DOKUMENTACJI ---
            AddHeader("Wyłącznik czasowy", accent);
            AddParagraph("Program Wyłącznik Czasowy to zaawansowane narzędzie do zarządzania stanem komputera, planowania wykonywania działań oraz uruchamiania określonych operacji wywołanych w czasie. ");
            AddSpace();

            AddHeader("1. Aktywatory", success);
            AddSubHeader("Aktywator Bezczynności");
            AddParagraph("System monitoruje aktywność wejściową użytkownika (Input Idle Time).");
            AddBullet("Wymagany format czasu: HH:MM:SS.");
            AddBullet("Licznik jest zerowany natychmiast po wykryciu ruchu myszy lub naciśnięciu klawisza. Wykrywany jest więc na wejściu ruch i aktywność użytkownika.");
            AddBullet("W ustawieniach programu, możesz wymusić automatyczny restart odliczania licznika, gdy operacja zostanie wykonana pomyślnie.");
            AddSpace();

            AddSubHeader("Aktywator Czasowy (Stoper)");
            AddParagraph("Umożliwia zaplanowanie jednorazowej operacji na konkretny moment.");
            AddBullet("Precyzja co do minuty. Program wykona zaplanowaną czynność w przyszłości, którą wybrał użytkownik.");
            AddBullet("Idealny do planowania zadań na konkretną godzinę.");
            AddSpace();

            AddSubHeader("Aktywator harmonogramu");
            AddParagraph("Powtarzalność wykonywanych zadań.");
            AddBullet("Obsługa wielu wariantów powtarzalności takich jak: (dni robocze, weekendy, interwały).");
            AddBullet("Tryb interwałowy: Pozwala na powtarzanie akcji co określoną liczbę minut/godzin.");
            AddBullet("Kolejka harmonogramu zadań- jest jedyną, która pozwala być wstrzymana i wznowiona w dowolnym momencie działania programu.");
            AddSubHeader("Dostępne opcje działania:");
            AddBullet("Aby zatrzymać harmonogram lub wznowić jego działanie, kliknij w menu kontekstowym na pasku zadań odpowiednią opcję, lub skorzystaj z opcji przycisku dostępnym w panelu Harmonogramu.");
            AddBullet("Przycisk włączenia/wyłączenia zadania- zatrzymuje działanie wybranego zadania w harmonogramie lub je wznawia.");
            AddBullet("Przycisk edycji pozwala na zmianę momentu aktywacji działania aktywatora, pozwala dodać notatkę widoczną dla osoby modyfikującej działanie, pozwala także zmienić opcje aktywatorów.");
            AddBullet("Przycisk usuwania zadania- całkowicie kasuje z kolejki harmonogramu aktywne zadanie, jednocześnie je zatrzymując.");
            AddSpace();

            AddHeader("2. Lista możliwych zadań do wykonania przez aktywatory", warning);
            AddParagraph("Program wspiera kilka natywnych akcji systemowych, oraz kilka własnych rozwiązań, pogrupowanych bezpośrednio w kategoriach opisanych niżej:");
            AddSubHeader("Zasilanie i Sesja");
            AddBullet("Zamknięcie systemu / Restart / Uśpienie / Hibernacja.");
            AddBullet("Blokada stacji roboczej oraz wylogowanie użytkownika.");
            AddSubHeader("Monitoring i Narzędzia");
            AddBullet("Uruchom polecenie: Egzekucja własnych skryptów .bat lub plików .exe. Jeśli program jest uruchomiony z uprawieniami Administratora- można wykonać też skrypty wymagające uprawień.");
            AddBullet("Notatka: Zapisuje komunikat dla użytkownika.");
            AddSubHeader("Moduł czyszczenia dysku");
            AddParagraph("To małe narzędzie konserwacyjne, które obejmuje usuwanie zbędnych plików zalegających na dysku systemowym. Pozwala to zaoszczędzić cenne miejsce na dysku.");
            AddBullet("Cache Windows Update: usuwanie pozostałości po instalacjach łatek.");
            AddBullet("Miniatury (Thumbcache): naprawianie błędów z ikonami plików.");
            AddBullet("Dane Prefetch: optymalizacja bazy startowej Windows.");
            AddBullet("Własne ścieżki: możliwość zdefiniowania dowolnych katalogów do cyklicznego czyszczenia.");
            AddBullet("Uwaga: niektóre operacje wymagają uprawień administracyjnych, aby móc je wykonać. Upewnij się, czy program jest uruchomiony jako administrator, aby móc wykonać wszystkie operacje czyszczenia plików tymczasowych.");
            AddSubHeader("Moduł zrzutów ekranu:");
            AddParagraph("Aplikacja posiada wbudowany funkcję do wykonywania zrzutów ekranu, która może pełnić zadanie monitoringu pracy lub archiwizacji stanu pulpitu przed zamknięciem systemu.");
            AddBullet("Format zapisu: Natywna obsługa formatów bezstratnych (PNG) oraz kompresowanych (JPG).");
            AddBullet("Serie (Burst Mode): Możliwość wykonania określonej liczby zdjęć (np. 10) w zadanym interwale milisekundowym.");
            AddBullet("Archiwizacja: Automatyczne tworzenie unikalnych nazw plików z sygnaturą czasową i możliwość zapisu w dedykowanym folderze lub na pulpicie.");
            AddSubHeader("Otwórz stronę internetową:");
            AddParagraph("Moduł ten umożliwia otworzenie w domyślnej przeglądarce wskazanej strony internetowej.");
            AddSubHeader("Otwórz dzwięk:");
            AddParagraph("Opcja uruchamia wskazany przez użytkownika plik dzwiękowy, z folderu /sounds dostępnym w katalogu aplikacji lub wskazanej ścieżki przez użytkownika. Jeśli nic nie zostanie znalezione oraz ustawione- program uruchomi domyślny plik dzwiekowy z folderu systemowego.");
            AddSpace();

            AddHeader("4. Blokada dostępu do interfejsu programu", Color.FromArgb(200, 0, 0));
            AddSubHeader("Ochrona Hasłem");
            AddParagraph("Program posiada wbudowany system autoryzacji blokujący dostęp do krytycznych ustawień.");
            AddBullet("Domyślne hasło: 123456. Ponieważ jest to domyślne hasło, oraz dosyć łatwe do odgadnięcia hasło- zalecane jest zaraz po aktywowaniu tej opcji - zmienienie hasła na inne.");
            AddBullet("Blokada GUI uniemożliwi wtedy niepowołanym użytkownikom systemu Windows na zmianę ustawień programu lub całkowitą dezaktywację jego działania.");
            AddSpace();

            AddHeader("5. Kontakt/wsparcie", Color.Gray);
            AddBullet("Jeśli masz problemy z tym programem, znalazłeś bugi lub masz sugestie, skontaktuj się z nami. Możesz nam pomóc rozwiązać problem, poprzez opisanie go w wątku 'Issues' na GitHub, pod tym linkiem: https://github.com/Mattronix7200/TimerSysApp/issues. Z góry dziękujemy!");
            AddBullet("Program ci się spodobał. Rozważ wspracie finansowe autorów programu. Link do przekazania wspracia znajdziesz tutaj: https://suppi.pl/jmichalski");
            // Powrót na górę dokumentu
            rtb.SelectionStart = 0;
            rtb.ScrollToCaret();

            var btnClose = new System.Windows.Forms.Button
            {
                Text = "OK",
                Dock = DockStyle.Bottom,
                Height = 45,
                FlatStyle = FlatStyle.Flat,
                BackColor = dark ? Color.FromArgb(45, 45, 48) : Color.FromArgb(220, 220, 220),
                ForeColor = fg,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => dlg.Close();

            dlg.Controls.Add(btnClose);
            dlg.ShowDialog(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add(new ToolStripMenuItem("Zamknij program", null, (s, ev) =>
            {
                Application.ExitThread();
            }));
            menu.Items.Add(new ToolStripMenuItem("Zatrzymaj odliczanie (bezczynność)", null, (s, ev) =>
            {
                timer1.Stop();
            }));
            menu.Items.Add(new ToolStripMenuItem("Zatrzymaj odliczanie (czasowe)", null, (s, ev) =>
            {
                timer2.Stop();
            }));
            menu.Items.Add(new ToolStripMenuItem("Wznów odliczanie (bezczynność)", null, (s, ev) =>
            {
                if (TimeSpan.TryParse(textBox4.Text, out var ts4m))
                    timer1EndTime = DateTime.Now.Add(ts4m);
                timer1.Start();
            }));
            menu.Items.Add(new ToolStripMenuItem("Wznów odliczanie (czasowe)", null, (s, ev) =>
            {
                DateTime selectedDate = dateTimePicker1.Value.Date;
                TimeSpan selectedTime = TimeSpan.Parse(textBox7.Text);
                DateTime fullSelectedDateTime = selectedDate + selectedTime;

                TimeSpan timeToWait = fullSelectedDateTime - DateTime.Now;
                if (timeToWait.TotalSeconds > 0)
                {
                    timer2EndTime = DateTime.UtcNow.Add(timeToWait);
                    timer2.Start();
                }
            }));
            menu.Items[0].Image = File.Exists(Path.Combine(appDirectory, "icons", "close.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "close.ico")) : null;
            menu.Items[3].Image = File.Exists(Path.Combine(appDirectory, "icons", "next.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "next.ico")) : null;
            menu.Items[4].Image = File.Exists(Path.Combine(appDirectory, "icons", "next.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "next.ico")) : null;
            menu.Items[1].Image = File.Exists(Path.Combine(appDirectory, "icons", "stop.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "stop.ico")) : null;
            menu.Items[2].Image = File.Exists(Path.Combine(appDirectory, "icons", "stop.ico")) ? Image.FromFile(Path.Combine(appDirectory, "icons", "stop.ico")) : null;
            menu.Items[1].Enabled = !timer1.Enabled;
            notifyIcon1.ContextMenuStrip = menu;


            label19.Location = new Point(0, 0);
            label19.Size = new Size(this.ClientSize.Width, this.ClientSize.Height);
            label19.BringToFront();

            label19.Click -= label19_Click;
            label19.Click += label19_Click;
            label19.Cursor = Cursors.Hand;

            if (checkBox6.Checked)
            {
                tabControl1.TabPages["tabPage1"].Enabled = false;
                tabControl1.TabPages["tabPage2"].Enabled = false;
                tabControl1.TabPages["tabPage3"].Enabled = false;
                button6.Enabled = false;
                label19.Visible = true;
                label19.Enabled = true;
            }
            else
            {

                label19.Visible = false;
                label19.Enabled = false;
            }


            textBoxUser.Width = 511;
            var btnBrowse = new System.Windows.Forms.Button
            {
                Text = "📂",
                Width = 30,
                Height = textBoxUser.Height,
                Location = new Point(textBoxUser.Right + 4, textBoxUser.Top),
                FlatStyle = FlatStyle.Flat,
                TabIndex = 100
            };
            var ttip = new System.Windows.Forms.ToolTip();
            ttip.SetToolTip(btnBrowse, "Przeglądaj plik (exe, bat, ps1, cmd, vbs)");
            btnBrowse.Click += (s, ev) =>
            {
                using var dlg = new OpenFileDialog
                {
                    Title = "Wybierz plik do uruchomienia",
                    Filter = "Programy i skrypty|*.exe;*.bat;*.cmd;*.ps1;*.vbs|Wszystkie pliki|*.*"
                };
                if (dlg.ShowDialog() == DialogResult.OK) textBoxUser.Text = dlg.FileName;
            };
            groupBox1.Controls.Add(btnBrowse);

            var presetMenu = new ContextMenuStrip();
            presetMenu.Items.Add("Notatnik", null, (s, ev) => textBoxUser.Text = "notepad.exe");
            presetMenu.Items.Add("Kalkulator", null, (s, ev) => textBoxUser.Text = "calc.exe");
            presetMenu.Items.Add("Menedżer zadań", null, (s, ev) => textBoxUser.Text = "taskmgr.exe");
            presetMenu.Items.Add("Eksplorator plików", null, (s, ev) => textBoxUser.Text = "explorer.exe");
            presetMenu.Items.Add("Wiersz poleceń", null, (s, ev) => textBoxUser.Text = "cmd.exe /k");
            presetMenu.Items.Add("PowerShell", null, (s, ev) => textBoxUser.Text = "powershell.exe");
            var btnPreset = new System.Windows.Forms.Button
            {
                Text = "▼",
                Width = 28,
                Height = textBoxUser.Height,
                Location = new Point(btnBrowse.Right + 2, textBoxUser.Top),
                FlatStyle = FlatStyle.Flat,
                TabIndex = 101
            };
            ttip.SetToolTip(btnPreset, "Wybierz gotowe polecenie z listy");
            btnPreset.Click += (s, ev) => presetMenu.Show(btnPreset, new Point(0, btnPreset.Height));
            groupBox1.Controls.Add(btnPreset);


            _lblScheduleStatus = new System.Windows.Forms.Label
            {
                Name = "lblScheduleStatus",
                Text = "Harmonogram nieaktywny",
                Location = new Point(20, 350),
                Size = new Size(620, 18),
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.DimGray,
                AutoSize = false
            };
            tabPage6.Controls.Add(_lblScheduleStatus);
            _lblScheduleStatus.BringToFront();


            var sepLine = new System.Windows.Forms.Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Location = new Point(20, 372),
                Size = new Size(620, 2),
                AutoSize = false
            };
            tabPage6.Controls.Add(sepLine);


            var lblSnoozeHeader = new System.Windows.Forms.Label
            {
                Text = "⏰  Jednorazowe powtórzenie akcji za:",
                Location = new Point(20, 378),
                Size = new Size(260, 17),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                AutoSize = false
            };
            tabPage6.Controls.Add(lblSnoozeHeader);

            var cboSnooze = new System.Windows.Forms.ComboBox
            {
                Location = new Point(20, 398),
                Size = new Size(130, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cboSnooze.Items.AddRange(new object[] {
                "5 minut", "10 minut", "30 minut",
                "1 godzina", "3 godziny", "5 godzin", "10 godzin" });
            cboSnooze.SelectedIndex = 0;
            tabPage6.Controls.Add(cboSnooze);

            var btnSnooze = new System.Windows.Forms.Button
            {
                Text = "▶ Uruchom",
                Location = new Point(156, 397),
                Size = new Size(95, 26),
                FlatStyle = FlatStyle.Flat
            };
            tabPage6.Controls.Add(btnSnooze);

            _lblSnoozeStatus = new System.Windows.Forms.Label
            {
                Text = "",
                Location = new Point(256, 401),
                Size = new Size(200, 18),
                ForeColor = Color.Coral,
                AutoSize = false
            };
            tabPage6.Controls.Add(_lblSnoozeStatus);


            _timerSnooze.Tick += (s, ev) =>
            {
                var rem = _snoozeEndTime - DateTime.Now;
                if (rem.TotalSeconds <= 0)
                {
                    _timerSnooze.Stop();
                    btnSnooze.Enabled = true;
                    _lblSnoozeStatus.Text = "";
                    if (comboBox3.SelectedItem != null)
                        ExecuteCommand(comboBox3.SelectedItem.ToString(), 3);
                }
                else
                {
                    _lblSnoozeStatus.Text = $"Odlicza: {rem:mm\\:ss}";
                }
            };
            btnSnooze.Click += (s, ev) =>
            {
                int[] minutes = { 5, 10, 30, 60, 180, 300, 600 };
                int delay = minutes[cboSnooze.SelectedIndex];
                _snoozeEndTime = DateTime.Now.AddMinutes(delay);
                _timerSnooze.Start();
                btnSnooze.Enabled = false;
            };


            int settingsBtnX = comboBox1.Right + 5;
            int settingsBtnY = comboBox1.Top;

            _btnActSettings1 = new System.Windows.Forms.Button
            {
                Text = "⚙",
                Size = new Size(30, comboBox1.Height),
                Location = new Point(settingsBtnX, settingsBtnY),
                FlatStyle = FlatStyle.Flat,
                TabStop = false
            };
            _actionTip.SetToolTip(_btnActSettings1, "Ustawienia wybranej akcji");
            _btnActSettings1.Click += (s, ev) => ShowActionSettingsDialog(comboBox1, 1);
            _btnActSettings1.Enabled = ActionHasParams(comboBox1.SelectedIndex);
            tabPage1.Controls.Add(_btnActSettings1);

            _btnActSettings2 = new System.Windows.Forms.Button
            {
                Text = "⚙",
                Size = new Size(30, comboBox2.Height),
                Location = new Point(comboBox2.Right + 5, comboBox2.Top),
                FlatStyle = FlatStyle.Flat,
                TabStop = false
            };
            _actionTip.SetToolTip(_btnActSettings2, "Ustawienia wybranej akcji");
            _btnActSettings2.Click += (s, ev) => ShowActionSettingsDialog(comboBox2, 2);
            _btnActSettings2.Enabled = ActionHasParams(comboBox2.SelectedIndex);
            tabPage2.Controls.Add(_btnActSettings2);

            _btnActSettings3 = new System.Windows.Forms.Button
            {
                Text = "⚙",
                Size = new Size(30, comboBox3.Height),
                Location = new Point(comboBox3.Right + 5, comboBox3.Top),
                FlatStyle = FlatStyle.Flat,
                TabStop = false
            };
            _actionTip.SetToolTip(_btnActSettings3, "Ustawienia wybranej akcji");
            _btnActSettings3.Click += (s, ev) => ShowActionSettingsDialog(comboBox3, 3);
            _btnActSettings3.Enabled = ActionHasParams(comboBox3.SelectedIndex);
            tabPage6.Controls.Add(_btnActSettings3);


            var settTip = new System.Windows.Forms.ToolTip();
            settTip.SetToolTip(checkBox1, "Tworzy skrót w folderze autostartu systemu Windows,\ntak aby program uruchamiał się razem z systemem.");
            settTip.SetToolTip(checkBox2, "Automatycznie uruchamia odliczanie aktywatora bezczynności\nzaraz po otwarciu programu.");
            settTip.SetToolTip(checkBox3, "Minimalizacja okna chowa program do zasobnika (ikony tray)\nzamiast na pasek zadań. Podwójne kliknięcie ikony przywraca okno.");
            settTip.SetToolTip(checkBox4, "Licznik bezczynności jest resetowany przy każdej aktywności użytkownika\ni ponownie uruchamiany – akcja wykona się po czasie CIĄGŁEJ bezczynności.");
            settTip.SetToolTip(checkBox5, "Importuje specjalny plan zasilania\ni utrzymuje go aktywnym – zapobiega automatycznemu usypianiu przez Windows.");
            settTip.SetToolTip(checkBox6, "Blokuje dostęp do ustawień programu hasłem.\nDomyślne hasło (jeśli niezmienione) to: 123456");
            settTip.SetToolTip(checkBox7, "Jeśli włączone – program poczeka na zakończenie uruchamianego procesu\nzanim przejdzie dalej (tryb synchroniczny).");
            settTip.SetToolTip(checkBox8, "Uwzględnia zmiany czasu systemowego \nprzy obliczaniu czasu do akcji w aktywatorze czasowym. Opcja ta nie wpływa na zmianę czasu w danej strefie.");
            settTip.SetToolTip(checkBox9, "Ukrywa ikonę programu z paska zadań (zasobnik systemowy).\nProgram nadal działa w tle.");
            settTip.SetToolTip(checkBox10, "Ukrywa wyświetlanie licznika odliczania na ekranie.\nProgram nadal odlicza i wykona akcję o wyznaczonym czasie.");
            settTip.SetToolTip(checkBox11, "Przełącza wygląd programu na ciemny motyw\n(ciemne tło, jasny tekst).");


            _timerStatusUI.Tick += (s, ev) =>
            {
                if (_lblScheduleStatus != null)
                {
                    if (!timer3.Enabled)
                    {
                        _lblScheduleStatus.Text = "Harmonogram nieaktywny";
                        _lblScheduleStatus.ForeColor = Color.DimGray;
                    }
                    else
                    {
                        var nextFire = GetNextScheduleFireTime();
                        if (nextFire.HasValue)
                        {
                            var diff = nextFire.Value - DateTime.Now;
                            string ds = diff.TotalHours >= 24
                                ? $"{(int)diff.TotalDays}d {diff.Hours:D2}h {diff.Minutes:D2}m"
                                : $"{(int)diff.TotalHours:D2}h {diff.Minutes:D2}m {diff.Seconds:D2}s";
                            _lblScheduleStatus.Text = $"⏱ Następne uruchomienie: {nextFire.Value:HH:mm} (za {ds})";
                            _lblScheduleStatus.ForeColor = Color.DarkGreen;
                        }
                        else
                        {
                            _lblScheduleStatus.Text = "Harmonogram aktywny – brak terminu w ciągu 8 dni";
                            _lblScheduleStatus.ForeColor = Color.DimGray;
                        }
                    }
                }

                {
                    var utcNow = DateTime.UtcNow;
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Tag is string rowId)
                        {
                            var t = _taskList.FirstOrDefault(x => x.Id == rowId);
                            if (t != null && t.ActivatorType == 2 && t.Enabled && t.EndTimeUtc != default)
                            {
                                var rem = t.EndTimeUtc - utcNow;
                                row.Cells[1].Value = rem.TotalSeconds > 0
                                    ? rem.ToString(@"hh\:mm\:ss")
                                    : "Wygasło";
                            }
                            else if (t != null && t.ActivatorType == 3 && t.Enabled
                                     && !string.IsNullOrEmpty(t.RepeatIntervalType))
                            {
                                double iMin = t.RepeatIntervalType switch
                                { "h" => t.RepeatIntervalValue * 60.0, "d" => t.RepeatIntervalValue * 1440.0, _ => t.RepeatIntervalValue };
                                var nextFire = t.LastFired.AddMinutes(iMin);
                                var rem = nextFire - DateTime.Now;
                                string unitLabel = t.RepeatIntervalType switch { "h" => "godz", "d" => "dni", _ => "min" };
                                row.Cells[1].Value = $"co {t.RepeatIntervalValue} {unitLabel}: "
                                    + (rem.TotalSeconds > 0 ? rem.ToString(@"hh\:mm\:ss") : "Zaraz...");
                            }
                        }
                    }
                }
            };
            _timerStatusUI.Start();


            UpdateComboHints(comboBox1, tip01, _btnActSettings1);
            UpdateComboHints(comboBox2, tip02, _btnActSettings2);
            UpdateComboHints(comboBox3, tip03, _btnActSettings3);

            stopStartHarmonogram.Click += (s, ev) => SetHarmonogramRunning(appSettings["timer3UserPaused"] == "1");
            ApplyHarmonogramUI(appSettings["timer3UserPaused"] != "1");

            this.FormClosed += (s, ev) => StopSound();


            if (checkBox8.Checked)
                SystemEvents.TimeChanged += SystemEvents_TimeChanged;
            if (checkBox9.Checked)
                notifyIcon1.Visible = false;
            if (checkBox10.Checked)
            {
                textBox5.Visible = false;
                textBox9.Visible = false;
            }
            if (checkBox11.Checked)
                ApplyTheme(true);


            checkBox12.CheckedChanged += (s, ev) =>
            {
                appSettings["autoStartQueue"] = checkBox12.Checked ? "1" : "0";
                SaveConfigFile();
            };
            settTip.SetToolTip(checkBox12,
                "Po uruchomieniu programu automatycznie wznawia timer dla zadań\ndodanych do kolejki Harmonogram (nie wymaga ręcznego ponownego dodawania).");


            checkBox16.Checked = appSettings["noBalloon"] == "1";
            checkBox16.CheckedChanged += (s, ev) =>
            {
                appSettings["noBalloon"] = checkBox16.Checked ? "1" : "0";
                SaveConfigFile();
            };
            settTip.SetToolTip(checkBox16, "Wyłącza dymki informacyjne pojawiające się nad paskiem zadań\ngdy aktywator wykonuje wybraną akcję.");


            string adminFlagPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "runasadmin.flag");
            checkBox15.Checked = File.Exists(adminFlagPath);
            if (IsAdmin()) { checkBox15.Text = "✓ " + checkBox15.Text; checkBox15.ForeColor = Color.DarkGreen; }
            checkBox15.CheckedChanged += (s, ev) =>
            {
                if (checkBox15.Checked)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(adminFlagPath)!);
                    File.WriteAllText(adminFlagPath, "1");
                    if (!IsAdmin())
                    {
                        var res = MessageBox.Show(
                            "Opcja zostanie zastosowana przy następnym uruchomieniu.\nUruchomić teraz jako administrator?",
                            "Uprawnienia administratora", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (res == DialogResult.Yes)
                        {
                            try
                            {
                                int pid15 = Environment.ProcessId;
                                string exe15 = Application.ExecutablePath.Replace("'", "''");
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName        = "cmd.exe",
                                    Arguments       = $"/c taskkill /F /PID {pid15} & timeout /t 2 /nobreak > nul & powershell -WindowStyle Hidden -NonInteractive -Command \"Start-Process -FilePath '{exe15}' -Verb RunAs\"",
                                    UseShellExecute = false, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden
                                });
                                Environment.Exit(0);
                            }
                            catch { checkBox15.Checked = false; File.Delete(adminFlagPath); }
                        }
                    }
                }
                else { if (File.Exists(adminFlagPath)) File.Delete(adminFlagPath); }
            };
            settTip.SetToolTip(checkBox15, IsAdmin()
                ? "Program działa już z uprawnieniami administratora."
                : "Przy każdym uruchomieniu programu zostanie wyświetlony monit UAC\no podwyższenie uprawnień do poziomu administratora.");


            menu.Items.Add(new ToolStripSeparator());
            _menuScheduleToggle = new ToolStripMenuItem(
                appSettings["timer3UserPaused"] == "1" ? "▶ Uruchom harmonogram" : "⏸ Zatrzymaj harmonogram",
                null, ToggleScheduleFromTray);
            _menuScheduleToggle.Enabled = _taskList.Any(t => t.ActivatorType == 3);
            menu.Items.Add(_menuScheduleToggle);
            menu.Items.Add(new ToolStripMenuItem("⚙️ Ustawienia", null, (s, ev) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon1.Visible = false;
                tabControl1.SelectedTab = tabControl1.TabPages["tabPage3"];
            }));

            menu.Opening += (s, ev) =>
            {
                if (_menuScheduleToggle != null)
                {
                    _menuScheduleToggle.Enabled = _taskList.Any(t => t.ActivatorType == 3);
                    _menuScheduleToggle.Text = appSettings["timer3UserPaused"] == "1"
                        ? "▶ Uruchom harmonogram"
                        : "⏸ Zatrzymaj harmonogram";
                }
            };


            button2.Click += (s, ev) => AddTaskAndRun(1);
            button4.Click += (s, ev) => AddTaskAndRun(2);
            button8.Click += (s, ev) => AddTaskAndRun(3);


            helpLink.LinkClicked += (s, ev) => ShowHelpDialog();


            button13.Click += (s, ev) => ShowBackupRestoreDialog();


            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;

            label13.Cursor = Cursors.Hand;
            label14.Cursor = Cursors.Hand;
            label15.Cursor = Cursors.Hand;


            void UpdateFilterLabels()
            {
                var boldFont   = new Font("Segoe UI", 9.75f, FontStyle.Bold);
                var normalFont = new Font("Segoe UI", 9.75f, FontStyle.Regular);
                label13.Font = _taskListFilter == 1 ? boldFont : normalFont;
                label14.Font = _taskListFilter == 2 ? boldFont : normalFont;
                label15.Font = _taskListFilter == 3 ? boldFont : normalFont;
                var activeColor  = Color.FromArgb(0, 100, 200);
                var normalColor  = label13.ForeColor;
                label13.ForeColor = _taskListFilter == 1 ? activeColor : SystemColors.ControlText;
                label14.ForeColor = _taskListFilter == 2 ? activeColor : SystemColors.ControlText;
                label15.ForeColor = _taskListFilter == 3 ? activeColor : SystemColors.ControlText;
            }

            label13.Click += (s, ev) => { _taskListFilter = _taskListFilter == 1 ? 0 : 1; UpdateFilterLabels(); RefreshTaskGrid(); };
            label14.Click += (s, ev) => { _taskListFilter = _taskListFilter == 2 ? 0 : 2; UpdateFilterLabels(); RefreshTaskGrid(); };
            label15.Click += (s, ev) => { _taskListFilter = _taskListFilter == 3 ? 0 : 3; UpdateFilterLabels(); RefreshTaskGrid(); };

            button10.Click += (s, ev) =>
            {
                if (dataGridView1.SelectedRows.Count == 0) return;
                string id = dataGridView1.SelectedRows[0].Tag as string;
                var removed = _taskList.FirstOrDefault(t => t.Id == id);
                if (removed == null) return;
                int removedType = removed.ActivatorType;
                _taskList.Remove(removed);

                switch (removedType)
                {
                    case 1:
                        if (!_taskList.Any(t => t.ActivatorType == 1 && t.Enabled))
                            timer1.Stop();
                        break;
                    case 2:
                        if (!_taskList.Any(t => t.ActivatorType == 2 && t.Enabled && t.EndTimeUtc > DateTime.UtcNow))
                        { timer2.Stop(); textBox9.Text = "00:00:00"; }
                        break;
                    case 3:
                        if (!_taskList.Any(t => t.ActivatorType == 3 && t.Enabled))
                            timer3.Stop();
                        break;
                }
                SaveConfigFile();
                RefreshTaskGrid();
            };

            button12.Click += (s, ev) =>
            {
                if (dataGridView1.SelectedRows.Count == 0) return;
                string id = dataGridView1.SelectedRows[0].Tag as string;
                var task = _taskList.FirstOrDefault(t => t.Id == id);
                if (task == null) return;
                task.Enabled = !task.Enabled;

                if (!task.Enabled)
                {

                    switch (task.ActivatorType)
                    {
                        case 1:
                            if (!_taskList.Any(t => t.ActivatorType == 1 && t.Enabled && t.Id != task.Id))
                                timer1.Stop();
                            break;
                        case 2:
                            if (!_taskList.Any(t => t.ActivatorType == 2 && t.Enabled && t.Id != task.Id && t.EndTimeUtc > DateTime.UtcNow))
                            { timer2.Stop(); textBox9.Text = "Wstrzymano"; }
                            break;
                        case 3:
                            if (!_taskList.Any(t => t.ActivatorType == 3 && t.Enabled && t.Id != task.Id))
                            { timer3.Stop(); appSettings["timer3Enabled"] = "0"; }
                            break;
                    }
                }
                else
                {

                    switch (task.ActivatorType)
                    {
                        case 1:
                            if (!timer1.Enabled && TimeSpan.TryParse(textBox4.Text, out var ts4e))
                            { timer1EndTime = DateTime.Now.Add(ts4e); timer1.Start(); }
                            break;
                        case 2:
                            if (!timer2.Enabled && task.EndTimeUtc > DateTime.UtcNow)
                                timer2.Start();
                            break;
                        case 3:
                            if (!timer3.Enabled && appSettings["timer3UserPaused"] != "1"
                                && _taskList.Any(t => t.ActivatorType == 3 && t.Enabled))
                            { timer3.Start(); appSettings["timer3Enabled"] = "1"; }
                            break;
                    }
                }
                SaveConfigFile();
                RefreshTaskGrid();
            };

            button11.Click += (s, ev) =>
            {
                if (dataGridView1.SelectedRows.Count == 0) return;
                string id = dataGridView1.SelectedRows[0].Tag as string ?? "";
                var task = _taskList.FirstOrDefault(t => t.Id == id);
                if (task == null) return;

                bool darkDlg = appSettings["darkModeEnabled"] == "1";
                int dlgH = task.ActivatorType == 3 ? 310 : task.ActivatorType == 2 ? 220 : 195;
                var editDlg = new Form
                {
                    Text = "Edytuj zadanie",
                    ClientSize = new Size(420, dlgH),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false, MinimizeBox = false,
                    BackColor = this.BackColor
                };

                int row = 12;
                void AddEditRow(string lbl, Control ctrl, int h = 23)
                {
                    editDlg.Controls.Add(new System.Windows.Forms.Label { Text = lbl, Location = new Point(12, row + 3), AutoSize = true });
                    ctrl.Location = new Point(130, row); ctrl.Size = new Size(270, h);
                    editDlg.Controls.Add(ctrl);
                    row += h + 10;
                }

                var cboEditAction = new System.Windows.Forms.ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
                foreach (var item in comboBox1.Items) cboEditAction.Items.Add(item);
                if (cboEditAction.Items.Contains(task.Action)) cboEditAction.SelectedItem = task.Action;
                else if (cboEditAction.Items.Count > 0) cboEditAction.SelectedIndex = 0;
                AddEditRow("Akcja:", cboEditAction);
                var txtNotes = new TextBox { Text = task.Notes };
                AddEditRow("Notatka:", txtNotes);


                TextBox? txtIdle = null;
                DateTimePicker? dtpDate = null;
                TextBox? txtTime2 = null;
                TextBox? txtHour3 = null;
                CheckedListBox? clbDays3 = null;
                System.Windows.Forms.NumericUpDown? nudRepN = null;
                System.Windows.Forms.RadioButton? rbRepMin = null, rbRepHour = null, rbRepDay = null;

                if (task.ActivatorType == 1)
                {
                    txtIdle = new TextBox { Text = string.IsNullOrEmpty(task.IdleTimeout) ? textBox4.Text : task.IdleTimeout };
                    AddEditRow("Bezczynnosc (HH:MM:SS):", txtIdle);
                }
                else if (task.ActivatorType == 2)
                {
                    dtpDate = new DateTimePicker { Value = task.LocalTarget != default ? task.LocalTarget.Date : DateTime.Today, Format = DateTimePickerFormat.Short };
                    AddEditRow("Data:", dtpDate);
                    string timeStr = task.LocalTarget != default ? task.LocalTarget.ToString("HH:mm") : "";
                    txtTime2 = new TextBox { Text = timeStr };
                    AddEditRow("Godzina (HH:MM):", txtTime2);
                }
                else if (task.ActivatorType == 3)
                {
                    bool isIntervalEdit = !string.IsNullOrEmpty(task.RepeatIntervalType);
                    if (isIntervalEdit)
                    {

                        int repVal = task.RepeatIntervalValue > 0 ? task.RepeatIntervalValue : 30;
                        nudRepN = new System.Windows.Forms.NumericUpDown
                        { Minimum = 1, Maximum = 99999, Value = repVal };
                        AddEditRow("Powtarzaj co (N):", nudRepN);


                        var panelRB = new System.Windows.Forms.Panel { Location = new Point(130, row), Size = new Size(270, 26) };
                        rbRepMin  = new System.Windows.Forms.RadioButton { Text = "minut",  Location = new Point(0,  4), AutoSize = true, Checked = task.RepeatIntervalType == "m" };
                        rbRepHour = new System.Windows.Forms.RadioButton { Text = "godzin", Location = new Point(75, 4), AutoSize = true, Checked = task.RepeatIntervalType == "h" };
                        rbRepDay  = new System.Windows.Forms.RadioButton { Text = "dni",    Location = new Point(155,4), AutoSize = true, Checked = task.RepeatIntervalType == "d" };
                        if (!rbRepMin.Checked && !rbRepHour.Checked && !rbRepDay.Checked) rbRepMin.Checked = true;
                        panelRB.Controls.Add(rbRepMin); panelRB.Controls.Add(rbRepHour); panelRB.Controls.Add(rbRepDay);
                        editDlg.Controls.Add(panelRB);
                        editDlg.Controls.Add(new System.Windows.Forms.Label { Text = "Jednostka:", Location = new Point(12, row + 4), AutoSize = true });
                        row += 36;
                    }
                    else
                    {

                        txtHour3 = new TextBox { Text = string.IsNullOrEmpty(task.ScheduleHour) ? textBox1.Text : task.ScheduleHour };
                        AddEditRow("Godzina (HH:MM):", txtHour3);
                        clbDays3 = new CheckedListBox { CheckOnClick = true };

                        for (int i = 0; i < SCHEDULE_INTERVAL_IDX; i++)
                            clbDays3.Items.Add(checkedListBox1.Items[i]);
                        var savedDays = (string.IsNullOrEmpty(task.ScheduleDays)
                            ? string.Join(",", checkedListBox1.CheckedIndices.Cast<int>())
                            : task.ScheduleDays)
                            .Split(',')
                            .Where(x => int.TryParse(x.Trim(), out _))
                            .Select(x => int.Parse(x.Trim()))
                            .ToHashSet();
                        for (int i = 0; i < clbDays3.Items.Count; i++)
                            clbDays3.SetItemChecked(i, savedDays.Contains(i));
                        int clbH = Math.Min(120, clbDays3.Items.Count * 18 + 4);
                        AddEditRow("Powtarzanie:", clbDays3, clbH);
                    }
                }

                row += 4;
                var btnSave2 = new System.Windows.Forms.Button
                { Text = "Zapisz", Location = new Point(130, row), Size = new Size(88, 28), FlatStyle = FlatStyle.Flat };
                btnSave2.Click += (s2, ev2) =>
                {
                    task.Action = cboEditAction.SelectedItem?.ToString()?.Trim() ?? task.Action;
                    task.Notes  = txtNotes.Text.Trim();
                    if (task.ActivatorType == 1 && txtIdle != null)
                    {
                        if (!TimeSpan.TryParse(txtIdle.Text, out _))
                        { MessageBox.Show("Nieprawidlowy format czasu (HH:MM:SS)."); return; }
                        task.IdleTimeout = txtIdle.Text.Trim();
                    }
                    else if (task.ActivatorType == 2 && dtpDate != null && txtTime2 != null)
                    {
                        if (!TimeSpan.TryParse(txtTime2.Text, out var newTime))
                        { MessageBox.Show("Nieprawidlowy format godziny (HH:MM)."); return; }
                        DateTime newTarget = dtpDate.Value.Date + newTime;
                        if (newTarget <= DateTime.Now)
                        { MessageBox.Show("Data i godzina musza byc w przyszlosci."); return; }
                        task.LocalTarget = newTarget;
                        task.EndTimeUtc  = newTarget.ToUniversalTime();
                        task.Enabled = true;
                        if (!timer2.Enabled) timer2.Start();
                    }
                    else if (task.ActivatorType == 3)
                    {
                        bool wasInterval = !string.IsNullOrEmpty(task.RepeatIntervalType);
                        if (wasInterval && nudRepN != null && rbRepMin != null)
                        {

                            task.RepeatIntervalType  = rbRepMin.Checked ? "m" : rbRepHour!.Checked ? "h" : "d";
                            task.RepeatIntervalValue = (int)nudRepN.Value;

                        }
                        else if (!wasInterval && txtHour3 != null && clbDays3 != null)
                        {
                            if (!TimeSpan.TryParse(txtHour3.Text, out _) && !TimeSpan.TryParse(txtHour3.Text + ":00", out _))
                            { MessageBox.Show("Nieprawidlowy format godziny (HH:MM)."); return; }
                            if (clbDays3.CheckedIndices.Count == 0)
                            { MessageBox.Show("Wybierz co najmniej jeden dzien."); return; }
                            task.ScheduleHour = txtHour3.Text.Trim();
                            task.ScheduleDays = string.Join(",", clbDays3.CheckedIndices.Cast<int>());
                            task.LastFired    = default;
                        }
                    }
                    SaveConfigFile();
                    RefreshTaskGrid();
                    editDlg.Close();
                };
                editDlg.Controls.Add(btnSave2);
                var btnCancel3 = new System.Windows.Forms.Button
                { Text = "Anuluj", Location = new Point(224, row), Size = new Size(88, 28), FlatStyle = FlatStyle.Flat, DialogResult = DialogResult.Cancel };
                editDlg.Controls.Add(btnCancel3);
                editDlg.CancelButton = btnCancel3;
                editDlg.ClientSize = new Size(420, row + 48);

                if (darkDlg) ApplyThemeToControls(editDlg.Controls,
                    Color.FromArgb(30, 30, 30), Color.FromArgb(220, 220, 220), Color.FromArgb(55, 55, 55), true);
                editDlg.ShowDialog(this);
            };

            RefreshTaskGrid();


            checkedListBox1.ItemCheck += (s, ev) =>
            {
                if (ev.Index == SCHEDULE_INTERVAL_IDX && ev.NewValue == CheckState.Checked)
                {

                    var (repType, repValue) = ShowRepeatIntervalDialog();
                    if (string.IsNullOrEmpty(repType))
                    {
                        ev.NewValue = CheckState.Unchecked;
                        return;
                    }
                    appSettings["timer3_repeat_type"]  = repType;
                    appSettings["timer3_repeat_value"] = repValue.ToString();

                    BeginInvoke(new Action(() =>
                    {
                        for (int i = 0; i < SCHEDULE_INTERVAL_IDX; i++)
                            checkedListBox1.SetItemChecked(i, false);
                    }));
                }
                else if (ev.Index != SCHEDULE_INTERVAL_IDX && ev.NewValue == CheckState.Checked)
                {

                    BeginInvoke(new Action(() =>
                        checkedListBox1.SetItemChecked(SCHEDULE_INTERVAL_IDX, false)));
                }
            };


            ShowLabelBasedOnVersion();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            appSettings["timer1ActivateBy"] = textBox4.Text;

            TimeSpan maxTime = new TimeSpan(24, 0, 0);
            TimeSpan inputTime;

            if (TimeSpan.TryParse(textBox4.Text, out inputTime) && inputTime > maxTime)
            {
                textBox5.Text = "Maksymalny okres odliczanego czasu w tym oknie wynosi 1 dzień.";
                timer1.Stop();
                textBox4.Text = "24:00:00";
            }

            SaveConfigFile();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            appSettings["timer2HourActivateBy"] = textBox7.Text;
            SaveConfigFile();
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
                textBox9.Text = "Wybrałeś datę z przeszłości, wybierz tę z przyszłości.";
            }
            else
            {
                _timer2LocalTarget = fullSelectedDateTime;
                timer2EndTime = DateTime.UtcNow.Add(timeToWait);
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
            this.FormClosing -= Form1_FormClosing;
            this.Resize -= Form1_Resize;
            notifyIcon1.DoubleClick -= NotifyIcon1_DoubleClick;
            if (checkBox3.Checked)
            {
                this.FormClosing += Form1_FormClosing;
                this.Resize += Form1_Resize;
                notifyIcon1.DoubleClick += NotifyIcon1_DoubleClick;
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

            }

            if (checkBox5.Checked)
            {

                var currentPowerPlanGuid = GetCurrentPowerPlanGuid();
                appSettings["powerPlanID"] = currentPowerPlanGuid;
                SavePowerPlanGuidToConfigFile(currentPowerPlanGuid);


                if (appSettings["powerPlanSetID"] == "")
                {

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
                checkBox5.Text = "Uruchom ponownie aby zastosować zmiany";
            }
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


            if (appSettings["protect"] == "1")
            {
                tabControl1.TabPages["tabPage1"].Enabled = false;
                tabControl1.TabPages["tabPage2"].Enabled = false;
                tabControl1.TabPages["tabPage3"].Enabled = false;
                button6.Enabled = false;
                label19.Visible = true;
                label19.Enabled = true;
                label19.BringToFront();
            }
        }

        private void textBoxUser_TextChanged(object sender, EventArgs e)
        {
            appSettings["userSet"] = textBoxUser.Text;
            SaveConfigFile();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var psi2 = new ProcessStartInfo
            {
                FileName = configFilePath,
                UseShellExecute = true
            };
            Process.Start(psi2);
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

        private void button7_Click(object sender, EventArgs e)
        {
            string scriptPath = GetDataFilePath("fixSettings.ps1");
            if (File.Exists(scriptPath))
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(scriptPath)
                };
                Process.Start(psi)?.WaitForExit();
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["userCommandBehaviour"] = checkBox7.Checked ? "1" : "0";
            SaveConfigFile();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["timezoneCheck"] = checkBox8.Checked ? "1" : "0";
            SaveConfigFile();
            if (checkBox8.Checked)
                SystemEvents.TimeChanged += SystemEvents_TimeChanged;
            else
                SystemEvents.TimeChanged -= SystemEvents_TimeChanged;
        }

        private void SystemEvents_TimeChanged(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                if (timer2.Enabled)
                {
                    foreach (var t in _taskList.Where(t => t.ActivatorType == 2 && t.Enabled && t.LocalTarget != default))
                        t.EndTimeUtc = t.LocalTarget.ToUniversalTime();
                    if (_timer2LocalTarget != default)
                        timer2EndTime = _timer2LocalTarget.ToUniversalTime();
                }
                if (timer3.Enabled)
                {
                    foreach (var t in _taskList.Where(t => t.ActivatorType == 3 && t.Enabled && string.IsNullOrEmpty(t.RepeatIntervalType)))
                        t.LastFired = t.LastFired.Date < DateTime.Now.Date ? t.LastFired : DateTime.MinValue;
                }
            }));
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["noTray"] = checkBox9.Checked ? "1" : "0";
            SaveConfigFile();
            notifyIcon1.Visible = !checkBox9.Checked;
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["noTimerShow"] = checkBox10.Checked ? "1" : "0";
            SaveConfigFile();
            bool show = !checkBox10.Checked;
            textBox5.Visible = show;
            textBox9.Visible = show;
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            appSettings["darkModeEnabled"] = checkBox11.Checked ? "1" : "0";
            SaveConfigFile();
            ApplyTheme(checkBox11.Checked);
        }

        private void ApplyTheme(bool dark)
        {
            Color bg = dark ? Color.FromArgb(30, 30, 30) : SystemColors.ButtonHighlight;
            Color fg = dark ? Color.FromArgb(220, 220, 220) : SystemColors.ControlText;
            Color input = dark ? Color.FromArgb(55, 55, 55) : SystemColors.Window;
            this.BackColor = dark ? Color.FromArgb(25, 25, 25) : SystemColors.Control;
            ApplyThemeToControls(this.Controls, bg, fg, input, dark);
        }

        private void ApplyThemeToControls(Control.ControlCollection controls, Color bg, Color fg, Color input, bool dark)
        {
            foreach (Control ctrl in controls)
            {
                string typeName = ctrl.GetType().Name;
                if (typeName == "TabPage" || typeName == "GroupBox" || typeName == "Panel")
                    ctrl.BackColor = bg;
                else if (typeName == "TextBox" || typeName == "ComboBox" || typeName == "DateTimePicker" || typeName == "CheckedListBox")
                {
                    ctrl.BackColor = input;
                    ctrl.ForeColor = fg;
                }
                else if (typeName == "Label")
                {

                    if (ctrl.BackColor != Color.SlateGray)
                    { ctrl.BackColor = Color.Transparent; ctrl.ForeColor = fg; }
                }
                else if (typeName == "DataGridView")
                {
                    var dgv = (DataGridView)ctrl;
                    dgv.BackgroundColor = dark ? Color.FromArgb(30, 30, 30) : SystemColors.AppWorkspace;
                    dgv.DefaultCellStyle.BackColor = dark ? Color.FromArgb(42, 42, 45) : SystemColors.Window;
                    dgv.DefaultCellStyle.ForeColor = fg;
                    dgv.DefaultCellStyle.SelectionBackColor = dark ? Color.FromArgb(60, 80, 115) : SystemColors.Highlight;
                    dgv.DefaultCellStyle.SelectionForeColor = dark ? Color.White : SystemColors.HighlightText;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = dark ? Color.FromArgb(50, 50, 55) : SystemColors.ButtonFace;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = fg;
                    dgv.RowHeadersDefaultCellStyle.BackColor = dark ? Color.FromArgb(45, 45, 48) : SystemColors.ButtonFace;
                    dgv.GridColor = dark ? Color.FromArgb(65, 65, 68) : SystemColors.ActiveBorder;
                    dgv.EnableHeadersVisualStyles = false;
                }
                else if (typeName == "CheckBox")
                {
                    var cb = ctrl as System.Windows.Forms.CheckBox;
                    if (cb != null) { cb.BackColor = bg; cb.ForeColor = fg; }
                }
                else if (typeName == "Button")
                {
                    var btn = ctrl as System.Windows.Forms.Button;
                    if (btn != null)
                    {
                        if (dark) { btn.FlatStyle = FlatStyle.Flat; btn.BackColor = Color.FromArgb(58, 58, 58); btn.FlatAppearance.BorderColor = Color.FromArgb(90, 90, 90); }
                        else { btn.FlatStyle = FlatStyle.Standard; btn.BackColor = SystemColors.ButtonFace; }
                        btn.ForeColor = fg;
                    }
                }
                else if (typeName == "TabControl")
                { ctrl.BackColor = bg; ctrl.ForeColor = fg; }
                if (ctrl.HasChildren && typeName != "DataGridView")
                    ApplyThemeToControls(ctrl.Controls, bg, fg, input, dark);
            }
        }


        private static string GetDataFilePath(string filename) =>
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", filename);


        private static void EnsureDataFiles()
        {
            string dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            Directory.CreateDirectory(dataDir);
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            string[] files = { "plan.pow", "w7restorePlan.ps1", "fixSettings.ps1" };
            foreach (var fn in files)
            {
                string dest = Path.Combine(dataDir, fn);
                if (File.Exists(dest)) continue;

                string[] candidates = { $"TimerSys.data.{fn}", $"Timer2.data.{fn}", $"Wyłącznik czasowy.data.{fn}" };
                foreach (var resName in candidates)
                {
                    using var stream = asm.GetManifestResourceStream(resName);
                    if (stream == null) continue;
                    using var fs = File.Create(dest);
                    stream.CopyTo(fs);
                    break;
                }
            }


            string soundsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sounds");
            Directory.CreateDirectory(soundsDir);
            string soundDest = Path.Combine(soundsDir, "sound.wma");
            if (!File.Exists(soundDest))
            {
                using var sndStream = asm.GetManifestResourceStream("TimerSys.data.sound.wma");
                if (sndStream != null)
                {
                    using var fs = File.Create(soundDest);
                    sndStream.CopyTo(fs);
                }
            }


            string versionFile = Path.Combine(dataDir, "version.txt");
            if (!File.Exists(versionFile))
                File.WriteAllText(versionFile, "1.0.6");


            string iconsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icons");
            Directory.CreateDirectory(iconsDir);
            var asmIcons = System.Reflection.Assembly.GetExecutingAssembly();
            foreach (string resName in asmIcons.GetManifestResourceNames())
            {
                if (!resName.EndsWith(".ico", StringComparison.OrdinalIgnoreCase)) continue;
                string[] parts  = resName.Split('.');
                string fileName = parts.Length >= 2 ? $"{parts[^2]}.{parts[^1]}" : resName;
                string destPath = Path.Combine(iconsDir, fileName);
                if (!File.Exists(destPath))
                {
                    using var srcIco  = asmIcons.GetManifestResourceStream(resName);
                    if (srcIco == null) continue;
                    using var dstIco = File.Create(destPath);
                    srcIco.CopyTo(dstIco);
                }
            }
        }


        private void StartSchedule()
        {
            if (!TimeSpan.TryParse(textBox1.Text, out _) && !TimeSpan.TryParse(textBox1.Text + ":00", out _))
            {
                MessageBox.Show("Podaj godzinę w formacie HH:MM", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Wybierz co najmniej jeden termin powtarzania.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            appSettings["timer3HourActivateBy"] = textBox1.Text;
            appSettings["timer3CheckedDays"] = string.Join(",", checkedListBox1.CheckedIndices.Cast<int>());
            appSettings["timer3Enabled"] = "1";
            SaveConfigFile();
            timer3.Start();
        }


        private void ToggleScheduleFromTray(object sender, EventArgs e) =>
            SetHarmonogramRunning(appSettings["timer3UserPaused"] == "1");

        private void ApplyHarmonogramUI(bool running)
        {
            comboBox3.Enabled = running;
            button8.Enabled = running;
            checkedListBox1.Enabled = running;
            textBox1.Enabled = running;
            if (_btnActSettings3 != null)
                _btnActSettings3.Enabled = running && ActionHasParams(comboBox3.SelectedIndex);
            stopStartHarmonogram.Text = running ? "⏸ Zatrzymaj harmonogram" : "▶ Uruchom harmonogram";
            if (_menuScheduleToggle != null)
                _menuScheduleToggle.Text = running ? "⏸ Zatrzymaj harmonogram" : "▶ Uruchom harmonogram";
        }

        private void SetHarmonogramRunning(bool running)
        {
            if (running)
            {
                if (_taskList.Any(t => t.ActivatorType == 3 && t.Enabled))
                    timer3.Start();
                appSettings["timer3Enabled"] = timer3.Enabled ? "1" : "0";
                appSettings["timer3UserPaused"] = "0";
            }
            else
            {
                timer3.Stop();
                appSettings["timer3Enabled"] = "0";
                appSettings["timer3UserPaused"] = "1";
            }
            SaveConfigFile();
            ApplyHarmonogramUI(running);
        }


        private void AddTaskAndRun(int activatorType)
        {
            string action = activatorType switch
            {
                1 => comboBox1.SelectedItem?.ToString() ?? "",
                2 => comboBox2.SelectedItem?.ToString() ?? "",
                3 => comboBox3.SelectedItem?.ToString() ?? "",
                _ => ""
            };
            if (string.IsNullOrEmpty(action)) return;


            var task = new TaskEntry { ActivatorType = activatorType, Action = action, Enabled = true };
            _taskList.Add(task);
            SaveConfigFile();
            RefreshTaskGrid();

            switch (activatorType)
            {
                case 1:
                    if (!TimeSpan.TryParse(textBox4.Text, out var ts1))
                    {
                        MessageBox.Show("Nieprawidłowy format czasu bezczynności.", "Błąd",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        _taskList.Remove(task); SaveConfigFile(); RefreshTaskGrid();
                        return;
                    }
                    task.IdleTimeout = textBox4.Text;
                    if (!timer1.Enabled)
                    {
                        timer1EndTime = DateTime.Now.Add(ts1);
                        timer1.Start();
                    }
                    break;

                case 2:
                    if (!TimeSpan.TryParse(textBox7.Text, out var selTime))
                    {
                        MessageBox.Show("Nieprawidłowy format czasu.", "Błąd",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        _taskList.Remove(task); SaveConfigFile(); RefreshTaskGrid();
                        return;
                    }
                    DateTime target = dateTimePicker1.Value.Date + selTime;
                    TimeSpan toWait = target - DateTime.Now;
                    if (toWait.TotalSeconds <= 0)
                    {
                        textBox9.Text = "Wybrałeś datę z przeszłości, wybierz tę z przyszłości.";
                        _taskList.Remove(task); SaveConfigFile(); RefreshTaskGrid();
                        return;
                    }

                    task.EndTimeUtc  = DateTime.UtcNow.Add(toWait);
                    task.LocalTarget = target;
                    _timer2LocalTarget = target;
                    timer2EndTime = task.EndTimeUtc;
                    SaveConfigFile();
                    RefreshTaskGrid();
                    timer2.Start();
                    break;

                case 3:
                    task.ScheduleHour = textBox1.Text;
                    task.ScheduleDays = string.Join(",", checkedListBox1.CheckedIndices.Cast<int>());

                    bool isInterval = checkedListBox1.GetItemChecked(SCHEDULE_INTERVAL_IDX);
                    if (isInterval)
                    {

                        string repType  = appSettings["timer3_repeat_type"]  ?? "m";
                        int    repValue = int.TryParse(appSettings["timer3_repeat_value"], out int rv) ? rv : 30;
                        task.RepeatIntervalType  = repType;
                        task.RepeatIntervalValue = repValue;
                        task.LastFired = DateTime.Now;
                        appSettings["timer3Enabled"] = "1";
                        SaveConfigFile();
                        timer3.Start();
                    }
                    else
                    {
                        SaveConfigFile();
                        StartSchedule();
                    }
                    break;
            }


            tabControl1.SelectedTab = tabControl1.TabPages["tabPage5"];
        }


        private void Timer3_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var current = new TimeSpan(now.Hour, now.Minute, 0);
            bool anyFired = false;

            var schTasks = _taskList.Where(t => t.ActivatorType == 3 && t.Enabled).ToList();
            foreach (var t in schTasks)
            {

                if (!string.IsNullOrEmpty(t.RepeatIntervalType))
                {
                    double intervalMinutes = t.RepeatIntervalType switch
                    {
                        "h" => t.RepeatIntervalValue * 60.0,
                        "d" => t.RepeatIntervalValue * 1440.0,
                        _   => t.RepeatIntervalValue
                    };
                    if ((now - t.LastFired).TotalMinutes >= intervalMinutes)
                    {
                        _currentlyFiringTask = t;
                        ExecuteCommand(t.Action, 3);
                        _currentlyFiringTask = null;
                        t.LastFired = now;
                        anyFired = true;
                    }
                    continue;
                }


                string hourStr = string.IsNullOrEmpty(t.ScheduleHour) ? textBox1.Text : t.ScheduleHour;
                if (!TimeSpan.TryParse(hourStr, out var targetTime))
                    if (!TimeSpan.TryParse(hourStr + ":00", out targetTime)) continue;

                if (Math.Abs((current - targetTime).TotalMinutes) >= 0.5) continue;
                if (t.LastFired.Date == now.Date) continue;

                string daysStr = string.IsNullOrEmpty(t.ScheduleDays)
                    ? string.Join(",", checkedListBox1.CheckedIndices.Cast<int>())
                    : t.ScheduleDays;
                if (!IsTaskScheduledToday(daysStr, t.LastFired, now)) continue;

                _currentlyFiringTask = t;
                ExecuteCommand(t.Action, 3);
                _currentlyFiringTask = null;
                t.LastFired = now;
                anyFired = true;
            }

            if (anyFired)
            {
                _lastFiredScheduleMinute = now.Minute;
                _lastFiredDate = now.Date;
                appSettings["timer3LastFired"] = now.ToString("yyyy-MM-dd");
                SaveConfigFile();
                RefreshTaskGrid();
            }
        }

        private DateTime? GetNextScheduleFireTime()
        {
            if (!TimeSpan.TryParse(textBox1.Text, out var targetTime))
                if (!TimeSpan.TryParse(textBox1.Text + ":00", out targetTime)) return null;
            if (checkedListBox1.CheckedItems.Count == 0) return null;

            for (int daysAhead = 0; daysAhead <= 8; daysAhead++)
            {
                var candidate = DateTime.Today.AddDays(daysAhead).Add(targetTime);
                if (candidate <= DateTime.Now) continue;
                if (IsDateScheduledForDisplay(candidate.Date)) return candidate;
            }
            return null;
        }

        private bool IsDateScheduledForDisplay(DateTime date)
        {
            var dow = date.DayOfWeek;
            DateTime.TryParse(appSettings["timer3LastFired"] ?? "", out DateTime lastFired);

            foreach (int idx in checkedListBox1.CheckedIndices.Cast<int>())
            {
                switch (idx)
                {
                    case 0: return true;
                    case 1: if ((date.Date - lastFired.Date).TotalDays >= 2) return true; break;
                    case 2: if ((date.Date - lastFired.Date).TotalDays >= 3) return true; break;
                    case 3: if ((date.Date - lastFired.Date).TotalDays >= 4) return true; break;
                    case 4: if ((date.Date - lastFired.Date).TotalDays >= 5) return true; break;
                    case 5: if ((date.Date - lastFired.Date).TotalDays >= 6) return true; break;
                    case 6: if ((date.Date - lastFired.Date).TotalDays >= 7) return true; break;
                    case 7: if (dow == DayOfWeek.Friday || dow == DayOfWeek.Saturday || dow == DayOfWeek.Sunday) return true; break;
                    case 8: if (dow == DayOfWeek.Monday) return true; break;
                    case 9: if (dow == DayOfWeek.Tuesday) return true; break;
                    case 10: if (dow == DayOfWeek.Wednesday) return true; break;
                    case 11: if (dow == DayOfWeek.Thursday) return true; break;
                    case 12: if (dow == DayOfWeek.Friday) return true; break;
                    case 13: if (dow == DayOfWeek.Saturday) return true; break;
                    case 14: if (dow == DayOfWeek.Sunday) return true; break;
                    case 15: if (dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday) return true; break;
                }
            }
            return false;
        }


        private bool IsTodayScheduled()
        {
            var daysStr = string.Join(",", checkedListBox1.CheckedIndices.Cast<int>());
            DateTime.TryParse(appSettings["timer3LastFired"] ?? "", out DateTime lastFired);
            return IsTaskScheduledToday(daysStr, lastFired, DateTime.Now);
        }


        private static bool IsTaskScheduledToday(string daysStr, DateTime lastFired, DateTime today)
        {
            if (string.IsNullOrEmpty(daysStr)) return false;
            var dow = today.DayOfWeek;
            foreach (var part in daysStr.Split(','))
            {
                if (!int.TryParse(part.Trim(), out int idx)) continue;
                switch (idx)
                {
                    case 0: return true;
                    case 1: if ((today.Date - lastFired.Date).TotalDays >= 2) return true; break;
                    case 2: if ((today.Date - lastFired.Date).TotalDays >= 3) return true; break;
                    case 3: if ((today.Date - lastFired.Date).TotalDays >= 4) return true; break;
                    case 4: if ((today.Date - lastFired.Date).TotalDays >= 5) return true; break;
                    case 5: if ((today.Date - lastFired.Date).TotalDays >= 6) return true; break;
                    case 6: if ((today.Date - lastFired.Date).TotalDays >= 7) return true; break;
                    case 7: if (dow == DayOfWeek.Friday || dow == DayOfWeek.Saturday || dow == DayOfWeek.Sunday) return true; break;
                    case 8: if (dow == DayOfWeek.Monday) return true; break;
                    case 9: if (dow == DayOfWeek.Tuesday) return true; break;
                    case 10: if (dow == DayOfWeek.Wednesday) return true; break;
                    case 11: if (dow == DayOfWeek.Thursday) return true; break;
                    case 12: if (dow == DayOfWeek.Friday) return true; break;
                    case 13: if (dow == DayOfWeek.Saturday) return true; break;
                    case 14: if (dow == DayOfWeek.Sunday) return true; break;
                    case 15: if (dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday) return true; break;
                }
            }
            return false;
        }


        private void RefreshTaskGrid()
        {
            if (dataGridView1 == null) return;
            dataGridView1.Rows.Clear();
            var items = _taskListFilter == 0
                ? _taskList
                : _taskList.Where(t => t.ActivatorType == _taskListFilter).ToList();
            foreach (var t in items)
            {
                string actName = t.ActivatorType switch
                {
                    1 => "Bezczynność",
                    2 => "Stoper",
                    3 => "Harmonogram",
                    _ => "?"
                };
                int rowIdx = dataGridView1.Rows.Add(
                    t.Action,
                    GetTaskNextActivation(t),
                    t.Enabled ? "✓" : "✗",
                    actName
                );
                dataGridView1.Rows[rowIdx].Tag = t.Id;

                if (!t.Enabled)
                    dataGridView1.Rows[rowIdx].DefaultCellStyle.ForeColor = Color.Gray;
            }
        }


        private string GetTaskNextActivation(TaskEntry t)
        {
            if (!t.Enabled) return "Wykonane";
            switch (t.ActivatorType)
            {
                case 2:
                    if (t.EndTimeUtc != default)
                    {
                        var rem = t.EndTimeUtc - DateTime.UtcNow;
                        return rem.TotalSeconds > 0 ? rem.ToString(@"hh\:mm\:ss") : "Wygasło";
                    }
                    return "-";
                case 3:
                    if (!string.IsNullOrEmpty(t.RepeatIntervalType))
                    {

                        double iMin = t.RepeatIntervalType switch
                        {
                            "h" => t.RepeatIntervalValue * 60.0,
                            "d" => t.RepeatIntervalValue * 1440.0,
                            _   => t.RepeatIntervalValue
                        };
                        var nextFire = t.LastFired.AddMinutes(iMin);
                        var rem = nextFire - DateTime.Now;
                        string unitLabel = t.RepeatIntervalType switch { "h" => "godz", "d" => "dni", _ => "min" };
                        string prefix = $"co {t.RepeatIntervalValue} {unitLabel}: ";
                        return prefix + (rem.TotalSeconds > 0 ? rem.ToString(@"hh\:mm\:ss") : "Zaraz...");
                    }
                    string hourStr = string.IsNullOrEmpty(t.ScheduleHour) ? textBox1.Text : t.ScheduleHour;
                    if (!TimeSpan.TryParse(hourStr, out var targetTime))
                        if (!TimeSpan.TryParse(hourStr + ":00", out targetTime)) return "-";
                    string daysStr = string.IsNullOrEmpty(t.ScheduleDays)
                        ? string.Join(",", checkedListBox1.CheckedIndices.Cast<int>())
                        : t.ScheduleDays;
                    for (int d = 0; d <= 8; d++)
                    {
                        var candidate = DateTime.Today.AddDays(d).Add(targetTime);
                        if (candidate <= DateTime.Now) continue;
                        if (IsTaskScheduledToday(daysStr, t.LastFired, candidate.Date.Add(TimeSpan.Zero)))
                            return candidate.ToString("dd.MM HH:mm");
                    }
                    return "-";
                case 1:
                    return timer1.Enabled ? "Przy bezczynnosci" : "Oczekuje";
                default:
                    return "-";
            }
        }

    }


    public class TaskEntry
    {
        public string   Id            { get; set; } = Guid.NewGuid().ToString("N")[..8];
        public int      ActivatorType { get; set; }
        public string   Action        { get; set; } = "";
        public bool     Enabled       { get; set; } = true;
        public string   Notes         { get; set; } = "";

        public DateTime EndTimeUtc    { get; set; }
        public DateTime LocalTarget   { get; set; }

        public string   ScheduleHour  { get; set; } = "";
        public string   ScheduleDays  { get; set; } = "";
        public DateTime LastFired     { get; set; }

        public string   IdleTimeout   { get; set; } = "";

        public string   RepeatIntervalType  { get; set; } = "";
        public int      RepeatIntervalValue { get; set; }
    }
}
