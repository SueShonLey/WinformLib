

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
            //测试：
            var inputs = new List<CustomizeValueInput>
            {
                new CustomizeValueInput { FormControlType = FormControlType.InputBox, Label = "姓名1", Value = new List<string>(),DefaultValue="张三" },
                new CustomizeValueInput { FormControlType = FormControlType.InputBox, Label = "姓名2", Value = new List<string>() },
                new CustomizeValueInput { FormControlType = FormControlType.InputBox, Label = "姓名3", Value = new List<string>() },
                new CustomizeValueInput { FormControlType = FormControlType.DropDown, Label = "性别", Value = new List<string> { "男", "女" },DefaultValue="女" },
                new CustomizeValueInput { FormControlType = FormControlType.CheckBox, Label = "爱好", Value = new List<string> { "阅读", "旅行", "运动","音乐","游戏","打扑克" },DefaultValue="旅行,运动" },
                new CustomizeValueInput { FormControlType = FormControlType.CheckBox, Label = "爱好2", Value = new List<string> { "阅读", "旅行", "运动","音乐","游戏","打扑克" },DefaultValue="旅行,运动,打扑克" },
                new CustomizeValueInput { FormControlType = FormControlType.RadioButton, Label = "学历", Value = new List<string> { "本科", "研究生", "博士" },DefaultValue="研究生" },
                new CustomizeValueInput { FormControlType = FormControlType.RadioButton, Label = "学历2", Value = new List<string> { "本科", "研究生", "博士" },DefaultValue="博士" },
            };

            var result = this.SetCustomizeForms(new CustomizeFormInput { inputs = inputs,FormTitle = "测试demo",LabelLocationX = 100});


            // 处理结果
            if (result.Any())
            {
                // 处理用户输入的内容
                Application.Exit();
            }
            else
            {
                // 用户取消了输入
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }


    }



}

