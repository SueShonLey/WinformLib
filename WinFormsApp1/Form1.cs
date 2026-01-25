

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

        }


        private void button1_Click(object sender, EventArgs e)
        {
            throw new Exception("自定义的报错");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int a = 0;
            int b = 1;
            //int c = b / a;
        }
    }

}