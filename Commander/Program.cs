using System;
using System.Windows.Forms;

namespace Commander
{
    static class Program
    {
        // TODO: GetVersion, GetExif
        // TODO: Sort by column
        // TODO: One path: delay 5s, the other not, cancel long call
        // TODO: Hide hidden

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Cef.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
