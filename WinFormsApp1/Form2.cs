using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinfromLib;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
        }        

        private void button1_Click(object sender, EventArgs e)
        {
            // 指定类库路径
            string libraryPath = @"F:\C_program\Winform_demo\WinFormsApp1\WinfromLib\bin\Debug\net6.0-windows\WinformLib.dll"; // 替换为你的 DLL 路径

            // 加载类库
            Assembly assembly = Assembly.LoadFrom(libraryPath);

            // 获取所有类型
            Type[] types = assembly.GetTypes();

            StringBuilder sb = new StringBuilder();

            List<ReflectResult> result = new List<ReflectResult>();

            // 遍历所有类型
            foreach (Type type in types.OrderBy(x => x.Name))
            {
                Console.WriteLine($"Type: {type.FullName}");

                // 获取所有方法
                MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

                // 打印方法信息
                foreach (MethodInfo method in methods)
                {
                    // 获取方法参数信息
                    ParameterInfo[] parameters = method.GetParameters();
                    string parameterList = string.Join(", ", Array.ConvertAll(parameters, p => $"{p.ParameterType.Name} {p.Name}"));

                    Debug.WriteLine($"【结果】ClassName:{method.ReflectedType?.Name} Method Name: {method.Name}, Return Type: {method.ReturnType.Name}, Parameters: ({parameterList})");
                    result.Add(new ReflectResult()
                    {
                        ClassName = method.ReflectedType?.Name,
                        MethodName = method.Name
                    });
                }
            }

            var groupList = result.Where(x => !x.ClassName.Contains("<") && !x.ClassName.Contains("`")).GroupBy(x => x.ClassName);
            foreach (var item in groupList)
            {
                sb.AppendLine($"【{item.Key}】");
                int count = 1;
                foreach (var j in item.Select(x => x.MethodName))
                {
                    sb.AppendLine($"{count}.{j}:");
                    count++;
                }
            }
            richTextBox1.Text = sb.ToString();
        }

        public void SetText(object obj)
        {
            
        }
    }
}
