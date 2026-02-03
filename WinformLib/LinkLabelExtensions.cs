using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public static class LinkLabel1Extensions
    {
        /// <summary>
        /// 打开链接
        /// </summary>
        public static void OpenLink(this LinkLabel linkLabel, string url = null, EnumLinkType linkType = EnumLinkType.Website)
        {
            string openUrl = string.IsNullOrEmpty(url) ? linkLabel.Text : url;
            switch (linkType)
            {
                case EnumLinkType.Website:
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = openUrl,
                        UseShellExecute = true // 关键：启用ShellExecute才能调用默认浏览器
                    };
                    // 启动进程（打开浏览器）
                    Process.Start(psi);
                    break;
                case EnumLinkType.Document:
                    FileExtentions.OpenFile(openUrl);
                    break;
                case EnumLinkType.Folder:
                    FileExtentions.OpenFolder(openUrl);
                    break;
                default:
                    break;
            }
            linkLabel.LinkVisited = true;//设置为已访问
        }

        /// <summary>
        /// 枚举-链接输入类型枚举(0:网站,1:文件,2:文件夹)
        /// </summary>
        [Description("链接输入类型枚举")]
        public enum EnumLinkType
        {
            /// <summary>
            ///网站
            /// </summary>
            [Description("网站")]
            Website = 0,
            /// <summary>
            ///文件
            /// </summary>
            [Description("文件")]
            Document = 1,
            /// <summary>
            ///文件夹
            /// </summary>
            [Description("文件夹")]
            Folder = 2,
        }

    }
}
