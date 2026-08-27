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
        public static void SetCommon<T>(this DataGridView dataGridView, List<T> list, List<(Expression<Func<T, object>> fields, string name, int width)> headtext, List<string> ButtonList = null,bool IsUseCheckbox = false) where T : class
        {
            dataGridView.ClearMerge();
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

            var boolField = field.Select((entity, index)=> (entity, index))
                                .Where(x => x.entity.PropertyType == typeof(bool))
                                .Select(x => x.index)
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
                var nowIndex = headtext.FindIndex(x => x == item);
                if (IsUseCheckbox && boolField.Contains(nowIndex))
                {
                    dataGridView.Columns.Add(new DataGridViewCheckBoxColumn  //增加文字列
                    {
                        DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },//剧中对齐
                        HeaderText = item.name,//中文标题
                        MinimumWidth = 6,
                        Name = field[headtext.FindIndex(x => x == item)].Name,//字段的名字 例如ID Name
                        ReadOnly = false,
                        SortMode = DataGridViewColumnSortMode.NotSortable,//不要列头排序，否则无法居中
                        Width = item.width
                    });
                    continue;
                }
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
        /// 渲染DataGridView（可控制Cell/UI，加强版）
        /// </summary>
        public static void SetCommonWithCell<T>(this DataGridView dataGridView, DataDisplayEntityCell<T> input) where T : class, new()
        {
            // 先渲染数据
            dataGridView.SetCommon(input.DataList, input.HeadtextList, input.ButtonList.Select(x => x.ButtonName).ToList(),input.IsUseCheckbox);
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
            // 列操作
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
                if (dataGridView.Columns[item.ButtonName] != null)
                {
                    dataGridView.Columns[item.ButtonName].HeaderText = item.TitileName;
                    dataGridView.Columns[item.ButtonName].Width = item.Width;
                }
            }
            //允许单元格内容换行
            if (input.IsAllowWrap)
            {
                dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
            // 处理合并表头
            if (input.IsMergeHeader)
            {
                dataGridView.MergeHeader();
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
            if (e.ColumnIndex == dataGridView1.Columns[title]?.Index && e.RowIndex >= 0)//若点击了【title】按钮
            {
                //bool类型的列
                var boolIndexList = dataGridView1.Columns.Cast<DataGridViewColumn>()
                                        .Select((entity, index) => (entity, index))
                                        .Where(p => p.entity.CellType == typeof(DataGridViewCheckBoxCell))
                                        .Select(x => x.index)
                                        .ToList();
                //获取点击的行
                var row = dataGridView1.Rows[e.RowIndex];
                //获取Tag
                var output = row.Tag as T;
                if (output != null)
                {
                    //反射维护bool类型的列
                    foreach (var item in boolIndexList)
                    {
                        //反射赋值一下
                        var feildName = row.Cells[item].OwningColumn.Name;
                        output.GetType().GetProperty(feildName)?.SetValue(output, row.Cells[item].Value);
                    }
                }
                return output;
            }
            return null;
        }

        #region 表格合并处理
        private static readonly Dictionary<DataGridView, MergeContext> _gridDict = new Dictionary<DataGridView, MergeContext>();

        private class MergeContext
        {
            public Dictionary<int, List<(int startRow, int endRow)>> ColMergeAreas { get; }
                = new Dictionary<int, List<(int, int)>>();
            // key=-1 代表表头行，其余为数据行
            public Dictionary<int, List<(int startCol, int endCol)>> RowMergeAreas { get; }
                = new Dictionary<int, List<(int, int)>>();
            public bool EventBinded { get; set; }
        }

        #region 表头合并
        /// <summary>
        /// 自动进行表头合并（水平合并）
        /// </summary>
        public static void MergeHeader(this DataGridView dgv)
        {
            if (dgv == null) return;
            var context = GetOrCreateContext(dgv);

            // 清空旧表头合并区域
            context.RowMergeAreas[-1] = new List<(int startCol, int endCol)>();
            var areaList = context.RowMergeAreas[-1];

            int colCount = dgv.Columns.Count;
            if (colCount < 2) return;

            // 线性扫描表头列标题
            string currentValue = dgv.Columns[0].HeaderText ?? string.Empty;
            int startCol = 0;

            for (int col = 1; col < colCount; col++)
            {
                string val = dgv.Columns[col].HeaderText ?? string.Empty;
                if (val != currentValue)
                {
                    if (col - 1 > startCol)
                        areaList.Add((startCol, col - 1));
                    startCol = col;
                    currentValue = val;
                }
            }
            // 处理最后一组
            if (colCount - 1 > startCol)
                areaList.Add((startCol, colCount - 1));

            EnsureEventsBinded(dgv, context);
            dgv.Invalidate(); // 重绘整个控件，刷新表头

            //背景色重置
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.HeaderCell.Style.BackColor = Color.White;
            }
        }
        #endregion

        #region 垂直列合并
        /// <summary>
        /// 传入列的索引，自动进行传入列合并（垂直合并）
        /// </summary>
        public static void MergeCols(this DataGridView dgv, List<int> cols)
        {
            if (dgv == null || cols == null || cols.Count == 0) return;
            GetOrCreateContext(dgv).RowMergeAreas.Clear();
            var context = GetOrCreateContext(dgv);

            foreach (int col in cols)
            {
                if (col < 0 || col >= dgv.Columns.Count) continue;
                context.ColMergeAreas[col] = new List<(int, int)>();
                var areaList = context.ColMergeAreas[col];

                if (dgv.Rows.Count == 0) continue;
                string currentValue = dgv.Rows[0].Cells[col].Value?.ToString() ?? string.Empty;
                int startRow = 0;

                for (int i = 1; i < dgv.Rows.Count; i++)
                {
                    string val = dgv.Rows[i].Cells[col].Value?.ToString() ?? string.Empty;
                    if (val != currentValue)
                    {
                        if (i - 1 > startRow)
                            areaList.Add((startRow, i - 1));
                        startRow = i;
                        currentValue = val;
                    }
                }
                if (dgv.Rows.Count - 1 > startRow)
                    areaList.Add((startRow, dgv.Rows.Count - 1));

                EnsureEventsBinded(dgv, context);
                dgv.InvalidateColumn(col);
            }
            dgv.Invalidate();

            //列头合并
            dgv.MergeHeader();
        }
        #endregion

        #region 水平行合并
        /// <summary>
        /// 传入行的索引，自动进行传入行合并（水平合并）
        /// 不传则默认所有行进行内容合并
        /// </summary>
        public static void MergeRows(this DataGridView dgv, List<int> rows = null)
        {
            if (dgv == null) return;
            GetOrCreateContext(dgv).ColMergeAreas.Clear();
            var context = GetOrCreateContext(dgv);

            List<int> targetRows = rows ?? new List<int>();
            if (targetRows.Count == 0)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                    targetRows.Add(i);
            }

            foreach (int row in targetRows)
            {
                if (row < 0 || row >= dgv.Rows.Count) continue;
                if (dgv.Rows[row].IsNewRow) continue;

                context.RowMergeAreas[row] = new List<(int, int)>();
                var areaList = context.RowMergeAreas[row];

                int colCount = dgv.Columns.Count;
                if (colCount < 2) continue;

                string currentValue = dgv.Rows[row].Cells[0].Value?.ToString() ?? string.Empty;
                int startCol = 0;
                for (int col = 1; col < colCount; col++)
                {
                    string val = dgv.Rows[row].Cells[col].Value?.ToString() ?? string.Empty;
                    if (val != currentValue)
                    {
                        if (col - 1 > startCol)
                            areaList.Add((startCol, col - 1));
                        startCol = col;
                        currentValue = val;
                    }
                }
                if (colCount - 1 > startCol)
                    areaList.Add((startCol, colCount - 1));

                EnsureEventsBinded(dgv, context);
                dgv.InvalidateRow(row);
            }
            dgv.Invalidate();

            //列头合并
            dgv.MergeHeader();
        }
        #endregion

        #region 辅助方法
        private static MergeContext GetOrCreateContext(DataGridView dgv)
        {
            if (!_gridDict.TryGetValue(dgv, out var context))
            {
                context = new MergeContext();
                _gridDict[dgv] = context;

                dgv.DataSourceChanged += (s, e) =>
                {
                    if (_gridDict.TryGetValue(dgv, out var ctx))
                    {
                        ctx.ColMergeAreas.Clear();
                        ctx.RowMergeAreas.Clear();
                    }
                };
            }
            return context;
        }

        private static void EnsureEventsBinded(DataGridView dgv, MergeContext context)
        {
            if (context.EventBinded) return;
            BindMergeEvents(dgv, context);
            context.EventBinded = true;
        }
        #endregion

        #region 统一事件绑定
        private static void BindMergeEvents(DataGridView dgv, MergeContext context)
        {
            dgv.SelectionChanged += (s, e) =>
            {
                // 禁止局部InvalidateColumn / InvalidateRow！局部刷新不会触发Paint事件，文字重影
                dgv.Invalidate();
            };


            dgv.CellPainting += (s, e) =>
            {
                bool handled = false;

                // 垂直合并（仅数据行）
                if (e.RowIndex >= 0 && context.ColMergeAreas.TryGetValue(e.ColumnIndex, out var colAreas))
                {
                    foreach (var area in colAreas)
                    {
                        if (e.RowIndex >= area.startRow && e.RowIndex <= area.endRow)
                        {
                            DrawVerticalCell(dgv, e, area);
                            handled = true;
                            break;
                        }
                    }
                }

                // 水平合并（兼容表头行-1和数据行）
                if (context.RowMergeAreas.TryGetValue(e.RowIndex, out var rowAreas))
                {
                    foreach (var area in rowAreas)
                    {
                        if (e.ColumnIndex >= area.startCol && e.ColumnIndex <= area.endCol)
                        {
                            DrawHorizontalCell(dgv, e, area, handled);
                            handled = true;
                            break;
                        }
                    }
                }

                if (handled) e.Handled = true;
            };

            dgv.Paint += (s, e) =>
            {
                int firstRow = dgv.FirstDisplayedScrollingRowIndex;
                int lastRow = firstRow + dgv.DisplayedRowCount(false);
                if (lastRow >= dgv.Rows.Count) lastRow = dgv.Rows.Count - 1;

                // 垂直合并文字
                foreach (var colPair in context.ColMergeAreas)
                {
                    int col = colPair.Key;
                    foreach (var area in colPair.Value)
                    {
                        if (area.endRow < firstRow || area.startRow > lastRow) continue;
                        int sss = Math.Max(area.startRow, firstRow);
                        int end = Math.Min(area.endRow, lastRow);

                        Rectangle top = dgv.GetCellDisplayRectangle(col, sss, false);
                        Rectangle bottom = dgv.GetCellDisplayRectangle(col, end, false);
                        Rectangle rect = new Rectangle(top.X, top.Y, top.Width, bottom.Bottom - top.Y);

                        bool selected = IsColGroupSelected(dgv, col, area);
                        Color fore = selected ? dgv.DefaultCellStyle.SelectionForeColor : dgv.DefaultCellStyle.ForeColor;
                        TextRenderer.DrawText(e.Graphics, dgv[col, area.startRow].Value?.ToString() ?? "",
                            dgv.DefaultCellStyle.Font, rect, fore,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                    }
                }

                // 水平合并文字（表头+数据行）
                foreach (var rowPair in context.RowMergeAreas)
                {
                    int row = rowPair.Key;
                    var areas = rowPair.Value;

                    // 表头行：永远可见，跳过可见性判断
                    if (row == -1)
                    {
                        foreach (var area in areas)
                        {
                            Rectangle left = dgv.GetCellDisplayRectangle(area.startCol, -1, false);
                            Rectangle right = dgv.GetCellDisplayRectangle(area.endCol, -1, false);
                            Rectangle rect = new Rectangle(left.X, left.Y, right.Right - left.X, left.Height);

                            // 用表头样式
                            Color fore = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
                            Font font = dgv.ColumnHeadersDefaultCellStyle.Font;
                            TextRenderer.DrawText(e.Graphics, dgv.Columns[area.startCol].HeaderText ?? "",
                                font, rect, fore,
                                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                        }
                        continue;
                    }

                    // 数据行：可见性判断
                    if (row < firstRow || row > lastRow) continue;
                    foreach (var area in areas)
                    {
                        Rectangle left = dgv.GetCellDisplayRectangle(area.startCol, row, false);
                        Rectangle right = dgv.GetCellDisplayRectangle(area.endCol, row, false);
                        Rectangle rect = new Rectangle(left.X, left.Y, right.Right - left.X, left.Height);

                        bool selected = IsRowGroupSelected(dgv, row, area);
                        Color fore = selected ? dgv.DefaultCellStyle.SelectionForeColor : dgv.DefaultCellStyle.ForeColor;
                        TextRenderer.DrawText(e.Graphics, dgv[area.startCol, row].Value?.ToString() ?? "",
                            dgv.DefaultCellStyle.Font, rect, fore,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak);
                    }
                }
            };

            dgv.Scroll += (s, e) =>
            {
                //滚动时强制全部重绘，解决Paint事件不触发导致文字残留重影
                dgv.Invalidate();
            };

            dgv.Disposed += (s, e) => _gridDict.Remove(dgv);
        }

        private static void DrawVerticalCell(DataGridView dgv, DataGridViewCellPaintingEventArgs e, (int startRow, int endRow) area)
        {
            bool selected = IsColGroupSelected(dgv, e.ColumnIndex, area);
            Color back = selected ? e.CellStyle.SelectionBackColor : e.CellStyle.BackColor;

            using (Brush brush = new SolidBrush(back))
                e.Graphics.FillRectangle(brush, e.CellBounds);

            using (Pen pen = new Pen(dgv.GridColor))
            {
                e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Left, e.CellBounds.Bottom);
                e.Graphics.DrawLine(pen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);
                if (e.RowIndex == area.startRow)
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Top);
                if (e.RowIndex == area.endRow)
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
            }
        }

        private static void DrawHorizontalCell(DataGridView dgv, DataGridViewCellPaintingEventArgs e, (int startCol, int endCol) area, bool hasVerticalMerge)
        {
            bool selected = e.RowIndex >= 0 && IsRowGroupSelected(dgv, e.RowIndex, area);
            Color back = selected ? e.CellStyle.SelectionBackColor : e.CellStyle.BackColor;

            if (!hasVerticalMerge)
            {
                using (Brush brush = new SolidBrush(back))
                    e.Graphics.FillRectangle(brush, e.CellBounds);
            }

            using (Pen pen = new Pen(dgv.GridColor))
            {
                e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Right, e.CellBounds.Top);
                e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                if (e.ColumnIndex == area.startCol)
                    e.Graphics.DrawLine(pen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Left, e.CellBounds.Bottom);
                if (e.ColumnIndex == area.endCol)
                    e.Graphics.DrawLine(pen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);
            }
        }

        private static bool IsColGroupSelected(DataGridView dgv, int col, (int startRow, int endRow) area)
        {
            for (int r = area.startRow; r <= area.endRow; r++)
                if (dgv[col, r].Selected) return true;
            return false;
        }

        private static bool IsRowGroupSelected(DataGridView dgv, int row, (int startCol, int endCol) area)
        {
            for (int c = area.startCol; c <= area.endCol; c++)
                if (dgv[c, row].Selected) return true;
            return false;
        }
        #endregion

        #region 手动清空
        /// <summary>
        /// 手动清空合并表格的相关字典
        /// </summary>
        /// <param name="dgv"></param>
        public static void ClearMerge(this DataGridView dgv)
        {
            if (_gridDict.TryGetValue(dgv, out var context))
            {
                context.ColMergeAreas.Clear();
                context.RowMergeAreas.Clear();
                dgv.Invalidate();
            }
        }
        #endregion

        #endregion

        #region 获取选择checkbox的数据
        /// <summary>
        /// 获取选择checkbox的数据
        /// </summary>
        public static List<T> GetCommonByCheckbox<T>(this DataGridView dgv, Expression<Func<T, object>> exp) where T : class
        {
            List<T> resultList = new List<T>();

            //获取复选框列
            int index = GetColIndex(dgv, exp);

            //bool类型的列
            var boolIndexList = dgv.Columns.Cast<DataGridViewColumn>()
                                    .Select((entity,index)=>(entity,index))
                                    .Where(p => p.entity.CellType == typeof(DataGridViewCheckBoxCell))
                                    .Select(x=>x.index)
                                    .ToList();

            //遍历所有数据行，跳过新行（底部*那一行）
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                //复选框列，取单元格bool值
                var cellCheck = row.Cells[index];
                bool isChecked = cellCheck.Value != null && (bool)cellCheck.Value;

                if (isChecked)
                {
                    T model = row.Tag as T;
                    if (model != null)
                    {
                        foreach (var item in boolIndexList)
                        {
                            //反射赋值一下
                            var feildName = row.Cells[item].OwningColumn.Name;
                            model.GetType().GetProperty(feildName)?.SetValue(model, row.Cells[item].Value);
                        }
                        resultList.Add(model);
                    }
                }
            }
            return resultList;
        }


        /// <summary>
        /// DataGridView 全选/取消全选 指定列的复选框
        /// </summary>
        /// <param name="dgv">表格控件</param>
        /// <param name="isCheck">true=全选，false=全部取消</param>
        public static void SetAllCheckbox<T>(this DataGridView dgv, Expression<Func<T, object>> exp, bool isCheck = true) where T : class
        {
            try
            {
                //获取复选框列
                int index = GetColIndex(dgv, exp);

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue; //跳过底部新增行
                    row.Cells[index].Value = isCheck; //第0列是复选框列
                }
                //提交编辑，刷新界面状态
                dgv.CommitEdit(DataGridViewDataErrorContexts.Commit);
                dgv.Refresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取复选框列
        /// </summary>
        private static int GetColIndex<T>(DataGridView dgv, Expression<Func<T, object>> exp) where T : class
        {
            //获取字段名称
            string FeildName = string.Empty;
            if (exp.Body is MemberExpression memberExpr)
            {
                FeildName = memberExpr.Member.Name;
            }
            else
            {
                FeildName = ((MemberExpression)((UnaryExpression)exp.Body).Operand).Member.Name;
            }

            //找出是哪一列
            var rows = dgv.Columns.Cast<DataGridViewColumn>().FirstOrDefault(x => x.Name.Equals(FeildName));
            int index = -1;
            if (rows != null)
            {
                index = rows.Index;
            }
            else
            {
                throw new Exception("找不到复选框列");
            }

            return index;
        }
        #endregion

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

            /// <summary>
            /// 是否自动合并表头
            /// </summary>
            public bool IsMergeHeader { get; set; } = true;
            /// <summary>
            /// 是否允许单元格内容换行
            /// </summary>
            public bool IsAllowWrap { get; set; } = false;

            /// <summary>
            /// 是否使用CheckBox渲染
            /// </summary>
            public bool IsUseCheckbox { get; set; } = false;

        }
        #endregion
    }
}
