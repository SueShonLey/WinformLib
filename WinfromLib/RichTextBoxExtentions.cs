using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinfromLib
{
    public static class RichTextBoxExtentions
    {
        public static void SetCommonWithColors(this RichTextBox richTextBox, List<(string text, Color color)> textColors)
        {
            if (richTextBox == null)
                throw new ArgumentNullException(nameof(richTextBox));

            if (textColors == null || textColors.Count == 0)
                return;

            // 保存当前光标位置和选择状态
            int originalSelectionStart = richTextBox.SelectionStart;
            int originalSelectionLength = richTextBox.SelectionLength;

            // 记录原始文本
            string originalText = richTextBox.Text;

            // 重置选择
            richTextBox.Select(0, 0);

            // 遍历每个要设置颜色的文本
            foreach (var (text, color) in textColors)
            {
                if (string.IsNullOrEmpty(text))
                    continue;

                SetTextColor(richTextBox, text, color);
            }

            // 恢复原始选择状态
            richTextBox.Select(originalSelectionStart, originalSelectionLength);
        }

        /// <summary>
        /// 在 RichTextBox 中查找并设置指定文本的颜色
        /// </summary>
        private static void SetTextColor(RichTextBox richTextBox, string searchText, Color color)
        {
            int startIndex = 0;

            while (startIndex < richTextBox.Text.Length)
            {
                // 查找文本
                int foundIndex = richTextBox.Find(searchText, startIndex, RichTextBoxFinds.None);

                if (foundIndex == -1) // 未找到
                    break;

                // 选择找到的文本
                richTextBox.Select(foundIndex, searchText.Length);

                // 设置颜色
                richTextBox.SelectionColor = color;

                // 重置选择（避免影响后续查找）
                richTextBox.Select(0, 0);

                // 更新查找起始位置
                startIndex = foundIndex + searchText.Length;
            }
        }
    }
}
