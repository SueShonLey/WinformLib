using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Text;
using System.Net;
using System.Runtime.InteropServices;
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //组装入参Dto
            CustomizeFormInput dto = new CustomizeFormInput
            {
                FormTitle = "批量参数配置弹窗",
                LabelLocationX = 90, //标签和输入控件的X偏移
                Size = (-1, -1),      //交给内部自动计算窗体大小
                inputs = new List<CustomizeValueInput>
        {
            //1.普通输入框
            new CustomizeValueInput
            {
                Label = "项目名称",
                FormControlType = FormControlType.InputBox,
                DefaultValue = "测试项目A",
                //VertiPadding = 45,
               Enable = true,
                Value = new List<string>()
            },
            //2.数字输入框
            new CustomizeValueInput
            {
                Label = "超时秒数",
                FormControlType = FormControlType.NumberBox,
                DefaultValue = "30",
               // VertiPadding = 45,
              Enable = true,
                Value = new List<string>()
            },
            //3.下拉选择框
            new CustomizeValueInput
            {
                Label = "运行模式",
                FormControlType = FormControlType.DropDown,
                DefaultValue = "正式",
                //VertiPadding = 45,
               Enable = true,
                Value = new List<string>{"开发","测试","正式","预发布"}
            },
            //4.单选框组
            new CustomizeValueInput
            {
                Label = "日志级别",
                FormControlType = FormControlType.RadioButton,
                DefaultValue = "Info",
                //VertiPadding = 45,
                Enable = true,
                Value = new List<string>{"Debug","Info","Warn","Error"}
            },
            //5.复选框组（多选，DefaultValue逗号分隔）
            new CustomizeValueInput
            {
                Label = "启用模块",
                FormControlType = FormControlType.CheckBox,
                DefaultValue = "数据库,缓存",
                //VertiPadding = 45,
                Enable = false,
                Value = new List<string>{"数据库","缓存","消息队列","文件日志"}
            },
        },
                IsinheritBackPics = true,
                funsForm = (form) =>
                {
                    //form.Width = 530;
                    //MessageBox.Show(form.Width.ToString());
                }
            };

            //调用扩展方法，弹出自定义窗体
            Dictionary<string, string> resultDict = this.SetCustomizeForms(dto);

            //判断：取消弹窗返回空字典；点击确定会有key‑value
            if (resultDict == null || resultDict.Count == 0)
            {
                
                return;
            }

            //读取各个控件返回值，Key就是每个CustomizeValueInput的Label
            string projectName = resultDict["项目名称"];
            string timeoutSec = resultDict["超时秒数"];
            string runMode = resultDict["运行模式"];
            string logLevel = resultDict["日志级别"];
            string enableModules = resultDict["启用模块"]; //复选框返回逗号拼接字符串

            //输出到弹窗看结果
            string showMsg =
                $"项目名称：{projectName}\r\n" +
                $"超时秒数：{timeoutSec}\r\n" +
                $"运行模式：{runMode}\r\n" +
                $"日志级别：{logLevel}\r\n" +
                $"启用模块：{enableModules}";

            MessageBox.Show(showMsg, "获取弹窗返回结果");

        }

    }


}