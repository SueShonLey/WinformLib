using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class ObjExtentions
    {
        /// <summary>
        /// 对象转换为控件(一般用于object sender)
        /// </summary>
        public static T? ToControl<T>(this object obj) where T : Control,new()
        {
            return (obj as T);
        }

    }
}
