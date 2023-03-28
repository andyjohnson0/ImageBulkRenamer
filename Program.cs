using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace uk.andyjohnson.ImageBulkRenamer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.SetCompatibleTextRenderingDefault(false);
#pragma warning disable CA1416 // Validate platform compatibility
            Application.Run(new MainForm());
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
}
