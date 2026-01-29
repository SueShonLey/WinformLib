using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinformLib;

namespace WinformLib
{
    public static class CustomizeFormsExtentions
    {
        #region
        /// <summary>
        /// 自定义窗体UI及控件选项
        /// </summary>
        public class CustomizeFormInput
        {
            /// <summary>
            /// 内容选项
            /// </summary>
            public List<CustomizeValueInput> inputs { get; set; } = new List<CustomizeValueInput>();
            /// <summary>
            /// 自定义窗体的标题
            /// </summary>
            public string FormTitle { get; set; } = "自定义输入对话框";
            /// <summary>
            /// 标签和控件之间的距离(可选)
            /// </summary>
            public int LabelLocationX { get; set; } = -1;
            /// <summary>
            /// 自定义窗体的大小(可选)
            /// </summary>
            public (int Width, int Height) Size { get; set; } = (-1, -1);
        }

        /// <summary>
        /// 自定义窗体控件选项
        /// </summary>
        public class CustomizeValueInput
        {
            /// <summary>
            /// 控件之间的垂直上下距离
            /// </summary>
            public int VertiPadding { get; set; } = 50;
            /// <summary>
            /// 控件类型
            /// </summary>

            public FormControlType FormControlType { get; set; } = FormControlType.InputBox;
            /// <summary>
            /// 标签内容
            /// </summary>
            public string Label { get; set; } = string.Empty;
            /// <summary>
            /// 控件的值
            /// </summary>
            public List<string> Value { get; set; } = new List<string>();
            /// <summary>
            /// 默认值(英文逗号分隔)
            /// </summary>
            public string DefaultValue { get; set; } = string.Empty;
        }

        /// <summary>
        /// 自定义窗体控件选项
        /// </summary>
        public enum FormControlType
        {
            InputBox = 1,     // 输入框
            DropDown = 2,     // 下拉框
            RadioButton = 3,  // 单选框
            CheckBox = 4      // 复选框
        }

        /// <summary>
        /// 自定义窗体方法
        /// </summary>
        public static Dictionary<string, string> SetCustomizeForms(this Form form,CustomizeFormInput inputDto)
        {
            // 基础验证
            var labels = inputDto.inputs.Select(x => x.Label).ToList();
            var labels_Distinct = labels.Distinct().ToList();
            if (labels.Count != labels_Distinct.Count)
            {
                throw new Exception("Label不允许重名！");
            }

            var inputs = inputDto.inputs;
            // 创建自定义窗体
            Form inputForm = new Form
            {
                Text = inputDto.FormTitle,
                MaximizeBox = false,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                Icon = form.Icon //继承原来的窗体的图标
            };

            // 常量声明，统一维护
            int defualtHeight = 30;
            int currentY = 15;
            int FormPadding = 30;//窗体内边距
            int defualtControlWidth = 250;//默认控件宽度（textbox,combobox）
            int horizontalSpacing = 10; // 单选框/复选框横向间距（解决文本重叠）
            int btnTopMargin = 10; // 按钮与上方控件的间距（解决高度不足）

            // 存储各控件组
            Dictionary<string, List<CheckBox>> checkBoxGroups = new Dictionary<string, List<CheckBox>>();
            Dictionary<string, List<RadioButton>> radioButtonGroups = new Dictionary<string, List<RadioButton>>();
            Dictionary<string, TextBox> textBoxes = new Dictionary<string, TextBox>();
            Dictionary<string, ComboBox> comboBoxes = new Dictionary<string, ComboBox>();

            // 算出最大标签宽度
            var maxLabel = inputs.MaxBy(x => x.Label.Length)?.Label;
            int maxLabelWidth = GetLabelWith(maxLabel);
            var LabelAndValuePadding = inputDto.LabelLocationX > 0 ? inputDto.LabelLocationX : maxLabelWidth + 10;

            //算出控件最大宽度
            int maxControlWidth = defualtControlWidth;

            //添加控件
            foreach (var input in inputs)
            {
                // 添加标签
                Label label = new Label
                {
                    Text = input.Label,
                    Location = new System.Drawing.Point(FormPadding, currentY + 2),
                    AutoSize = true
                };
                inputForm.Controls.Add(label);

                switch (input.FormControlType)
                {
                    case FormControlType.InputBox:
                        TextBox textBox = new TextBox
                        {
                            Location = new System.Drawing.Point(FormPadding + LabelAndValuePadding, currentY),
                            Width = defualtControlWidth,
                            Height = defualtHeight
                        };
                        inputForm.Controls.Add(textBox);
                        textBoxes[input.Label] = textBox;
                        if (!string.IsNullOrEmpty(input.DefaultValue))
                        {
                            textBox.Text = input.DefaultValue;
                        }
                        break;

                    case FormControlType.DropDown:
                        ComboBox comboBox = new ComboBox
                        {
                            Location = new System.Drawing.Point(FormPadding + LabelAndValuePadding, currentY),
                            Width = defualtControlWidth,
                            Height = defualtHeight,
                            DropDownStyle = ComboBoxStyle.DropDownList
                        };
                        comboBox.Items.AddRange(input.Value.ToArray());
                        inputForm.Controls.Add(comboBox);
                        comboBoxes[input.Label] = comboBox;
                        if (!string.IsNullOrEmpty(input.DefaultValue))
                        {
                            comboBox.SetCommonItems(input.DefaultValue);
                        }
                        else
                        {
                            if(comboBox.Items != null && comboBox.Items.Count > 0)
                            {
                                comboBox.SelectedIndex = 0;
                            }
                        }
                        break;

                    case FormControlType.RadioButton:
                        #region 修复核心：为每组单选框创建独立Panel容器
                        int radioButtonX = 0;
                        // 创建Panel，承载当前组所有单选框，解决全局互斥问题
                        Panel radioPanel = new Panel
                        {
                            Location = new System.Drawing.Point(FormPadding + LabelAndValuePadding, currentY),
                            Height = defualtHeight,
                            Width = 0 // 宽度动态计算
                        };
                        radioButtonGroups[input.Label] = new List<RadioButton>();
                        foreach (var value in input.Value)
                        {
                            RadioButton radioButton = new RadioButton
                            {
                                Text = value,
                                Location = new System.Drawing.Point(radioButtonX, 0), // Panel内X轴从0开始
                                AutoSize = true,
                                Height = defualtHeight
                            };
                            radioButtonGroups[input.Label].Add(radioButton);
                            if (!string.IsNullOrEmpty(input.DefaultValue) && value == input.DefaultValue)
                            {
                                radioButton.Checked = true;
                            }
                            radioPanel.Controls.Add(radioButton);
                            // 累加Panel内控件位置，加间距
                            radioButtonX += radioButton.Width + horizontalSpacing;
                        }
                        // 设置Panel的实际宽度，刚好包裹所有单选框
                        radioPanel.Width = radioButtonX + horizontalSpacing;
                        inputForm.Controls.Add(radioPanel);
                        // 更新最大控件宽度，适配窗体
                        maxControlWidth = Math.Max(maxControlWidth, radioPanel.Width);
                        #endregion
                        break;

                    case FormControlType.CheckBox:
                        #region 优化：为每组复选框创建独立Panel容器，解决横向溢出
                        int checkButtonX = 0;
                        var defaultValues = !string.IsNullOrEmpty(input.DefaultValue) ? input.DefaultValue.Split(',').ToList() : new List<string>();
                        // 创建Panel，承载当前组所有复选框
                        Panel checkPanel = new Panel
                        {
                            Location = new System.Drawing.Point(FormPadding + LabelAndValuePadding, currentY),
                            Height = defualtHeight,
                            Width = 0
                        };
                        checkBoxGroups[input.Label] = new List<CheckBox>();
                        foreach (var value in input.Value)
                        {
                            CheckBox checkBox = new CheckBox
                            {
                                Text = value,
                                Location = new System.Drawing.Point(checkButtonX, 0), // Panel内X轴从0开始
                                AutoSize = true,
                                Height = defualtHeight
                            };
                            checkBoxGroups[input.Label].Add(checkBox);
                            if (defaultValues.Contains(value))
                            {
                                checkBox.Checked = true;
                            }
                            checkPanel.Controls.Add(checkBox);
                            // 累加Panel内控件位置，加间距
                            checkButtonX += checkBox.Width + horizontalSpacing;
                        }
                        // 设置Panel的实际宽度，刚好包裹所有复选框
                        checkPanel.Width = checkButtonX + horizontalSpacing;
                        inputForm.Controls.Add(checkPanel);
                        // 更新最大控件宽度，适配窗体
                        maxControlWidth = Math.Max(maxControlWidth, checkPanel.Width);
                        #endregion
                        break;
                }
                // 保底最小值，防止控件垂直重叠
                currentY += Math.Max(input.VertiPadding, defualtHeight + 5);
            }

            // 重新计算窗体宽度，确保所有控件都能完整显示
            var FormWidth = FormPadding * 3 + LabelAndValuePadding + maxControlWidth;
            inputForm.Width = FormWidth;

            // 所有控件同宽度
            foreach (Control item in inputForm.Controls)
            {
                item.Width = maxControlWidth;
            }

            // 创建一个任务以便返回选定的值
            var tcs = new Dictionary<string, string>();

            // 创建确认按钮
            Button btnOK = new Button
            {
                Text = "确定",
                Height = defualtHeight,
                Width = 80,
                Location = new System.Drawing.Point(FormPadding + LabelAndValuePadding, currentY + btnTopMargin)
            };
            btnOK.Click += (s, args) =>
            {
                tcs.Clear();
                // 获取输入框的值
                foreach (var textBox in textBoxes)
                {
                    tcs[textBox.Key] = textBox.Value.Text;
                }

                // 获取下拉框的值
                foreach (var comboBox in comboBoxes)
                {
                    tcs[comboBox.Key] = comboBox.Value.SelectedItem?.ToString() ?? string.Empty;
                }

                // 获取复选框的值
                foreach (var group in checkBoxGroups)
                {
                    List<string> selectedCheckBoxValues = group.Value.Where(cb => cb.Checked).Select(cb => cb.Text).ToList();
                    tcs[group.Key] = string.Join(",", selectedCheckBoxValues);
                }

                // 获取单选框的值【完全正常：组内单选、组间独立】
                foreach (var group in radioButtonGroups)
                {
                    string selectedRadioButtonValue = group.Value.FirstOrDefault(rb => rb.Checked)?.Text ?? string.Empty;
                    tcs[group.Key] = selectedRadioButtonValue;
                }

                inputForm.Close();
            };

            // 创建取消按钮
            Button btnCancel = new Button
            {
                Text = "取消",
                Height = defualtHeight,
                Width = 80,
                Location = new System.Drawing.Point(btnOK.Right + 80, currentY + btnTopMargin)
            };

            btnCancel.Click += (s, args) =>
            {
                tcs.Clear();
                inputForm.Close();
            };

            // 将按钮添加到窗体
            inputForm.Controls.Add(btnOK);
            inputForm.Controls.Add(btnCancel);

            // 核心修复：设置客户区高度，按钮完整显示，无遮挡
            inputForm.ClientSize = new Size(inputForm.ClientSize.Width, btnOK.Bottom + FormPadding);

            // 用户自定义大小
            if (inputDto.Size.Width > 0 && inputDto.Size.Height > 0)
            {
                inputForm.Width = inputDto.Size.Width;
                inputForm.Height = inputDto.Size.Height;
            }

            //调整按钮位置
            var btnOk_X = (inputForm.ClientSize.Width - 80 * 3) / 2;
            var btnCancel_X = btnOk_X + 80*2;
            btnOK.Location = new Point(btnOk_X, currentY + btnTopMargin);
            btnCancel.Location = new Point(btnCancel_X, currentY + btnTopMargin);


            // 显示窗体并等待用户操作
            inputForm.ShowDialog();

            // 返回用户输入的结果
            return tcs;
        }

        /// <summary>
        /// 获取label字符串的宽度
        /// </summary>
        /// <param name="maxLabel"></param>
        /// <returns></returns>
        private static int GetLabelWith(string? maxLabel)
        {
            Label lbl = new Label();
            lbl.Text = maxLabel;
            //lbl.Font = new System.Drawing.Font("宋体",15) ;//如果有字体格式还要设置好，可以用默认的
            Graphics g = Graphics.FromHwnd(lbl.Handle);
            SizeF size = g.MeasureString(lbl.Text, lbl.Font);//获取大小
            g.Dispose();
            return Convert.ToInt32(size.Width);
        }

        #endregion
    }
}
