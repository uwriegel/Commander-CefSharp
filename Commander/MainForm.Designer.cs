using CefSharp.WinForms;
using Commander.Properties;

namespace Commander
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        System.ComponentModel.IContainer components = null;

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
            browser = new ChromiumWebBrowser("");
            this.SuspendLayout();

            browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            browser.Location = new System.Drawing.Point(0, 0);
            browser.Size = new System.Drawing.Size(800, 450);
            browser.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            
            this.Icon = Resources.Kirk;
            this.Controls.Add(browser);
            this.Name = "MainForm";
            this.Text = "Commander";
            this.ResumeLayout(false);
        }

        #endregion

        ChromiumWebBrowser browser;
    }
}

