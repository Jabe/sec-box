﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace SecBox
{
    /// <summary>
    /// A password box implementation which behaves like a normal textbox, but stores all data in a <see cref="SecureString"/>.
    /// </summary>
    public class SecurePasswordBox : TextBox
    {
        #region Native Functions

        private const int WmPaste = 0x302;
        private const int WmChar = 0x102;

        [DllImport("user32.dll", EntryPoint = "SendMessageW", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, string lParam);

        #endregion

        private SecureString _mutableSecureText = new SecureString();

        public SecurePasswordBox()
        {
            UseSystemPasswordChar = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_mutableSecureText != null) _mutableSecureText.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets a copy of the current <see cref="SecureString"/> buffer.
        /// </summary>
        /// <remarks>You own the copy, so remember to <see cref="SecureString.Dispose"/> of it.</remarks>
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
            set
            {
                if (_mutableSecureText != null) _mutableSecureText.Dispose();
                _mutableSecureText = value.Copy();
                Text = "".PadLeft(_mutableSecureText.Length, '*');
            }
        }

        /// <summary>
        /// Get a managed <see cref="string"/> representation of the current <see cref="SecureString"/> buffer.
        /// </summary>
        /// <remarks>This is useful for debugging. Do not use in productive environments, as it defeats the purpose of this control and its backing <see cref="SecureString"/>.</remarks>
        [Browsable(false)]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string UnsecureText
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
            if (ReadOnly || !Enabled)
            {
                base.OnKeyDown(e);
                return;
            }

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
            if (ReadOnly || !Enabled)
            {
                base.OnKeyPress(e);
                return;
            }

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
                e.KeyChar = (char)0x08;
            }
            else if (!char.IsControl(e.KeyChar))
            {
                if (SelectionLength > 0)
                {
                    // overwrite
                    HandleDelete();
                }

                _mutableSecureText.InsertAt(SelectionStart, e.KeyChar);
                e.KeyChar = '*';
            }

            base.OnKeyPress(e);
        }

        /// <summary>
        /// Resets the <see cref="P:System.Windows.Forms.Control.Text"/> property to its default value.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void ResetText()
        {
            _mutableSecureText.Clear();
            base.ResetText();
        }

        private void HandlePaste()
        {
            if (ReadOnly || !Enabled)
            {
                return;
            }

            string text;

            try
            {
                if (!Clipboard.ContainsText()) return;
                text = Clipboard.GetText();
            }
            catch
            {
                return;
            }

            // send each char to ourself
            foreach (char c in text)
            {
                SendMessage(Handle, WmChar, new IntPtr(c), null);
            }
        }

        private void HandleDelete()
        {
            if (ReadOnly || !Enabled)
            {
                return;
            }

            int start = SelectionStart;
            int length = SelectionLength;

            // no selection: run once
            //    selection: run for each char
            for (int i = 0; i < Math.Max(1, length); i++)
            {
                if (start < _mutableSecureText.Length)
                {
                    _mutableSecureText.RemoveAt(start);
                }
            }
        }
    }
}