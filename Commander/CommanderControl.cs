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
        public CommanderControl(CommanderView leftView, CommanderView rightView)
        {
            this.leftView = leftView;
            this.rightView = rightView;
        }

        public void AdaptPath() => Other.ChangePath(focusedView.Path);

        CommanderView Other { get => focusedView == leftView ? rightView : leftView; }

        public void OnFocus(string id) => focusedView = id == "left" ? leftView : rightView;

        readonly CommanderView leftView;
        readonly CommanderView rightView;
        CommanderView focusedView;
    }
}
