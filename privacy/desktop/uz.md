[Languages](README.md)

# Maxfiylik siyosati

Oxirgi yangilanish: 2026-yil 20-sentabr

**Screenshot Annotator** by Siarhei Kuchuk

Ilova nomi: Screenshot Annotator
Ishlab chiquvchi nomi: Siarhei Kuchuk

Dastur ushbu kompyuterda ekran tasvirini oladi, izoh qo‘yish imkonini beradi va tanlangan hududdagi matnni OCR bilan o‘qishi mumkin. Bulut hisoblarini yaratmaydi. Ishlab chiquvchi skrinshotlaringiz, loyihalaringiz yoki foydalanish ma’lumotlarini qabul qiladigan server ishlatmaydi.

## Ishlab chiquvchi yig‘maydigan ma’lumotlar

Ilovada reklama, tahlil, nosozlik hisoboti yoki kuzatuv SDK yo‘q. Ishlab chiquvchi shaxsiy ma’lumotlarni yig‘maydi, sotmaydi va ulashmaydi.

## Kompyuteringizda saqlanadigan ma’lumotlar

### Sozlamalar

Ilova sozlamalari (Print Screen yorlig‘i, tizim bilan ishga tushirish, OCR dvigateli va tillar, marker rangi hamda Litsenziya yoki Maxfiylik oynasida oxirgi tanlangan til) faqat shu kompyuterda saqlanadi:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Loyihalar va tasvirlar

Izohlangan loyihalar va ko‘rinishlar Rasmlar jildida saqlanadi:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Skrinshotlar, import qilingan tasvirlar va izoh matni shu mahalliy fayllarda qoladi (nusxa olsangiz buferda ham). Ilova ularni yuklamaydi.

### Jurnallar

Tashxis jurnallari quyidagiga yozilishi mumkin:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Ilova ishdan chiqsa, Ish stoliga mahalliy xato hisoboti yozishi mumkin. Bu fayl hech qayerga yuborilmaydi.

Bu qiymatlar ishlab chiquvchiga yuklanmaydi.

Ma’lumotlaringizni saqlash uchun ishlab chiquvchi serveri ishlatilmaydi.

## Ekranni olish va OCR

Print Screen (yoki skrinshot buyrug‘i) ishlatganda ilova joriy ekranni oladi, shunda kesib izoh qo‘yasiz. Olish faqat shu qurilmada bo‘ladi.

OCR mahalliy ishlaydi:

- **Windows OCR** ushbu PC dagi operatsion tizimning `Windows.Media.Ocr` API laridan foydalanadi.
- **Tesseract** o‘rnatilgan va PATH da bo‘lsa, ushbu kompyuterda ishlaydi.

Aniqlangan matn ilovada ko‘rsatiladi, nusxa olish yoki tahrirlash uchun. Ishlab chiquvchiga yuborilmaydi.

## Tarmoqdan foydalanish

### Yangilanishni tekshirish

Store dan tashqari yig‘ilmalar GitHub dagi so‘nggi relizni so‘rashi mumkin:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) oddiy HTTPS so‘rovini (IP manzil, user-agent, vaqt) qabul qiladi. Ishlab chiquvchi bu trafikni olmaydi.

Microsoft Store o‘rnatmalari bu tekshiruvdan foydalanmaydi; yangilanishlarni Store yetkazadi.

### Ochgan havolalaringiz

Ilova tizim brauzerida ushbu sahifalarni ochishi mumkin. Bu saytlarning o‘z maxfiylik siyosati bor:

- Loyiha bosh sahifasi: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- So‘nggi reliz: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Litsenziya ilova ichida ko‘rsatiladi. Veb-sahifa sifatida ochilmaydi.

## Boshqa mahalliy xulq

Windows yoki Linux ga kirganda ilovani ishga tushirishingiz mumkin. Windows da ishga tushirish yozuvi (Store o‘rnatmalarida Microsoft Store ishga tushirish vazifasi) ishlatiladi. Linux da autostart yozuvi. Bu kompyuteringizda faqat shu ilovani ishga tushiradi.

Ixtiyoriy global Print Screen yorlig‘i tanlovchi ochilishi uchun ilova ishlayotganda xotirada qoladi.

## Bolalar

Ilova skrinshot izohlash vositasidir. 13 yoshdan kichik bolalarga mo‘ljallanmagan.

## Uchinchi tomonlar

GitHub yuqoridagidek yangilanish tekshiruvi va ochgan sahifalaringizni qayta ishlaydi. Microsoft Store Store o‘rnatmalari va yangilanishlarini qayta ishlaydi. Windows OCR ni operatsion tizim ta’minlaydi. Ishlab chiquvchi bu trafikni olmaydi.

## O‘zgarishlar

Ushbu siyosat yangilanishlari loyiha omboridagi ushbu faylda e’lon qilinadi.

## Aloqa

Ilova nomi: Screenshot Annotator
Ishlab chiquvchi nomi: Siarhei Kuchuk

Savollar: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
