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
            // TODO: Menu DevTools
            // TODO: Themes
            InitializeComponent();
            browser.Load("file:///C:/Users/urieg/Projects/CommanderUI/dist/Commander/index.html");
        }
    }
}
