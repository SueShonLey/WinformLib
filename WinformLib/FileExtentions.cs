using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformLib
{
    public class FileExtentions
    {
        /// <summary>
        /// 弹出文件夹选择框
        /// </summary>
        /// <returns></returns>
        public static string PopUpFolder()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    // 用户选择了一个有效的文件夹路径
                    string path = dialog.SelectedPath;
                    return path;

                }
                return string.Empty;
            }
        }

        /// <summary>
        /// 弹出文件选择框（单选）
        /// </summary>
        /// <returns></returns>
        public static FileInfoEntity PopUpFile(string limit="")//例如"PDF Files (*.pdf)|*.pdf"
        {
            return PopUpMutiFile(limit,false).FirstOrDefault() ?? new FileInfoEntity();
        }

        /// <summary>
        /// 弹出文件选择框（多选）
        /// </summary>
        /// <returns></returns>
        public static List<FileInfoEntity> PopUpMutiFile(string limit="",bool isMultiSelect = true)
        {
            using (var dialog = new OpenFileDialog())
            {
                // 设置为允许多选 （如果是单选则注释掉下面的代码）
                dialog.Multiselect = isMultiSelect;

                if (!string.IsNullOrEmpty(limit))
                {
                    // 设置过滤器，只显示 .pdf 文件
                    dialog.Filter = limit;//例如"PDF Files (*.pdf)|*.pdf"
                }
                

                // 显示文件选择对话框
                DialogResult result = dialog.ShowDialog();

                // 如果用户选择了文件并点击了“确定”
                List<FileInfoEntity> data = new List<FileInfoEntity>();
                if (result == DialogResult.OK)
                {
                    // 获取用户选择的所有文件路径
                    string[] selectedFiles = dialog.FileNames;

                    // 遍历并处理每个选择的文件
                    foreach (var file in selectedFiles)
                    {
                        var fileDetail = SplitFileName(file);
                        data.Add(new FileInfoEntity
                        {
                            AllPath = file,
                            DirectoryName = fileDetail.DirectoryName,
                            FileExtension = fileDetail.FileExtension,
                            FileName = fileDetail.FileName,
                            FullFileName = fileDetail.FullFileName
                        });
                    }
                }
                return data;
            }
        }


        /// <summary>
        /// 输入文件夹路径，打开文件夹
        /// </summary>
        public static void OpenFolder(string FolderPath)
        {
            if (File.Exists(FolderPath))//假设这是一个文件
            {
                FolderPath = Path.GetDirectoryName(FolderPath);
            }
            if (Directory.Exists(FolderPath))
            {
                Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", FolderPath);
            }
            else
            {
                throw new FileNotFoundException("指定的文件夹路径不存在！");
            }
        }

        /// <summary>
        /// 输入文件路径，打开文件
        /// </summary>
        public static void OpenFile(string FilePath)
        {
            if (File.Exists(FilePath))
            {
                // 打开指定路径的 Excel 文件
                Process.Start(new ProcessStartInfo()
                {
                    FileName = FilePath,
                    UseShellExecute = true  // 启用 shell 执行以便打开 Excel 文件
                });
            }
            else
            {
                MessageBox.Show("指定的文件不存在！");
            }
        }

        /// <summary>
        /// 将文件的完整路径拆分为文件夹路径、文件全称（文件名+扩展名）、文件名、扩展名
        /// </summary>
        /// <returns></returns>
        public static  (string DirectoryName, string FullFileName, string FileName, string FileExtension) SplitFileName(string filepath)
        {
            try
            {
                string directoryName = string.Empty;
                string fullFileName = string.Empty;
                string fileName = string.Empty;
                string fileExtension = string.Empty;

                if (File.Exists(filepath))
                {
                    directoryName = Path.GetDirectoryName(filepath);
                    fullFileName = Path.GetFileName(filepath);
                    fileName = Path.GetFileNameWithoutExtension(filepath);
                    fileExtension = Path.GetExtension(filepath);
                }

                return (directoryName, fullFileName, fileName, fileExtension);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }

    public class FileInfoEntity
    {
        /// <summary>
        /// 文件夹
        /// </summary>
        public string DirectoryName { get; set; }
        /// <summary>
        /// 文件名（带后缀）
        /// </summary>
        public string FullFileName { get; set; }
        /// <summary>
        /// 文件名（不带后缀）
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExtension { get; set; }
        /// <summary>
        /// 完整的文件路径
        /// </summary>
        public string AllPath { get; set; }
    }

}
