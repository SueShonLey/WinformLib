using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using WinformLib;
using WinfromLib;
using static System.Windows.Forms.DataFormats;
using static WinfromLib.FormExtentions;

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
            TimerExtentions.RegisterTimer("myTest", 3000, () =>
            {
                richTextBox1.Text = DateTime.Now.ToString();
            }, false);
            button1.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TimerExtentions.StopTimer("myTest");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TimerExtentions.StartTimer("myTest");

        }
    }

    public class ReflectResult
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }
    }

    

}

