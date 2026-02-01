using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformLib
{
    public static class DateTimePickerExtentions
    {
        public static void SetCommon(this DateTimePicker dateTimePicker1, EnumEasyDateTimePicker type = EnumEasyDateTimePicker.DateAndTime)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            switch (type)
            {
                case EnumEasyDateTimePicker.Date:
                    dateTimePicker1.CustomFormat = "yyyy-MM-dd";
                    break;
                case EnumEasyDateTimePicker.Time:
                    dateTimePicker1.CustomFormat = "HH:mm:ss";
                    dateTimePicker1.ShowUpDown = true;
                    break;
                case EnumEasyDateTimePicker.DateAndTime:
                    dateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
                    break;
                default:
                    break;
            }
        }

        public static (DateTime date, DayOfWeek dayOfWeek) GetCommon(this DateTimePicker dateTimePicker)
        {
            // 获取 DateTimePicker 的值
            DateTime dateValue = dateTimePicker.Value;

            // 获取日期和星期
            DayOfWeek dayOfWeek = dateValue.DayOfWeek;

            // 返回元组
            return (dateValue, dayOfWeek);
        }

        /// <summary>
        /// 枚举-时间控件选项(0:日期,1:时间,2:日期和时间)
        /// </summary>
        [Description("时间控件选项")]
        public enum EnumEasyDateTimePicker
        {
            /// <summary>
            ///日期
            /// </summary>
            [Description("日期")]
            Date = 0,
            /// <summary>
            ///时间
            /// </summary>
            [Description("时间")]
            Time = 1,
            /// <summary>
            ///日期和时间
            /// </summary>
            [Description("日期和时间")]
            DateAndTime = 2,
        }

    }
}
