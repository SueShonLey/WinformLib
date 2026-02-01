

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
            dateTimePicker1.SetCommon( DateTimePickerExtentions.EnumEasyDateTimePicker.Date);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            label1.Text = dateTimePicker1.GetCommon().date.ToString();
            label2.Text = dateTimePicker1.GetCommon().dayOfWeek.ToString();
        }
    }



}