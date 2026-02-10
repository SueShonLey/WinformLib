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

        /// <summary>
        /// 给任意WinForm控件开启双缓冲（反射获取原生隐藏属性，无兼容性问题）
        /// </summary>
        /// <typeparam name="T">控件类型（TableLayoutPanel/Panel/DataGridView等）</typeparam>
        /// <param name="control">目标控件</param>
        /// <param name="enable">是否开启双缓冲</param>
        public static void SetDoubleBuffered<T>(this T control, bool enable) where T : Control
        {
            if (control == null) return;
            var prop = control.GetType().GetProperty(
                "DoubleBuffered",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
            );
            prop?.SetValue(control, enable, null);
        }

        /// <summary>
        /// 获取指定类型的子控件(适用对象：Panel类、GroupBox类等)
        /// </summary>
        public static List<T> GetChildrenControls<T>(this Control parent) where T : Control
        {
            return parent.Controls.OfType<T>().ToList();
        }
    }
}
