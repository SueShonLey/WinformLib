using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class GroupBoxExtentions
    {
        /// <summary>
        /// 清空文本框、复选框、富文本框
        /// </summary>
        public static void ClearAll(this GroupBox groupBox1)
        {
            foreach (Control control in groupBox1.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Clear(); // 清空文本框
                }
                else if (control is CheckBox checkBox)
                {
                    checkBox.Checked = false; // 清空复选框
                }
                else if (control is RichTextBox richbox)
                {
                    richbox.Clear(); // 清空富文本框
                }
            }
        }        
        
        /// <summary>
        /// 设置可用性
        /// </summary>
        public static void SetAllEnable(this GroupBox groupBox1,bool enable = true)
        {
            foreach (Control control in groupBox1.Controls)
            {
                control.Enabled = enable;
            }
        }
    }
}
