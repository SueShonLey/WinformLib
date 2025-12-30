using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinformLib
{
    public static class StatusStripExtensions
    {
        /// <summary>
        /// 设置StatusStrip（下边框栏）的文本和进度
        /// </summary>
        public static void SetStatusStripCommon(this Form form, string text, decimal? rates = null, Color? color = null)
        {
            // 如果已存在StatusStrip，先移除
            RemoveStatusStrip(form);

            // 创建 StatusStrip
            var statusStrip = new StatusStrip
            {
                Name = "statusStrip"
            };

            // 创建左侧的 ToolStripLabel
            var statusLabel = new ToolStripLabel
            {
                Text = text,
                Font = new Font("宋体", 10),
                Name = "statusLabel",
                AutoSize = true,
                ForeColor = color ?? Color.Black
            };

            // 将状态标签添加到 StatusStrip
            statusStrip.Items.Add(statusLabel);

            // 添加弹簧控件，让后续控件靠右对齐
            statusStrip.Items.Add(new ToolStripStatusLabel
            {
                Spring = true,
                Text = string.Empty
            });

            ToolStripProgressBar progressBar = null;
            ToolStripLabel percentLabel = null;

            if (rates.HasValue)
            {
                // 创建进度条
                progressBar = new ToolStripProgressBar
                {
                    Name = "statusProgressBar",
                    Width = 100,
                    Value = Convert.ToInt32(rates.Value * 100),
                    Maximum = 100,
                    Size = new Size(150, 16)
                };

                // 创建百分比标签
                percentLabel = new ToolStripLabel
                {
                    Name = "percentLabel",
                    Text = $" {rates.Value*100:F2}%", // 格式化保留一位小数
                    Font = new Font("宋体", 10),
                    Margin = new Padding(5, 0, 10, 0)
                };

                statusStrip.Items.Add(progressBar);
                statusStrip.Items.Add(percentLabel);
            }

            // 添加到窗体
            form.Controls.Add(statusStrip);

            // 确保StatusStrip在最前面显示
            statusStrip.BringToFront();

            // 存储控件引用
            form.Tag = new StatusStripControls
            {
                StatusLabel = statusLabel,
                ProgressBar = progressBar,
                PercentLabel = percentLabel,
                StatusStrip = statusStrip
            };
        }

        /// <summary>
        /// 更新StatusStrip的文本和进度
        /// </summary>
        /// <param name="form">目标窗体</param>
        /// <param name="text">状态文本</param>
        /// <param name="rates">进度百分比</param>
        public static void SetStatusStripTextAndRate(this Form form, string text, decimal? rates = null, Color? color = null)
        {
            if (form.Tag is not StatusStripControls controls || controls.StatusStrip == null)
            {
                // 如果StatusStrip不存在，创建它
                form.SetStatusStripCommon(text, rates,color);
                return;
            }

            // 更新状态文本
            if (controls.StatusLabel != null)
            {
                controls.StatusLabel.Text = text;
                controls.StatusLabel.ForeColor = color ?? Color.Black;
            }

            // 更新进度
            if (rates.HasValue)
            {
                // 如果之前没有进度条，现在需要创建
                if (controls.ProgressBar == null || controls.PercentLabel == null)
                {
                    // 需要重新创建包含进度条的StatusStrip
                    form.SetStatusStripCommon(text, rates, color);
                    return;
                }

                var rateValue = Math.Min(rates.Value, 100);
                var add = rates.Value != 100 ? " " : "";
                controls.ProgressBar.Value = Convert.ToInt32(rateValue);
                controls.PercentLabel.Text = $"{add}{rates.Value:F2}%";

                // 显示进度控件
                controls.ProgressBar.Visible = true;
                controls.PercentLabel.Visible = true;
            }
            else
            {
                // 隐藏进度控件
                if (controls.ProgressBar != null)
                    controls.ProgressBar.Visible = false;
                if (controls.PercentLabel != null)
                    controls.PercentLabel.Visible = false;
            }
        }

        #region 私有方法
        /// <summary>
        /// 移除StatusStrip
        /// </summary>
        private static void RemoveStatusStrip(this Form form)
        {
            if (form.Tag is StatusStripControls controls)
            {
                if (controls.StatusStrip != null && !controls.StatusStrip.IsDisposed)
                {
                    form.Controls.Remove(controls.StatusStrip);
                    controls.StatusStrip.Dispose();
                }
                form.Tag = null;
            }

            // 同时查找并移除可能已存在的StatusStrip
            foreach (Control control in form.Controls)
            {
                if (control is StatusStrip statusStrip)
                {
                    form.Controls.Remove(statusStrip);
                    statusStrip.Dispose();
                    break;
                }
            }
        }

        // 用于存储 StatusStrip 控件的类
        private class StatusStripControls
        {
            public ToolStripLabel StatusLabel { get; set; }
            public ToolStripProgressBar ProgressBar { get; set; }
            public ToolStripLabel PercentLabel { get; set; }
            public StatusStrip StatusStrip { get; set; }
        }

        #endregion
    }
}
