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

        /// <summary>
        /// 重设定时器间隔（定时器名称、新间隔时间 ms）
        /// </summary>
        /// <param name="TimerName">定时器名称</param>
        /// <param name="newInterval">新的间隔时间（毫秒），必须大于0</param>
        /// <param name="restartAfterSet">是否修改后立即重启（默认true，立即按新间隔重新计时；若为false，新间隔生效前会先走完当前未完成的计时周期）</param>
        public static void ResetInterval(string TimerName, int newInterval, bool restartAfterSet = true)
        {
            // 1. 校验新间隔的合法性
            if (newInterval <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(newInterval), "定时器新间隔必须大于0毫秒！");
            }

            // 2. 获取指定名称的定时器
            if (!timerDict.TryGetValue(TimerName, out var timer))
            {
                throw new Exception("操作失败！找不到Timer，请确认Key的正确性！");
            }

            // 3. 修改间隔（若定时器正在运行，重启后新间隔才会生效）
            bool wasRunning = timer.Enabled; // 记录修改前定时器的运行状态
            timer.Interval = newInterval;

            // 4. 若需要立即生效，或原定时器正在运行，则重启定时器
            if (restartAfterSet && wasRunning)
            {
                timer.Stop();
                timer.Start();
            }
        }

        /// <summary>
        /// 查看指定名称定时器的运行状态
        /// </summary>
        /// <param name="TimerName">定时器名称（不能为空/空字符串）</param>
        /// <returns>返回TimerStatus枚举，明确表示定时器的状态</returns>
        public static LibTimerStatus GetStatus(string TimerName)
        {
            if (string.IsNullOrWhiteSpace(TimerName))
            {
                throw new ArgumentNullException(nameof(TimerName), "定时器名称不能为空或空字符串！");
            }

            if (!timerDict.TryGetValue(TimerName, out var timer))
            {
                return LibTimerStatus.NotExist;
            }

            return timer.Enabled ? LibTimerStatus.Running : LibTimerStatus.Stopped;
        }

        /// <summary>
        /// 定时器状态枚举
        /// </summary>
        public enum LibTimerStatus
        {
            /// <summary>定时器不存在</summary>
            NotExist,
            /// <summary>定时器已停止</summary>
            Stopped,
            /// <summary>定时器运行中</summary>
            Running
        }
    }
}
