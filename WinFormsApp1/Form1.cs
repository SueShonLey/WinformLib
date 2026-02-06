

using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
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
            //panel1.ReceiveFiles((path) =>
            //{
            //    label1.Text = path;
            //});

            panel1.ReceiveMutiFiles((path) =>
            {
                label1.Text = string.Join("\n", path);
            });
        }


    }
}



