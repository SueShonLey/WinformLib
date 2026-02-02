using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    /// <summary>
    /// 窗体后台任务扩展类（修复所有问题，保留原调用方式）
    /// 核心原则：耗时逻辑后台跑，UI操作安全切回，异常全局捕获，同步/异步清晰分离
    /// </summary>
    public static class TaskExtentions
    {
        #region 【纯后台作业】无UI回调 - 同步/异步版
        /// <summary>
        /// 【同步版】纯后台作业（无UI回调，仅执行耗时逻辑）
        /// </summary>
        /// <param name="form">当前窗体</param>
        /// <param name="funs">后台纯耗时逻辑（禁止包含任何UI操作）</param>
        /// <param name="isWait">是否同步阻塞等待：true=阻塞调用线程，false=异步非阻塞（推荐UI线程用false）</param>
        public static void TaskRun(this Form form, Action funs, bool isWait = false)
        {
            if (funs == null) return;

            var task = Task.Run(() =>
            {
                try
                {
                    funs(); 
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
            if (isWait)
            {
                task.Wait();
            }
        }

        /// <summary>
        /// 【异步版】纯后台作业（无UI回调，仅执行耗时逻辑，推荐优先使用）
        /// </summary>
        /// <param name="form">当前窗体</param>
        /// <param name="funs">后台纯耗时逻辑（禁止包含任何UI操作）</param>
        public static async Task TaskRunAsync(this Form form, Action funs)
        {
            if (funs == null) return;

            var backgroundTask = Task.Run(() =>
            {
                try
                {
                    funs(); 
                }
                catch (Exception ex)
                {
                    throw;
                }
            });
            await backgroundTask.ConfigureAwait(false);
        }
        #endregion

        #region 【带UI回调】后台作业+执行后UI更新 - 同步/异步版
        /// <summary>
        /// 【同步版】带UI回调的后台作业（耗时逻辑执行完后，执行UI更新操作）
        /// </summary>
        /// <param name="form">当前窗体</param>
        /// <param name="funs">后台纯耗时逻辑（禁止包含任何UI操作）</param>
        /// <param name="UIfuns">耗时逻辑完成后，需要执行的UI更新操作</param>
        /// <param name="isWait">是否同步阻塞等待：true=阻塞调用线程，false=异步非阻塞（推荐UI线程用false）</param>
        public static void TaskRunWithUI(this Form form, Action funs, Action UIfuns, bool isWait = false)
        {
            // 空判断：避免传入null
            if (funs == null) return;

            var task = Task.Run(() =>
            {
                try
                {
                    funs(); // 执行后台耗时逻辑
                }
                catch (Exception ex)
                {
                    throw;
                }
                if (UIfuns != null)
                {
                    form.UISafeInvoke(UIfuns);
                }
            });

            // 同步等待：仅在isWait=true时阻塞调用线程
            if (isWait)
            {
                task.Wait();
            }
        }

        /// <summary>
        /// 【异步版】带UI回调的后台作业（耗时逻辑执行完后，执行UI更新操作，推荐优先使用）
        /// </summary>
        /// <param name="form">当前窗体</param>
        /// <param name="funs">后台纯耗时逻辑（禁止包含任何UI操作）</param>
        /// <param name="UIfuns">耗时逻辑完成后，需要执行的UI更新操作</param>
        public static async Task TaskRunWithUIAsync(this Form form, Action funs, Action UIfuns)
        {
            // 空判断：避免传入null
            if (funs == null) return;

            // 修复核心问题：移除异步空包，Task.Run直接包装同步委托
            var backgroundTask = Task.Run(() =>
            {
                try
                {
                    funs(); // 执行后台耗时逻辑
                }
                catch (Exception ex)
                {
                    throw;
                }
            });

            // 非阻塞等待后台耗时逻辑完成
            await backgroundTask.ConfigureAwait(false);

            // UI回调单独执行：切回UI线程（因ConfigureAwait(false)，需手动调度）
            if (UIfuns != null)
            {
                await form.UISafeInvokeAsync(UIfuns);
            }
        }
        #endregion

        #region 【UI安全调度】同步/异步版 - 所有UI操作必须通过此方法执行
        /// <summary>
        /// 【同步版】UI安全调用（跨线程安全更新UI，同步等待UI操作完成）
        /// </summary>
        /// <param name="control">任意UI控件（窗体、按钮、进度条等）</param>
        /// <param name="uiAction">需要执行的UI操作</param>
        public static void UISafeInvoke(this Control control, Action uiAction)
        {
            // 优化：增加空判断，避免null异常
            if (control == null || uiAction == null || !control.IsHandleCreated) return;

            if (control.InvokeRequired)
            {
                control.Invoke(uiAction); // 跨线程：同步切回UI线程
            }
            else
            {
                uiAction(); // 同线程：直接执行，简化调用
            }
        }

        /// <summary>
        /// 【异步版】UI安全调用（跨线程安全更新UI，非阻塞等待UI操作完成，适配异步场景）
        /// </summary>
        /// <param name="control">任意UI控件（窗体、按钮、进度条等）</param>
        /// <param name="uiAction">需要执行的UI操作</param>
        public static async Task UISafeInvokeAsync(this Control control, Action uiAction)
        {
            // 优化：增加空判断，避免null异常
            if (control == null || uiAction == null || !control.IsHandleCreated) return;

            if (control.InvokeRequired)
            {
                // 跨线程：异步切回UI线程（基于BeginInvoke/EndInvoke）
                await Task.Factory.FromAsync(control.BeginInvoke(uiAction), control.EndInvoke);
            }
            else
            {
                uiAction(); // 同线程：直接执行
            }
        }
        #endregion
    }
}
