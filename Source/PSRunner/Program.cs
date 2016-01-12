using System;
using System.Windows.Forms;

namespace PSRunner
{
    static class Program
    {
        private static Run RunWindow { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var hook = new KeyboardHook();

            try
            {
                hook.KeyPressed += (_, __) =>
                {
                    // Activate it if it's already there, otherwise create a new one.
                    if (RunWindow != null)
                    {
                        RunWindow.Activate();
                    }
                    else
                    {
                        RunWindow = new Run();
                        RunWindow.ShowDialog();
                        RunWindow = null;
                    }
                };

                // TODO: Make the key combination configurable or alterable somehow.
                hook.RegisterHotKey(ModifierKeys.Win, Keys.F);
            }
            catch
            {
                MessageBox.Show("Unable to register the global hotkey Win+F.\r\n\r\nCheck to " +
                                "make sure that the key combination is not already in use by " +
                                "Windows or another application, and that PSRunner is not " +
                                "already running. Only one instance of PSRunner may run at a time.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Application.Run();

            hook.Dispose();
        }
    }
}
