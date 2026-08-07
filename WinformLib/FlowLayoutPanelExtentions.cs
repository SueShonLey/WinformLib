using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class FlowLayoutPanelExtentions
    {
        #region btnList
        // 按钮列表输入参数实体
        public class FlowLayOutListInput
        {
            /// <summary>
            /// 按钮名称列表
            /// </summary>
            public List<string> NameList { get; set; } = new List<string>();

            /// <summary>
            /// 按钮边框样式
            /// </summary>
            public BorderStyle BorderStyle { get; set; } = BorderStyle.None;

            /// <summary>
            /// 按钮间距
            /// </summary>
            public int VerticalSpacing { get;  set; } = 10;
        }

        /// <summary>
        /// FlowLayoutPanel扩展方法：批量创建垂直单列按钮，支持点击事件传出索引+名称元组
        /// </summary>
        /// <param name="flowPanel">当前FlowLayoutPanel控件</param>
        /// <param name="input">按钮创建的输入参数</param>
        /// <param name="onBtnClick">按钮点击委托：传出(索引, 按钮名称)元组</param>
        public static void AddButtons(this FlowLayoutPanel flowPanel, FlowLayOutListInput input, Action<int,Button> onBtnClick)
        {
            // 空值校验
            if (flowPanel == null)
                throw new ArgumentNullException(nameof(flowPanel), "FlowLayoutPanel控件不能为空");
            if (input == null)
                throw new ArgumentNullException(nameof(input), "输入参数实体不能为空");
            if (input.NameList == null || input.NameList.Count == 0)
                throw new ArgumentException("按钮名称列表不能为空", nameof(input.NameList));

            // 清空原有控件
            flowPanel.Controls.Clear();

            // 核心布局配置
            flowPanel.FlowDirection = FlowDirection.TopDown;
            flowPanel.WrapContents = false;
            flowPanel.AutoScroll = false;
            flowPanel.Padding = new Padding(0);
            flowPanel.Margin = new Padding(0);
            flowPanel.AutoSize = false;

            // 循环创建按钮
            var allCount = input.NameList.Count;
            for (int i = 0; i < input.NameList.Count; i++)
            {
                // 配置按钮上下间距（仅顶部加间距，左右无）
                Padding btnMargin = new Padding(0);
                if (i > 0)
                {
                    btnMargin.Top = input.VerticalSpacing;
                }

                string btnName = input.NameList[i];
                // 存储当前索引和名称（闭包捕获，避免循环变量问题）
                int currentIndex = i;

                Button btn = new Button
                {
                    Name = $"btn_{i}_{btnName}",
                    Text = btnName,
                    Margin = btnMargin,
                    Width = flowPanel.ClientSize.Width, // 改用ClientSize避免边框导致的空隙                                
                    Height = (flowPanel.ClientSize.Height - (allCount - 1) * input.VerticalSpacing) / allCount,
                };

                // 核心：绑定点击事件，触发委托并传递元组
                btn.Click += (sender, e) =>
                {
                    onBtnClick?.Invoke(currentIndex,btn);
                };

                flowPanel.Controls.Add(btn);
            }
        }
        #endregion

        #region chatPanel

        private static ConcurrentDictionary<FlowLayoutPanel, int> dict = new ConcurrentDictionary<FlowLayoutPanel, int>();

        /// <summary>
        /// 添加消息，实现聊天窗体效果
        /// </summary>
        /// <param name="panelChat"></param>
        /// <param name="input"></param>
        public static void SetCommonFlowMsg(this FlowLayoutPanel panelChat,ChatMessage input)
        {
            if (dict!= null && !dict.ContainsKey(panelChat))//不存在则绑定
            {
                panelChat.Resize += ScrollBarResize;
                dict.TryAdd(panelChat, 0);
            }
            panelChat.FlowDirection = FlowDirection.TopDown;
            panelChat.WrapContents = false;
            panelChat.AutoScroll = true;
            panelChat.AutoSize = false;

            Panel msgCard = new Panel();
            // 左右预留2像素，避免贴边布局异常
            msgCard.Width = panelChat.ClientSize.Width - 2;
            // 只保留底部外边距，上下内部间距统一在内部offset处理
            msgCard.Margin = new Padding(0, 0, 0, input.Margin);
            msgCard.BackColor = Color.Transparent;

            // 初始顶部偏移从0开始，不再提前叠加Margin
            int offsetY = input.Margin;
            TaskExtentions.UISafeInvoke(panelChat, () =>
            {
                if (input.ChatType == ChatMessageEnum.Text)
                {
                    Label lblTitle = new Label();
                    lblTitle.Text = input.Msg;
                    lblTitle.Location = new Point(0, offsetY);
                    lblTitle.AutoSize = true;
                    lblTitle.MaximumSize = new Size(msgCard.Width, 0);
                    if (input.funsLabel != null)
                    {
                        input.funsLabel(lblTitle);
                    }
                    msgCard.Controls.Add(lblTitle);
                    offsetY += GetLabelHeight(lblTitle, msgCard.Width) + input.Margin;
                }
                else
                {

                        Image pic = null;
                        using (var wc = new WebClient())
                        {
                            byte[] data = wc.DownloadData(input.Msg);
                            using (var ms = new MemoryStream(data))
                            {
                                pic = Image.FromStream(ms);
                            }
                        }
                        PictureBox picBox = new PictureBox();
                        var width = input.PicSize.Width;
                        var height = input.PicSize.Height;
                        picBox.SizeMode = PictureBoxSizeMode.Zoom;
                        picBox.MaximumSize = new Size(width, height);
                        picBox.Image = ResizeImagePro(pic, width, height);
                        picBox.Location = new Point(0, offsetY);
                        picBox.Size = picBox.MaximumSize;
                        picBox.BorderStyle = BorderStyle.FixedSingle;
                        if (input.funsimage != null)
                        {
                            input.funsimage(picBox);
                        }
                        msgCard.Controls.Add(picBox);
                        offsetY += picBox.Height + 10 + input.Margin;
                        pic.Dispose();

                    }
            // 移除固定+6冗余高度，卡片高度=实际内容高度即可
            msgCard.Height = offsetY;

            panelChat.Controls.Add(msgCard);
            panelChat.PerformLayout();
            panelChat.ScrollControlIntoView(msgCard);
            // 配套之前隐藏滚动条逻辑，新增消息后刷新隐藏滚动条
            ScrollBarHideHelper.HideAllScrollBarVisual(panelChat);
            });
        }

        /// <summary>
        /// 图片等比缩放，不放大
        /// </summary>
        public static Bitmap ResizeImagePro(Image src, int maxW, int maxH)
        {
            float ratio = Math.Min((float)maxW / src.Width, (float)maxH / src.Height);
            ratio = Math.Min(ratio, 1.0f);
            int newW = (int)(src.Width * ratio);
            int newH = (int)(src.Height * ratio);

            Bitmap bmp = new Bitmap(newW, newH);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.DrawImage(src, 0, 0, newW, newH);
            }
            return bmp;
        }

        /// <summary>
        /// 获取label字符串的宽度
        /// </summary>
        /// <param name="maxWidth">Label的MaximumSize.Width，允许文字换行的最大宽度</param>
        private static int GetLabelHeight(Label lbl, int maxWidth)
        {
            using (Graphics g = lbl.CreateGraphics())
            {
                SizeF sizeF = g.MeasureString(lbl.Text, lbl.Font, maxWidth);
                int realHeight = (int)Math.Ceiling(sizeF.Height);
                return realHeight;
            }
        }

        /// <summary>
        /// 订阅：控件尺寸变化时，隐藏滚动条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ScrollBarResize(object sender, EventArgs e)
        {
            if (sender is FlowLayoutPanel panel)
            {
                ScrollBarHideHelper.HideAllScrollBarVisual(panel);
            }
        }

        /// <summary>
        /// 聊天信息入参
        /// </summary>
        public class ChatMessage
        {
            /// <summary>
            /// 聊天类型
            /// </summary>
            public ChatMessageEnum ChatType { get; set; } = ChatMessageEnum.Text;

            /// <summary>
            /// 消息内容（文字类型传入文字，图片类型传入图片url）
            /// </summary>
            public string Msg { get; set; }

            /// <summary>
            /// 消息间距
            /// </summary>
            public int Margin { get; set; } = 1;

            /// <summary>
            /// 图片尺寸
            /// </summary>
            public (int Width, int Height) PicSize { get; set; } = (200, 200);
            /// <summary>
            /// 文字委托
            /// </summary>
            public Action<Label> funsLabel { get; set; } = null;
            /// <summary>
            /// 图片委托
            /// </summary>
            public Action<PictureBox> funsimage { get; set; } = null;


        }

        /// <summary>
        /// 枚举-聊天消息枚举(0:普通消息,1:图片消息)
        /// </summary>
        [Description("聊天消息枚举")]
        public enum ChatMessageEnum
        {
            /// <summary>
            ///普通消息
            /// </summary>
            [Description("普通消息")]
            Text = 0,
            /// <summary>
            ///图片消息
            /// </summary>
            [Description("图片消息")]
            Image = 1,
        }

        /// <summary>
        /// 滚动条隐藏
        /// </summary>
        private static class ScrollBarHideHelper
        {
            [DllImport("user32.dll")]
            private static extern int ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

            [DllImport("user32.dll")]
            private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

            private const int SB_HORZ = 0;
            private const int SB_VERT = 1;

            /// <summary>
            /// 隐藏【水平+垂直滚动条UI】，但是保留垂直方向全部滚动逻辑（鼠标滚轮、代码滚动都可用）
            /// </summary>
            public static void HideAllScrollBarVisual(Control ctrl)
            {
                ShowScrollBar(ctrl.Handle, SB_HORZ, false); //隐藏水平滚动条
                ShowScrollBar(ctrl.Handle, SB_VERT, false); //隐藏垂直滚动条【重点：只是看不见，滚动功能保留】
            }
        }
        #endregion
    }
}
