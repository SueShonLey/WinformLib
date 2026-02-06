
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Forms;

namespace WinformLib
{
    public class FormSettings
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string TitleText { get; set; } = string.Empty;

        /// <summary>
        /// 是否固定尺寸
        /// </summary>
        public bool isFixedSize { get; set; } = true;

        /// <summary>
        /// 是否居中
        /// </summary>
        public bool isCenter { get; set; } = true;

        /// <summary>
        /// 是否询问退出
        /// </summary>
        public bool isExitAsk { get; set; } = true;
    }

    public static class FormExtentions
    {
        #region 初始化相关
        /// <summary>
        /// 初始化默认设置（禁调大小、窗口居中、标题设定、询问退出）
        /// </summary>
        public static void SetCommon(this Form form, FormSettings settings = null)
        {
            if (settings == null)
            {
                settings = new FormSettings();
            }
            if (settings.isFixedSize)
            {
                form.MaximizeBox = false;
                form.FormBorderStyle = FormBorderStyle.FixedSingle;
            }
            if (settings.isCenter)
            {
                form.StartPosition = FormStartPosition.CenterScreen;
            }
            if (!string.IsNullOrEmpty(settings.TitleText))
            {
                form.Text = settings.TitleText;
            }
            if (settings.isExitAsk)
            {
                form.FormClosing += Form_FormClosing;
            }
        }

        // 事件处理代码
        private static void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 如果关闭操作不是由用户点击 X 按钮触发的（例如，程序代码关闭），则直接返回
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // 弹出确认框
                DialogResult result = MessageBox.Show("确定要退出吗?", "退出确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // 如果用户选择了 "No"（取消退出），则阻止窗体关闭
                if (result == DialogResult.No)
                {
                    e.Cancel = true;  // 取消关闭操作
                }
            }
        }

        #endregion

        #region 消息绑定相关
        public static Dictionary<string, Action<string>> msgDict = new Dictionary<string, Action<string>>();
        public class BindFormInput<TForm1, TForm2>
        {
            public TForm1 Form1 { get; set; }
            public Action<string> Funs1 { get; set; }

            public TForm2 Form2 { get; set; }
            public Action<string> Funs2 { get; set; }
        }

        /*
            示例：
            FormExtentions.BindForm(new FormExtentions.BindFormInput<Form1, Form2>
            {
                Form1 = this,
                Funs1 = this.SetText,//Form1的委托
                Form2 = form,
                Funs2 = form.SetText // Form2的委托
            });
         */
        /// <summary>
        /// 窗体传递双向绑定（主窗体1对象，主窗体1委托，从窗体2对象，从窗体2委托；委托是处理生产者发送消息的方法）
        /// </summary>
        public static void BindForm<TForm1, TForm2>(BindFormInput<TForm1, TForm2> input)
        {
            string key1 = $"{typeof(TForm1).Name}To{typeof(TForm2).Name}";
            string key2 = $"{typeof(TForm2).Name}To{typeof(TForm1).Name}";

            msgDict[key1] = input.Funs2;

            if (input.Funs1 != null)
            {
                msgDict[key2] = input.Funs1;
            }
        }

        /// <summary>
        /// 发送消息（示例：this.SendMessage<Form2>("hello world!")）
        /// </summary>
        public static void SendMessage<TReceiver>(this Form sender, string message)
        {
            string key = $"{sender.GetType().Name}To{typeof(TReceiver).Name}";
            if (msgDict.TryGetValue(key, out var handler))
            {
                if (sender.InvokeRequired)
                {
                    sender.Invoke(new Action(() => handler?.Invoke(message)));
                }
                else
                {
                    handler?.Invoke(message);
                }
            }
            else
            {
                throw new InvalidOperationException($"未找到消息处理器！Key: {key}");
            }
        }
        #endregion

        #region 控件判断空值相关
        public static Dictionary<string, ErrorProvider> errDict = new Dictionary<string, ErrorProvider>();

        /// <summary>
        /// 判断传入的控件是否为都不为空，是则返回True(可指定提示词)
        /// </summary>
        public static bool CheckNotNull(this Form form,string tips, params Control[] controls)
        {
            var key = typeof(Form).Name;
            errDict.TryAdd(key, new ErrorProvider()
            {
                BlinkStyle = ErrorBlinkStyle.NeverBlink
            });
            string errorMsg = string.Empty;
            int NullCount = 0;
            foreach (var control in controls)
            {
                errorMsg = string.IsNullOrEmpty(control.Text) ? tips : "";
                NullCount = string.IsNullOrEmpty(control.Text) ? NullCount + 1 : NullCount;
                if (errDict.TryGetValue(key, out var errorProvider))
                {
                    errorProvider.SetError(control, errorMsg);
                }
            }
            return NullCount==0;
        }

        /// <summary>
        /// 判断传入的控件是否为都不为空，是则返回True
        /// </summary>
        public static bool CheckNotNull(this Form form, params Control[] controls)
        {
            return CheckNotNull(form, "不可为空", controls);
        }
        #endregion

        #region 权限判断相关
        /// <summary>
        /// 是否以管理员身份运行该Winform程序
        /// </summary>
        /// <returns></returns>
        public static bool IsRunningByAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        #endregion

        #region MDI设计
        // 全局变量
        private static Panel panel1 = new Panel();
        private static Form mainForm = null;//主窗体
        /// <summary>
        /// 设置子窗体菜单及绑定窗体
        /// </summary>
        public static void SetMenuMDIForm(this Form parentForm, List<(string MenuName, Form TargetForm)> menuFormList)
        {
            #region 1. 遍历父窗体所有控件 → 全部移入panel1，排除panel1自身
            foreach (Control ctl in parentForm.Controls.OfType<Control>().ToArray())
            {
                if (ctl != panel1)
                {
                    ctl.Parent = panel1;  // 控件移入Panel，根治悬浮穿透
                }
            }
            #endregion

            #region 2. Panel基础配置
            panel1.Dock = DockStyle.Fill;
            panel1.SetCommonDefualt(parentForm);
            parentForm.Controls.Add(panel1);
            #endregion

            #region 3. 动态创建MenuStrip菜单 + 遍历集合自动添加菜单项
            MenuStrip menuStrip = new MenuStrip();
            menuStrip.Dock = DockStyle.Top;      // 菜单置顶，永不被遮挡
            parentForm.MainMenuStrip = menuStrip; // 绑定窗体主菜单，防止菜单失效
            // 遍历传入的集合，自动创建菜单项
            foreach (var item in menuFormList)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(item.MenuName);
                menuItem.Tag = item.TargetForm;
                menuItem.Click += Universal_MenuItem_Click;
                menuStrip.Items.Add(menuItem);
            }
            parentForm.Controls.Add(menuStrip);
            mainForm = parentForm;
            #endregion
        }

        /// <summary>
        /// 点击打开相应窗体
        /// </summary>
        private static void Universal_MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickMenu = sender as ToolStripMenuItem;
            Form targetForm = clickMenu.Tag as Form;
            if (targetForm == mainForm)// 点击了主窗体
            {
                panel1.SetCommonRecover(mainForm);
            }
            else
            {
                panel1.SetCommon(targetForm);
            }
        }
        #endregion

        #region 子窗体相关
        /// <summary>
        /// 尝试打开窗体（不重复打开，入参：是否继承父窗体UI）
        /// </summary>
        public static T ShowOnlyOne<T>(this Form currentForm,bool inheritUI=true) where T : Form, new()
        {
            // 1. 查找应用程序中已打开的指定类型窗体（排除已释放的实例）
            T targetForm = Application.OpenForms.OfType<T>().FirstOrDefault();

            // 2. 如果不存在，则创建新实例
            if (targetForm == null)
            {
                targetForm = new T();
                targetForm.StartPosition = FormStartPosition.CenterScreen;
                if (inheritUI)
                {
                    targetForm.Icon = currentForm.Icon;
                    targetForm.Font = currentForm.Font;
                    targetForm.MaximizeBox = currentForm.MaximizeBox;
                    targetForm.TopMost = currentForm.TopMost;
                    targetForm.FormBorderStyle = currentForm.FormBorderStyle;
                }
            }
            else
            {
                // 3. 如果存在，先判断是否被最小化，恢复窗口
                if (targetForm.WindowState == FormWindowState.Minimized)
                {
                    targetForm.WindowState = FormWindowState.Normal;
                }

                // 4. 将窗体置顶显示（核心：BringToFront + Activate 双重保障）
                targetForm.BringToFront();
                targetForm.Activate();
            }

            // 5. 显示窗体（新建实例需要Show，已存在的实例Show无副作用）
            targetForm.Show();

            return targetForm;
        }
        #endregion

        #region 出错相关
        /// <summary>
        /// 设置全局错误弹窗提示（仅开发环境生效）
        /// </summary>
        public static void SetGlobalErrorTips() 
        {
            if (System.Diagnostics.Debugger.IsAttached)//是开发环境
            {
                // 1. 捕获UI线程未处理异常（窗体、控件相关报错）
                Application.ThreadException += (sender,e) =>
                {
                    MessageBox.Show(
                        $"程序运行出错：{e.Exception.Message}\r\n详细信息：{e.Exception.StackTrace}",
                        "错误提示",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
  
                };
                // 2. 捕获非UI线程未处理异常（后台线程、异步任务报错）
                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    Exception? ex = e.ExceptionObject as Exception;
                    if (ex != null)
                    {
                        MessageBox.Show(
                            $"后台线程出错：{ex.Message}\r\n详细信息：{ex.StackTrace}\r\n{(e.IsTerminating ? "程序即将退出" : "")}",
                            "后台错误",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                };
            }
        }
        #endregion
    }

    public static class HideFormExtensions
    {
        private static NotifyIcon notifyIcon = new NotifyIcon();
        private static Form MainForm;

        static HideFormExtensions()
        {
            // 添加双击事件以显示窗体
            notifyIcon.Click += NotifyIcon_Click;
        }

        private static void NotifyIcon_Click(object sender, EventArgs e)
        {
            // 这里可以通过某种方式获取当前活动的窗体
            Form activeForm = MainForm;
            activeForm.Show(); // 显示窗体
            activeForm.WindowState = FormWindowState.Normal; // 确保窗体不是最小化状态
            notifyIcon.Visible = false; // 隐藏托盘图标

        }

        /// <summary>
        /// 任务栏隐藏该窗体，并且显示右下角托盘图标（点击图标恢复）
        /// </summary>
        public static void HideForm(this Form form)
        {
            MainForm = form;
            form.Hide(); // 隐藏窗体
            notifyIcon.Visible = true; // 显示托盘图标
            notifyIcon.Icon = form.Icon;
        }
    }
}
