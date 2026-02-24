

using Newtonsoft.Json;
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

        /// <summary>
        /// 初始化加载
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SetCommonWithEnum<EnumDataSource>();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            var res = comboBox1.GetCommonSelectWithEnum<EnumDataSource>();
            this.PopUpTips(JsonConvert.SerializeObject(res.ToString()));
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var res = comboBox1.GetCommonSelectWithEnumDetails<EnumDataSource>();
            this.PopUpTips(JsonConvert.SerializeObject(res));
        }


        /// <summary>
        /// 枚举-数据源(0:创建,1:运行,2:就绪,3:阻塞,4:结束)
        /// </summary>
        [Description("数据源")]
        public enum EnumDataSource
        {
            /// <summary>
            ///全部
            /// </summary>
            [Description("全部")]
            All = -1,
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
}



