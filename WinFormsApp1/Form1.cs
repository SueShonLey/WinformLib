
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
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            var a = sender.ToControl<CheckBox>();
            var b = sender.ToControl<RadioButton>();
            var c = sender.ToControl<Button>();
        }


    }
}