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
            panel1.SetDoubleBuffered(true);
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
            panel1.SetDoubleBuffered(true);
            panel1.Controls.Clear();//清空旧控件
            form2.TopLevel = false;//嵌入模式
            form2.Parent = panel1;//转移控件
            form2.Dock = DockStyle.Fill;//转移控件
            form2.FormBorderStyle = FormBorderStyle.None;//不显示标题栏
            form2.Show();
        }

        /// <summary>
        /// 允许面板接收文件拖放(多个)
        /// </summary>
        /// <param name="panel">需要开启拖放功能的面板</param>
        public static void ReceiveMutiFiles(this Panel panel, Action<List<string>> funs)
        {
            // 1. 初始化面板基础属性（程序启动时立即生效）
            panel.AllowDrop = true;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.BackColor = Color.DarkGray; // 初始背景色，打开就显示Gray

            // 2. 拖入事件：鼠标进入面板且有文件时
            panel.DragEnter += (sender, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    e.Effect = DragDropEffects.All;
                    panel.BackColor = Color.LightGray; // 拖入时高亮（可选，区分初始状态）
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            };

            // 3. 拖离事件：鼠标拖入后未放下就离开面板，恢复初始色
            panel.DragLeave += (sender, e) =>
            {
                panel.BackColor = Color.DarkSlateGray;
            };

            // 4. 拖放完成事件：放下文件后
            panel.DragDrop += (sender, e) =>
            {
                // 放下文件后设置为DarkGray
                panel.BackColor = Color.DarkGray;

                string[] filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
                List<string> result = new List<string>();
                if (filePaths != null && filePaths.Length > 0)
                {
                    foreach (string filePath in filePaths)
                    {
                        if (File.Exists(filePath))
                        {
                            result.Add(filePath);
                        }
                    }
                    funs(result);
                }
            };
        }        
        
        /// <summary>
        /// 允许面板接收文件拖放(单个)
        /// </summary>
        /// <param name="panel">需要开启拖放功能的面板</param>
        public static void ReceiveFiles(this Panel panel, Action<string> funs)
        {
            panel.ReceiveMutiFiles((list) =>
            {
                funs(list.FirstOrDefault() ?? string.Empty);
            });
        }
    }
}
