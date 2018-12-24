using CefSharp;
using CefSharp.WinForms;
using Commander.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Commander.Extension;
using System.Threading;

namespace Commander
{
    public partial class MainForm : Form, ILoadHandler, IKeyboardHandler
    {
        #region ILoadHandler

        public void OnFrameLoadStart(IWebBrowser browserControl, FrameLoadStartEventArgs frameLoadStartArgs) {}

        public void OnFrameLoadEnd(IWebBrowser browserControl, FrameLoadEndEventArgs frameLoadEndArgs)
        {
            if (frameLoadEndArgs.Frame.IsMain && frameLoadEndArgs.Frame.Url == commanderUrl)
            {
                browser.EvaluateScriptAsync(JavaScriptFunctions.SetTheme, Settings.Default.Theme);
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
            InitializeComponent();
            browser.Load(commanderUrl);
            browser.RegisterAsyncJsObject("CommanderLeft", new Commander("left"), new BindingOptions { CamelCaseJavascriptNames = true });
            accelerators = GetMenuItems(Menu.MenuItems.ToIEnumerable()).Select(n => (Accelerator?)new Accelerator(n)).ToArray();
        }

        class Commander
        {
            public Commander(string name)
            {

            }

            public string Test(string text)
            {
                Thread.Sleep(10000);
                return "Test" + text;
            }
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
            browser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
            | AnchorStyles.Left
            | AnchorStyles.Right;
            browser.Location = new System.Drawing.Point(0, 0);
            browser.Size = new System.Drawing.Size(800, 450);
            browser.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
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

            var itemFile = new MenuItem(Resources.MenuFile);
            menu.MenuItems.Add(itemFile);

            var itemView = new MenuItem(Resources.MenuView);
            menu.MenuItems.Add(itemView);

            var itemTheme = new MenuItem(Resources.MenuThemes);
            itemView.MenuItems.Add(itemTheme);
            itemView.MenuItems.Add("-");

            var itemThemeBlue = new MenuItem(Resources.MenuThemeBlue);
            var itemThemeLightBlue = new MenuItem(Resources.MenuThemeLightBlue);
            var itemThemeDark = new MenuItem(Resources.MenuThemeDark);
            switch (Settings.Default.Theme)
            {
                case Themes.LightBlue:
                    itemThemeLightBlue.Checked = true;
                    break;
                case Themes.Dark:
                    itemThemeDark.Checked = true;
                    break;
                default:
                    itemThemeBlue.Checked = true;
                    break;
            }
                       
            void OnTheme(object src, EventArgs args)
            {
                if (src == itemThemeBlue)
                {
                    itemThemeBlue.Checked = true;
                    itemThemeLightBlue.Checked = false;
                    itemThemeDark.Checked = false;
                    browser.EvaluateScriptAsync(JavaScriptFunctions.SetTheme, Themes.Blue);
                    Settings.Default.Theme = Themes.Blue;
                }
                else if (src == itemThemeLightBlue)
                {
                    itemThemeBlue.Checked = false;
                    itemThemeLightBlue.Checked = true;
                    itemThemeDark.Checked = false;
                    browser.EvaluateScriptAsync(JavaScriptFunctions.SetTheme, Themes.LightBlue);
                    Settings.Default.Theme = Themes.LightBlue;
                }
                else if (src == itemThemeDark)
                {
                    itemThemeBlue.Checked = false;
                    itemThemeLightBlue.Checked = false;
                    itemThemeDark.Checked = true;
                    browser.EvaluateScriptAsync(JavaScriptFunctions.SetTheme, Themes.Dark);
                    Settings.Default.Theme = Themes.Dark;
                }
                Settings.Default.Save();
            }

            itemThemeBlue.Click += OnTheme;
            itemThemeLightBlue.Click += OnTheme;
            itemThemeDark.Click += OnTheme;

            itemThemeBlue.RadioCheck = true;
            itemThemeLightBlue.RadioCheck = true;
            itemThemeDark.RadioCheck = true;
            itemTheme.MenuItems.Add(itemThemeBlue);
            itemTheme.MenuItems.Add(itemThemeLightBlue);
            itemTheme.MenuItems.Add(itemThemeDark);

            var itemDevTools = new MenuItem(Resources.MenuDeveloperTools, OnDevTools, Shortcut.F12);
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

        const string commanderUrl = "serve://commander/";
        readonly ChromiumWebBrowser browser = new ChromiumWebBrowser("");
        Accelerator?[] accelerators;
    }
}
