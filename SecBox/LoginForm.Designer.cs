using System;

namespace SecBox
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.uxLogin = new System.Windows.Forms.Button();
            this.uxUsername = new System.Windows.Forms.TextBox();
            this.uxPassword = new SecBox.SecurePasswordBox();
            this.SuspendLayout();
            // 
            // uxLogin
            // 
            this.uxLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uxLogin.Location = new System.Drawing.Point(118, 64);
            this.uxLogin.Name = "uxLogin";
            this.uxLogin.Size = new System.Drawing.Size(75, 23);
            this.uxLogin.TabIndex = 2;
            this.uxLogin.Text = "Login";
            this.uxLogin.UseVisualStyleBackColor = true;
            // 
            // uxUsername
            // 
            this.uxUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxUsername.Location = new System.Drawing.Point(12, 12);
            this.uxUsername.Name = "uxUsername";
            this.uxUsername.Size = new System.Drawing.Size(181, 20);
            this.uxUsername.TabIndex = 0;
            this.uxUsername.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyOverrides);
            // 
            // uxPassword
            // 
            this.uxPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uxPassword.Location = new System.Drawing.Point(12, 38);
            this.uxPassword.Name = "uxPassword";
            this.uxPassword.Size = new System.Drawing.Size(181, 20);
            this.uxPassword.TabIndex = 1;
            this.uxPassword.UseSystemPasswordChar = true;
            this.uxPassword.TextChanged += new System.EventHandler(this.UxPasswordTextChanged);
            this.uxPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.KeyOverrides);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(205, 104);
            this.Controls.Add(this.uxUsername);
            this.Controls.Add(this.uxPassword);
            this.Controls.Add(this.uxLogin);
            this.Name = "LoginForm";
            this.Text = "Login Test";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button uxLogin;
        private SecBox.SecurePasswordBox uxPassword;
        private System.Windows.Forms.TextBox uxUsername;
    }
}

