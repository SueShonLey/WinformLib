
using System;
using System.Reflection;
using System.Windows.Forms;

namespace WinfromLib
{
    public class FormSettings
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string TitleText { get; set; } = "Form";

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

            if (!msgDict.ContainsKey(key1))
            {
                msgDict.Add(key1, input.Funs2);
            }

            if (!msgDict.ContainsKey(key2))
            {
                msgDict.Add(key2, input.Funs1);
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
