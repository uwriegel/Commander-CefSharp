using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Commander
{
    class GripPanel : Panel
    {
        public Control ChildTop { get; set; }
        public Control ChildBottom { get; set; }

        public GripPanel(int initialPosition)
        {
            Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            Cursor = System.Windows.Forms.Cursors.SizeNS;
            Location = new System.Drawing.Point(0, initialPosition);

            ChildTop = new Panel
            {
                Location = new Point(0, 0),
                Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)))
            };

            ChildBottom = new Panel
            {
                Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)))
            };

            ParentChanged += (object sender, EventArgs e) =>
            {
                if (Parent != null)
                {
                    Size = new System.Drawing.Size(Parent.ClientSize.Width, gripHeight);
                    ChildTop.Parent = Parent;
                    ChildTop.Size = new Size(Parent.ClientSize.Width, initialPosition);
                    ChildBottom.Parent = Parent;
                    ChildBottom.Location = new Point(0, initialPosition + Height);
                    ChildBottom.Size = new Size(Parent.ClientSize.Width, Parent.ClientSize.Height - initialPosition - Height);
                }
            };

            MouseDown += (object sender, MouseEventArgs e) =>
            {
                var startpos = PointToScreen(e.Location).Y;
                var startpoint = Location.Y;

                void mouseMove(object o, MouseEventArgs mea)
                {
                    var y = startpoint + (PointToScreen(mea.Location).Y - startpos);
                    if (y < 5)
                        y = 5;
                    if (y > Parent.ClientSize.Height - Height - 5)
                        y = Parent.ClientSize.Height - Height - 5;

                    Location = new Point(0, y);
                    ChildBottom.Location = new Point(0, Location.Y + Height);
                    ChildBottom.Size = new Size(Parent.ClientSize.Width, Parent.ClientSize.Height - Location.Y - Height);
                    ChildTop.Size = new Size(Parent.ClientSize.Width, Location.Y);
                }

                void mouseUp(object o, MouseEventArgs mea)
                {
                    MouseMove -= mouseMove;
                    MouseUp -= mouseUp;
                }

                MouseMove += mouseMove;
                MouseUp += mouseUp;
            };
        }

        public void AddControls(Control top, Control bottom)
        {
            top.Size = ChildTop.ClientSize;
            ChildTop.Controls.Add(top);
            bottom.Size = ChildBottom.ClientSize;
            ChildBottom.Controls.Add(bottom);
            bottom.Anchor = (System.Windows.Forms.AnchorStyles)
                System.Windows.Forms.AnchorStyles.Top 
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right 
                | System.Windows.Forms.AnchorStyles.Bottom;
        }

        const int gripHeight = 5;
    }
}
