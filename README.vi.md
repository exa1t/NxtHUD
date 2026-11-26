<p align="center">
  <img src="docs/promo/banner.png" alt="NxtHUD: flyout phong cách Fluent cho phím media và phím tắt, ngay trên Windows 11." width="100%">
</p>

<p align="center">
  <a href="https://github.com/exa1t/NxtHUD/releases/latest"><img src="https://img.shields.io/github/v/release/exa1t/NxtHUD?style=flat-square&color=6D5BF6&label=release" alt="Phiên bản mới nhất"></a>
  <a href="https://github.com/exa1t/NxtHUD/releases"><img src="https://img.shields.io/github/downloads/exa1t/NxtHUD/total?style=flat-square&color=4F7FE8&label=downloads" alt="Lượt tải"></a>
  <img src="https://img.shields.io/badge/Windows-10%20%7C%2011-2E9BD6?style=flat-square&logo=windows11&logoColor=white" alt="Windows 10 và 11">
  <img src="https://img.shields.io/badge/.NET-8-1AA9C4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8">
  <a href="LICENSE"><img src="https://img.shields.io/github/license/exa1t/NxtHUD?style=flat-square&color=12B5C9" alt="Giấy phép MIT"></a>
</p>

<p align="center">
  <a href="README.md">English</a> · <b>Tiếng Việt</b> · <a href="README.zh-CN.md">简体中文</a>
</p>

<p align="center">
  Ứng dụng khay hệ thống nhẹ, mang <b>flyout phong cách Fluent</b> đến phím media, âm lượng và các phím khóa,<br>
  kèm <b>widget đang phát</b> với hiệu ứng sóng nhạc trực tiếp ngay trên thanh tác vụ.
</p>

<p align="center">
  <a href="https://github.com/exa1t/NxtHUD/releases/latest"><img src="https://img.shields.io/badge/T%E1%BA%A3i%20cho%20Windows-6D5BF6?style=for-the-badge&logo=windows11&logoColor=white" alt="Tải cho Windows"></a>
</p>

<p align="center">
  <img src="docs/promo/hero.png" alt="Flyout media, Up Next, thanh âm lượng, widget và thông số trên thanh tác vụ cùng flyout phím khóa của NxtHUD trên nền tím và xanh ngọc" width="100%">
</p>

## ✨ Điểm nổi bật

<p align="center">
  <img src="docs/promo/media.png" alt="Flyout media: ảnh bìa, tên bài, nghệ sĩ, trình phát, thanh tiến trình và nút điều khiển" width="49%">
  <img src="docs/promo/widget.png" alt="Widget trên thanh tác vụ: ảnh bìa, bài hát và nút điều khiển kèm hiệu ứng sóng nhạc" width="49%">
</p>
<p align="center">
  <img src="docs/promo/stats.png" alt="Thông số trên thanh tác vụ: CPU, RAM, GPU, nhiệt độ GPU, mạng và ổ đĩa cạnh khay hệ thống" width="49%">
  <img src="docs/promo/upnext.png" alt="Flyout Up Next báo bài tiếp theo" width="49%">
</p>
<p align="center">
  <img src="docs/promo/volume.png" alt="Flyout âm lượng thay cho lớp phủ của Windows" width="49%">
  <img src="docs/promo/lockkeys.png" alt="Flyout phím khóa cho thấy Caps Lock đang bật" width="49%">
</p>
<p align="center">
  <img src="docs/promo/settings.png" alt="Cài đặt giao diện: chủ đề, chất liệu nền, độ trong suốt, vị trí flyout, màn hình, bố cục gọn và tốc độ hiệu ứng" width="98%">
</p>

## 📥 Tải về và cài đặt

1. Vào [**Releases**](https://github.com/exa1t/NxtHUD/releases/latest) và tải
   `NxtHUD-Setup-<phiên bản>.exe`.
2. Chạy tệp đó. Không cần quyền quản trị và không cần cài .NET: mọi thứ cần thiết đã có sẵn bên trong.
   Windows có thể báo *"Windows đã bảo vệ PC của bạn"* (*"Windows protected your PC"*) vì trình
   cài đặt chưa được ký số; hãy bấm **Thông tin thêm → Vẫn chạy** (**More info → Run anyway**).
3. Giữ ô *"Start NxtHUD automatically when I sign in"* được đánh dấu rồi hoàn tất. NxtHUD sẽ khởi
   động và mở cửa sổ cài đặt; sau đó ứng dụng nằm ở khu vực thông báo (có thể ẩn sau mũi tên `^`).

**Từng dùng NxtFlyout?** NxtHUD chính là ứng dụng đó với tên mới (từ bản 1.3.0). Chạy trình cài đặt
NxtHUD đè lên bản NxtFlyout: nó nâng cấp tại chỗ, xóa các lối tắt cũ, và ở lần khởi động đầu tiên
NxtHUD sẽ chuyển cài đặt cùng lựa chọn khởi động cùng Windows của bạn sang. Thư mục cũ
`%AppData%\NxtFlyout` được giữ nguyên.

Từ bản 1.4.0, NxtHUD báo cho bạn khi có phiên bản mới: một thông báo và mục *Download* trong menu
khay, cả hai đều mở trang phát hành. Ứng dụng kiểm tra GitHub khi khởi động và mỗi ngày một lần; bạn
có thể tắt trong **Settings → About**.

Để nâng cấp, hãy chạy trình cài đặt mới hơn: nó tự đóng bản đang chạy và giữ nguyên cài đặt của bạn.
Để gỡ, vào **Cài đặt → Ứng dụng → Ứng dụng đã cài đặt → NxtHUD → Gỡ cài đặt**.

Không muốn dùng trình cài đặt? Tải `NxtHUD-<phiên bản>-portable-x64.zip`, giải nén ở đâu cũng được
rồi chạy `NxtHUD.exe`.

## 🎵 Tính năng

- **Flyout media** – nhấn phím media (phát/tạm dừng, bài tiếp, bài trước, dừng) để xem bài đang
  phát: ảnh bìa, tên bài, nghệ sĩ, tên trình phát, thanh tiến trình trực tiếp và các nút bài trước /
  phát-tạm dừng / bài tiếp.
- **Flyout âm lượng** (tùy chọn) – chỉ báo âm lượng phong cách Fluent cho các phím âm lượng và tắt
  tiếng. Mặc định NxtHUD tự xử lý các phím này, nên lớp phủ âm lượng có sẵn của Windows không hiện
  thêm nữa.
- **Flyout phím khóa** – hiện trạng thái mới khi bạn bật/tắt Caps Lock, Num Lock hoặc Scroll Lock.
- **Flyout Up Next** (tùy chọn) – thông báo ngắn bài mới khi bài hát tự chuyển.
- **Widget trên thanh tác vụ** (tùy chọn) – widget đang phát gọn gàng trên thanh tác vụ với ảnh
  bìa, tên bài, nghệ sĩ và các nút bài trước / phát-tạm dừng / bài tiếp. Bấm vào để mở flyout media.
- **Hiệu ứng sóng nhạc trên thanh tác vụ** – các cột chuyển động cạnh widget, nhảy theo âm thanh
  (WASAPI loopback). 10–20 cột mọc từ giữa, từ dưới lên hoặc từ trên xuống; màu có thể theo thanh tác
  vụ, theo màu nhấn của Windows hoặc một mã hex tùy chọn.
- **Thông số trên thanh tác vụ** (tùy chọn) – mức dùng CPU, bộ nhớ và GPU, mạng lên/xuống và đọc/ghi
  ổ đĩa, đặt trong khoảng trống cạnh các biểu tượng thông báo, cập nhật mỗi giây (hoặc theo nhịp bạn
  chọn). Có thể thêm bộ nhớ GPU và nhiệt độ GPU nếu driver đồ họa hỗ trợ. Bấm vào các con số để mở
  Task Manager. Không cần quyền quản trị hay driver riêng.
- **Vị trí riêng cho từng flyout** – mỗi flyout có công tắc bật/tắt và vị trí trên màn hình riêng.
- **Mica / Acrylic** – flyout và cửa sổ cài đặt dùng chất liệu của Windows 11 (hoặc một thẻ nền đặc
  theo chủ đề khi bạn chọn "None" hoặc dùng Windows cũ hơn).
- **Hỗ trợ nhiều màn hình và DPI** – flyout hiện trên màn hình chính, màn hình có con trỏ hoặc màn
  hình có cửa sổ đang hoạt động, với kích thước đúng theo tỉ lệ của từng màn hình.
- **Không bao giờ cản trở** – flyout không bao giờ lấy tiêu điểm bàn phím, không hiện trong Alt+Tab,
  vẫn mở khi con trỏ nằm trên nó và có thể tạm dừng khi đang chơi game toàn màn hình.
- **Ứng dụng khay** – chỉ chạy một bản, menu khay phong cách Fluent, tùy chọn khởi động cùng Windows,
  chủ đề Hệ thống / Sáng / Tối đi theo Windows và màu nhấn của nó theo thời gian thực.
- **Thông báo cập nhật** – báo một lần khi có bản phát hành mới và đưa bạn tới trang tải về.
- **Tiếng Anh và tiếng Việt** – chọn ngôn ngữ trong Cài đặt → Chung, hoặc để tự theo Windows. Mọi
  chuỗi chữ đều lấy từ tệp resx, nên thêm ngôn ngữ khác rất dễ.

## 💻 Yêu cầu

- Windows 10 phiên bản 2004 (build 19041) trở lên. Cần **Windows 11 22H2 trở lên** cho flyout
  Mica/Acrylic và góc bo tròn.
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) hoặc mới hơn để build.

## 🛠️ Build và chạy

```powershell
dotnet build
dotnet test
dotnet run --project src/NxtHUD
```

Lần build đầu tiên sẽ tải [WPF-UI](https://github.com/lepoco/wpfui),
[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) và xUnit (chỉ cho kiểm thử).

Sau `dotnet run`, hãy tìm biểu tượng NxtHUD ở khu vực thông báo (có thể nằm sau mũi tên `^`). Bấm
chuột trái để mở cài đặt, chuột phải để mở menu.

## 📦 Build trình cài đặt

Trình cài đặt là gói [Inno Setup](https://jrsoftware.org/isinfo.php) cho từng người dùng: không cần
quyền quản trị, cài vào `%LocalAppData%\Programs\NxtHUD`, thêm mục vào menu Start, có thể khởi động
ứng dụng khi đăng nhập và đăng ký trình gỡ cài đặt. Ứng dụng bên trong là bản tự chứa, nên máy đích
không cần .NET.

### Trên GitHub (khuyên dùng)

Các bản phát hành được GitHub Actions build trên máy Windows:

```powershell
git tag v1.4.0
git push origin v1.4.0
```

Workflow **Release** (`.github/workflows/release.yml`) lấy số phiên bản từ tag, chạy kiểm thử, build
trình cài đặt và tệp zip portable, rồi đăng chúng kèm mã băm dưới dạng một GitHub Release. Dùng
**Actions → Release → Run workflow** để build bản thử mà không phát hành; các tệp được đính kèm vào
lần chạy workflow đó.

Ghi chú phát hành là phần của phiên bản đó trong `CHANGELOG.md` (dưới "What's new"), tiếp theo là
các bước cài đặt trong `installer/release-notes.md`, nên hãy thêm mục changelog trước khi gắn tag. Xem
trước bằng `.\installer\release-notes.ps1 -Version 1.4.0 -OutFile notes.md`.

Workflow **CI** build và chạy kiểm thử cho mỗi lần push lên `main`.

### Trên máy của bạn

1. Cài Inno Setup 6.3 trở lên, chỉ một lần:
   ```powershell
   winget install JRSoftware.InnoSetup
   ```
2. Thoát NxtHUD nếu đang chạy, rồi chạy từ thư mục solution:
   ```powershell
   powershell -ExecutionPolicy Bypass -File .\installer\build-installer.ps1
   ```
3. Trình cài đặt, tệp zip portable và `SHA256SUMS.txt` nằm trong `artifacts\installer`.

Tùy chọn: `-Version 1.2.3` để ghi đè phiên bản trong `Directory.Build.props`, `-FrameworkDependent`
cho bản build nhỏ hơn nhiều nhưng cần .NET 8 Desktop Runtime, `-SkipTests` để bỏ qua kiểm thử.

Trình cài đặt chưa được ký số, nên SmartScreen sẽ cảnh báo nhà phát hành không xác định cho tới khi
tệp tải về có đủ uy tín. Gỡ cài đặt vẫn giữ lại cài đặt của bạn trong `%AppData%\NxtHUD`.

## ⚙️ Cài đặt

Cài đặt được lưu dưới dạng JSON trong `%AppData%\NxtHUD\settings.json` và được ghi lại mỗi khi bạn
thay đổi gì đó (About → *Open folder* sẽ đưa bạn tới đó). Tệp chấp nhận chỉnh sửa tay; tệp không đọc
được sẽ được sao lưu thành `settings.corrupt.json` và giá trị mặc định được dùng.

Bảng dưới đây dùng tên trang trong giao diện tiếng Anh; ở giao diện tiếng Việt, chúng lần lượt là
Chung, Widget, Giao diện và Giới thiệu.

| Trang | Tùy chọn |
| --- | --- |
| General | Khởi động cùng Windows, ngôn ngữ, tạm dừng khi chơi game toàn màn hình |
| Flyouts | Bật/tắt từng flyout và widget trên thanh tác vụ; vị trí flyout âm lượng, phím khóa và Up Next; thay lớp phủ âm lượng của Windows; độ lệch của widget; bật/tắt hiệu ứng sóng nhạc, số cột, hướng và màu; thời gian tự ẩn |
| Appearance | Chủ đề, chất liệu nền, màn hình sử dụng, vị trí flyout media, bố cục gọn, tốc độ hiệu ứng |
| About | Phiên bản, kiểm tra cập nhật (bật/tắt, kiểm tra ngay, tải về), thư mục cài đặt, khôi phục mặc định |

Lỗi được ghi vào `%AppData%\NxtHUD\logs\nxthud.log`.

## 🗂️ Cấu trúc dự án

```
NxtHUD.sln
src/
  NxtHUD.Core/        net8.0 – không dùng WPF hay API Windows, kiểm thử đơn vị được hoàn toàn
    Settings/            AppSettings, SettingsService (JSON), bộ chuyển enum chịu lỗi
    Media/               MediaSnapshot, IMediaService, bảng phím media, định dạng chữ
    Audio/               IVolumeService
    Input/               bảng phím âm lượng và phím khóa
    Flyouts/             FlyoutPlacement, FlyoutCoordinator
    Localization/        Localizer (tra cứu resx, đổi ngôn ngữ trực tiếp)
    ViewModels/          FlyoutViewModelBase và một view model cho mỗi flyout, widget và cài đặt
    Resources/           Strings.resx
  NxtHUD/             net8.0-windows – ứng dụng WPF
    App.xaml(.cs)        khởi động, một bản duy nhất, tắt, ghi log khi lỗi
    AppHost.cs           gốc kết hợp: tạo và nối các service
    Services/            media SMTC, âm lượng Core Audio, hook bàn phím, khay, chủ đề, khởi động,
                         trình hiển thị flyout, widget trên thanh tác vụ
    Interop/             khai báo Win32, tra cứu màn hình/DPI
    Views/               FlyoutWindowBase, các cửa sổ flyout, widget, cửa sổ cài đặt
    Localization/        markup extension {loc:Loc Key}
    Assets/              biểu tượng và logo ứng dụng
tests/
  NxtHUD.Tests/       kiểm thử xUnit cho toàn bộ Core
installer/               script Inno Setup, script build, mẫu ghi chú phát hành
.github/workflows/       CI (build + kiểm thử) và Release (trình cài đặt → GitHub Release)
tools/icon/              nguồn SVG và script tạo tệp .ico
docs/screenshots/        ảnh chụp ứng dụng dùng để tạo các thẻ trong README
docs/promo/              banner và thẻ của README: sửa cards.html, chạy render.ps1
```

## 🔍 Cách hoạt động

- **Media** – `SmtcMediaService` theo dõi phiên media hiện tại của hệ thống qua
  `Windows.Media.Control` (SMTC) và phát ra một `MediaSnapshot` bất biến mỗi khi có thay đổi.
- **Phím** – `KeyboardHookService` cài một hook bàn phím cấp thấp (`WH_KEYBOARD_LL`), chỉ khi có
  flyout cần đến nó đang bật. Phím media và phím khóa được theo dõi rồi chuyển tiếp. Phím âm lượng bị
  chặn lại khi bật "Replace the Windows volume overlay", và ứng dụng tự đổi âm lượng qua Core Audio
  API; nếu việc đó thất bại, các phím được trả lại cho Windows.
- **View model** – toàn bộ logic flyout (khi nào hiện, tự ẩn, rê chuột, quy tắc toàn màn hình, mỗi
  lần một flyout) nằm trong `NxtHUD.Core` và được kiểm thử đơn vị mà không cần giao diện.
- **Cửa sổ** – `FlyoutWindowBase` tạo mỗi flyout là một cửa sổ công cụ `WS_EX_NOACTIVATE` luôn ở
  trên cùng, đặt nó theo điểm ảnh vật lý trên màn hình đã chọn, và hoặc yêu cầu Windows cấp nền
  Mica/Acrylic, hoặc tự vẽ một thẻ bo góc nền đặc.
- **Widget trên thanh tác vụ** – một cửa sổ nhỏ thuộc sở hữu của cửa sổ thanh tác vụ, được đặt lại vị
  trí theo một bộ hẹn giờ chậm. `TaskbarButtonScanner` tìm các nút của thanh tác vụ bằng UI Automation
  trên luồng phụ, và `TaskbarWidgetFit` chọn khoảng trống cùng bố cục đầy đủ nhất vừa với nó.
- **Thông số trên thanh tác vụ** – `SystemStatsSource` đọc CPU (`GetSystemTimes`), bộ nhớ
  (`GlobalMemoryStatusEx`), GPU, bộ nhớ GPU và ổ đĩa (các bộ đếm hiệu năng mà Task Manager dùng, qua
  PDH), mạng (số byte của card mạng) và nhiệt độ GPU (`D3DKMTQueryAdapterInfo`) trên luồng phụ.
  `TaskbarStatsService` giữ bảng sát khay hệ thống và tránh chồng lên widget.

## 🌍 Thêm ngôn ngữ

1. Sao chép `src/NxtHUD.Core/Resources/Strings.resx` thành `Strings.<culture>.resx` (ví dụ
   `Strings.fr.resx`) rồi dịch các giá trị. `Strings.vi.resx` là một ví dụ đã hoàn chỉnh.
2. Thêm tên culture vào `Localizer.SupportedLanguages`.

Khi đó bộ chọn ngôn ngữ sẽ có ngôn ngữ mới, và việc chuyển đổi có hiệu lực ngay không cần khởi động
lại.

## ⚠️ Hạn chế đã biết

- **Trình phát không có dữ liệu SMTC.** Thông tin media lấy từ System Media Transport Controls. Trình
  phát không gửi dữ liệu lên SMTC thì không thể hiển thị hay điều khiển, và một số chỉ gửi một phần
  (không có ảnh bìa, không có dòng thời gian, hoặc vị trí chỉ cập nhật khi phát/tạm dừng/tua, khi đó
  thanh tiến trình chỉ là ước lượng).
- **"Up Next" hiện lúc chuyển bài, không phải trước đó.** Windows không cho biết hàng chờ của trình
  phát, nên bài tiếp theo chỉ được biết khi nó bắt đầu.
- **Chưa có tua, trộn bài/lặp lại, chuyển phiên phát hay bộ trộn âm lượng theo ứng dụng.** Flyout đi
  theo phiên mà Windows coi là hiện tại, và thanh tiến trình chỉ để hiển thị.
- **Âm lượng thay đổi không qua bàn phím.** Việc thay lớp phủ âm lượng của Windows hoạt động bằng cách
  xử lý các phím âm lượng. Âm lượng đổi theo cách khác (núm vặn phần cứng không gửi phím, thanh trượt
  trên thanh tác vụ, ứng dụng khác) vẫn dùng lớp phủ của Windows.
- **Cửa sổ chạy quyền quản trị.** Hook bàn phím cấp thấp không nhận được phím khi một cửa sổ quản trị
  đang có tiêu điểm, trừ khi NxtHUD cũng chạy quyền quản trị; lúc đó flyout không hiện, và nếu đang bật
  thay lớp phủ thì lớp phủ của Windows sẽ hiện thay.
- **Widget trên thanh tác vụ.** Windows không có API để thêm nội dung vào thanh tác vụ. Widget là một
  cửa sổ riêng đặt trên thanh tác vụ chính nằm ngang. Nó đọc vị trí các nút của thanh tác vụ qua UI
  Automation và chỉ dùng khoảng trống giữa chúng: ở đầu trái khi biểu tượng căn giữa, sau biểu tượng
  cuối khi căn trái. Khi thiếu chỗ, nó rút gọn tên bài, rồi ẩn hiệu ứng sóng nhạc, rồi ẩn chữ, và tự ẩn
  nếu ngay cả ảnh bìa và nút cũng không vừa. Nó kiểm tra lại mỗi 1,5 giây, nên ngay sau khi mở một ứng
  dụng, các biểu tượng có thể đè lên nó trong chốc lát. Widget ẩn với thanh tác vụ dọc, thanh tác vụ tự
  ẩn và ứng dụng toàn màn hình.
- **Thông số trên thanh tác vụ.** Cũng như widget, bảng thông số là một cửa sổ riêng cạnh khay, không
  phải một phần của đồng hồ, và nó thu về một dòng, rồi ẩn, khi hết chỗ trống. Nhiệt độ GPU cần driver
  đồ họa mới (WDDM 2.5 trở lên); nhiệt độ CPU và ổ đĩa không được hiển thị vì việc đọc chúng cần quyền
  quản trị hoặc driver nhân hệ thống. Số liệu mạng chỉ tính card mạng vật lý, để lưu lượng VPN không bị
  tính hai lần.
- **Nền.** Flyout Mica/Acrylic cần Windows 11 22H2 trở lên, và Windows tắt chúng khi bật tiết kiệm
  năng lượng (khi đó hãy dùng Translucent: nhìn xuyên được nhưng không làm mờ). Trên phiên bản cũ hơn,
  flyout dùng thẻ nền đặc. Khi dùng nền của hệ thống, nội dung có hiệu ứng mờ dần nhưng bản thân chất
  liệu hiện và biến mất không có hiệu ứng.
- **Nhận diện toàn màn hình** dựa vào trạng thái thông báo của Windows, vốn cũng coi trình duyệt ở chế
  độ toàn màn hình F11 là đang bận. Tắt "Pause in fullscreen games" để thấy flyout trong trường hợp đó.
- **Khởi động cùng Windows** dùng khóa Run trong registry của người dùng.
- **Hiệu ứng sóng nhạc.** Nó lắng nghe thiết bị phát mặc định tại thời điểm bắt đầu phát; sau khi đổi
  thiết bị đầu ra, nó có thể đứng yên cho tới lần tạm dừng/phát tiếp theo. Nó phản ứng với mọi âm thanh
  của hệ thống, không chỉ trình phát nhạc.

## 🙌 Ghi công

Mã nguồn, biểu tượng và thương hiệu của NxtHUD là nguyên bản. Ứng dụng được xây dựng trên
[WPF-UI](https://github.com/lepoco/wpfui) (MIT),
[CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) (MIT) và
[NAudio](https://github.com/naudio/NAudio) (MIT). Thông báo bản quyền và giấy phép của chúng nằm trong
[THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt), tệp này cũng được cài cạnh `NxtHUD.exe`.

## 📄 Giấy phép

NxtHUD được phát hành theo [Giấy phép MIT](LICENSE).
