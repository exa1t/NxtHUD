<p align="center">
  <img src="docs/promo/banner.png" alt="NxtHUD：为媒体键和按键带来 Fluent 风格弹出窗口，原生融入 Windows 11。" width="100%">
</p>

<p align="center">
  <a href="https://github.com/exa1t/NxtHUD/releases/latest"><img src="https://img.shields.io/github/v/release/exa1t/NxtHUD?style=flat-square&color=6D5BF6&label=release" alt="最新版本"></a>
  <a href="https://github.com/exa1t/NxtHUD/releases"><img src="https://img.shields.io/github/downloads/exa1t/NxtHUD/total?style=flat-square&color=4F7FE8&label=downloads" alt="下载量"></a>
  <img src="https://img.shields.io/badge/Windows-10%20%7C%2011-2E9BD6?style=flat-square&logo=windows11&logoColor=white" alt="Windows 10 和 11">
  <img src="https://img.shields.io/badge/.NET-8-1AA9C4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8">
  <a href="LICENSE"><img src="https://img.shields.io/github/license/exa1t/NxtHUD?style=flat-square&color=12B5C9" alt="MIT 许可证"></a>
</p>

<p align="center">
  <a href="README.md">English</a> · <a href="README.vi.md">Tiếng Việt</a> · <b>简体中文</b>
</p>

<p align="center">
  一款轻量级托盘应用，为媒体键、音量键和锁定键带来 <b>Fluent 风格弹出窗口</b>，<br>
  并在任务栏上提供带实时音频可视化的<b>正在播放小组件</b>。
</p>

<p align="center">
  <a href="https://github.com/exa1t/NxtHUD/releases/latest"><img src="https://img.shields.io/badge/%E4%B8%8B%E8%BD%BD%20Windows%20%E7%89%88-6D5BF6?style=for-the-badge&logo=windows11&logoColor=white" alt="下载 Windows 版"></a>
</p>

<p align="center">
  <img src="docs/promo/hero.png" alt="NxtHUD 的媒体弹窗、Up Next、音量条、任务栏小组件、任务栏性能信息和锁定键弹窗，背景为紫色与青色" width="100%">
</p>

## ✨ 亮点

<p align="center">
  <img src="docs/promo/media.png" alt="媒体弹窗：封面、曲名、艺人、播放器、进度条和播放控制" width="49%">
  <img src="docs/promo/widget.png" alt="任务栏小组件：封面、曲目和控制按钮，配有实时音频可视化" width="49%">
</p>
<p align="center">
  <img src="docs/promo/stats.png" alt="任务栏性能信息：托盘旁显示 CPU、内存、GPU、GPU 温度、网络和磁盘" width="49%">
  <img src="docs/promo/upnext.png" alt="Up Next 弹窗提示下一首" width="49%">
</p>
<p align="center">
  <img src="docs/promo/volume.png" alt="替代 Windows 自带音量浮层的音量弹窗" width="49%">
  <img src="docs/promo/lockkeys.png" alt="锁定键弹窗显示大写锁定已开启" width="49%">
</p>
<p align="center">
  <img src="docs/promo/settings.png" alt="外观设置：主题、背景材质、不透明度、弹窗位置、显示器、紧凑布局和动画速度" width="98%">
</p>

## 📥 下载与安装

1. 前往 [**Releases**](https://github.com/exa1t/NxtHUD/releases/latest) 下载
   `NxtHUD-Setup-<版本号>.exe`。
2. 运行它。无需管理员权限，也无需安装 .NET：所需的一切都已打包在内。
   由于安装程序没有代码签名，Windows 可能会提示 *"Windows 已保护你的电脑"*
   （*"Windows protected your PC"*），请点击 **更多信息 → 仍要运行**（**More info → Run anyway**）。
3. 保持勾选 *"Start NxtHUD automatically when I sign in"*（登录时自动启动），然后完成安装。
   NxtHUD 会启动并打开设置窗口；之后它会常驻在通知区域（可能在 `^` 箭头里）。

**之前用过 NxtFlyout？** NxtHUD 就是改了名字的同一款应用（从 1.3.0 起）。直接在 NxtFlyout 上运行
NxtHUD 安装程序即可：它会原地升级、删除旧的快捷方式，NxtHUD 首次启动时会迁移你的设置和开机启动选项。
旧的 `%AppData%\NxtFlyout` 文件夹会原样保留。

从 1.4.0 起，NxtHUD 会在有新版本时提醒你：弹出一条通知，并在托盘菜单中加入 *Download* 项，二者都会
打开发布页面。它会在启动时和每天检查一次 GitHub；可在 **Settings → About** 中关闭。

升级时运行更新的安装程序即可：它会关闭正在运行的程序并保留你的设置。卸载请前往
**设置 → 应用 → 已安装的应用 → NxtHUD → 卸载**。

不想用安装程序？下载 `NxtHUD-<版本号>-portable-x64.zip`，解压到任意位置后运行 `NxtHUD.exe`。

## 🎵 功能

- **媒体弹窗** – 按下媒体键（播放/暂停、下一首、上一首、停止）即可查看当前曲目：专辑封面、曲名、
  艺人、播放器名称、实时进度条，以及上一首 / 播放-暂停 / 下一首按钮。
- **音量弹窗**（可选）– 为音量键和静音键提供 Fluent 风格的音量指示。默认由 NxtHUD 自己处理这些按键，
  因此不会再同时弹出 Windows 自带的音量浮层。
- **锁定键弹窗** – 切换大写锁定、数字锁定或滚动锁定时显示新的状态。
- **Up Next 弹窗**（可选）– 歌曲自动切换时简短提示新的曲目。
- **任务栏小组件**（可选）– 任务栏上紧凑的正在播放小组件，包含封面、曲名、艺人以及上一首 /
  播放-暂停 / 下一首。点击即可打开媒体弹窗。
- **任务栏音频可视化** – 小组件旁随音频跳动的动画柱（WASAPI 环回采集）。10–20 根柱子，可从中间、
  从下往上或从上往下生长；颜色可跟随任务栏、使用系统强调色或自定义十六进制颜色。
- **任务栏性能信息**（可选）– 在通知图标旁的空白处显示 CPU、内存和 GPU 占用、网络上传/下载以及
  磁盘读/写，每秒刷新（或按你设定的频率）。显卡驱动支持时还可加入显存和 GPU 温度。点击数字可打开
  任务管理器。无需管理员权限或额外驱动。
- **每个弹窗单独设置位置** – 每个弹窗都有自己的开关和屏幕位置。
- **Mica / Acrylic** – 弹窗和设置窗口使用 Windows 11 的材质（选择"None"或在较旧的 Windows 上则使用
  跟随主题的纯色卡片）。
- **支持多显示器和 DPI** – 弹窗可显示在主显示器、鼠标所在显示器或活动窗口所在显示器上，并按每块
  显示器的缩放正确调整大小。
- **从不打扰** – 弹窗从不抢占键盘焦点，不出现在 Alt+Tab 中，鼠标悬停时保持显示，并可在全屏游戏时
  暂停。
- **托盘应用** – 单实例运行、Fluent 风格托盘菜单、可选开机启动，系统 / 浅色 / 深色主题实时跟随
  Windows 及其强调色。
- **更新提醒** – 有新版本时提醒一次，并带你前往下载页面。
- **支持本地化** – 所有文字都来自 resx 文件；目前先提供英文。

## 💻 系统要求

- Windows 10 2004 版本（版本号 19041）或更高。Mica/Acrylic 弹窗和圆角需要 **Windows 11 22H2 或更高**。
- 构建需要 [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) 或更新版本。

## 🛠️ 构建与运行

```powershell
dotnet build
dotnet test
dotnet run --project src/NxtHUD
```

首次构建会还原 [WPF-UI](https://github.com/lepoco/wpfui)、
[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) 和 xUnit（仅用于测试）。

运行 `dotnet run` 后，在通知区域找到 NxtHUD 图标（可能在 `^` 箭头里）。左键单击打开设置，右键单击
打开菜单。

## 📦 构建安装程序

安装程序是按用户安装的 [Inno Setup](https://jrsoftware.org/isinfo.php) 安装包：无需管理员权限，安装到
`%LocalAppData%\Programs\NxtHUD`，添加开始菜单项，可选择登录时启动应用，并注册卸载程序。其中的应用是
自包含的，因此目标电脑无需安装 .NET。

### 在 GitHub 上构建（推荐）

发布版本由 GitHub Actions 在 Windows 机器上构建：

```powershell
git tag v1.4.0
git push origin v1.4.0
```

**Release** 工作流（`.github/workflows/release.yml`）从标签中读取版本号，运行测试，构建安装程序和
便携版 zip，并连同校验和一起发布为 GitHub Release。使用 **Actions → Release → Run workflow** 可以
构建测试版本而不正式发布；生成的文件会附在该次工作流运行中。

发布说明由 `CHANGELOG.md` 中该版本的段落（放在 "What's new" 下）和 `installer/release-notes.md`
中的安装步骤组成，因此请在打标签前先写好更新日志。可用
`.\installer\release-notes.ps1 -Version 1.4.0 -OutFile notes.md` 预览。

**CI** 工作流会在每次推送到 `main` 时构建并测试。

### 在你的电脑上构建

1. 安装 Inno Setup 6.3 或更高版本（只需一次）：
   ```powershell
   winget install JRSoftware.InnoSetup
   ```
2. 如果 NxtHUD 正在运行请先退出，然后在解决方案文件夹中运行：
   ```powershell
   powershell -ExecutionPolicy Bypass -File .\installer\build-installer.ps1
   ```
3. 安装程序、便携版 zip 和 `SHA256SUMS.txt` 位于 `artifacts\installer`。

选项：`-Version 1.2.3` 覆盖 `Directory.Build.props` 中的版本号，`-FrameworkDependent` 生成体积小得多、
但需要 .NET 8 桌面运行时的版本，`-SkipTests` 跳过测试。

安装程序没有代码签名，因此在下载积累足够信誉之前，SmartScreen 会提示发布者未知。卸载后你的设置仍保留在
`%AppData%\NxtHUD`。

## ⚙️ 设置

设置以 JSON 格式保存在 `%AppData%\NxtHUD\settings.json`，每次修改都会立即写入（About → *Open folder*
可直接打开该位置）。文件可以手动编辑；无法解析的文件会被复制为 `settings.corrupt.json`，并使用默认值。

应用界面目前只有英文，因此下表保留页面名称原文。

| 页面 | 选项 |
| --- | --- |
| General | 开机启动、语言、全屏游戏时暂停 |
| Flyouts | 每个弹窗和任务栏小组件的开关；音量、锁定键和 Up Next 弹窗的位置；替代 Windows 音量浮层；任务栏小组件偏移；可视化开关、柱数、方向和颜色；自动隐藏延迟 |
| Appearance | 主题、背景材质、使用的显示器、媒体弹窗位置、紧凑布局、动画速度 |
| About | 版本、检查更新（开关、立即检查、下载）、设置文件夹、恢复默认 |

错误会记录到 `%AppData%\NxtHUD\logs\nxthud.log`。

## 🗂️ 项目结构

```
NxtHUD.sln
src/
  NxtHUD.Core/        net8.0 – 不依赖 WPF 或 Windows API，可完全单元测试
    Settings/            AppSettings、SettingsService（JSON）、容错的枚举转换器
    Media/               MediaSnapshot、IMediaService、媒体键映射、文本格式化
    Audio/               IVolumeService
    Input/               音量键和锁定键映射
    Flyouts/             FlyoutPlacement、FlyoutCoordinator
    Localization/        Localizer（resx 查找，支持实时切换语言）
    ViewModels/          FlyoutViewModelBase，以及每个弹窗、小组件和设置各自的视图模型
    Resources/           Strings.resx
  NxtHUD/             net8.0-windows – WPF 应用
    App.xaml(.cs)        启动、单实例、退出、崩溃日志
    AppHost.cs           组合根：创建并连接各项服务
    Services/            SMTC 媒体、Core Audio 音量、键盘钩子、托盘、主题、开机启动、
                         弹窗呈现、任务栏小组件
    Interop/             Win32 声明、显示器/DPI 查询
    Views/               FlyoutWindowBase、各弹窗窗口、任务栏小组件、设置窗口
    Localization/        {loc:Loc Key} 标记扩展
    Assets/              应用图标和标志
tests/
  NxtHUD.Tests/       覆盖 Core 全部内容的 xUnit 测试
installer/               Inno Setup 脚本、构建脚本、发布说明模板
.github/workflows/       CI（构建 + 测试）和 Release（安装程序 → GitHub Release）
tools/icon/              SVG 源文件和生成 .ico 的脚本
docs/screenshots/        用于制作 README 卡片的应用截图
docs/promo/              README 横幅和卡片：编辑 cards.html，运行 render.ps1
```

## 🔍 工作原理

- **媒体** – `SmtcMediaService` 通过 `Windows.Media.Control`（SMTC）跟踪系统当前的媒体会话，并在每次
  变化时发布一个不可变的 `MediaSnapshot`。
- **按键** – `KeyboardHookService` 只在需要它的弹窗启用时安装一个低级键盘钩子（`WH_KEYBOARD_LL`）。
  媒体键和锁定键只是被观察并照常传递。开启 "Replace the Windows volume overlay" 时音量键会被拦截，由
  应用通过 Core Audio API 自行调节音量；如果调节失败，按键会交还给 Windows。
- **视图模型** – 所有弹窗逻辑（何时显示、自动隐藏、悬停、全屏规则、一次一个弹窗）都在
  `NxtHUD.Core` 中，无需界面即可单元测试。
- **窗口** – `FlyoutWindowBase` 将每个弹窗创建为置顶的 `WS_EX_NOACTIVATE` 工具窗口，按物理像素放在
  所选显示器上，并向 Windows 申请 Mica/Acrylic 背景，或自行绘制圆角纯色卡片。
- **任务栏小组件** – 一个归任务栏窗口所有的小窗口，由慢速定时器重新定位。`TaskbarButtonScanner`
  在工作线程上用 UI Automation 找到任务栏按钮的位置，`TaskbarWidgetFit` 选出空白区域以及能放下的最
  完整布局。
- **任务栏性能信息** – `SystemStatsSource` 在工作线程上读取 CPU（`GetSystemTimes`）、内存
  （`GlobalMemoryStatusEx`）、GPU、显存和磁盘（任务管理器使用的性能计数器，通过 PDH）、网络（网卡字节
  计数）和 GPU 温度（`D3DKMTQueryAdapterInfo`）。`TaskbarStatsService` 让面板紧贴托盘，并避开小组件。

## 🌍 添加语言

1. 将 `src/NxtHUD.Core/Resources/Strings.resx` 复制为 `Strings.<culture>.resx`（例如
   `Strings.zh-CN.resx`），然后翻译其中的值。
2. 在 `Localizer.SupportedLanguages` 中加入该区域名称。

之后语言选择器就会出现这一语言，切换后无需重启即可生效。

## ⚠️ 已知限制

- **不提供 SMTC 数据的播放器。** 媒体信息来自系统媒体传输控件（SMTC）。不向 SMTC 发布信息的播放器
  无法显示或控制，有些播放器只发布部分信息（没有专辑封面、没有时间轴，或者只在播放/暂停/跳转时更新
  进度，这时进度条只是估算值）。
- **"Up Next" 在切歌时显示，而不是提前显示。** Windows 不提供播放器的播放队列，因此只有下一首开始播放
  时才能知道它是什么。
- **暂不支持拖动进度、随机/循环、切换会话或按应用调节音量。** 弹窗跟随 Windows 认定的当前会话，进度
  条仅用于显示。
- **不经过键盘的音量变化。** 替代 Windows 音量浮层是通过处理音量键实现的。以其他方式改变音量（不发送
  按键的硬件旋钮、任务栏音量滑块、其他应用）时，仍会显示 Windows 自带的浮层。
- **以管理员身份运行的窗口。** 当管理员窗口拥有焦点时，除非 NxtHUD 也以管理员身份运行，否则低级键盘
  钩子收不到按键，此时弹窗不会出现；如果开启了浮层替代，则会显示 Windows 自带的浮层。
- **任务栏小组件。** Windows 没有向任务栏添加内容的 API。小组件是一个独立窗口，覆盖在水平的主任务栏
  上。它通过 UI Automation 读取任务栏按钮的位置，只使用按钮之间的空白：图标居中时在最左端，图标靠左
  时在最后一个图标之后。空间不足时，它会先缩短曲名，再隐藏可视化，再隐藏文字；如果连封面和按钮都放
  不下就会自动隐藏。它每 1.5 秒检查一次，因此刚打开应用的瞬间图标可能会与它重叠。任务栏为竖向、自动
  隐藏或有全屏应用时，小组件会隐藏。
- **任务栏性能信息。** 和小组件一样，性能信息是托盘旁的独立窗口，而不是时钟的一部分；空间不足时会先
  缩成一行，再隐藏。GPU 温度需要较新的显卡驱动（WDDM 2.5 或更高）；不显示 CPU 和磁盘温度，因为读取
  它们需要管理员权限或内核驱动。网络数据只统计物理网卡，因此 VPN 流量不会被重复计算。
- **背景材质。** Mica/Acrylic 弹窗需要 Windows 11 22H2 或更高，而 Windows 在节能模式下会关闭它们
  （这时可改用 Translucent：半透明但没有模糊）。在旧版本上弹窗使用纯色卡片。使用系统背景时，内容会
  淡入淡出，但材质本身会直接出现和消失，没有渐变。
- **全屏检测** 依赖 Windows 的通知状态，它也会把按 F11 全屏的浏览器视为忙碌。关闭
  "Pause in fullscreen games" 即可在这种情况下看到弹窗。
- **开机启动** 使用当前用户注册表中的 Run 键。
- **应用界面目前只有英文**；支持其他语言的结构已经就绪。
- **音频可视化。** 它监听开始播放时的默认播放设备；切换输出设备后，可能会保持静止，直到下一次暂停/
  播放。它会响应所有系统声音，而不仅仅是媒体播放器。

## 🙌 致谢

NxtHUD 的代码、图标和品牌均为原创。它基于
[WPF-UI](https://github.com/lepoco/wpfui)（MIT）、
[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)（MIT）和
[NAudio](https://github.com/naudio/NAudio)（MIT）构建。它们的版权和许可声明位于
[THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt)，该文件也会安装在 `NxtHUD.exe` 旁边。

## 📄 许可证

NxtHUD 基于 [MIT 许可证](LICENSE) 发布。
