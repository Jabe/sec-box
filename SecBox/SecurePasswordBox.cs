using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using JetBrains.Annotations;

namespace SecBox
{
    [PublicAPI]
    public class SecurePasswordBox : TextBox
    {
        private const int WmPaste = 0x302;
        private const int WmChar = 0x102;

        private readonly SecureString _mutableSecureText = new SecureString();

        [PublicAPI]
        public SecurePasswordBox()
        {
            UseSystemPasswordChar = true;
        }

        [PublicAPI]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SecureString SecureText
        {
            get
            {
                SecureString copy = _mutableSecureText.Copy();
                copy.MakeReadOnly();
                return copy;
            }
        }

        [PublicAPI]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UnsafeString
        {
            get
            {
                IntPtr ptr = IntPtr.Zero;

                try
                {
                    ptr = Marshal.SecureStringToBSTR(_mutableSecureText);
                    return Marshal.PtrToStringBSTR(ptr);
                }
                finally
                {
                    if (ptr != IntPtr.Zero) Marshal.ZeroFreeBSTR(ptr);
                }
            }
        }

        [DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, string lParam);

        /// <summary>
        /// Processes Windows messages.
        /// </summary>
        /// <param name="m">A Windows Message object. </param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WmPaste)
            {
                HandlePaste();
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyDown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyEventArgs"/> that contains the event data. </param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (e.Modifiers == Keys.Control)
                {
                    // del right to end
                    if (SelectionLength == 0)
                    {
                        SelectionLength = Text.Length - SelectionStart;
                    }
                }
                else if (e.Modifiers == Keys.Shift && SelectionLength == 0)
                {
                    // del left: rewrite as backspace... but only if sellength = 0
                    // otherwise it's copy, which is is blocked by windows
                    SendMessage(Handle, WmChar, new IntPtr(0x08), null);
                    e.SuppressKeyPress = true;
                }

                HandleDelete();
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.KeyPress"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.KeyPressEventArgs"/> that contains the event data. </param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 0x08)
            {
                // backspace
                if (SelectionLength == 0)
                {
                    if (SelectionStart > 0 && _mutableSecureText.Length > 0)
                    {
                        _mutableSecureText.RemoveAt(SelectionStart - 1);
                    }
                }
                else
                {
                    HandleDelete();
                }
            }
            else if (e.KeyChar == 0x7F)
            {
                if (SelectionLength == 0)
                {
                    // del left to end
                    int caret = SelectionStart;
                    SelectionStart = 0;
                    SelectionLength = caret;
                }

                HandleDelete();

                // continue like a normal delete
                e.KeyChar = (char) 0x08;
            }
            else if (!char.IsControl(e.KeyChar))
            {
                _mutableSecureText.InsertAt(SelectionStart, e.KeyChar);
                e.KeyChar = '*';
            }

            base.OnKeyPress(e);
        }

        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            SecureTextChanged();
        }

        private void HandlePaste()
        {
            if (!Clipboard.ContainsText())
            {
                return;
            }

            string text;

            try
            {
                text = Clipboard.GetText();
            }
            catch
            {
                return;
            }

            foreach (char c in text)
            {
                SendMessage(Handle, WmChar, new IntPtr(c), null);
            }
        }

        private void HandleDelete()
        {
            int start = SelectionStart;
            int length = SelectionLength;

            for (int i = 0; i < Math.Max(1, length); i++)
            {
                if (start < _mutableSecureText.Length)
                {
                    _mutableSecureText.RemoveAt(start);
                }
            }
        }

        private void SecureTextChanged()
        {
#if DEBUG
            Console.WriteLine("New length: {0,4} | Text is: {1} | Dump: {2}", _mutableSecureText.Length, Text,
                              UnsafeString);
#endif
        }
    }
}