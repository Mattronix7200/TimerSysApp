namespace Timer2
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
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
    }
}
