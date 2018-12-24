using CefSharp;
using CefSharp.WinForms;
using Commander.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Commander.Extension;

namespace Commander
{
    public partial class MainForm : Form, ILoadHandler, IKeyboardHandler
    {
        #region ILoadHandler

        public void OnFrameLoadStart(IWebBrowser browserControl, FrameLoadStartEventArgs frameLoadStartArgs) {}

        public void OnFrameLoadEnd(IWebBrowser browserControl, FrameLoadEndEventArgs frameLoadEndArgs)
        {
            if (frameLoadEndArgs.Frame.IsMain && frameLoadEndArgs.Frame.Url == "serve://commander/")
            {
                // TODO: read from Property
                browser.EvaluateScriptAsync("setTheme", "dark");
                BeginInvoke((Action)(() => browser.Focus()));
            }
                
        }

        public void OnLoadError(IWebBrowser browserControl, LoadErrorEventArgs loadErrorArgs) {}

        public void OnLoadingStateChange(IWebBrowser browserControl, LoadingStateChangedEventArgs loadingStateChangedArgs) {}

        #endregion

        #region IKeyboardHandler

        public bool OnPreKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser ibrowser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (type == KeyType.RawKeyDown)
            {
                var accelerator = accelerators.FirstOrDefault(n => n.HasValue ? n.Value.Key == windowsKeyCode
                    && (modifiers.HasFlag(CefEventFlags.AltDown) ? n.Value.Alt : !n.Value.Alt)
                    && (modifiers.HasFlag(CefEventFlags.ShiftDown) ? n.Value.Shift : !n.Value.Shift)
                    && (modifiers.HasFlag(CefEventFlags.ControlDown) ? n.Value.Ctrl : !n.Value.Ctrl) : false);

                if (accelerator.HasValue)
                {
                    accelerator.Value.MenuItem.PerformClick();
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool OnKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
            => false;

        #endregion
        public MainForm()
        {
            // TODO: Themes
            // TODO: Persistent Properties
            // TODO: Language
            InitializeComponent();
            browser.Load("serve://commander");
            accelerators = GetMenuItems(Menu.MenuItems.ToIEnumerable()).Select(n => (Accelerator?)new Accelerator(n)).ToArray();
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            SuspendLayout();

            CreateMenu(this);

            KeyPreview = true;
            browser.LoadHandler = this;
            browser.KeyboardHandler = this;
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

            var itemTheme = new MenuItem("&Theme");
            itemView.MenuItems.Add(itemTheme);
            itemView.MenuItems.Add("-");

            var itemThemeBlue = new MenuItem("&Blue");
            var itemThemeLightBlue = new MenuItem("&Light blue");
            var itemThemeDark = new MenuItem("&Dark");
            itemThemeBlue.Checked = true;

            void OnTheme(object src, EventArgs args)
            {
                if (src == itemThemeBlue)
                {
                    itemThemeBlue.Checked = true;
                    itemThemeLightBlue.Checked = false;
                    itemThemeDark.Checked = false;
                    browser.EvaluateScriptAsync("setTheme", "blue");
                    // TODO: save Property
                }
                else if (src == itemThemeLightBlue)
                {
                    itemThemeBlue.Checked = false;
                    itemThemeLightBlue.Checked = true;
                    itemThemeDark.Checked = false;
                    browser.EvaluateScriptAsync("setTheme", "lightblue");
                    // TODO: save Property
                }
                else if (src == itemThemeDark)
                {
                    itemThemeBlue.Checked = false;
                    itemThemeLightBlue.Checked = false;
                    itemThemeDark.Checked = true;
                    browser.EvaluateScriptAsync("setTheme", "dark");
                    // TODO: save Property
                }
            }

            itemThemeBlue.Click += OnTheme;
            itemThemeLightBlue.Click += OnTheme;
            itemThemeDark.Click += OnTheme;

            // TODO: read from Property
            itemThemeBlue.RadioCheck = true;
            itemThemeLightBlue.RadioCheck = true;
            itemThemeDark.RadioCheck = true;
            itemTheme.MenuItems.Add(itemThemeBlue);
            itemTheme.MenuItems.Add(itemThemeLightBlue);
            itemTheme.MenuItems.Add(itemThemeDark);

            var itemDevTools = new MenuItem("&Developer Tools", OnDevTools, Shortcut.F12);
            itemView.MenuItems.Add(itemDevTools);

            Menu = menu;
        }

        IEnumerable<MenuItem> GetMenuItems(IEnumerable<MenuItem> menuItems)
            => menuItems.SelectMany(n => GetMenuItems(n)).Where(n => n.Shortcut != Shortcut.None);

        IEnumerable<MenuItem> GetMenuItems(MenuItem menuItem)
        {
            var indirectShortcuts = menuItem.MenuItems.ToIEnumerable();
            return indirectShortcuts.Append(menuItem);
        }

        void OnDevTools(object src, EventArgs args) => browser.GetBrowser().ShowDevTools();

        readonly ChromiumWebBrowser browser = new ChromiumWebBrowser("");
        Accelerator?[] accelerators;
    }
}
