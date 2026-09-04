using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
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
            this.TopMost = true;


        }

        private void button1_Click(object sender, EventArgs e)
        {

            dataGridView1.SetCommonWithCell(new DataGridViewExtentions.DataDisplayEntityCell<AAA>
            {
                DataList = new List<AAA>()
                    {
                        new AAA() {Id=1, Detail = "1班", Name = "学生A", Before = "A1", After = "A2" },
                        new AAA() {Id=2, Detail = "1班", Name = "学生B", Before = "B1", After = "B2" },
                        new AAA() {Id=3, Detail = "2班", Name = "学生C", Before = "B1", After = "C2" },
                        new AAA() { Id=4,Detail = "2班", Name = "学生D", Before = "D1", After = "D2"},
                        new AAA() {Id=5, Detail = "2班", Name = "学生E", Before = "E1", After = "E2" },
                    },
                ButtonList = new List<(string ButtonName, string TitileName, int Width)>()
                {
                    ("点击", "操作", 100),
                    ("点击5", "操作", 100),
                },
                HeadtextList = new List<(System.Linq.Expressions.Expression<Func<AAA, object>> Feild, string TitileName, int Width)>
                {
                    //(x => x.IsCheck, "选择1", 80),
                    (x => x.Detail, "明细", 100),
                    (x => x.Name, "姓名", 100),
                    (x => x.Before, "分班前", 100),
                    (x => x.After, "分班后", 100),
                    //(x => x.IsCheck2, "选择2", 100),
                },
                IsUseCheckbox = true,
            });
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var entity = dataGridView1.GetCommonByButton<AAA>("点击", e);
            var entity1 = dataGridView1.GetCommonByButton<AAA>("点击1", e);
            var entity2 = dataGridView1.GetCommonByButton<AAA>("点击2", e);
            AAA print = entity ?? entity1 ?? entity2;
            if (print != null)
            {
                this.PopUpTips(JsonConvert.SerializeObject(print));
            }
        }

        /// <summary>
        /// 获取选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //var list = dataGridView1.GetCommonByCheckbox<AAA>( x=>x.IsCheck);
            //this.PopUpTips(JsonConvert.SerializeObject(list, Formatting.Indented));

            var entity = comboBox1.GetCommonSelectWithEntity<AAA>();
            //var res = (entity) == EnumDataSource.Run;
            this.PopUpTips(JsonConvert.SerializeObject(entity));
        }




        /// <summary>
        /// 设置选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //dataGridView1.SetAllCheckbox<AAA>(x=>x.IsCheck,true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SetCommonWithEntity<AAA>(new List<AAA>()
                    {
                        new AAA() {Id=1, Detail = "1班", Name = "学生A", Before = "A1", After = "A2" },
                        new AAA() {Id=2, Detail = "1班", Name = "学生B", Before = "B1", After = "B2" },
                        new AAA() {Id=3, Detail = "2班", Name = "学生C", Before = "B1", After = "C2" },
                        new AAA() { Id=4,Detail = "2班", Name = "学生D", Before = "D1", After = "D2"},
                        new AAA() {Id=5, Detail = "2班", Name = "学生E", Before = "E1", After = "E2" },
                    },x=>x.Name,isSuggest:true);
        }
    }

    public class AAA
    {
        public int Id { get; set; }
        public string Detail { get; set; }
        public string Name { get; set; }
        public string Before { get; set; }
        public string After { get; set; }

        //public bool IsCheck     { get; set; }
        //public bool IsCheck2     { get; set; }
    }

    /// <summary>
    /// 枚举-数据源(0:创建,1:运行,2:就绪,3:阻塞,4:结束)
    /// </summary>
    [Description("数据源")]
    public enum EnumDataSource
    {
        /// <summary>
        ///创建
        /// </summary>
        [Description("创建")]
        Create = 0,
        /// <summary>
        ///运行
        /// </summary>
        [Description("运行")]
        Run = 1,
        /// <summary>
        ///就绪
        /// </summary>
        [Description("就绪")]
        Ready = 2,
        /// <summary>
        ///阻塞
        /// </summary>
        [Description("阻塞")]
        Block = 3,
        /// <summary>
        ///结束
        /// </summary>
        [Description("结束")]
        End = 4,
    }





}