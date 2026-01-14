using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class TaskExtentions
    {
        /// <summary>
        /// 后台作业（无返回值）
        /// </summary>
        public static void TaskRun(this Form form, Action funs,bool isWait = false)
        {
            Task.Run(() =>
            {
                if (isWait)
                {
                    form.Invoke(funs);//执行完再返回
                }
                else
                {
                    form.BeginInvoke(funs);//后台执行
                }
            });
        }        
        
        /// <summary>
        /// 后台作业（无返回值）
        /// </summary>
        public static async Task TaskRunAsync(this Form form, Action funs,bool isWait = false)
        {
            await Task.Run(() =>
            {
                if (isWait)
                {
                    form.Invoke(funs);//执行完再返回
                }
                else
                {
                    form.BeginInvoke(funs);//后台执行
                }
            });
        }        
        
    }
}
