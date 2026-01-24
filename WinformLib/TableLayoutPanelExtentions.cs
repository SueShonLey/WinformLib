using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WinformLib.CustomizeFormsExtentions;

namespace WinformLib
{
    public static class TableLayoutPanelExtentions
    {
        /// <summary>
        /// 渲染控件：根据CustomizeValueInput列表动态生成Label+对应控件（输入框/下拉/单选/复选）
        /// </summary>
        /// <param name="Inputs">控件配置列表</param>
        /// <param name="tableLayoutPanel">目标TableLayoutPanel</param>
        /// <param name="leftPercent">Label列宽度占比（0-100）</param>
        public static void SetCommon(this TableLayoutPanel tableLayoutPanel,List<CustomizeValueInput> Inputs, TableLayoutPanelCellBorderStyle CellStyle = TableLayoutPanelCellBorderStyle.Single, double? LeftPercent = null)
        {
            // 空值校验
            if (Inputs == null || Inputs.Count == 0)
            {
                tableLayoutPanel.Controls.Clear();
                tableLayoutPanel.RowCount = 0;
                tableLayoutPanel.ColumnCount = 0;
                return;
            }

            // 1. 清空原有控件和样式
            tableLayoutPanel.Controls.Clear();
            tableLayoutPanel.RowStyles.Clear();
            tableLayoutPanel.ColumnStyles.Clear();

            // 2. 基础布局配置：2列（Label列 + 控件列）
            tableLayoutPanel.ColumnCount = 2;
            // 设置列宽度占比
            if (LeftPercent != null)
            {
                if (LeftPercent.Value < 0 || LeftPercent.Value > 100)
                {
                    throw new ArgumentException("leftPercent（标签宽度比例）的值必须在0到100之间");
                }
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (float)LeftPercent.Value)); // Label列
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, (float)(100 - LeftPercent.Value))); // 控件列
            }
            else
            {
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Label列自适应
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // 第二列占剩余空间
            }

            // 3. 设置行数和行高
            tableLayoutPanel.RowCount = Inputs.Count;
            for (int i = 0; i < Inputs.Count; i++)
            {
                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / Inputs.Count));//平均设置行高

                // 4. 创建Label控件
                Label label = new Label
                {
                    Text = Inputs[i].Label,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                tableLayoutPanel.Controls.Add(label, 0, i); // Label放在第0列

                // 5. 根据控件类型创建对应控件
                Control control = CreateControlByType(Inputs[i], Convert.ToInt32(tableLayoutPanel.GetColumnWidths()[1]));
                tableLayoutPanel.Controls.Add(control, 1, i); // 控件放在第1列

            }

            // 可选：TableLayoutPanel整体样式
            tableLayoutPanel.CellBorderStyle = CellStyle; // 显示网格（调试用）
        }

        /// <summary>
        /// 读取控件值：返回 Label -> 选中/输入值 的字典
        /// </summary>
        /// <param name="tableLayoutPanel">目标TableLayoutPanel</param>
        /// <returns>键=Label文本，值=控件值（复选框用英文逗号分隔多个值）</returns>
        public static Dictionary<string, string> GetCommon(this TableLayoutPanel tableLayoutPanel)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (tableLayoutPanel.RowCount == 0 || tableLayoutPanel.ColumnCount < 2)
            {
                return result;
            }

            // 遍历每一行，读取Label和对应控件值
            for (int i = 0; i < tableLayoutPanel.RowCount; i++)
            {
                // 获取Label文本（第0列）
                Label label = tableLayoutPanel.GetControlFromPosition(0, i) as Label;
                if (label == null || string.IsNullOrEmpty(label.Text))
                {
                    continue;
                }

                // 获取控件（第1列）
                Control control = tableLayoutPanel.GetControlFromPosition(1, i);
                if (control == null)
                {
                    result.Add(label.Text, string.Empty);
                    continue;
                }

                // 根据控件类型读取值
                string value = GetControlValue(control);
                result.Add(label.Text, value);
            }

            return result;
        }

        #region 私有辅助方法
        /// <summary>
        /// 根据FormControlType创建对应控件，并设置默认值
        /// </summary>
        private static Control CreateControlByType(CustomizeValueInput input, int width)
        {
            Control control = null;
            List<string> defaultValues = string.IsNullOrEmpty(input.DefaultValue)
                ? new List<string>()
                : input.DefaultValue.Split(',').ToList();

            switch (input.FormControlType)
            {
                case FormControlType.InputBox:
                    // 输入框
                    TextBox textBox = new TextBox
                    {
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Left,
                        Width = width,
                        TabStop =false,//不自动获取焦点
                        Text = defaultValues.FirstOrDefault() ?? string.Empty,
                    };
                    control = textBox;
                    break;

                case FormControlType.DropDown:
                    // 下拉框
                    ComboBox comboBox = new ComboBox
                    {
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Left,
                        Width = width,
                        DropDownStyle = ComboBoxStyle.DropDownList, // 仅选择，不可输入
                    };
                    comboBox.Items.AddRange(input.Value.ToArray());
                    // 设置默认值
                    if (defaultValues.Any() && comboBox.Items.Contains(defaultValues.First()))
                    {
                        comboBox.SelectedItem = defaultValues.First();
                    }
                    control = comboBox;
                    break;

                case FormControlType.RadioButton:
                    // 单选框组（FlowLayoutPanel包裹，水平排列）
                    FlowLayoutPanel radioPanel = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Left,
                        Width = width,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true,
                        AutoSize = true,
                    };
                    // 创建单选框（同一组互斥）
                    string radioGroupName = $"radio_{input.Label.Replace(" ", "_")}";
                    foreach (string option in input.Value)
                    {
                        RadioButton rb = new RadioButton
                        {
                            Text = option,
                            Name = radioGroupName,
                        };
                        // 设置默认选中
                        if (defaultValues.Contains(option))
                        {
                            rb.Checked = true;
                        }
                        radioPanel.Controls.Add(rb);
                    }
                    control = radioPanel;
                    break;

                case FormControlType.CheckBox:
                    // 复选框组（FlowLayoutPanel包裹，水平排列）
                    FlowLayoutPanel checkPanel = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        Anchor = AnchorStyles.Left,
                        Width = width,
                        FlowDirection = FlowDirection.LeftToRight,
                        WrapContents = true,
                        AutoSize = true,
                    };
                    foreach (string option in input.Value)
                    {
                        CheckBox cb = new CheckBox
                        {
                            Text = option,
                        };
                        // 设置默认选中
                        if (defaultValues.Contains(option))
                        {
                            cb.Checked = true;
                        }
                        checkPanel.Controls.Add(cb);
                    }
                    control = checkPanel;
                    break;
            }

            return control ?? new TextBox(); // 兜底返回输入框
        }

        /// <summary>
        /// 读取不同类型控件的值
        /// </summary>
        private static string GetControlValue(Control control)
        {
            switch (control)
            {
                // 输入框
                case TextBox textBox:
                    return textBox.Text;

                // 下拉框
                case ComboBox comboBox:
                    return comboBox.SelectedItem?.ToString() ?? string.Empty;

                // 单选框组（FlowLayoutPanel）
                case FlowLayoutPanel radioPanel when radioPanel.Controls.OfType<RadioButton>().Any():
                    var selectedRadio = radioPanel.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked);
                    return selectedRadio?.Text ?? string.Empty;

                // 复选框组（FlowLayoutPanel）
                case FlowLayoutPanel checkPanel when checkPanel.Controls.OfType<CheckBox>().Any():
                    var selectedChecks = checkPanel.Controls.OfType<CheckBox>().Where(c => c.Checked).Select(c => c.Text);
                    return string.Join(",", selectedChecks);

                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}
