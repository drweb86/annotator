[Languages](README.md)

# Chính sách quyền riêng tư

Cập nhật lần cuối: 20 tháng 9 năm 2026

**Screenshot Annotator** by Siarhei Kuchuk

Tên ứng dụng: Screenshot Annotator
Tên nhà phát triển: Siarhei Kuchuk

Phần mềm chụp ảnh màn hình trên máy tính này, cho phép chú thích và có thể đọc chữ trong vùng đã chọn bằng OCR. Ứng dụng không tạo tài khoản đám mây. Nhà phát triển không vận hành máy chủ nhận ảnh chụp, dự án hoặc dữ liệu sử dụng của bạn.

## Dữ liệu nhà phát triển không thu thập

Ứng dụng không có quảng cáo, phân tích, báo cáo sự cố hay SDK theo dõi. Nhà phát triển không thu thập, bán hay chia sẻ dữ liệu cá nhân.

## Dữ liệu lưu trên máy tính của bạn

### Cài đặt

Cài đặt ứng dụng (phím tắt Print Screen, khởi động cùng hệ thống, công cụ OCR và ngôn ngữ, màu đánh dấu, và ngôn ngữ chọn lần cuối trong cửa sổ Giấy phép hoặc Quyền riêng tư) chỉ được lưu trên máy tính này:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Dự án và hình ảnh

Dự án đã chú thích và bản xem trước được lưu trong thư mục Ảnh:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Ảnh chụp, hình nhập vào và chữ chú thích ở lại trong các tệp cục bộ đó (hoặc khay nhớ tạm nếu bạn sao chép). Ứng dụng không tải chúng lên.

### Nhật ký

Nhật ký chẩn đoán có thể được ghi vào:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Nếu ứng dụng gặp sự cố, có thể ghi tệp báo cáo lỗi cục bộ trên Máy tính để bàn. Tệp đó không được gửi đi đâu.

Các giá trị này không được tải lên nhà phát triển.

Không dùng máy chủ của nhà phát triển để lưu dữ liệu của bạn.

## Chụp màn hình và OCR

Khi dùng Print Screen (hoặc lệnh chụp), ứng dụng chụp màn hình hiện tại để bạn cắt và chú thích. Việc chụp chỉ diễn ra trên thiết bị này.

OCR chạy cục bộ:

- **Windows OCR** dùng API `Windows.Media.Ocr` của hệ điều hành trên PC này.
- **Tesseract** chạy trên máy tính này nếu đã cài và có trong PATH.

Chữ nhận dạng được hiện trong ứng dụng để sao chép hoặc sửa. Không gửi cho nhà phát triển.

## Sử dụng mạng

### Kiểm tra cập nhật

Bản không phải Store có thể yêu cầu bản phát hành GitHub mới nhất:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) nhận yêu cầu HTTPS thông thường (địa chỉ IP, user-agent, thời gian). Nhà phát triển không nhận lưu lượng đó.

Cài đặt từ Microsoft Store không dùng kiểm tra này; Store cung cấp bản cập nhật.

### Liên kết bạn mở

Ứng dụng có thể mở các trang này trong trình duyệt hệ thống. Các trang đó có chính sách quyền riêng tư riêng:

- Trang chủ dự án: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Bản phát hành mới nhất: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Giấy phép được hiện trong ứng dụng. Không mở dưới dạng trang web.

## Hành vi cục bộ khác

Bạn có thể khởi động ứng dụng khi đăng nhập Windows hoặc Linux. Trên Windows dùng mục khởi động (hoặc tác vụ khởi động Microsoft Store với bản Store). Trên Linux dùng mục autostart. Việc này chỉ khởi chạy ứng dụng này trên máy của bạn.

Phím tắt Print Screen toàn cục tùy chọn ở lại bộ nhớ khi ứng dụng chạy để mở bộ chọn.

## Trẻ em

Ứng dụng là công cụ chú thích ảnh chụp màn hình. Không hướng tới trẻ em dưới 13 tuổi.

## Bên thứ ba

GitHub xử lý kiểm tra cập nhật và các trang bạn mở như trên. Microsoft Store xử lý cài đặt và cập nhật Store. Windows OCR do hệ điều hành cung cấp. Nhà phát triển không nhận lưu lượng đó.

## Thay đổi

Cập nhật chính sách này sẽ được đăng trong tệp này trong kho dự án.

## Liên hệ

Tên ứng dụng: Screenshot Annotator
Tên nhà phát triển: Siarhei Kuchuk

Câu hỏi: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
