using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
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


        #region setWithEntity
        // 线程安全字典：存储ComboBox与「值索引-实体」列表的映射关系，支持多线程操作
        private static readonly ConcurrentDictionary<ComboBox, object> _comboEntityMap = new ConcurrentDictionary<ComboBox, object>();

        /// <summary>
        /// 根据实体列表设置下拉框选项（含延迟加载、自动选中第一项、联想提示）
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="comboBox">目标下拉框</param>
        /// <param name="dataList">实体数据源</param>
        /// <param name="isSelectFirst">是否自动选中第一项</param>
        /// <param name="isLazyLoading">是否启用后台延迟加载</param>
        /// <param name="isSuggest">是否启用输入联想提示</param>
        public static void SetCommonWithEntity<T>(this ComboBox comboBox, List<T> dataList,Func<T,object> funs, bool isSelectFirst = true, bool isLazyLoading = true)
        {
            // 空值校验：防止空引用异常
            if (comboBox == null) throw new ArgumentNullException(nameof(comboBox), "下拉框控件不能为空");
            if (dataList == null) dataList = new List<T>(); // 空数据源处理为空列表

            // 启用延迟加载：后台线程处理数据，主线程更新UI（避免UI卡顿）
            if (isLazyLoading)
            {
                Task.Run(() =>
                {
                    // 后台线程仅处理数据转换，UI操作通过BeginInvoke切回主线程（WinForm UI线程安全要求）
                    comboBox.BeginInvoke(new Action(() =>
                    {
                        SetDataWithEntity<T>(comboBox, dataList, funs, isSelectFirst);
                    }));
                });
            }
            else
            {
                // 同步加载：直接在当前线程（需确保是UI线程）更新下拉框
                SetDataWithEntity<T>(comboBox, dataList, funs, isSelectFirst);
            }
        }

        /// <summary>
        /// 获取下拉框当前选中的实体
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="comboBox">目标下拉框</param>
        /// <returns>选中的实体（无选中项/无映射时返回default(T)）</returns>
        public static T GetCommonSelectWithEntity<T>(this ComboBox comboBox)
        {
            // 空值校验
            if (comboBox == null) return default;
            // 无选中项直接返回默认值
            if (comboBox.SelectedIndex < 0) return default;

            // 从线程安全字典中获取当前下拉框的「索引-实体」映射列表
            if (_comboEntityMap.TryGetValue(comboBox, out var obj) && obj is List<(int Index, T Entity)> entityList)
            {
                // 根据下拉框当前选中索引，匹配并返回对应的实体
                var selectedItem = entityList.Find(item => item.Index == comboBox.SelectedIndex);
                return selectedItem.Entity;
            }

            // 无映射关系时返回默认值
            return default;
        }

        /// <summary>
        /// 核心：绑定实体数据到下拉框，建立索引-实体映射并更新UI（支持委托自定义显示字段）
        /// </summary>
        /// <param name="comboBox">目标下拉框</param>
        /// <param name="dataList">实体数据源</param>
        /// <param name="funs">自定义显示字段的委托（入参：实体，出参：要显示的字段值）</param>
        /// <param name="isSelectFirst">是否自动选中第一项</param>
        /// <param name="isSuggest">是否启用输入联想提示</param>
        private static void SetDataWithEntity<T>(ComboBox comboBox, List<T> dataList, Func<T, object> funs, bool isSelectFirst)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;//实体类的都需要列表形式
            // 1. 清空下拉框原有数据和映射关系，避免数据残留
            comboBox.Items.Clear();
            _comboEntityMap.TryRemove(comboBox, out _);

            // 2. 转换实体列表为「值索引-实体」元组列表，建立索引映射
            var entityWithIndex = new List<(int Index, T Entity)>();
            for (int i = 0; i < dataList.Count; i++)
            {
                var entity = dataList[i];
                entityWithIndex.Add((i, entity));
                var displayValue = entity == null ? string.Empty : funs(entity)?.ToString() ?? string.Empty;
                comboBox.Items.Add(displayValue);
            }

            // 4. 将映射关系存入线程安全字典，供后续获取选中实体使用
            if (entityWithIndex.Count > 0)
            {
                _comboEntityMap.TryAdd(comboBox, entityWithIndex);
            }

            // 6. 自动选中第一项（数据源非空时）
            if (isSelectFirst && comboBox.Items.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }
        }
        #endregion

        #region 私有方法
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
        #endregion
    }
}
