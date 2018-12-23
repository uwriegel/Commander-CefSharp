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
            InitializeComponent();

            browser.Load("https://www.google.de");
        }
    }
}
