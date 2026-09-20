[Languages](README.md)

# 隐私政策

最后更新：2026年9月20日

**Screenshot Annotator** by Siarhei Kuchuk

应用程序名称：Screenshot Annotator
开发者名称：Siarhei Kuchuk

本软件在此计算机上截取屏幕、供你添加注释，并可用 OCR 读取选定区域中的文字。它不创建云帐户。开发者不运营接收你的截图、项目或使用数据的服务器。

## 开发者不收集的数据

应用不含广告、分析、崩溃报告或跟踪 SDK。开发者不收集、出售或共享个人数据。

## 存储在你计算机上的数据

### 设置

应用程序设置（Print Screen 快捷键、随系统启动、OCR 引擎和语言、荧光笔颜色，以及在许可或隐私窗口中上次选择的语言）仅存储在此计算机上：

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### 项目和图像

带注释的项目和预览图存储在“图片”文件夹中：

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

你拍摄的截图、导入的图像以及注释中的文字保留在这些本地文件中（若复制则也在剪贴板中）。应用不会上传它们。

### 日志

诊断日志可能写入：

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

如果应用崩溃，可能在桌面上写入本地错误报告文件。该文件不会被发送到任何地方。

这些内容不会上传给开发者。

不使用开发者服务器存储你的数据。

## 屏幕捕获和 OCR

当你使用 Print Screen（或截图命令）时，应用捕获当前屏幕，以便裁剪并注释。捕获仅在此设备上进行。

OCR 在本地运行：

- **Windows OCR** 使用此电脑操作系统的 `Windows.Media.Ocr` API。
- **Tesseract** 在已安装且位于 PATH 中时，于此计算机上运行。

识别出的文字显示在应用中，供你复制或编辑。不会发送给开发者。

## 网络使用

### 更新检查

非 Store 版本可能会请求 GitHub 的最新发行版：

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub（Microsoft）会收到普通的 HTTPS 请求（IP 地址、user-agent、时间）。开发者不会收到该流量。

来自 Microsoft Store 的安装不使用此检查；更新由 Store 提供。

### 你打开的链接

应用可以在系统浏览器中打开这些页面。这些网站有各自的隐私政策：

- 项目主页：[github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- 最新发行版：[github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

许可证在应用内显示。不会作为网页打开。

## 其他本地行为

你可以让应用在 Windows 或 Linux 登录时启动。在 Windows 上使用启动项（Store 安装则使用 Microsoft Store 启动任务）。在 Linux 上使用自动启动项。这只会在你的计算机上启动本应用。

可选的全局 Print Screen 热键在应用运行期间保留在内存中，以便打开选择器。

## 儿童

本应用是截图注释工具。不以 13 岁以下儿童为对象。

## 第三方

GitHub 按上文处理更新检查和你打开的页面。Microsoft Store 处理 Store 安装和更新。Windows OCR 由操作系统提供。开发者不会收到该流量。

## 变更

对本政策的更新将发布在项目仓库的此文件中。

## 联系

应用程序名称：Screenshot Annotator
开发者名称：Siarhei Kuchuk

问题：[github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
