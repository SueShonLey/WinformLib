using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class CheckedListBoxExtentions
    {
        /// <summary>
        /// 为CheckBoxList设置数据源
        /// </summary>
        public static void SetCommon(this CheckedListBox checkedListBox1, List<string> data)
        {
            checkedListBox1.CheckOnClick = true;
            foreach (var item in data)
            {
                checkedListBox1.Items.Add(item);
            }
        }

        /// <summary>
        /// 全选和取消全选（flag = True=全选）
        /// </summary>
        public static void SetCommonAll(this CheckedListBox checkedListBox1, bool flag)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, flag);
            }
        }

        /// <summary>
        /// 获取CheckedListBox数据(True=获取选中的项目，False=获取未选中的项目)
        /// </summary>
        public static List<string> GetCommonStatus(this CheckedListBox checkedListBox1, bool isSelect = true)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                bool isChecked = checkedListBox1.GetItemChecked(i);

                // 根据 isSelect 的值来决定是获取选中的项目还是未选中的项目
                if (isSelect)
                {
                    if (isChecked)
                        result.Add(checkedListBox1.Items[i].ToString());
                }
                else
                {
                    if (!isChecked)
                        result.Add(checkedListBox1.Items[i].ToString());
                }
            }
            return result;
        }
    }
}
