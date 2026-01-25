

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

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.OpenLink("www.baidu.com");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel2.OpenLink("F:\\Test\\Backup", LinkLabel1Extensions.EnumLinkType.Folder);


        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel3.OpenLink("F:\\Test\\Backup\\helloWorld.txt", LinkLabel1Extensions.EnumLinkType.Document);
        }
    }



}