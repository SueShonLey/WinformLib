

using System.Text;
using WinformLib;
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

            // 准备控件配置列表
            List<CustomizeValueInput> inputs = new List<CustomizeValueInput>
            {
                // 输入框
                new CustomizeValueInput
                {
                    Label = "服务器名称1234567890",
                    FormControlType = FormControlType.InputBox,
                    DefaultValue = "测试服务器"
                },
                // 下拉框
                new CustomizeValueInput
                {
                    Label = "部署环境",
                    FormControlType = FormControlType.DropDown,
                    Value = new List<string> { "生产", "测试", "开发" },
                    DefaultValue = "测试"
                },
                // 单选框
                new CustomizeValueInput
                {
                    Label = "运行状态",
                    FormControlType = FormControlType.RadioButton,
                    Value = new List<string> { "运行中", "已停止", "维护中" },
                    DefaultValue = "运行中"
                },
                // 复选框
                new CustomizeValueInput
                {
                    Label = "支持协议",
                    FormControlType = FormControlType.CheckBox,
                    Value = new List<string> { "HTTP", "HTTPS", "TCP" },
                    DefaultValue = "HTTP,HTTPS"
                }
            };
            tableLayoutPanel1.SetCommon(inputs);
        }


        private void button1_Click(object sender, EventArgs e)
        {
           var list = tableLayoutPanel1.GetCommon();
            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                sb.AppendLine($"key={item.Key},value={item.Value}");
            }
            this.PopUpTips(sb.ToString());
        }
    }

}