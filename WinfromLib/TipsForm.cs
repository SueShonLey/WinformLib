using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinfromLib
{
    public static class TipsForm
    {
        #region 静态方法
        /// <summary>
        /// 提示弹窗
        /// </summary>
        /// <param name="contnet">内容图标</param>
        /// <param name="title">左上角提示</param>
        /// <param name="icon">None无图标  Information蓝色告示（默认） Question蓝色提问  Error红色错误  Warning黄色警告</param>
        /// <returns>无实际返回值</returns>
        public static DialogResult PopUpTips(string contnet, string title = "提示", MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            return MessageBox.Show(contnet, title, MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// 提示弹窗(右下角)
        /// </summary>
        /// <param name="contnet">内容图标</param>
        /// <param name="title">左上角提示</param>
        /// <returns>无实际返回值</returns>
        public static void PopUpTipsRight(string contnet, string title = "提示", ToolTipIcon icon = ToolTipIcon.Info)
        {
            var notifyIcon1 = new NotifyIcon();
            notifyIcon1.Visible = true;
            notifyIcon1.Icon = SystemIcons.Exclamation;
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = contnet;
            notifyIcon1.BalloonTipIcon = icon;

            notifyIcon1.ShowBalloonTip(30000);
        }

        /// <summary>
        /// 询问弹窗
        /// </summary>
        /// <param name="contnet">内容图标</param>
        /// <param name="title">左上角提示</param>
        /// <param name="btn">YesNo 是否按钮 OKCancel确认取消按钮</param>
        /// <param name="icon">None无图标  Information蓝色告示（默认） Question蓝色提问  Error红色错误  Warning黄色警告</param>
        /// <returns>用户选择是或者确认则返回True</returns>
        public static bool PopUpDialog(string contnet, string title = "请选择", MessageBoxButtons btn = MessageBoxButtons.YesNo, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            DialogResult result = MessageBox.Show(contnet, title, btn, icon);
            return result == DialogResult.OK || result == DialogResult.Yes;
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 提示弹窗
        /// </summary>
        /// <param name="contnet">内容图标</param>
        /// <param name="title">左上角提示</param>
        /// <param name="icon">None无图标  Information蓝色告示（默认） Question蓝色提问  Error红色错误  Warning黄色警告</param>
        /// <returns>无实际返回值</returns>
        public static DialogResult PopUpTips(this Form form, string contnet, string title = "提示", MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            return PopUpTips(contnet, title, icon);
        }

        /// <summary>
        /// 提示弹窗(右下角)
        /// </summary>
        /// <param name="contnet">内容图标</param>
        /// <param name="title">左上角提示</param>
        /// <returns>无实际返回值</returns>
        public static void PopUpTipsRight(this Form form, string contnet, string title = "提示", ToolTipIcon icon = ToolTipIcon.Info)
        {
            PopUpTipsRight(contnet, title, icon);
        }

        /// <summary>
        /// 询问弹窗
        /// </summary>
        /// <param name="contnet">内容图标</param>
        /// <param name="title">左上角提示</param>
        /// <param name="btn">YesNo 是否按钮 OKCancel确认取消按钮</param>
        /// <param name="icon">None无图标  Information蓝色告示（默认） Question蓝色提问  Error红色错误  Warning黄色警告</param>
        /// <returns>用户选择是或者确认则返回True</returns>
        public static bool PopUpDialog(this Form form, string contnet, string title = "请选择", MessageBoxButtons btn = MessageBoxButtons.YesNo, MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            return PopUpDialog(contnet,title,btn,icon);
        }
        #endregion
    }
}
