using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.Net;
using System.Runtime.InteropServices;
using WinformLib;
using static WinformLib.FlowLayoutPanelExtentions;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var pc = panelChat2;
            pc.SetCommonFlowMsg(new ChatMessage
            {
                ChatType = ChatMessageEnum.Text,
                Msg = "【张三】"+ $"你好，这是一条聊天消息({DateTime.Now})",
                funsLabel = (label) =>
                {
                    label.ForeColor = Color.Red;
                }
            });
            pc.SetCommonFlowMsg(new ChatMessage
            {
                ChatType = ChatMessageEnum.Text,
                Msg = "【王五】"+ $"你好，我在的！这是一条聊天消息({DateTime.Now})",
                funsLabel = (label) =>
                {
                    label.ForeColor = Color.Blue;
                }
            });
            pc.SetCommonFlowMsg(new ChatMessage
            {
                ChatType = ChatMessageEnum.Image,
                Msg = "https://img1.baidu.com/it/u=1064052654,1225889315&fm=253&app=138&f=JPEG?w=800&h=800",
                PicSize = (200, 200)
            });

        }

    }


}