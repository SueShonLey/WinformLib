using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class ListBoxExtentions
    {
        private static readonly ConcurrentDictionary<ListBox, Action<string>> _actionMap = new ConcurrentDictionary<ListBox, Action<string>>();
        private static readonly Dictionary<ListBox, Action<string, string>> _actionRightMap = new Dictionary<ListBox, Action<string, string>>();

        /// <summary>
        /// 设置ListBox的内容（内容、回调=选中内容）
        /// </summary>
        public static void SetCommon(this ListBox listbox, IEnumerable<string> datas, Action<string> ClickAction = null)
        {
            if (listbox == null) throw new ArgumentNullException(nameof(listbox));

            // 一、清理旧数据和事件绑定
            listbox.Items.Clear();
            RemovePreviousHandler(listbox);

            // 二、设置新内容
            listbox.Items.AddRange(datas.ToArray());

            // 三、设置新的事件处理器
            if (ClickAction != null)
            {
                _actionMap[listbox] = ClickAction;
                listbox.SelectedIndexChanged += OnListBoxSelectedIndexChanged;//设置点击时的方法

                // 可选：在控件释放时清理资源
                listbox.HandleDestroyed += (s, e) => RemovePreviousHandler(listbox);
            }
        }


        /// <summary>
        /// 设置ListBox右键菜单功能（右键列表、回调=选中内容+右键菜单名称）
        /// </summary>
        /// <param name="listbox">目标ListBox控件</param>
        /// <param name="datas">右键菜单选项列表</param>
        /// <param name="RightClickAction">右键回调：参数1=被点击的ListBox项，参数2=选择的右键菜单项</param>
        public static void SetRightCommon(this ListBox listbox, IEnumerable<string> datas, Action<string, string> RightClickAction)
        {
            if (listbox == null) throw new ArgumentNullException(nameof(listbox));

            // 清理旧的右键处理
            RemoveRightPreviousHandler(listbox);

            // 设置右键菜单
            if (RightClickAction != null && datas != null && datas.Any())
            {
                // 存储右键操作到字典
                _actionRightMap[listbox] = RightClickAction;

                // 创建右键菜单
                var contextMenu = new ContextMenuStrip();
                foreach (var item in datas)
                {
                    var menuItem = new ToolStripMenuItem(item);
                    menuItem.Tag = item; // 存储菜单项文本
                    menuItem.Click += (sender, e) => OnRightMenuItemClick(listbox, sender);
                    contextMenu.Items.Add(menuItem);
                }

                // 绑定到ListBox
                listbox.ContextMenuStrip = contextMenu;
                listbox.MouseDown += OnListBoxMouseDown; // 用于获取被点击的ListBox项

                // 控件释放时清理资源
                listbox.HandleDestroyed += (s, e) => RemoveRightPreviousHandler(listbox);
            }
            else
            {
                // 如果不需要右键，清空菜单
                listbox.ContextMenuStrip = null;
                listbox.MouseDown -= OnListBoxMouseDown;
            }
        }

        #region 辅助方法
        private static void OnListBoxMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            var listbox = sender as ListBox;
            if (listbox == null) return;

            // 找到鼠标位置对应的项
            int index = listbox.IndexFromPoint(e.Location);
            if (index >= 0 && index < listbox.Items.Count)
            {
                // 临时存储被点击的项
                listbox.Tag = listbox.Items[index].ToString();
            }
            else
            {
                listbox.Tag = null; // 点击空白处
            }
        }

        private static void OnRightMenuItemClick(ListBox listbox, object menuItemSender)
        {
            var menuItem = menuItemSender as ToolStripMenuItem;
            if (menuItem == null || listbox.Tag == null) return;

            // 获取被点击的ListBox项和选择的菜单项
            string listBoxItem = listbox.Tag.ToString();
            string menuItemText = menuItem.Tag?.ToString() ?? menuItem.Text;

            // 执行回调
            if (_actionRightMap.TryGetValue(listbox, out var action))
            {
                action(listBoxItem, menuItemText);
            }

            // 清理临时存储
            listbox.Tag = null;
        }

        private static void RemoveRightPreviousHandler(ListBox listbox)
        {
            // 移除事件
            listbox.MouseDown -= OnListBoxMouseDown;

            // 清理菜单
            if (listbox.ContextMenuStrip != null)
            {
                listbox.ContextMenuStrip.Dispose();
                listbox.ContextMenuStrip = null;
            }

            // 清理字典
            if (_actionRightMap.ContainsKey(listbox))
            {
                _actionRightMap.Remove(listbox);
            }

            // 清理临时存储
            listbox.Tag = null;
        }

        private static void OnListBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            var listbox = sender as ListBox;
            if (listbox?.SelectedItem == null) return;

            if (_actionMap.TryGetValue(listbox, out var action))
            {
                action(listbox.SelectedItem.ToString() ?? string.Empty);
            }
        }

        private static void RemovePreviousHandler(ListBox listbox)
        {
            // 移除事件处理器
            listbox.SelectedIndexChanged -= OnListBoxSelectedIndexChanged;

            // 清理映射关系
            if (_actionMap.ContainsKey(listbox))
            {
                _actionMap.Remove(listbox, out _);
            }
        }
        #endregion
    }
}
