using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class TabControlExtensions
    {
        private static ConcurrentDictionary<TabControl, List<string>> dict = new ConcurrentDictionary<TabControl, List<string>>();

        /// <summary>
        /// 设置透明Tab
        /// </summary>
        /// <param name="tabControl1">控件</param>
        /// <param name="menuList">菜单名称</param>
        /// <param name="form">继承UI则传入该窗体</param>
        public static void SetTransMenu(this TabControl tabControl1, List<string> menuList, Form form = null)
        {
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(0, 1); //Height不能写0，写1即可压没标签栏
            dict[tabControl1] = menuList;
            int index = 0;
            foreach (var item in tabControl1.TabPages)
            {
                var tabPage = (TabPage)item;
                var tabName = menuList.ElementAtOrDefault(index);
                tabPage.Text = tabName;
                tabPage.Name = tabName;
                tabPage.BorderStyle = BorderStyle.None;
                if (form != null)
                {
                    tabPage.BackColor = form.BackColor;
                    tabPage.BackgroundImage = form.BackgroundImage;
                    tabPage.BackgroundImageLayout = form.BackgroundImageLayout;
                }
                index++;
            }
        }

        /// <summary>
        /// 跳转透明Tab的指定界面
        /// </summary>
        public static void SetTransMenuSelect(this TabControl tabControl1, string MenuName)
        {
            var index = tabControl1.TabPages.IndexOfKey(MenuName);
            tabControl1.SelectedIndex = index;
        }

        /// <summary>
        /// 获取当前选择的透明Tab的界面
        /// </summary>
        public static (int SelectIndex, string SelectMenu) GetTransMenuSelect(this TabControl tabControl1)
        {
            var index = tabControl1.SelectedIndex;
            var menu = dict[tabControl1].ElementAtOrDefault(index) ?? string.Empty;
            return (SelectIndex: index, SelectMenu: menu);
        }
    }
}
