using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinfromLib
{
    public static class ProgressBarExtentions
    {
        /// <summary>
        /// 渲染进度条
        /// </summary>
        /// <param name="progressBar1">你的进度条控件</param>
        /// <param name="index">第几个任务</param>
        /// <param name="Allcount">共几个任务(默认100)</param>
        /// <return>百分比</return>
        public static decimal SetCommon(this ProgressBar progressBar1, int index, int Allcount=100)
        {
            var rate = Convert.ToInt32((decimal)index / Allcount * 100);
            var result = Math.Round(Convert.ToDecimal((decimal)index / Allcount * 100),2);
            progressBar1.Value = rate;
            progressBar1.Maximum = 100;
            return result;
        }

    }
}
