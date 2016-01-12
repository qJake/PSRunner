using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/// <summary>
/// The enumeration of possible modifiers.
/// </summary>
[Flags]
public enum ModifierKeys : uint
{
    Alt = 1,
    Control = 2,
    Shift = 4,
    Win = 8
}

/// <summary>
/// Event Args for the event that is fired after the hot key has been pressed.
/// </summary>
public class KeyPressedEventArgs : EventArgs
{
    internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
    {
        Modifier = modifier;
        Key = key;
    }

    public ModifierKeys Modifier { get; }

    public Keys Key { get; }
}

public sealed  class KeyboardHook : IDisposable
{
    /// <summary>
    /// Occurs when one of the registered hotkeys has been pressed.
    /// </summary>
    public event EventHandler<KeyPressedEventArgs> KeyPressed;

    /// <summary>
    /// Registers a global hotkey with the OS.
    /// </summary>
    /// <returns>True on success, false otherwise.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    /// <summary>
    /// Unregisters a global hotkey with the OS.
    /// </summary>
    /// <returns>True on success, false otherwise.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private readonly Window window = new Window();
    private int currentId;

    public KeyboardHook()
    {
        // register the event of the inner native window.
        window.KeyPressed += (_, e) =>
        {
            KeyPressed?.Invoke(this, e);
        };
    }

    /// <summary>
    /// Registers a hot key in the system.
    /// </summary>
    /// <param name="modifier">The modifiers that are associated with the hot key.</param>
    /// <param name="key">The key itself that is associated with the hot key.</param>
    public void RegisterHotKey(ModifierKeys modifier, Keys key)
    {
        currentId = currentId + 1;

        if (!RegisterHotKey(window.Handle, currentId, (uint)modifier, (uint)key))
        {
            throw new InvalidOperationException("Couldn’t register the hot key. Code: " + Marshal.GetLastWin32Error() + ", Error: " + new Win32Exception(Marshal.GetLastWin32Error()).Message);
        }
    }

    #region IDisposable Members

    public void Dispose()
    {
        // unregister all the registered hot keys.
        for (var i = currentId; i > 0; i--)
        {
            UnregisterHotKey(window.Handle, i);
        }

        // dispose the inner native window.
        window.Dispose();
    }

    #endregion

    #region Window subclass

    /// <summary>
    /// Represents the window that is used internally to get the messages.
    /// </summary>
    private sealed class Window : NativeWindow, IDisposable
    {
        private const int WM_HOTKEY = 0x0312;

        public Window()
        {
            // create the handle for the window.
            CreateHandle(new CreateParams());
        }

        /// <summary>
        /// Overridden to get the notifications.
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // check if we got a hot key pressed.
            if (m.Msg == WM_HOTKEY)
            {
                // get the keys.
                var key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                var modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                // invoke the event to notify the parent.
                if (KeyPressed != null)
                    KeyPressed(this, new KeyPressedEventArgs(modifier, key));
            }
        }

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        #region IDisposable Members

        public void Dispose()
        {
            DestroyHandle();
        }

        #endregion
    }

    #endregion
}