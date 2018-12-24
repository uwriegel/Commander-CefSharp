using CefSharp;
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
    public partial class MainForm : Form, ILoadHandler
    {
        #region ILoadHandler

        public void OnFrameLoadStart(IWebBrowser browserControl, FrameLoadStartEventArgs frameLoadStartArgs) {}

        public void OnFrameLoadEnd(IWebBrowser browserControl, FrameLoadEndEventArgs frameLoadEndArgs)
        {
            if (frameLoadEndArgs.Frame.IsMain)
                BeginInvoke((Action)(() => browser.Focus()));
        }

        public void OnLoadError(IWebBrowser browserControl, LoadErrorEventArgs loadErrorArgs) {}

        public void OnLoadingStateChange(IWebBrowser browserControl, LoadingStateChangedEventArgs loadingStateChangedArgs) {}

        #endregion

        public MainForm()
        {
            // TODO: Keyboardhandler: Shortcuts
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
            CreateMenu(this);

            browser = new ChromiumWebBrowser("")
            {
                LoadHandler = this
            };
            SuspendLayout();

            KeyPreview = true;

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

            Controls.Add(browser);
            Name = "MainForm";
            Text = "Commander";

            ResumeLayout(false);

            browser.Focus();
        }

        void CreateMenu(Control parent)
        {
            var menu = new MainMenu();

            var itemFile = new MenuItem("&File");
            menu.MenuItems.Add(itemFile);

            var itemView = new MenuItem("&View");
            menu.MenuItems.Add(itemView);

            var itemDevTools = new MenuItem("&Developer Tools", OnDevTools, Shortcut.F12);
            itemView.MenuItems.Add(itemDevTools);
            Menu = menu;
        }

        void OnDevTools(object src, EventArgs args) => browser.GetBrowser().ShowDevTools();

        ChromiumWebBrowser browser;
    }
}
