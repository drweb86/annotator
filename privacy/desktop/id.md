[Languages](README.md)

# Kebijakan privasi

Terakhir diperbarui: 20 September 2026

**Screenshot Annotator** by Siarhei Kuchuk

Nama aplikasi: Screenshot Annotator
Nama pengembang: Siarhei Kuchuk

Perangkat lunak mengambil tangkapan layar di komputer ini, memungkinkan anotasi, dan dapat membaca teks dari area yang dipilih dengan OCR. Tidak membuat akun cloud. Pengembang tidak mengoperasikan server yang menerima tangkapan layar, proyek, atau data penggunaan Anda.

## Data yang tidak dikumpulkan pengembang

Aplikasi tidak berisi iklan, analitik, pelaporan kerusakan, atau SDK pelacakan. Pengembang tidak mengumpulkan, menjual, atau membagikan data pribadi.

## Data yang disimpan di komputer Anda

### Pengaturan

Pengaturan aplikasi (pintasan Print Screen, mulai bersama sistem, mesin OCR dan bahasa, warna pena sorot, dan bahasa terakhir di jendela Lisensi atau Privasi) hanya disimpan di komputer ini:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Proyek dan gambar

Proyek beranotasi dan pratinjau disimpan di folder Gambar:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Tangkapan layar, gambar yang diimpor, dan teks anotasi tetap di berkas lokal itu (atau papan klip jika Anda menyalin). Aplikasi tidak mengunggahnya.

### Log

Log diagnostik dapat ditulis di:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Jika aplikasi mogok, mungkin menulis berkas laporan kesalahan lokal di Desktop. Berkas itu tidak dikirim ke mana pun.

Nilai-nilai itu tidak diunggah ke pengembang.

Tidak ada server pengembang yang digunakan untuk menyimpan data Anda.

## Tangkap layar dan OCR

Saat Anda memakai Print Screen (atau perintah tangkapan), aplikasi menangkap layar saat ini agar dapat dipotong dan dianotasi. Penangkapan hanya terjadi di perangkat ini.

OCR berjalan secara lokal:

- **Windows OCR** memakai API `Windows.Media.Ocr` sistem operasi di PC ini.
- **Tesseract** berjalan di komputer ini jika terpasang dan ada di PATH.

Teks yang dikenali ditampilkan di aplikasi agar dapat disalin atau diedit. Tidak dikirim ke pengembang.

## Penggunaan jaringan

### Pemeriksaan pembaruan

Build di luar Store dapat meminta rilis GitHub terbaru:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) menerima permintaan HTTPS biasa (alamat IP, user-agent, waktu). Pengembang tidak menerima lalu lintas itu.

Pemasangan dari Microsoft Store tidak memakai pemeriksaan ini; Store menyediakan pembaruan.

### Tautan yang Anda buka

Aplikasi dapat membuka halaman ini di peramban sistem. Situs itu punya kebijakan privasi sendiri:

- Beranda proyek: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Rilis terbaru: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Lisensi ditampilkan di dalam aplikasi. Tidak dibuka sebagai halaman web.

## Perilaku lokal lain

Anda dapat memulai aplikasi saat masuk Windows atau Linux. Di Windows memakai entri startup (atau tugas startup Microsoft Store untuk pemasangan Store). Di Linux memakai entri autostart. Itu hanya menjalankan aplikasi ini di komputer Anda.

Pintasan Print Screen global opsional tetap di memori saat aplikasi berjalan agar pemilih dapat dibuka.

## Anak-anak

Aplikasi ini alat anotasi tangkapan layar. Tidak ditujukan untuk anak di bawah 13 tahun.

## Pihak ketiga

GitHub memproses pemeriksaan pembaruan dan halaman yang Anda buka seperti di atas. Microsoft Store memproses pemasangan dan pembaruan Store. Windows OCR disediakan sistem operasi. Pengembang tidak menerima lalu lintas itu.

## Perubahan

Pembaruan kebijakan ini akan diposting di berkas ini di repositori proyek.

## Kontak

Nama aplikasi: Screenshot Annotator
Nama pengembang: Siarhei Kuchuk

Pertanyaan: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
