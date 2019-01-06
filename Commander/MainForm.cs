using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using CefSharp;
using CefSharp.WinForms;

using Commander.Extension;
using Commander.Properties;

namespace Commander
{
    public class MainForm : Form, ILoadHandler, IKeyboardHandler
    {
        #region ILoadHandler

        public void OnFrameLoadStart(IWebBrowser browserControl, FrameLoadStartEventArgs frameLoadStartArgs) { }

        public void OnFrameLoadEnd(IWebBrowser browserControl, FrameLoadEndEventArgs frameLoadEndArgs)
        {
            if (frameLoadEndArgs.Frame.IsMain && frameLoadEndArgs.Frame.Url == commanderUrl)
            {
                browser.EvaluateScriptAsync($"themes.theme = '{Settings.Default.Theme}'");
                BeginInvoke((Action)(() => browser.Focus()));
            }

        }

        public void OnLoadError(IWebBrowser browserControl, LoadErrorEventArgs loadErrorArgs) { }

        public void OnLoadingStateChange(IWebBrowser browserControl, LoadingStateChangedEventArgs loadingStateChangedArgs) { }

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
                    Invoke((Action)(() => accelerator.Value.MenuItem.PerformClick()));
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

        #region Constructor

        public MainForm()
        {
            InitializeComponent();
            viewer = new Viewer(this);
            browser.Load(commanderUrl);
            viewLeft = new CommanderView(CommanderView.ID.Left, Handle, browser, new LeftHost());
            viewRight = new CommanderView(CommanderView.ID.Right, Handle, browser, new RightHost());
            browser.RegisterJsObject("CommanderLeft", viewLeft, new BindingOptions { CamelCaseJavascriptNames = true });
            browser.RegisterJsObject("CommanderRight", viewRight, new BindingOptions { CamelCaseJavascriptNames = true });
            browser.RegisterJsObject("Program", viewer, new BindingOptions { CamelCaseJavascriptNames = true });
            accelerators = GetMenuItems(Menu.MenuItems.ToIEnumerable()).Select(n => (Accelerator?)new Accelerator(n)).ToArray();
        }

        #endregion

        #region Menu

        void CreateMenu(Control parent)
        {
            var menu = new MainMenu();

            var itemFile = new MenuItem(Resources.MenuFile);
            menu.MenuItems.Add(itemFile);
            var itemRename = new MenuItem(Resources.MenuRename, (s, e) => { }, Shortcut.F2);
            itemFile.MenuItems.Add(itemRename);
            itemFile.MenuItems.Add("-");
            var itemCopy = new MenuItem(Resources.MenuCopy, (s, e) => { }, Shortcut.F5);
            itemFile.MenuItems.Add(itemCopy);
            var itemMove = new MenuItem(Resources.MenuMove, (s, e) => { }, Shortcut.F6);
            itemFile.MenuItems.Add(itemMove);
            var itemDelete = new MenuItem(Resources.MenuDelete, (s, e) => { }, Shortcut.Del);
            itemFile.MenuItems.Add(itemDelete);
            itemFile.MenuItems.Add("-");
            var itemCreateFolder = new MenuItem(Resources.MenuCreateFolder, (s, e) => { }, Shortcut.F7);
            itemFile.MenuItems.Add(itemCreateFolder);
            var itemProperties = new MenuItem(Resources.MenuProperties, (s, e) => { });
            itemFile.MenuItems.Add(itemProperties);
            itemFile.MenuItems.Add("-");
            var itemExit = new MenuItem(Resources.MenuExit, (s, e) => Close(), Shortcut.AltF4);
            itemFile.MenuItems.Add(itemExit);

            var itemNavigation = new MenuItem(Resources.MenuNavigation);
            menu.MenuItems.Add(itemNavigation);
            var itemFavourites = new MenuItem(Resources.MenuFavourites, (s, e) => { }, Shortcut.F1);
            itemNavigation.MenuItems.Add(itemFavourites);
            var itemSameFolder = new MenuItem(Resources.MenuSameFolder, (s, e) => OpenSame(), Shortcut.F9);
            itemNavigation.MenuItems.Add(itemSameFolder);

            var itemSelection = new MenuItem(Resources.MenuSelection);
            menu.MenuItems.Add(itemSelection);
            var itemAll = new MenuItem(Resources.MenuAll, (s, e) => { });
            itemSelection.MenuItems.Add(itemAll);
            var itemUnselectAll = new MenuItem(Resources.MenuUnselectAll, (s, e) => { });
            itemSelection.MenuItems.Add(itemUnselectAll);

            var itemView = new MenuItem(Resources.MenuView);
            menu.MenuItems.Add(itemView);

            var itemShowHidden = new MenuItem(Resources.MenuShowHidden, OnShowHidden, Shortcut.CtrlH)
            {
                Checked = false
            };
            itemView.MenuItems.Add(itemShowHidden);
            var itemRefresh = new MenuItem(Resources.MenuRefresh, (s, e) => OnRefresh(), Shortcut.CtrlR);
            itemView.MenuItems.Add(itemRefresh);
            itemView.MenuItems.Add("-");

            var itemPreview = new MenuItem(Resources.MenuPreview, OnViewer, Shortcut.F3)
            {
                Checked = false
            };
            itemView.MenuItems.Add(itemPreview);

            itemView.MenuItems.Add("-");

            var itemTheme = new MenuItem(Resources.MenuThemes);
            itemView.MenuItems.Add(itemTheme);
            itemView.MenuItems.Add("-");

            var itemThemeBlue = new MenuItem(Resources.MenuThemeBlue)
            {
                RadioCheck = true
            };
            var itemThemeLightBlue = new MenuItem(Resources.MenuThemeLightBlue)
            {
                RadioCheck = true
            };
            var itemThemeDark = new MenuItem(Resources.MenuThemeDark)
            {
                RadioCheck = true
            };
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

            async void OnTheme(object src, EventArgs args)
            {
                if (src == itemThemeBlue)
                {
                    itemThemeBlue.Checked = true;
                    itemThemeLightBlue.Checked = false;
                    itemThemeDark.Checked = false;
                    await browser.EvaluateScriptAsync($"themes.theme = '{Themes.Blue}'");
                    Settings.Default.Theme = Themes.Blue;
                }
                else if (src == itemThemeLightBlue)
                {
                    itemThemeBlue.Checked = false;
                    itemThemeLightBlue.Checked = true;
                    itemThemeDark.Checked = false;
                    await browser.EvaluateScriptAsync($"themes.theme = '{Themes.LightBlue}'");
                    Settings.Default.Theme = Themes.LightBlue;
                }
                else if (src == itemThemeDark)
                {
                    itemThemeBlue.Checked = false;
                    itemThemeLightBlue.Checked = false;
                    itemThemeDark.Checked = true;
                    await browser.EvaluateScriptAsync($"themes.theme = '{Themes.Dark}'");
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

            var itemFullscreen = new MenuItem(Resources.MenuFullscreen, (s, e) => { }, Shortcut.F11);
            itemView.MenuItems.Add(itemFullscreen);

            itemView.MenuItems.Add("-");

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

        void OnViewer(object src, EventArgs args)
        {
            (src as MenuItem).Checked = !(src as MenuItem).Checked;
            viewer.Show((src as MenuItem).Checked);
            browser.ExecuteScriptAsync($"commander.setViewer", (src as MenuItem).Checked);
        }

        void OnRefresh() { }

        void OpenSame() { }

        void OnShowHidden(object src, EventArgs args)
        {
            (src as MenuItem).Checked = !(src as MenuItem).Checked;
            viewLeft.ShowHidden = (src as MenuItem).Checked;
            viewRight.ShowHidden = (src as MenuItem).Checked;
        }
               
        #endregion

        #region Methods
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            SuspendLayout();

            CreateMenu(this);

            if (Settings.Default.WindowLocation.X != -1 && Settings.Default.WindowLocation.Y != -1)
            {
                StartPosition = FormStartPosition.Manual;
                Location = Settings.Default.WindowLocation;
            }
            Size = Settings.Default.WindowSize;
            if (Settings.Default.WindowState != FormWindowState.Minimized)
                WindowState = Settings.Default.WindowState;

            KeyPreview = true;
            browser.LoadHandler = this;
            browser.KeyboardHandler = this;
            browser.Anchor = AnchorStyles.Top | AnchorStyles.Bottom
            | AnchorStyles.Left
            | AnchorStyles.Right;
            browser.Location = new System.Drawing.Point(0, 0);
            browser.Size = ClientSize;
            browser.TabIndex = 0;

            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Icon = Resources.Kirk;

            Controls.Add(browser);
            Name = "MainForm";
            Text = "Commander";

            ResumeLayout(false);

            FormClosing += (object sender, FormClosingEventArgs e) =>
            {
                if (WindowState == FormWindowState.Normal)
                {
                    Settings.Default.WindowLocation = Location;
                    Settings.Default.WindowSize = Size;
                }
                else
                {
                    Settings.Default.WindowLocation = RestoreBounds.Location;
                    Settings.Default.WindowSize = RestoreBounds.Size;
                }

                Settings.Default.Save();
                Settings.Default.WindowState = WindowState;
            };

            browser.Focus();
        }

        #endregion

        #region Types

        class LeftHost : IHost
        {
            public string RecentPath { get => Settings.Default.LeftRecentPath; set => Settings.Default.LeftRecentPath = value; }
            public string Class { get => "commanderViewLeft"; }
        }

        class RightHost : IHost
        {
            public string RecentPath { get => Settings.Default.RightRecentPath; set => Settings.Default.RightRecentPath = value; }
            public string Class { get => "commanderViewRight"; }
        }

        #endregion

        #region Fields

        const string commanderUrl = "serve://commander/";
        readonly ChromiumWebBrowser browser = new ChromiumWebBrowser("");
        readonly CommanderView viewLeft;
        readonly CommanderView viewRight;
        readonly Viewer viewer;
        Accelerator?[] accelerators;

        #endregion
    }
}
