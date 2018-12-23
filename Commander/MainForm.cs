﻿using CefSharp;
using CefSharp.WinForms;
using Commander.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Commander
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            // TODO: Themes
            InitializeComponent();
            browser.Load("serve://commander");
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            browser = new ChromiumWebBrowser("");
            SuspendLayout();

            browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            browser.Location = new System.Drawing.Point(0, 0);
            browser.Size = new System.Drawing.Size(800, 450);
            browser.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Icon = Resources.Kirk;

            CreateMenu(this);

            Controls.Add(browser);
            Name = "MainForm";
            Text = "Commander";

            ResumeLayout(false);
        }

        void CreateMenu(Control parent)
        {
            var menu = new MenuStrip()
            {
                Parent = parent
            };

            var itemFile = new ToolStripMenuItem("&File");
            menu.Items.Add(itemFile);

            var itemView = new ToolStripMenuItem("&View");
            menu.Items.Add(itemView);

            var itemDevTools = new ToolStripMenuItem("&Developer Tools", null, OnDevTools, Keys.F12);
            itemView.DropDownItems.Add(itemDevTools);
            itemView.ShortcutKeys = Keys.F10;

        }

        void OnDevTools(object src, EventArgs args) => browser.GetBrowser().ShowDevTools();

        ChromiumWebBrowser browser;
    }
}
