

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
            // 模拟实体数据源
            var userList = new List<User>()
                {
                    new User(){Id=1, Name="张三"},
                    new User(){Id=2, Name="李四"},
                    new User(){Id=3, Name="王五"}
                };

            // 绑定实体到下拉框（启用延迟加载、自动选中第一项、联想提示）
            comboBox1.SetCommonWithEntity(userList,x=>x.Name);
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            User selectedUser = comboBox1.GetCommonSelectWithEntity<User>();
            if (selectedUser != null)
            {
                MessageBox.Show($"选中用户：{selectedUser.Name}，ID：{selectedUser.Id}");
            }
            else
            {
                MessageBox.Show("未选中任何用户");
            }
        }
    }



}