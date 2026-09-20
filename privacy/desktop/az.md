[Languages](README.md)

# Azərbaycanlıq siyasəti

Son yenilənmə: 20 sentyabr 2026

**Screenshot Annotator** by Siarhei Kuchuk

Tətbiq adı: Screenshot Annotator
Tərtibatçı adı: Siarhei Kuchuk

Proqram bu kompüterdə ekran şəkilləri çəkir, annotasiya etməyə imkan verir və seçilmiş sahədəki mətni OCR ilə oxuya bilər. Bulud hesabları yaratmır. Tərtibatçı ekran şəkillərinizi, layihələrinizi və ya istifadə məlumatlarınızı qəbul edən server işləmir.

## Tərtibatçının toplamadığı məlumatlar

Tətbiqdə reklam, analitika, qəza hesabatı və ya izləmə SDK-sı yoxdur. Tərtibatçı şəxsi məlumat toplamır, satmır və paylaşmır.

## Kompüterinizdə saxlanılan məlumatlar

### Parametrlər

Tətbiq parametrləri (Print Screen qısayolu, sistemlə başlatma, OCR mühərriki və dillər, marker rəngi və Lisenziya və ya Məxfilik pəncərəsində son seçilmiş dil) yalnız bu kompüterdə saxlanılır:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Layihələr və şəkillər

Annotasiya olunmuş layihələr və önizləmələr Şəkillər qovluğunda saxlanılır:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Ekran şəkilləri, idxal olunmuş şəkillər və annotasiya mətni bu yerli fayllarda qalır (kopyalasanız mübadilə buferində də). Tətbiq onları yükləmir.

### Jurnallar

Diaqnostika jurnalları buraya yazıla bilər:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Tətbiq qəzaya uğrasa, İş masasında yerli xəta hesabatı yaza bilər. Həmin fayl heç yerə göndərilmir.

Bu dəyərlər tərtibatçıya yüklənmir.

Məlumatlarınızı saxlamaq üçün tərtibatçı serveri istifadə olunmur.

## Ekran tutma və OCR

Print Screen (və ya ekran şəkli əmri) istifadə etdikdə tətbiq cari ekranı tutur ki, kəsib annotasiya edəsiniz. Tutma yalnız bu cihazda olur.

OCR yerli işləyir:

- **Windows OCR** bu PC-də əməliyyat sisteminin `Windows.Media.Ocr` API-lərindən istifadə edir.
- **Tesseract** quraşdırılıbsa və PATH-dadırsa bu kompüterdə işləyir.

Tanınmış mətn tətbiqdə göstərilir ki, kopyalaya və ya redaktə edəsiniz. Tərtibatçıya göndərilmir.

## Şəbəkə istifadəsi

### Yeniləmə yoxlaması

Store-dan kənar yığımlar GitHub-dakı son buraxılışı istəyə bilər:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) adi HTTPS sorğusu (IP ünvanı, user-agent, vaxt) alır. Tərtibatçı həmin trafikı almir.

Microsoft Store quraşdırmaları bu yoxlamanı istifadə etmir; yeniləmələri Store çatdırır.

### Açdığınız keçidlər

Tətbiq sistem brauzerində bu səhifələri aça bilər. Həmin saytların öz məxfilik siyasətləri var:

- Layihə ana səhifəsi: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Son buraxılış: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Lisenziya tətbiqin içində göstərilir. Veb səhifə kimi açılmır.

## Digər yerli davranış

Windows və ya Linux-a daxil olanda tətbiqi başlada bilərsiniz. Windows-da başlanğıc qeydi (Store quraşdırmalarında Microsoft Store başlanğıc tapşırığı) istifadə olunur. Linux-da autostart qeydi. Bu, kompüterinizdə yalnız bu tətbiqi işə salır.

İstəyə bağlı qlobal Print Screen qısayolu seçici açılsın deyə tətbiq işləyərkən yaddaşda qalır.

## Uşaqlar

Tətbiq ekran şəkli annotasiya alətidir. 13 yaşdan kiçik uşaqlara yönəlməyib.

## Üçüncü tərəflər

GitHub yuxarıdakı kimi yeniləmə yoxlamasını və açdığınız səhifələri emal edir. Microsoft Store, Store quraşdırma və yeniləmələrini emal edir. Windows OCR-i əməliyyat sistemi təmin edir. Tərtibatçı həmin trafikı almir.

## Dəyişikliklər

Bu siyasətin yeniləmələri layihə anbarındakı bu faylda dərc olunacaq.

## Əlaqə

Tətbiq adı: Screenshot Annotator
Tərtibatçı adı: Siarhei Kuchuk

Suallar: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
