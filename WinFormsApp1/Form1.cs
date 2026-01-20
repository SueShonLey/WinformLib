
using Microsoft.VisualBasic.ApplicationServices;
using System.Linq.Expressions;
using WinformLib;
using static WinformLib.DataGridViewExtentions;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            MainForm_Load();

        }
        private void MainForm_Load()
        {
            // 1. 准备测试数据（用户实体）
            var userList = new List<User>
            {
                new User { Id = 1, Name = "张三", Age = 25, Status = "正常", Dept = "技术部" },
                new User { Id = 2, Name = "李四", Age = 30, Status = "禁用", Dept = "财务部" },
                new User { Id = 3, Name = "王五", Age = 35, Status = "正常", Dept = "人事部" },
                new User { Id = 4, Name = "赵六", Age = 40, Status = "禁用", Dept = "技术部" }
            };

            // 2. 构建DataDisplayEntity<User>实体
            var displayEntity = new DataDisplayEntity<User>
            {
                // 2.1 核心数据列表
                DataList = userList,

                // 2.2 表头配置（字段表达式、显示名称、宽度）
                headtextList = new List<(Expression<Func<User, object>> fields, string name, int width)>
                {
                    (u => u.Id, "用户ID", 80),
                    (u => u.Name, "姓名", 120),
                    (u => u.Age, "年龄", 80),
                    (u => u.Status, "状态", 100),
                    (u => u.Dept, "部门", 120)
                },

                // 2.3 按钮列表
                ButtonList = new List<string> { "编辑", "删除", "详情" },

                // 2.4 自定义行样式（核心：根据字段/值修改样式）
                changeLineFuns = (fieldName, fieldValue, style) =>
                {
                    if (fieldName == "Id" && fieldValue == 1.ToString())
                    {
                        style.ForeColor = Color.Green;
                    }
                },

                // 2.4 自定义单元格样式（核心：根据字段/值修改样式）
                changeCellFunsList = new List<Action<string, string, DataGridViewCellStyle>>
                {
                    // 规则1：Status字段值为"禁用"时，文字红色、背景浅红
                    (fieldName, fieldValue, style) =>
                    {
                        if (fieldName == "Status" && fieldValue == "禁用")
                        {
                            style.ForeColor = Color.Red;
                            style.BackColor = Color.LightPink;
                            style.Font = new Font("宋体", 9, FontStyle.Italic);
                        }
                    },
                    // 规则2：Age字段值大于35时，文字蓝色、加粗
                    (fieldName, fieldValue, style) =>
                    {
                        if (fieldName == "Age" && int.TryParse(fieldValue, out int age) && age > 35)
                        {
                            style.ForeColor = Color.Blue;
                            style.Font = new Font("宋体", 9, FontStyle.Bold);
                        }
                    }
                },

                // 2.5 按钮显示控制（核心：根据字段/值控制按钮是否显示）
                changeBtnList = new List<Func<string, string, string, bool>>
                {
                    // 规则1：Status为"禁用"时，隐藏"编辑"按钮
                    (fieldName, fieldValue, btnName) =>
                    {
                        if (fieldName == "Status" && fieldValue == "禁用" && btnName == "编辑")
                        {
                            return false; // 返回false则隐藏按钮
                        }
                        return true; // 其他情况显示按钮
                    },
                    // 规则2：Dept为"财务部"时，隐藏"删除"按钮
                    (fieldName, fieldValue, btnName) =>
                    {
                        if (fieldName == "Dept" && fieldValue == "财务部" && btnName == "删除")
                        {
                            return false;
                        }
                        return true;
                    }
                }
            };

            // 3. 调用扩展方法渲染DataGridView
            dataGridView1.SetCommonWithUI(displayEntity);

        }

        /// <summary>
        /// 用户实体（测试用）
        /// </summary>
        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            public string Status { get; set; }
            public string Dept { get; set; }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var r = dataGridView1.GetCommonByButton<User>("详情", e);
        }
    }
}