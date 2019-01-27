using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Commander.Extension
{
    static class ControlExtension
    {
        public static async Task<T> DeferredExecution<T>(this Control dispatcher, Func<T> action, int delayInMilliseconds)
        {
            await Task.Delay(delayInMilliseconds);
            return await Task<T>.Factory.FromAsync(dispatcher.BeginInvoke(action), ar => (T)dispatcher.EndInvoke(ar));
        }
    }
}
