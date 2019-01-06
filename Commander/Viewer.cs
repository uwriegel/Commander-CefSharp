using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Commander.Enums;
using Commander.Extension;

namespace Commander
{
    class Viewer
    {
        public Viewer(Form parent) => this.parent = parent;

        public void SetViewerRatio(double ratio)
        {
            Debugger.Log(1, "Main", $"Viewer Ratio: {ratio}\r\n");
            parent.Invoke((Action)(() =>
            {
                viewer.Height = (int)(parent.ClientSize.Height * ratio);
                viewer.Location = new Point(0, parent.ClientSize.Height - viewer.Height - statusHeight);
            }));
        }

        public void SetStatusRatio(double ratio) => parent.Invoke((Action)(() => statusHeight = (int)(parent.ClientSize.Height * ratio)));

        public void Show(bool show)
        {
            if (show)
            {
                viewer = new Control
                {
                    BackColor = Color.BlueViolet,
                    Size = new Size(parent.ClientSize.Width, 0),
                    Location = new Point(0, parent.ClientSize.Height),
                    Parent = parent,
                    Anchor = (System.Windows.Forms.AnchorStyles)
                                System.Windows.Forms.AnchorStyles.Left
                                | System.Windows.Forms.AnchorStyles.Right
                                | System.Windows.Forms.AnchorStyles.Bottom
                };
                parent.Controls.Add(viewer);
                parent.Controls.SetChildIndex(viewer, 0);
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
