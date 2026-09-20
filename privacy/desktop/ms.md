[Languages](README.md)

# Polisi privasi

Kali terakhir dikemas kini: 20 September 2026

**Screenshot Annotator** by Siarhei Kuchuk

Nama aplikasi: Screenshot Annotator
Nama pembangun: Siarhei Kuchuk

Perisian meraih tangkapan skrin pada komputer ini, membolehkan anotasi, dan boleh membaca teks dari kawasan dipilih dengan OCR. Ia tidak mencipta akaun awan. Pembangun tidak mengendalikan pelayan yang menerima tangkapan skrin, projek atau data penggunaan anda.

## Data yang tidak dikumpul pembangun

Aplikasi tidak termasuk iklan, analitik, pelapor ranap atau SDK penjejakan. Pembangun tidak mengumpul, menjual atau berkongsi data peribadi.

## Data disimpan pada komputer anda

### Tetapan

Tetapan aplikasi (pintasan Print Screen, mula dengan sistem, enjin OCR dan bahasa, warna penanda, dan bahasa terakhir dalam tetingkap Lesen atau Privasi) disimpan hanya pada komputer ini:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projek dan imej

Projek beranotasi dan pratonton disimpan dalam folder Gambar:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Tangkapan skrin, imej diimport dan teks anotasi kekal dalam fail setempat itu (atau papan keratan jika anda salin). Aplikasi tidak memuat naiknya.

### Log

Log diagnostik mungkin ditulis di bawah:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Jika aplikasi ranap, ia mungkin menulis fail laporan ralat setempat pada Desktop. Fail itu tidak dihantar ke mana-mana.

Nilai itu tidak dimuat naik kepada pembangun.

Tiada pelayan pembangun digunakan untuk menyimpan data anda.

## Tangkap skrin dan OCR

Apabila anda menggunakan Print Screen (atau perintah tangkapan), aplikasi meraih skrin semasa supaya anda boleh potong dan anotasi. Penangkapan berlaku pada peranti ini sahaja.

OCR berjalan secara setempat:

- **Windows OCR** menggunakan API `Windows.Media.Ocr` sistem pada PC ini.
- **Tesseract** berjalan pada komputer ini jika dipasang dan dalam PATH.

Teks dikenali dipaparkan dalam aplikasi untuk disalin atau diedit. Ia tidak dihantar kepada pembangun.

## Penggunaan rangkaian

### Semakan kemas kini

Binaan bukan Store mungkin meminta keluaran GitHub terkini:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) menerima permintaan HTTPS biasa (alamat IP, user-agent, masa). Pembangun tidak menerima trafik itu.

Pemasangan Microsoft Store tidak menggunakan semakan ini; Store menyampaikan kemas kini.

### Pautan yang anda buka

Aplikasi boleh membuka halaman ini dalam pelayar sistem. Laman itu mempunyai polisi privasi sendiri:

- Laman utama projek: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Keluaran terkini: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Lesen dipaparkan dalam aplikasi. Ia tidak dibuka sebagai halaman web.

## Tingkah laku setempat lain

Anda boleh mulakan aplikasi apabila log masuk Windows atau Linux. Pada Windows ia menggunakan entri permulaan (atau tugas permulaan Microsoft Store untuk pemasangan Store). Pada Linux entri autostart. Itu hanya melancarkan aplikasi ini pada komputer anda.

Pintasan Print Screen global pilihan kekal dalam ingatan semasa aplikasi berjalan supaya pemilih boleh dibuka.

## Kanak-kanak

Aplikasi ini alat anotasi tangkapan skrin. Ia tidak ditujukan kepada kanak-kanak bawah 13 tahun.

## Pihak ketiga

GitHub memproses semakan kemas kini dan halaman yang anda buka seperti di atas. Microsoft Store memproses pemasangan dan kemas kini Store. Windows OCR disediakan oleh sistem pengendalian. Pembangun tidak menerima trafik itu.

## Perubahan

Kemas kini polisi ini akan disiarkan dalam fail ini dalam repositori projek.

## Hubungan

Nama aplikasi: Screenshot Annotator
Nama pembangun: Siarhei Kuchuk

Soalan: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
