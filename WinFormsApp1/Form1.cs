using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.Net;
using System.Runtime.InteropServices;
using WinformLib;
using static WinformLib.CustomizeFormsExtentions;
using static WinformLib.FlowLayoutPanelExtentions;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.SetCommon();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int a = 0;
            int b = 2 / a;

        }

    }


}