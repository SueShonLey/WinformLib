using System.Collections.Concurrent;
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
            listBox1.SetCommon(new List<string> { "aa","bbb","ccc"}, ShowMyItem);

            listBox1.SetRightCommon(new List<string> { "选项一", "选项二", "选项三" }, (str1, str2) => { MessageBox.Show($"【选中】{str1}\n【右键内容】{str2}"); });
        }

        private void ShowMyItem(string str )
        {
            MessageBox.Show(str);
        }
    }
}

