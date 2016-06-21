using System;
using System.Windows.Forms;

namespace PSRunner
{
    static class Program
    {
        private const Keys DEFAULT_HOTKEY = Keys.N;

        private static Run RunWindow { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
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

                hook.RegisterHotKey(ModifierKeys.Win, GetHotkey(args));
            }
            catch
            {
                MessageBox.Show("Unable to register the global hotkey Win+" + GetHotkey(args) + ".\r\n\r\nCheck to " +
                                "make sure that the key combination is not already in use by " +
                                "Windows or another application, and that PSRunner is not " +
                                "already running. Only one instance of PSRunner may run at a time.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Application.Run();

            hook.Dispose();
        }

        private static Keys GetHotkey(string[] programArgs)
        {
            try
            {
                if (programArgs.Length >= 1)
                {
                    return (Keys)Enum.Parse(typeof(Keys), programArgs[0].Trim());
                }
                return DEFAULT_HOTKEY;
            }
            catch
            {
                return DEFAULT_HOTKEY;
            }
        }
    }
}
