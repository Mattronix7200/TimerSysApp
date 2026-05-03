namespace Timer2
{
    internal static class Program
    {
        // Mutex statyczny – musi żyć przez cały czas działania procesu
        private static System.Threading.Mutex? _appMutex;

        [STAThread]
        static void Main(string[] args)
        {
            // ── Blokada jednego egzemplarza ──
            _appMutex = new System.Threading.Mutex(true, "TimerSys_SingleInstance_v1", out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Program TimerSys jest już uruchomiony.",
                    "TimerSys", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _appMutex.Dispose();
                return;
            }

            // ── Opcja "Uruchamiaj jako administrator" ──
            // Jeśli plik flagi istnieje i program nie ma jeszcze uprawnień, uruchom ponownie z UAC
            string adminFlagFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "runasadmin.flag");
            if (File.Exists(adminFlagFile) && !IsAdmin())
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName        = Application.ExecutablePath,
                        UseShellExecute = true,
                        Verb            = "runas",
                        Arguments       = string.Join(" ", args)
                    });
                    return; // zamknij tę instancję – uruchomiona elevated
                }
                catch
                {
                    // Użytkownik anulował UAC – kontynuuj bez uprawnień administratora
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form1 = new Form1();
            if (args.Length > 0 && args[0] == "--minimized")
            {
                form1.WindowState = FormWindowState.Minimized;
                form1.ShowInTaskbar = false;
                form1.StartTimer1();
            }
            Application.Run(form1);
        }

        private static bool IsAdmin()
        {
            using var id = System.Security.Principal.WindowsIdentity.GetCurrent();
            return new System.Security.Principal.WindowsPrincipal(id)
                .IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}
