using System;
using System.Diagnostics;
using System.Windows;

namespace PSWrap
{
    /// <summary>
    /// Wraps a PowerShell invocation into a silent process, so that no command window is displayed.
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Startup += (_, e) =>
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = string.Join(" ", e.Args),
                        CreateNoWindow = true,
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        WorkingDirectory = Environment.CurrentDirectory
                    }
                };
                p.Start();
                p.WaitForExit();
                Environment.Exit(p.ExitCode);
            };
        }
    }
}
