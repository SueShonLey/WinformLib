using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Text;
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

            if (radioButton1.Checked)
            {
                dataGridView1.SetCommonWithCell(new DataGridViewExtentions.DataDisplayEntityCell<AAA>
                {
                    DataList = new List<AAA>()
                    {
                        new AAA() { Detail = "1班", Name = "学生A", Before = "A1", After = "A2" },
                        new AAA() { Detail = "1班", Name = "学生B", Before = "B1", After = "B2" },
                        new AAA() { Detail = "2班", Name = "学生C", Before = "B1", After = "C2" },
                        new AAA() { Detail = "2班", Name = "学生D", Before = "D1", After = "D2" },
                        new AAA() { Detail = "2班", Name = "学生E", Before = "E1", After = "E2" },
                    },
                    ButtonList = new List<(string ButtonName, string TitileName, int Width)>()
                {
                    ("点击", "操作", 100),
                    ("点击5", "操作", 100),
                },
                    HeadtextList = new List<(System.Linq.Expressions.Expression<Func<AAA, object>> Feild, string TitileName, int Width)>
                {
                    (x => x.Detail, "明细", 100),
                    (x => x.Name, "姓名", 100),
                    (x => x.Before, "分班前", 100),
                    (x => x.After, "分班后", 100),
                }
                });
                dataGridView1.MergeCols(new List<int> { 0, 2 });
            }
            else if (radioButton2.Checked)
            {
                dataGridView1.SetCommonWithCell(new DataGridViewExtentions.DataDisplayEntityCell<AAA>
                {
                    DataList = new List<AAA>()
                {
                    new AAA() { Detail = "1班", Name = "学生A", Before = "学生A", After = "A2" },
                    new AAA() { Detail = "2班", Name = "学生B", Before = "B1", After = "B2" },
                    new AAA() { Detail = "3班", Name = "学生C", Before = "学生C", After = "学生C" },
                    new AAA() { Detail = "4班", Name = "学生D", Before = "D1", After = "D2" },
                },
                    ButtonList = new List<(string ButtonName, string TitileName, int Width)>()
                {
                    ("点击1", "操作", 100),
                    ("点击2", "操作", 100),
                },
                    HeadtextList = new List<(System.Linq.Expressions.Expression<Func<AAA, object>> Feild, string TitileName, int Width)>
                {
                    (x => x.Detail, "明细", 100),
                    (x => x.Name, "姓名", 100),
                    (x => x.Before, "分班前", 100),
                    (x => x.After, "分班后", 100),
                }
                });
                dataGridView1.MergeRows();
            }
            else if (radioButton3.Checked)
            {
                dataGridView1.SetCommonWithCell(new DataGridViewExtentions.DataDisplayEntityCell<AAA>
                {
                    DataList = new List<AAA>()
                    {
                        //new AAA() { Detail = "1班", Name = "学生A", Before = "A1", After = "A2" },
                        //new AAA() { Detail = "1班", Name = "学生B", Before = "B1", After = "B2" },
                        //new AAA() { Detail = "2班", Name = "学生C", Before = "B1", After = "C2" },
                        //new AAA() { Detail = "2班", Name = "学生D", Before = "D1", After = "D2" },
                    },
                    ButtonList = new List<(string ButtonName, string TitileName, int Width)>()
                {
                    ("点击", "操作", 100),
                    ("点击5", "操作", 100),
                },
                    HeadtextList = new List<(System.Linq.Expressions.Expression<Func<AAA, object>> Feild, string TitileName, int Width)>
                {
                    (x => x.Detail, "明细", 100),
                    (x => x.Name, "姓名", 100),
                    (x => x.Before, "分班前", 100),
                    (x => x.After, "分班后", 100),
                }
                    ,IsMergeHeader = true
                });
                //dataGridView1.MergeHeader();
            }
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
    }

    public class AAA
    {
        public string Detail { get; set; }
        public string Name { get; set; }
        public string Before { get; set; }
        public string After { get; set; }
    }





}