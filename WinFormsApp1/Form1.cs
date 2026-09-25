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

        private void Form1_Load(object sender, EventArgs e)
        {
           

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<AAA> list = new List<AAA>();
            dataGridView1.SetCommonWithCell(new DataGridViewExtentions.DataDisplayEntityCell<AAA>
            {
                DataList = list,
                ButtonList = new List<(string ButtonName, string TitileName, int Width)>
                    {
                        ("扫描","操作",60)
                    },
                HeadtextList = new List<(System.Linq.Expressions.Expression<Func<AAA, object>> Feild, string TitileName, int Width)>
                    {
                        (x=>x.Id,"文件名称",180),
                        (x=>x.Name,"大小",100),
                        (x=>x.After,"修改时间",180),
                    }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
          
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