using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinfromLib
{
    public static class ComboBoxExtentions
    {
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="dataList">内容</param>
        /// <param name="isSelectFirst">默认选中内容第一个</param>
        /// <param name="isLazyLoading">延迟加载（不卡死）</param>
        /// <param name="isSuggest">开启建议模式</param>
        public static void SetCommon(this ComboBox comboBox, List<string> dataList, bool isSelectFirst = true, bool isLazyLoading = true, bool isSuggest = true)
        {
            // 启用延迟加载时，使用 Task.Run() 在后台加载数据
            if (isLazyLoading)
            {
                // 使用 Task.Run 异步加载数据
                Task.Run(() =>
                {
                    // 在后台线程加载完数据后，通过 BeginInvoke 将更新 UI 的操作切换到主线程
                    comboBox.BeginInvoke(new Action(() =>
                    {
                        SetData(comboBox, dataList, isSelectFirst, isSuggest);
                    }));
                });
            }
            else
            {
                SetData(comboBox, dataList, isSelectFirst, isSuggest);
            }

        }

        /// <summary>
        /// 获取当前选择的索引 & 文字
        /// </summary>
        public static (int SelectIndex, string SelectContent) GetCommonSelect(this ComboBox comboBox)
        {
            return (SelectIndex: comboBox.SelectedIndex, SelectContent: comboBox.SelectedItem?.ToString() ?? string.Empty);
        }

        /// <summary>
        /// 根据文字锁定相应的下拉框
        /// </summary>
        public static void SetCommonItems(this ComboBox comboBox, string content = null)
        {
            if (!string.IsNullOrEmpty(content))
            {
                // 查找该内容对应的项
                int itemIndex = comboBox.Items.IndexOf(content.Trim());
                if (itemIndex >= 0)
                {
                    comboBox.SelectedIndex = itemIndex;  // 通过文字内容设置选中项
                }
                else
                {
                    // 默认选中第一个项（如果有项存在）
                    if (comboBox.Items.Count > 0)
                    {
                        comboBox.SelectedIndex = 0;
                    }
                }
            }
            else
            {
                // 默认选中第一个项（如果有项存在）
                if (comboBox.Items.Count > 0)
                {
                    comboBox.SelectedIndex = 0;
                }
            }
        }

        private static void SetData(ComboBox comboBox, List<string> dataList, bool isSelectFirst, bool isSuggest)
        {
            // 如果没有启用延迟加载，直接在 UI 线程更新 ComboBox
            comboBox.Items.Clear();
            comboBox.Items.AddRange(dataList.ToArray());

            // 设置自动完成功能（如果启用）
            if (isSuggest)
            {
                comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            }

            // 设置默认选中第一个项（如果启用）
            if (isSelectFirst && comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;  // 默认选中第一个项
            }
        }

    }
}
