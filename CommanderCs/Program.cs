using System;
using System.Windows.Forms;

namespace Commander
{
    static class Program
    {
        public static string[] CommandLine { get; private set; }

        public static string CommanderUrl {
            get => IsAngularServing
                ? "http://localhost:4200/"
                : "serve://commander/";
            }

        public static bool IsAngularServing { get => CommandLine.Length > 0 && CommandLine[0] == "-serve"; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(params string[] args)
        {
            CommandLine = args;
            Cef.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
