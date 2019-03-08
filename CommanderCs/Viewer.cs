using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp.WinForms;
using Commander.Enums;
using Commander.Extension;

namespace Commander
{
    class Viewer
    {
        public Viewer(Form parent) => this.parent = parent;

        public void SetViewerRatio(double ratio)
            => parent.Invoke((Action)(() =>
            {
                viewer.Height = (int)(parent.ClientSize.Height * ratio);
                viewer.Location = new Point(0, parent.ClientSize.Height - viewer.Height - statusHeight);
            }));

        public void SetStatusRatio(double ratio) => parent.Invoke((Action)(() => statusHeight = (int)(parent.ClientSize.Height * ratio)));

        public void SetFile(string file)
        {
            if (viewer == null)
                return;

            parent.Invoke((Action)(() =>
            {
                if (file == null && viewer.Visible)
                    viewer.Visible = false;
                else if (file != null && !viewer.Visible)
                    viewer.Visible = true;

                if (file?.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase)?? false)
                {
                    var dateTime = DateTime.Now;
                    (parent as MainForm).Browser.LostFocus += FocusControl;

                    void FocusControl(object s, EventArgs e)
                    {
                        if (DateTime.Now - dateTime > TimeSpan.FromSeconds(3))
                            (parent as MainForm).Browser.LostFocus -= FocusControl;
                        parent.BeginInvoke((Action)(() => (parent as MainForm).Browser.Focus()));
                    }
                                       
                    viewer.Controls.Clear();
                    var browser = new ChromiumWebBrowser("");
                    viewer.Controls.Add(browser);
                    //browser.Load($"file:///{file}");
                    browser.Load($"https://www.caseris.de");
                }
            }));
        }

        public void Show(bool show)
        {
            if (show)
            {
                viewer = new Control
                {
                    Size = new Size(parent.ClientSize.Width, 0),
                    Location = new Point(0, parent.ClientSize.Height),
                    Parent = parent,
                    Anchor = (AnchorStyles) AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
                };
                parent.Controls.Add(viewer);
                parent.Controls.SetChildIndex(viewer, 0);
                viewer.Visible = false;
            }
            else
            {
                parent.Controls.Remove(viewer);
                viewer.Dispose();
            }
        }

        readonly Form parent;
        int statusHeight;
        Control viewer;
    }
}
