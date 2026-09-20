[Languages](README.md)

# Gizlilik politikası

Son güncelleme: 20 Eylül 2026

**Screenshot Annotator** by Siarhei Kuchuk

Uygulama adı: Screenshot Annotator
Geliştirici adı: Siarhei Kuchuk

Yazılım bu bilgisayarda ekran görüntüsü alır, açıklama eklemenizi sağlar ve seçilen alandaki metni OCR ile okuyabilir. Bulut hesabı oluşturmaz. Geliştirici, ekran görüntülerinizi, projelerinizi veya kullanım verilerinizi alan bir sunucu işletmez.

## Geliştiricinin toplamadığı veriler

Uygulamada reklam, analitik, çökme raporu veya izleme SDK’sı yoktur. Geliştirici kişisel verileri toplamaz, satmaz veya paylaşmaz.

## Bilgisayarınızda saklanan veriler

### Ayarlar

Uygulama ayarları (Print Screen kısayolu, sistemle başlatma, OCR motoru ve diller, vurgulayıcı rengi ve Lisans veya Gizlilik penceresinde son seçilen dil) yalnızca bu bilgisayarda saklanır:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projeler ve görüntüler

Açıklamalı projeler ve önizlemeler Resimler klasörünüze kaydedilir:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Aldığınız ekran görüntüleri, içe aktardığınız görüntüler ve açıklama metinleri bu yerel dosyalarda kalır (kopyalarsanız panoda da). Uygulama bunları yüklemez.

### Günlükler

Tanılama günlükleri şuraya yazılabilir:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Uygulama çökerse Masaüstüne yerel bir hata raporu yazabilir. Bu dosya hiçbir yere gönderilmez.

Bu değerler geliştiriciye yüklenmez.

Verilerinizi saklamak için geliştirici sunucusu kullanılmaz.

## Ekran yakalama ve OCR

Print Screen (veya ekran görüntüsü komutu) kullandığınızda uygulama geçerli ekranı yakalar; böylece kırpıp açıklama ekleyebilirsiniz. Yakalama yalnızca bu cihazda yapılır.

OCR yerelde çalışır:

- **Windows OCR** bu PC’deki işletim sisteminin `Windows.Media.Ocr` API’lerini kullanır.
- **Tesseract**, kuruluysa ve PATH’teyse bu bilgisayarda çalışır.

Tanınan metin uygulamada gösterilir; kopyalayabilir veya düzenleyebilirsiniz. Geliştiriciye gönderilmez.

## Ağ kullanımı

### Güncelleme denetimi

Store dışı derlemeler GitHub’daki en son sürümü isteyebilir:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) normal bir HTTPS isteği (IP adresi, user-agent, zaman) alır. Geliştirici bu trafiği almaz.

Microsoft Store kurulumları bu denetimi kullanmaz; güncellemeleri Store sağlar.

### Açtığınız bağlantılar

Uygulama sistem tarayıcısında şu sayfaları açabilir. Bu sitelerin kendi gizlilik politikaları vardır:

- Proje ana sayfası: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- En son sürüm: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Lisans uygulama içinde gösterilir. Web sayfası olarak açılmaz.

## Diğer yerel davranış

Uygulamayı Windows veya Linux oturum açılışında başlatabilirsiniz. Windows’ta bir başlangıç kaydı (Store kurulumlarında Microsoft Store başlangıç görevi) kullanılır. Linux’ta otomatik başlatma kaydı kullanılır. Bu, bilgisayarınızda yalnızca bu uygulamayı başlatır.

İsteğe bağlı genel Print Screen kısayolu, seçiciyi açmak için uygulama çalışırken bellekte kalır.

## Çocuklar

Uygulama bir ekran görüntüsü açıklama aracıdır. 13 yaşından küçük çocuklara yönelik değildir.

## Üçüncü taraflar

GitHub, yukarıdaki gibi güncelleme denetimini ve açtığınız sayfaları işler. Microsoft Store, Store kurulumlarını ve güncellemelerini işler. Windows OCR işletim sistemi tarafından sağlanır. Geliştirici bu trafiği almaz.

## Değişiklikler

Bu politikanın güncellemeleri proje deposundaki bu dosyada yayınlanır.

## İletişim

Uygulama adı: Screenshot Annotator
Geliştirici adı: Siarhei Kuchuk

Sorular: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
