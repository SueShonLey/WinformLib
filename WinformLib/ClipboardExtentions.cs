using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class ClipboardExtentions
    {
        public static void ToClipboard(this string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                Clipboard.SetText(data);
            }
        }        
        
        public static string GetClipboard()
        {
            return Clipboard.GetText(); 
        }
    }
}
