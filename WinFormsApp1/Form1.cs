

using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using WinformLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static WinformLib.CustomizeFormsExtentions;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            // 构造测试用例
            var testInput = new CustomizeFormInput
            {
                FormTitle = "测试自定义窗体",
                inputs = new List<CustomizeValueInput>
                {
                    // 测试1：输入框（带默认值）
                    new CustomizeValueInput
                    {
                        Label = "用户名",
                        FormControlType = FormControlType.InputBox,
                        DefaultValue = "测试用户",
                        VertiPadding = 0
                    },
                    // 测试2：数字框（带默认值）
                    new CustomizeValueInput
                    {
                        Label = "年龄",
                        FormControlType = FormControlType.NumberBox,
                        DefaultValue = "25",
                        VertiPadding = 0
                    },
                    // 测试3：下拉框（带默认值）
                    new CustomizeValueInput
                    {
                        Label = "性别",
                        FormControlType = FormControlType.DropDown,
                        Value = new List<string> { "男", "女", "未知" },
                        DefaultValue = "男",
                        VertiPadding = 0
                    },
                    // 测试4：单选框（分组，带默认值）
                    new CustomizeValueInput
                    {
                        Label = "支付方式",
                        FormControlType = FormControlType.RadioButton,
                        Value = new List<string> { "微信", "支付宝", "银行卡" },
                        DefaultValue = "支付宝",
                        VertiPadding = 0
                    },
                    // 测试5：复选框（分组，带默认值）
                    new CustomizeValueInput
                    {
                        Label = "爱好",
                        FormControlType = FormControlType.CheckBox,
                        Value = new List<string> { "读书", "运动", "游戏", "音乐" },
                        DefaultValue = "读书,音乐",
                        VertiPadding = 0
                    },
                    // 测试6：重复Label（用于验证异常）
                    // new CustomizeValueInput { Label = "用户名", FormControlType = FormControlType.InputBox }
                }
            };

            // 调用自定义窗体方法
            var result = this.SetCustomizeForms(testInput);
            this.PopUpTips(JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            }));
        }

        /// <summary>
        /// 渲染panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            // 2. 构造测试用的控件配置列表（覆盖所有控件类型）
            var testInputs = new List<CustomizeValueInput>
        {
            // 测试1：输入框（带默认值）
            new CustomizeValueInput
            {
                Label = "用户名",
                FormControlType = FormControlType.InputBox,
                DefaultValue = "测试用户123",
                VertiPadding = 50
            },
            // 测试2：数字框（带默认值）
            new CustomizeValueInput
            {
                Label = "年龄",
                FormControlType = FormControlType.NumberBox,
                DefaultValue = "25",
                VertiPadding = 50
            },
            // 测试3：下拉框（带默认值）
            new CustomizeValueInput
            {
                Label = "性别",
                FormControlType = FormControlType.DropDown,
                Value = new List<string> { "男", "女", "未知" },
                DefaultValue = "男",
                VertiPadding = 50
            },
            // 测试4：单选框（带默认值）
            new CustomizeValueInput
            {
                Label = "支付方式",
                FormControlType = FormControlType.RadioButton,
                Value = new List<string> { "微信支付", "支付宝", "银行卡", "现金" },
                DefaultValue = "支付宝",
                VertiPadding = 50
            },
            // 测试5：复选框（多默认值）
            new CustomizeValueInput
            {
                Label = "兴趣爱好",
                FormControlType = FormControlType.CheckBox,
                Value = new List<string> { "读书", "运动", "游戏", "音乐", "旅行" },
                DefaultValue = "读书,音乐", // 多个默认值用逗号分隔
                VertiPadding = 50
            }
        };

            // 3. 核心调用：SetCommon方法
            // 参数说明：
            // - testInputs：控件配置列表
            // - CellStyle：单元格边框样式（Single=单线）
            // - LeftPercent：Label列占比30%，控件列占比70%
            tableLayoutPanel1.SetCommon(testInputs);
        }

        /// <summary>
        /// 获取panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var dict = tableLayoutPanel1.GetCommon();
            this.PopUpTips(JsonSerializer.Serialize(dict, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            }));
        }
    }
}



