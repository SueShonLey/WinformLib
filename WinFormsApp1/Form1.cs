

using System.Windows.Forms;
using WinfromLib;
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
        // 窗体全局唯一的错误提示组件，初始化并配置样式
        private readonly ErrorProvider _errorProvider = new ErrorProvider()
        {
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };

        private void button1_Click_1(object sender, EventArgs e)
        {
            if(this.CheckNotNull(textBox1, textBox2, textBox3))
            {
                this.PopUpTips("都不为空");
            }
            else
            {
                this.PopUpTips("输入框存在空值！");
            }
        }



    }



}

