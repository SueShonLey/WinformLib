
using Microsoft.VisualBasic.ApplicationServices;
using System.Linq.Expressions;
using WinformLib;
using static WinformLib.DataGridViewExtentions;
using static WinformLib.FlowLayoutPanelExtentions;

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
            // 1. 准备输入参数
            var input = new FlowLayOutListInput
            {
                NameList = new List<string> { "新增", "编辑", "删除", "查询" },
                BorderStyle = BorderStyle.Fixed3D, // 3D边框
                VerticalSpacing =20
            };

            // 2. 调用扩展方法（假设窗体中有一个名为flowLayoutPanel1的控件）
            flowLayoutPanel1.AddButtons(input, (Index,Name) =>
            {
                // tuple 就是传出的(Index, Name)元组
                MessageBox.Show($"点击了按钮：索引={Index}，名称={Name}");

                // 按按钮名称执行不同逻辑
                switch (Name)
                {
                    case "新增":
                        // 新增逻辑
                        break;
                    case "编辑":
                        // 编辑逻辑
                        break;
                        // 其他按钮逻辑
                }
            });            
        }
    }
}