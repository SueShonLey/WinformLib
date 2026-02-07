

using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
using WinformLib;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
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
                    new User(){Id=1, Name="张三",Age = 15},
                    new User(){Id=2, Name="李四",Age = 20},
                    new User(){Id=3, Name="王五",Age = 35}
                };

            dataGridView1.SetCommonWithCell<User>(new DataGridViewExtentions.DataDisplayEntityCell<User>
            {
                DataList = userList,
                ButtonList = new List<(string ButtonName, string TitileName, int Width)>
                {
                    ("修改","操作1",80),
                    ("删除","操作2",200),
                },
                HeadtextList = new List<(System.Linq.Expressions.Expression<Func<User, object>> fields, string name, int width)>
                {
                    (u => u.Id, "ID", 50),
                    (u => u.Name, "姓名", 200)
                },
                RowAction = (user, row) =>
                {
                    if (user.Name.Equals("李四"))
                    {
                        row.DefaultCellStyle.ForeColor = Color.Red;
                    }
                },
                ColumnAction = (col) =>
                {
                    if (col.Name.Equals("Name"))
                    {
                        col.ReadOnly = false;
                    }
                },
                CellAction = (user, col, cell) =>
                {
                    if (user.Name.Equals("张三") && col.Name.Equals("Name"))
                    {
                        cell.Style.BackColor = Color.Yellow;
                    }
                    if (user.Age > 30 && col.Name.Equals("修改"))
                    {
                        cell.Value = "不可改";
                    }
                },
            });
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; internal set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = dataGridView1.GetCommon<User>(new List<(System.Linq.Expressions.Expression<Func<User, object>> fields, string name)>
            {
                    (u => u.Id, "ID"),
                    (u => u.Name, "姓名")
            });
            MessageBox.Show(JsonSerializer.Serialize(result,new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var update = dataGridView1.GetCommonByButton<User>("修改",e);
            var delete = dataGridView1.GetCommonByButton<User>("删除",e);
            if (update != null)
            {
                MessageBox.Show("修改:"+JsonSerializer.Serialize(update, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
            }           
            if (delete != null)
            {
                MessageBox.Show("删除:"+JsonSerializer.Serialize(delete, new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
            }
          
        }
    }
}



