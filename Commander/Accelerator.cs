using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Commander
{
    struct Accelerator
    {
        public Accelerator(MenuItem menuItem)
        {
            MenuItem = menuItem;
            switch (menuItem.Shortcut)
            {
                case Shortcut.CtrlH:
                    Key = 72;
                    Alt = false;
                    Ctrl = true;
                    Shift = false;
                    break;
                case Shortcut.CtrlR:
                    Key = 82;
                    Alt = false;
                    Ctrl = true;
                    Shift = false;
                    break;
                case Shortcut.F3:
                    Key = 114;
                    Alt = false;
                    Ctrl = false;
                    Shift = false;
                    break;
                case Shortcut.F7:
                    Key = 118;
                    Alt = false;
                    Ctrl = false;
                    Shift = false;
                    break;
                case Shortcut.F9:
                    Key = 120;
                    Alt = false;
                    Ctrl = false;
                    Shift = false;
                    break;
                case Shortcut.F12:
                    Key = 123;
                    Alt = false;
                    Ctrl = false;
                    Shift = false;
                    break;
                default:
                    Key = 0;
                    Alt = false;
                    Ctrl = false;
                    Shift = false;
                    break;
            }
        }

        public MenuItem MenuItem;
        public int Key;
        public bool Alt;
        public bool Ctrl;
        public bool Shift;
    }
}
