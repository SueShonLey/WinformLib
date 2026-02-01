# WinformLib

欢迎使用 **WinformLib**！我是作者 **SueShonley**，本Nuget包对许多常用控件进行了实用的扩展方法封装。期待您的反馈与建议！

您可以观看以下视频以获取更多信息：[观看视频](https://www.bilibili.com/video/BV1frq6BTEnM/?vd_source=686b87d4b7bcc024dd5ea31a4b332769)

您可以访问Github查看源代码：[跳转链接](https://github.com/SueShonLey/WinformLib)

您如果有疑问或建议，敬请联系：sueshonley@qq.com

## 扩展方法说明

### CheckedListBoxExtentions
- **SetCommon**: 设置数据源
- **SetCommonAll**: 设置全选和取消全选
- **GetCommonStatus**: 获取选中或未选中的数据

### ClipboardExtentions
- **ToClipboard**: 将字符串导出到剪切板
- **GetClipboard**: 从剪切板读取数据

### ComboBoxExtentions
- **SetCommon**: 设置下拉框内容
- **GetCommonSelect**: 获取当前选择索引/文字
- **SetCommonItems**: 根据文字锁定下拉框
- **SetCommonWithEntity**: 根据实体列表和指定字段渲染
- **GetCommonSelectWithEntity**: 根据选择的下拉框获取实体

### DataGridViewExtentions
- **SetCommon**: 设置表格内容
- **GetCommon**: 获取表格内容
- **GetCommonByButton**: 根据按钮上的文字获取实体
- **SetCommonWithUI**: 设置表格内容+UI（复杂情况）

### FileExtentions
- **PopUpFolder**: 文件夹选择
- **PopUpFile**: 文件选择
- **PopUpMutiFile**: 多文件选择
- **OpenFolder**: 打开指定的文件夹
- **OpenFile**: 打开文件
- **SplitFileName**: 劈分文件名

### FormExtentions
- **SetCommon**: 常见窗体设置
- **BindForm**: 绑定双向窗体传输
- **SendMessage**: 发送字符串到另一个窗体
- **HideForm**: 隐藏任务栏窗体，显示到右下角托盘
- **CheckNotNull**: 判断控件值都非空
- **IsRunningByAdmin**: 是否以管理员身份运行该Winform程序
- **SetMenuMDIForm**: MDI窗体设计
- **ShowOnlyOne**: 打开窗体（不重复）
- **SetGlobalErrorTips**: 全局报错不退出系统（仅限开发环境）


### FlowLayoutPanelExtentions
- **AddButtons**: 为流布局控件增加按钮

### GroupBoxExtentions
- **ClearAll**: 清空文本框、复选框、富文本框
- **SetAllEnable**: 设置控件的可用性

### PanelExtentions
- **SetCommonDefault**: 设置默认窗体（以便后续恢复）
- **SetCommonRecover**: 恢复默认窗体（必须先设置默认窗体才能恢复）
- **SetCommon**: 切换窗体

### ProgressBarExtentions
- **SetCommon**: 设置进度条

### RichTextBoxExtentions
- **SetCommonWithColors**: 富文本框颜色设定

### StatusStripExtensions
- **SetStatusStripCommon**: 设置 StatusStrip（下边框栏）的文本和进度
- **SetStatusStripTextAndRate**: 更新 StatusStrip 的文本和进度

### TaskExtentions
- **TaskRun**: 后台运行（同步）
- **TaskRunAsync**: 后台运行（异步）

### TipsForm
- **PopUpTips**: 提示弹窗
- **PopUpTipsRight**: 提示弹窗（右下角）
- **PopUpDialog**: 询问弹窗

### TimerExtentions
- **RegisterTimer**: 定时器注册（可传入方法）
- **StartTimer**: 定时器启动
- **StopTimer**: 定时器停止
- **ReStartTimer**: 定时器重启

### TableLayoutPanelExtentions
- **SetCommon**: 设置表格容器
- **GetCommon**: 获取表格容器

### CustomizeFormsExtentions
- **SetCustomizeForms**: 自定义窗体（传入控件及控件的内容）

### ObjExtentions
- **ToControl**: 尝试将object sender转化为控件

### DateTimePickerExtentions
- **SetCommon**: 渲染日期/时间/日期和时间效果
- **GetCommon**: 获取日期值和星期几


##### 感谢您的使用与支持！如有任何建议，请随时联系我。
---

# WinformLib

Welcome to **WinformLib**! I'm the author **SueShonley**. This NuGet package encapsulates practical extension methods for many commonly used controls. We look forward to your feedback and suggestions!

You can watch the following video to obtain more information: [Watch Video](https://www.bilibili.com/video/BV1frq6BTEnM/?vd_source=686b87d4b7bcc024dd5ea31a4b332769)

You can visit Github to view the source code: [Link](https://github.com/SueShonLey/WinformLib)

If you have any questions or suggestions, please contact: sueshonley@qq.com

## Description of extension methods

### CheckedListBoxExtentions
- **SetCommon**: Set data source
- **SetCommonAll**: Set to select all and cancel all
- **GetCommonStatus**: Get the selected or unselected data

### ClipboardExtentions
- **ToClipboard**: Export the string to the clipboard
- **GetClipboard**: Read data from the clipboard

### ComboBoxExtentions
- **SetCommon**: Set the content of the dropdown box
- **GetCommonSelect**: Get the current selected index/text
- **SetCommonItems**: Lock the dropdown box based on the text

### DataGridViewExtentions
- **SetCommon**: Set table content
- **GetCommon**: Get table content
- **GetCommonByButton**: Get entities based on the text on the button
- **SetCommonWithUI**: Set table content + UI (complex case)

### FileExtentions
- **PopUpFolder**: Folder selection
- **PopUpFile**: File selection
- **PopUpMutiFile**: Multi-file selection
- **OpenFolder**: Open the specified folder
- **OpenFile**: Open a file
- **SplitFileName**: Split File Name

### FormExtentions
- **SetCommon**: Common window settings
- **BindForm**: Bind two-way form transmission
- **SendMessage**: Send a string to another form
- **HideForm**: Hide the taskbar window and display it in the lower right corner tray
- **CheckNotNull**: Check if all control values are not null
- **IsRunningByAdmin**: Whether to run the Winform program as an administrator
- **SetMenuMDIForm**: MDI window design
- **ShowOnlyOne**: Open the window (without duplication)
- **SetGlobalErrorTips**: Display global error tips without exiting the system (only in development environment)


### FlowLayoutPanelExtentions
- **AddButtons**: Adds buttons to the flow layout control

### GroupBoxExtentions
- **ClearAll**: Clear the text box, check box, and rich text box
- **SetAllEnable**: Set the availability of controls

### PanelExtentions
- **SetCommonDefault**: Set the default window (for subsequent restoration)
- **SetCommonRecover**: Restore the default form (the default form must be set before it can be restored)
- **SetCommon**: Switch window

### ProgressBarExtentions
- **SetCommon**: Set the progress bar

### RichTextBoxExtentions
- **SetCommonWithColors**: Set rich text box colors

### StatusStripExtensions
- **SetStatusStripCommon**: Set the text and progress of the StatusStrip (lower border bar)
- **SetStatusStripTextAndRate**: Updates the text and progress of the StatusStrip

### TaskExtentions
- **TaskRun**: Background execution (synchronous)
- **TaskRunAsync**: Run in the background (asynchronously)

### TipsForm
- **PopUpTips**: Pop-up tips
- **PopUpTipsRight**: Pop-up tips (located in the bottom right corner)
- **PopUpDialog**: Pop-up dialog for inquiry

### TimerExtentions
- **RegisterTimer**: Timer registration (method can be passed in)
- **StartTimer**: Timer start
- **StopTimer**: Timer stop
- **ReStartTimer**: Timer restart

### TableLayoutPanelExtentions
- **SetCommon**: Set the table container
- **GetCommon**: Get the table container

### CustomizeFormsExtentions
- **SetCustomizeForms**: Customize forms (passing in controls and their contents)

### ObjExtentions
- **ToControl**: Attempt to convert the object sender into a control


##### Thank you for your use and support! If you have any suggestions, please feel free to contact me.
---

