using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class LabelExtensions
    {
        /// <summary>
        /// 存储每个Label滑动条效果状态：定时器、滚动速度、当前X偏移、要滚动的文本
        /// </summary>
        private static ConcurrentDictionary<Label, ScrollState> _scrollDict = new ConcurrentDictionary<Label, ScrollState>();

        // 使用class引用类型，不要用值元组！方便修改内部字段，不用每次整体替换
        private class ScrollState
        {
            public System.Windows.Forms.Timer Timer;
            public int SpeedPixel;   //每次移动像素
            public int CurrentX;     //文字X坐标
            public string ScrollText;//要滚动的文本
        }

        /// <summary>
        /// 开启Label滑动条效果
        /// </summary>
        /// <param name="label">目标标签</param>
        /// <param name="oriText">文字内容</param>
        /// <param name="Width">文字宽度</param>
        /// <param name="speedPixel">每次向左移动像素，越大越快</param>
        public static void SetSlideStart(this Label label, string oriText, int Width = 500, int speedPixel = 2)
        {
            if (_scrollDict.ContainsKey(label) && _scrollDict.TryRemove(label, out var state))//若有重复则注销之前的
            {
                state.Timer.Stop();
                state.Timer.Dispose();
                label.Paint -= Label_Paint;
                label.Invalidate();
            }

            label.AutoSize = false;
            label.Text = string.Empty; //清空原生绘制
            label.Paint -= Label_Paint;
            label.Paint += Label_Paint;
            label.Width = Width;


            var timer = new System.Windows.Forms.Timer { Interval = 50 };
            timer.Tick += (s, e) => label.Invalidate();
            timer.Start();

            var scrollState = new ScrollState
            {
                Timer = timer,
                SpeedPixel = speedPixel,
                CurrentX = 0, //初始位置在控件左侧
                ScrollText = oriText
            };
            _scrollDict.TryAdd(label, scrollState);
        }

        /// <summary>
        /// 停止滑动条效果，变为静态显示
        /// </summary>
        public static void SetSlideStop(this Label label)
        {
            if (_scrollDict.TryRemove(label, out var state))
            {
                state.Timer.Stop();
                state.Timer.Dispose();
                label.Paint -= Label_Paint;
                label.Invalidate();
                label.Text = state.ScrollText;
            }
        }

        private static void Label_Paint(object sender, PaintEventArgs e)
        {
            var label = sender as Label;
            if (label == null || label.IsDisposed) return;
            if (!_scrollDict.TryGetValue(label, out var state))
            {
                return;
            }

            string scrollText = state.ScrollText;
            if (string.IsNullOrEmpty(scrollText)) return;

            SizeF textSize = e.Graphics.MeasureString(scrollText, label.Font);

            state.CurrentX -= state.SpeedPixel;

            // 文字完全滑出左侧，重置到控件右侧外面，等待下一轮滑入
            if (state.CurrentX < -textSize.Width)
            {
                state.CurrentX = 0;
            }

            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            using (SolidBrush brush = new SolidBrush(label.ForeColor))
            {
                // 只画一份文字，删掉第二份DrawString！！
                e.Graphics.DrawString(state.ScrollText, label.Font, brush, state.CurrentX, 0);
            }
        }
    }
}
