
using WinformLib;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var menuFormList = new List<(string MenuName, Form TargetForm)>()
            {
                ("窗体1", this), 
                ("窗体2", new Form2()),
                ("窗体3", new Form3()),
            };
            this.SetMenuMDIForm(menuFormList);
        }
    }
}