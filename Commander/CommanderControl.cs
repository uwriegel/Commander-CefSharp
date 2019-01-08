using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander
{
    class CommanderControl
    {
        public CommanderControl()
        {

        }

        public void OnFocus(string id)
        {
            Debugger.Log(1, "Affe", $"Fokus: {id}\n");
        }
    }
}
