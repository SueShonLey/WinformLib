

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


    }
}



