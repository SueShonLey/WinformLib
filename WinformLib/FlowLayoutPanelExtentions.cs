using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void AddButtons(this FlowLayoutPanel flowPanel, FlowLayOutListInput input, Action<int, string,Button> onBtnClick)
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
                string currentBtnName = btnName;

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
                    onBtnClick?.Invoke(currentIndex, currentBtnName,btn);
                };

                flowPanel.Controls.Add(btn);
            }
        }
        #endregion
    }
}
