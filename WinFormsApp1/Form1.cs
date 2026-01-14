using WinformLib;

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


        private void button1_Click_1(object sender, EventArgs e)
        {
            if(this.CheckNotNull("输入有误，不准为空！",textBox1, textBox2, textBox3))
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

