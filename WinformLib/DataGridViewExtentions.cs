using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformLib
{
    public static class DataGridViewExtentions
    {
        /// <summary>
        /// 渲染DataGridView
        /// </summary>
        /// <param name="dataGridView">被渲染控件</param>
        /// <param name="list">数据集</param>
        /// <param name="headtext">字段、展示名称、宽度</param>
        /// <param name="ButtonList">按钮名称，可为空</param>
        public static void SetCommon<T>(this DataGridView dataGridView, List<T> list, List<(Expression<Func<T, object>> fields, string name, int width)> headtext, List<string> ButtonList = null) where T : class
        {
            if (list == null || !list.Any())//无数据
            {
                dataGridView.Rows.Clear();
                return;
            }

            // 使用 LINQ 通过直接提取表达式来获取字段名称
            var propertyNames = headtext
                .Select(x =>
                    x.fields.Body is MemberExpression memberExpr
                    ? memberExpr.Member.Name
                    : ((MemberExpression)((UnaryExpression)x.fields.Body).Operand).Member.Name)
                .ToList();

            //反射获取字段列表
            var field = typeof(T).GetProperties()
                .Where(x => propertyNames.Contains(x.Name))
                .OrderBy(x => propertyNames.Contains(x.Name) ? propertyNames.IndexOf(x.Name) : int.MaxValue)
                .ToList();

            //设置表头样式和属性
            dataGridView.AllowUserToAddRows = false;//不允许添加、删除
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = false;//允许编辑（列中控制只读）
            dataGridView.RowHeadersVisible = false;//隐藏最左边的空白栏
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;//不采用自适应宽度
                                                                                    // 设置表头样式
            dataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter, // 中间对齐
                BackColor = Color.LightGray, // 表头背景色
                ForeColor = Color.Black, // 表头文字颜色
                Font = new Font("宋体", 10, FontStyle.Bold), // 表头字体
            };


            //设置表头内容（按实体顺序依次设置名字）
            dataGridView.Columns.Clear();
            foreach (var item in headtext)
            {
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn  //增加文字列
                {
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },//剧中对齐
                    HeaderText = item.name,//中文标题
                    MinimumWidth = 6,
                    Name = field[headtext.FindIndex(x => x == item)].Name,//字段的名字 例如ID Name
                    ReadOnly = true,
                    SortMode = DataGridViewColumnSortMode.NotSortable,//不要列头排序，否则无法居中
                    Width = item.width
                });
            }

            //设置表头按钮
            if (ButtonList != null)
            {
                foreach (var item in ButtonList)
                {
                    //增加按钮(含样式)
                    dataGridView.Columns.Add(new DataGridViewButtonColumn
                    {
                        DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                        HeaderText = "操作",//中文标题
                        MinimumWidth = 6,
                        Name = item,
                        ReadOnly = true,
                        SortMode = DataGridViewColumnSortMode.NotSortable,
                        Width = 110
                    });
                }
            }

            // 清空现有数据
            dataGridView.Rows.Clear();

            //添加数据
            foreach (var item in list)
            {
                int rowIndex = dataGridView.Rows.Add();
                foreach (var jtem in field)
                {
                    //添加普通内容数据
                    dataGridView.Rows[rowIndex].Cells[jtem.Name.ToString()].Value = jtem.GetValue(item);//字段
                    dataGridView.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Black;
                    //添加按钮数据
                }
                if (ButtonList != null)
                {
                    foreach (var j in ButtonList)
                    {
                        dataGridView.Rows[rowIndex].Cells[j].Value = j;//按钮名称
                    }
                }
                dataGridView.Rows[rowIndex].Tag = item;//绑定到Tag上方便后续调用
            }
        }

        /// <summary>
        /// 渲染DataGridView（可控制UI）
        /// </summary>
        public static void SetCommonWithUI<T>(this DataGridView dataGridView, DataDisplayEntity<T> input) where T : class
        {
            if (input == null || input.DataList == null || !input.DataList.Any())
            {
                dataGridView.Rows.Clear();
                return;
            }
            if (input.headtextList == null || !input.headtextList.Any()) return;

            var list = input.DataList;
            var headtext = input.headtextList;
            var ButtonList = input.ButtonList;

            // 1. 解析表头配置，获取字段名称（通用逻辑，不绑定具体字段）
            var propertyNames = headtext
                .Select(x =>
                    x.fields.Body is MemberExpression memberExpr
                    ? memberExpr.Member.Name
                    : ((MemberExpression)((UnaryExpression)x.fields.Body).Operand).Member.Name)
                .ToList();

            var entityProperties = typeof(T).GetProperties()
                .Where(x => propertyNames.Contains(x.Name))
                .OrderBy(x => propertyNames.IndexOf(x.Name))
                .ToList();

            // 2. 基础样式+列配置（通用）
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.ReadOnly = input.IsReadOnly;
            dataGridView.RowHeadersVisible = false;
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                Font = new Font("宋体", 10, FontStyle.Bold),
            };

            // 清空旧数据
            dataGridView.Columns.Clear();
            dataGridView.Rows.Clear();

            // 3. 添加普通列（通用，根据配置动态生成）
            foreach (var header in headtext)
            {
                var propName = entityProperties[headtext.FindIndex(x => x == header)].Name;
                dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                {
                    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                    HeaderText = header.name,
                    Name = propName,
                    Width = header.width,
                    ReadOnly = input.IsReadOnly,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
            }

            // 4. 添加按钮列（通用，根据配置动态生成）
            if (ButtonList != null && ButtonList.Any())
            {
                foreach (var btnName in ButtonList)
                {
                    dataGridView.Columns.Add(new DataGridViewButtonColumn
                    {
                        DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                        HeaderText = "操作",
                        Name = btnName,
                        Width = 110,
                        ReadOnly = true,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    });
                }
            }

            // 5. 填充数据（核心：全通用，无任何硬编码字段）
            foreach (var dataItem in list)
            {
                int rowIndex = dataGridView.Rows.Add();
                var currentRow = dataGridView.Rows[rowIndex];

                // 5.1 填充普通单元格+执行单元格样式（通用）
                foreach (var prop in entityProperties)
                {
                    // 赋值单元格值
                    var cellValue = prop.GetValue(dataItem) ?? string.Empty;
                    currentRow.Cells[prop.Name].Value = cellValue;

                    // 执行单元格样式规则（用户自定义，方法不干预）
                    if (input.changeCellFunsList != null)
                    {
                        string fieldName = prop.Name;
                        string fieldValue = cellValue.ToString() ?? string.Empty;
                        foreach (var cellStyleFunc in input.changeCellFunsList)
                        {
                            cellStyleFunc(fieldName, fieldValue, currentRow.Cells[fieldName].Style);
                        }
                    }
                }

                // 5.2 执行行样式规则（通用：遍历所有字段传递给用户委托，由用户决定判断逻辑）
                if (input.changeLineFuns != null)
                {
                    foreach (var prop in entityProperties)
                    {
                        string fieldName = prop.Name;
                        string fieldValue = prop.GetValue(dataItem)?.ToString() ?? string.Empty;
                        input.changeLineFuns(fieldName, fieldValue, currentRow.DefaultCellStyle);
                    }
                }

                // 5.3 处理按钮显示规则
                if (ButtonList != null && ButtonList.Any())
                {
                    foreach (var btnName in ButtonList)
                    {
                        bool isShowBtn = true;
                        // 遍历所有按钮规则，由用户规则决定是否显示
                        foreach (var btnRuleFunc in input.changeBtnList)
                        {
                            foreach (var prop in entityProperties)
                            {
                                string fieldName = prop.Name;
                                string fieldValue = prop.GetValue(dataItem)?.ToString() ?? string.Empty;
                                isShowBtn = btnRuleFunc(fieldName, fieldValue, btnName);
                                if (!isShowBtn) break;
                            }
                            if (!isShowBtn) break;
                        }

                        // 处理按钮单元格（修复ReadOnly顺序）
                        if (isShowBtn)
                        {
                            var btnCell = new DataGridViewButtonCell { Value = btnName };
                            currentRow.Cells[btnName] = btnCell;
                            btnCell.ReadOnly = false;
                        }
                        else
                        {
                            var textCell = new DataGridViewTextBoxCell { Value = string.Empty };
                            currentRow.Cells[btnName] = textCell;
                            textCell.ReadOnly = true;
                        }
                    }
                }

                currentRow.Tag = dataItem;
            }
        }

        /// <summary>
        /// 渲染DataGridView（可控制Cell/UI，加强版）
        /// </summary>
        public static void SetCommonWithCell<T>(this DataGridView dataGridView, DataDisplayEntityCell<T> input) where T : class, new()
        {
            // 先渲染数据
            dataGridView.SetCommon(input.DataList, input.HeadtextList, input.ButtonList.Select(x => x.ButtonName).ToList());
            // 单独处理UI
            foreach (DataGridViewRow item in dataGridView.Rows)
            {
                // 行操作
                if (input.RowAction != null)
                {
                    input.RowAction(item.Tag as T, item);
                }

                //单元格操作
                foreach (DataGridViewCell cell in item.Cells)
                {
                    if (input.CellAction != null)
                    {
                        input.CellAction(item.Tag as T, cell.OwningColumn, cell);
                    }

                }
            }
            //列操作
            foreach (DataGridViewColumn item in dataGridView.Columns)
            {
                if (input.ColumnAction != null)
                {
                    input.ColumnAction(item);
                }
            }
            // 处理按钮宽度
            foreach (var item in input.ButtonList)
            {
                dataGridView.Columns[item.ButtonName].HeaderText =item.TitileName;
                dataGridView.Columns[item.ButtonName].Width = item.Width;
            }
        }

        /// <summary>
        /// DataGridView转List<T>
        /// 情况一：不传参，默认读Tag内容
        /// 情况二：传参（字段和标题头名称），读Tag内容 + 用户填写的内容（优先级高于Tag）
        /// </summary>
        public static List<T> GetCommon<T>(this DataGridView dataGridView, List<(Expression<Func<T, object>> fields, string name)> headtext = null) where T : new()
        {
            List<T> list = new List<T>();
            // 解析需要更新的字段名（用户编辑的列）
            List<string> editableFieldNames = headtext?.Select(x =>
            {
                if (x.fields.Body is MemberExpression memberExpression)
                {
                    return memberExpression.Member.Name;
                }
                return (x.fields.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression memberExpression2)
                    ? memberExpression2.Member.Name : string.Empty;
            }).Where(name => !string.IsNullOrEmpty(name)).ToList() ?? new List<string>();

            // 遍历每一行
            foreach (DataGridViewRow item in dataGridView.Rows)
            {
                T val = default(T);

                // 核心1：优先从Tag读取原始对象（包含完整字段如Id）
                if (item.Tag != null && item.Tag is T originalObj)
                {
                    val = originalObj; // 直接复用原始对象，保留Id等未编辑字段
                }
                else
                {
                    val = new T(); // Tag为空时新建对象（兼容新增行）
                }

                // 核心2：覆盖用户编辑后的单元格值（仅更新指定字段）
                if (editableFieldNames.Any() && !item.IsNewRow)
                {
                    foreach (string fieldName in editableFieldNames)
                    {
                        PropertyInfo property = typeof(T).GetProperty(fieldName);
                        if (property == null || !property.CanWrite || !dataGridView.Columns.Contains(fieldName))
                        {
                            continue;
                        }

                        // 获取单元格最新值
                        object cellValue = item.Cells[fieldName].Value;
                        if (cellValue == DBNull.Value || cellValue == null)
                        {
                            // 空值时赋类型默认值（避免报错）
                            property.SetValue(val, Activator.CreateInstance(property.PropertyType));
                        }
                        else
                        {
                            try
                            {
                                // 转换类型并更新值（用户编辑后的最新值）
                                property.SetValue(val, Convert.ChangeType(cellValue, property.PropertyType));
                            }
                            catch
                            {
                                property.SetValue(val, Activator.CreateInstance(property.PropertyType));
                            }
                        }
                    }
                }

                // 排除新增行（空行）
                if (!item.IsNewRow)
                {
                    list.Add(val);
                }
            }

            return list;
        }

        /// <summary>
        /// 根据按钮上的文字获取实体
        /// 示例：var entity = dataGridView1.GetCommonByButton<Product>("删除",e);
        /// </summary>
        /// <returns></returns>
        public static T? GetCommonByButton<T>(this DataGridView dataGridView1, string title, DataGridViewCellEventArgs e) where T : class, new()
        {
            if (e.ColumnIndex == dataGridView1.Columns[title].Index && e.RowIndex >= 0)//若点击了【title】按钮
            {
                return dataGridView1.Rows[e.RowIndex].Tag as T;
            }
            return null;
        }


        #region 辅助方法
        /// <summary>
        /// 通用数据展示实体（包含数据列表、表头配置、按钮列表）
        /// </summary>
        /// <typeparam name="T">数据模型的类型</typeparam>
        public class DataDisplayEntity<T>
        {
            /// <summary>
            /// 核心数据列表
            /// </summary>
            public List<T> DataList { get; set; } = new List<T>();

            /// <summary>
            /// 表头配置列表
            /// fields: 对应T类型的属性表达式（指定要展示的字段）
            /// name: 表头显示名称
            /// width: 表头列宽
            /// </summary>
            public List<(Expression<Func<T, object>> fields, string name, int width)> headtextList { get; set; } = new List<(Expression<Func<T, object>>, string, int)>();

            /// <summary>
            /// 按钮列表（如：新增、编辑、删除等按钮名称）
            /// </summary>
            public List<string> ButtonList { get; set; } = new List<string>();

            /// <summary>
            /// 传入字段、值，传出DataGridViewCellStyle（用于【某行】修改样式，对指定字段指定值进行处理）
            /// </summary>
            public Action<string, string, DataGridViewCellStyle> changeLineFuns { get; set; } = null;

            /// <summary>
            /// 传入字段、值，传出DataGridViewCellStyle（用于【单元格】修改样式，对指定字段指定值进行处理）
            /// </summary>
            public List<Action<string, string, DataGridViewCellStyle>> changeCellFunsList { get; set; } = new List<Action<string, string, DataGridViewCellStyle>>();

            /// <summary>
            /// 传入字段、值、不显示的按钮（用于【按钮】是否显示，false则不显示按钮）
            /// </summary>
            public List<Func<string, string, string, bool>> changeBtnList { get; set; } = new List<Func<string, string, string, bool>>();

            /// <summary>
            /// 是否只读
            /// </summary>
            public bool IsReadOnly { get; set; } = true;
        }

        /// <summary>
        /// UI设置入参Dto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class DataDisplayEntityCell<T>
        {
            /// <summary>
            /// 数据列表
            /// </summary>
            public List<T> DataList { get; set; } = new List<T>();

            /// <summary>
            /// 字段、标题名称及宽度
            /// </summary>
            public List<(Expression<Func<T, object>> Feild, string TitileName, int Width)> HeadtextList { get; set; } = new List<(Expression<Func<T, object>> Feild, string TitileName, int width)>();

            /// <summary>
            /// 按钮名称、标题名称及宽度
            /// </summary>
            public List<(string ButtonName, string TitileName, int Width)> ButtonList { get; set; } = new List<(string ButtonName, string TitileName, int Width)>();

            /// <summary>
            /// 行样式样式委托（实体、实体对应的行）
            /// 示例：if (user.Name.Equals("李四"))row.DefaultCellStyle.ForeColor =  Color.Red;
            /// </summary>
            public Action<T, DataGridViewRow>? RowAction { get; set; } = null;

            /// <summary>
            /// 列按钮委托（列）
            /// 示例：if (col.Name.Equals("Name"))col.ReadOnly = false;
            /// </summary>
            public Action<DataGridViewColumn>? ColumnAction { get; set; } = null;

            /// <summary>
            /// 单元格样式委托（实体、当前列、通过实体和当前列筛选得到的单元格）
            /// 示例：if(user.Name.Equals("张三") && col.Name.Equals("Name"))cell.Style.BackColor = Color.Yellow;
            /// </summary>
            public Action<T, DataGridViewColumn, DataGridViewCell>? CellAction { get; set; } = null;

        }
        #endregion
    }
}
