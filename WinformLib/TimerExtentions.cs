using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WinformLib
{
    public static class TimerExtentions
    {
        private static ConcurrentDictionary<string, System.Windows.Forms.Timer> timerDict = new ConcurrentDictionary<string, System.Windows.Forms.Timer>();

        /// <summary>
        /// 注册定时器（定时器名称、间隔触发时间 ms、方法、是否立即开始）
        /// </summary>
        public static void RegisterTimer(string TimerName, int interval,Action funs, bool isStartNow = false)
        {
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = interval;
            timer.Tick += (sender, e) => funs.Invoke(); 
            if (!timerDict.TryAdd(TimerName, timer))
            {
                throw new Exception("添加失败！Timer已存在，请确认Key的唯一性！");
            }
            if (isStartNow)
            {
                timer.Start();
            }
        }

        /// <summary>
        /// 启动定时器（定时器名称）
        /// </summary>
        public static void StartTimer(string TimerName)
        {
            if (!timerDict.TryGetValue(TimerName, out var timer))
            {
                throw new Exception("操作失败！找不到Timer，请确认Key的正确性！");
            }
            timer.Start();
        }

        /// <summary>
        /// 停止定时器（定时器名称）
        /// </summary>
        public static void StopTimer(string TimerName)
        {
            if (!timerDict.TryGetValue(TimerName, out var timer))
            {
                throw new Exception("操作失败！找不到Timer，请确认Key的正确性！");
            }
            timer.Stop();
        }

        /// <summary>
        /// 重启定时器（定时器名称）
        /// </summary>
        public static void ReStartTimer(string TimerName)
        {
            if (!timerDict.TryGetValue(TimerName, out var timer))
            {
                throw new Exception("操作失败！找不到Timer，请确认Key的正确性！");
            }
            timer.Stop();
            timer.Start();
        }



    }
}
