using System;
using System.Collections.Generic;
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
        public static void SetCommon<T>(this DataGridView dataGridView, List<T> list, List<(Expression<Func<T, object>> fields, string name,int width)> headtext, List<string> ButtonList = null) where T : class
        {

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
            dataGridView.ReadOnly = true;//设置只读
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
                    if (ButtonList != null)
                    {
                        int index = 1;
                        foreach (var j in ButtonList)
                        {
                            dataGridView.Rows[rowIndex].Cells[j].Value = j;//按钮名称
                            index++;
                        }
                    }

                }
                dataGridView.Rows[rowIndex].Tag = item;//绑定到Tag上方便后续调用
            }
        }

        /// <summary>
        /// DataGridView转List<T>
        /// 对应关系：DataGridView的DataPropertyName=实体的属性名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGridView"></param>
        /// <returns></returns>
        public static List<T> GetCommon<T>(this DataGridView dataGridView, List<(Expression<Func<T, object>> fields, string name)> headtext = null) where T : new()
        {
            List<T> list = new List<T>();

            // 如果传入了 headtext，先解析字段名列表
            List<string> propertyNames = null;
            if (headtext != null)
            {
                propertyNames = headtext.Select(x =>
                {
                    if (x.fields.Body is MemberExpression memberExpr)
                        return memberExpr.Member.Name;
                    else if (x.fields.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression memberOperand)
                        return memberOperand.Member.Name;
                    else
                        return string.Empty;
                }).ToList();
            }

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                T obj = new T();

                if (propertyNames != null)
                {
                    // 按 headtext 指定字段反射
                    foreach (var propName in propertyNames)
                    {
                        PropertyInfo property = typeof(T).GetProperty(propName);
                        if (property != null && property.CanWrite)
                        {
                            if (dataGridView.Columns.Contains(propName))
                            {
                                object cellValue = row.Cells[propName].Value;
                                if (cellValue != DBNull.Value && cellValue != null)
                                {
                                    try
                                    {
                                        property.SetValue(obj, Convert.ChangeType(cellValue, property.PropertyType));
                                    }
                                    catch
                                    {
                                        property.SetValue(obj, Activator.CreateInstance(property.PropertyType));
                                    }
                                }
                                else
                                {
                                    property.SetValue(obj, Activator.CreateInstance(property.PropertyType));
                                }
                            }
                        }
                    }
                }
                else
                {
                    // 默认按所有列反射
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        string propertyName = string.Empty;

                        if (column is DataGridViewTextBoxColumn || column is DataGridViewComboBoxColumn || column is DataGridViewCheckBoxColumn)
                        {
                            propertyName = column.DataPropertyName;
                        }

                        PropertyInfo property = typeof(T).GetProperty(propertyName);

                        if (property != null && property.CanWrite)
                        {
                            object cellValue = row.Cells[column.Name].Value;
                            if (cellValue != DBNull.Value && cellValue != null)
                            {
                                try
                                {
                                    property.SetValue(obj, Convert.ChangeType(cellValue, property.PropertyType));
                                }
                                catch
                                {
                                    property.SetValue(obj, Activator.CreateInstance(property.PropertyType));
                                }
                            }
                            else
                            {
                                property.SetValue(obj, Activator.CreateInstance(property.PropertyType));
                            }
                        }
                    }
                }

                list.Add(obj);
            }

            return list;
        }

        /// <summary>
        /// 根据按钮上的文字获取实体
        /// 示例：var entity = dataGridView1.GetCommonByButton<Product>("删除",e);
        /// </summary>
        /// <returns></returns>
        public static T? GetCommonByButton<T>(this DataGridView dataGridView1,string title, DataGridViewCellEventArgs e) where T : class,new ()
        {
            if (e.ColumnIndex == dataGridView1.Columns[title].Index && e.RowIndex >= 0)//若点击了【title】按钮
            {
                return dataGridView1.Rows[e.RowIndex].Tag as T;
            }
            return null;
        }

    }
}
