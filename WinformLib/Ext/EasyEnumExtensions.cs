using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib.Ext
{
    public static class EasyEnumExtensions
    {
        /// <summary>
        /// 获取枚举详情
        /// </summary>
        public static List<EasyEnumDetails<T>> GetEnumDetails<T>(string defualtComment = "无注释") where T : Enum
        {
            Type enumType = typeof(T);
            var list = enumType.GetFields().Where(x => !x.IsSpecialName).ToList();
            var res = new List<EasyEnumDetails<T>>();
            foreach (var item in list)
            {
                var value = item.GetValue(null);
                if (value == null)
                {
                    continue;
                }
                var entity = new EasyEnumDetails<T>()
                {
                    Name = item.Name,
                    Index = Convert.ToInt32(value),
                    Description = enumType.GetField(item.Name)?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? defualtComment,
                    EnumData = (T)value
                };
                res.Add(entity);
            }
            return res;
        }

        /// <summary>
        /// 枚举详情列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class EasyEnumDetails<T> where T : Enum
        {
            /// <summary>
            /// 枚举索引
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// 枚举值（字符串）
            /// </summary>
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// 枚举值
            /// </summary>
            public Enum EnumData { get; set; }

            /// <summary>
            /// 枚举描述
            /// </summary>
            public string Description { get; set; } = string.Empty;
        }

    }
}
