using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class PanelExtentions
    {

        public static ConcurrentDictionary<string, List<Control>> dict = new ConcurrentDictionary<string, List<Control>>();

        /// <summary>
        /// 设置默认窗体（以便后续恢复）
        /// </summary>
        public static void SetCommonDefualt(this Panel panel1,Form form2)
        {
            List<Control> defaultControlList = new List<Control>();
            foreach (var item in panel1.Controls)
            {
                defaultControlList.Add((Control)item);//记录下默认值，后面恢复时加上
            }
            dict.TryAdd(form2.GetType().Name, defaultControlList);
        }        
        
        /// <summary>
        /// 恢复默认窗体（要设置默认窗体才可以恢复）
        /// </summary>
        public static void SetCommonRecover(this Panel panel1,Form form2)
        {
            var res = dict.GetValueOrDefault(form2.GetType().Name);
            if (res != null)
            {
                // 显示默认控件
                panel1.Controls.Clear();
                panel1.Controls.AddRange(res.ToArray());
            }
        }

        /// <summary>
        /// 切换窗体(Panel控件、切换到的窗体)
        /// </summary>
        public static void SetCommon<T>(this Panel panel1) where T : Form,new()
        {
            SetPanelDoubleBuffered(panel1, true);
            T form2 = new T();
            panel1.Controls.Clear();//清空旧控件
            form2.TopLevel = false;//嵌入模式
            form2.Parent = panel1;//转移控件
            form2.Dock = DockStyle.Fill;//转移控件
            form2.FormBorderStyle = FormBorderStyle.None;//不显示标题栏
            form2.Show();
        }        
        
        /// <summary>
        /// 切换窗体(Panel控件、切换到的窗体)
        /// </summary>
        public static void SetCommon<T>(this Panel panel1,T form2) where T : Form,new()
        {
            SetPanelDoubleBuffered(panel1, true);
            panel1.Controls.Clear();//清空旧控件
            form2.TopLevel = false;//嵌入模式
            form2.Parent = panel1;//转移控件
            form2.Dock = DockStyle.Fill;//转移控件
            form2.FormBorderStyle = FormBorderStyle.None;//不显示标题栏
            form2.Show();
        }

        #region 辅助方法
        /// <summary>
        /// 辅助方法：给Panel开启双缓冲（
        /// </summary>
        private static void SetPanelDoubleBuffered(Panel panel, bool enable)
        {
            try
            {
                Type panelType = panel.GetType();
                // 获取Panel的非公共DoubleBuffered属性（WinForm原生隐藏属性）
                System.Reflection.PropertyInfo pi = panelType.GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
                );
                pi.SetValue(panel, enable, null);
            }
            catch (Exception)
            {

            }

        }
        #endregion
    }
}
