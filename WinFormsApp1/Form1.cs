using System.Collections.Concurrent;
using System.Drawing.Text;
using WinformLib;
using static WinformLib.LinkLabel1Extensions;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer scrollTimer;
        private int currentPosition = 0;
        // 要滚动的文本
        private string scrollText = "这是一段很长的文字，用于演示滚动效果，欢迎使用C# WinForm开发！";

        public Form1()
        {
            InitializeComponent();
            label1.SetSlideStart(scrollText);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            label1.SetSlideStart(textBox1.Text,speedPixel: Convert.ToInt32(numericUpDown1.Value));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.SetSlideStop();//停止
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            label1.Visible = false;//不显示
        }
    }

    
}