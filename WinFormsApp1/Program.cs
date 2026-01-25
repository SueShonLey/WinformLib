using WinformLib;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            FormExtentions.SetGlobalErrorTips();

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}