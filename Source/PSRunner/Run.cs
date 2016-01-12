using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PSRunner
{
    /// <summary>
    /// The Run dialog's codebehind.
    /// </summary>
    /// <remarks>
    /// TODO: Make this WPF instead of WinForms.
    /// </remarks>
    public partial class Run : Form
    {
        /// <summary>
        /// Stores the registry location of the history items.
        /// </summary>
        private const string HISTORY_LOCATION = @"HKEY_CURRENT_USER\Software\PSRunner";

        /// <summary>
        /// Stores the number of history items to save in the registry.
        /// </summary>
        private const int HISTORY_ITEM_COUNT = 15;

        /// <summary>
        /// Initializes a new instance of the Run form class.
        /// </summary>
        public Run()
        {
            InitializeComponent();

            // Pull and build the history list.
            var list = Registry.GetValue(HISTORY_LOCATION, "AutocompleteList", null);
            if (list != null)
            {
                foreach (var item in list.ToString().Split('`'))
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        CommandBox.Items.Add(item);
                    }
                }
                CommandBox.Text = CommandBox.Items[0].ToString();
            }
        }

        /// <summary>
        /// Occurs when the window has loaded.
        /// </summary>
        private void Run_Load(object sender, EventArgs e)
        {
            // Set the position of the window to behave nearly identically to the Windows Run dialog.ff
            Top = Screen.PrimaryScreen.WorkingArea.Height - Height - 4;
            Left = 8;
        }

        /// <summary>
        /// Occurs when the form is shown on the screen.
        /// </summary>
        private void Run_Shown(object sender, EventArgs e)
        {
            // Ensure that the window and text area both have focus when the window comes into view.
            Activate();
            CommandBox.Focus();
        }

        /// <summary>
        /// Occurs when a key is pressed in the command box.
        /// </summary>
        private void CommandBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Execute the command on Enter, and check if we need to run as admin.
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                if (!string.IsNullOrWhiteSpace(CommandBox.Text))
                {
                    var elevate = (ModifierKeys & Keys.Control) > 0 && (ModifierKeys & Keys.Shift) > 0;
                    SaveAutocomplete();
                    RunCommand(CommandBox.Text, elevate);
                }

                // After executing (which is somewhat asynchronous), close the dialog.
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        /// <summary>
        /// Occurs when the user clicks the OK button.
        /// </summary>
        private void OKButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CommandBox.Text))
            {
                SaveAutocomplete();
                RunCommand(CommandBox.Text, false);
            }
        }

        /// <summary>
        /// Occurs when the user presses a key.
        /// </summary>
        private void Escape_KeyUp(object sender, KeyEventArgs e)
        {
            // If the user hits escape, close the dialog and do nothing.
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>
        /// Runs the specified PowerShell command by creating a new PowerShell.exe process.
        /// </summary>
        /// <param name="command">The command text to run.</param>
        /// <param name="elevate">Whether or not to run the command in an elevated context.</param>
        private void RunCommand(string command, bool elevate)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Environment.ExpandEnvironmentVariables(@"%systemroot%\System32\WindowsPowerShell\v1.0\powershell.exe"),
                    Arguments = command,
                    UseShellExecute = true
                }
            };
            if (elevate)
            {
                p.StartInfo.Verb = "runas";
            }
            p.Start();
        }

        /// <summary>
        /// Saves the autocomplete history, and prunes the list as necessary, removing duplicates and limiting the number of items.
        /// </summary>
        private void SaveAutocomplete()
        {
            // Get history list
            var history = (from object item in CommandBox.Items 
                           select item.ToString()).ToList();

            // Add new item if not duplicate
            if (history.Contains(CommandBox.Text))
            {
                history.Remove(CommandBox.Text);
            }
            history.Insert(0, CommandBox.Text);

            // Trim to the configured number of items
            if (history.Count > HISTORY_ITEM_COUNT)
            {
                history = history.Take(HISTORY_ITEM_COUNT).ToList();
            }

            // Save to registry
            // TODO: Use a better delimiter character than a backtick ("`").
            Registry.SetValue(HISTORY_LOCATION, "AutocompleteList", string.Join("`", history.ToArray()));
        }
    }
}
