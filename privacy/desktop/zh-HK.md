[Languages](README.md)

# 私隱政策

最後更新：2026年9月20日

**Screenshot Annotator** by Siarhei Kuchuk

應用程式名稱：Screenshot Annotator
開發者名稱：Siarhei Kuchuk

本軟件喺呢部電腦擷取畫面、俾你加註釋，同埋可以用 OCR 讀取選定範圍嘅文字。唔會建立雲端帳戶。開發者唔會營運接收你嘅截圖、專案或使用數據嘅伺服器。

## 開發者唔收集嘅資料

應用冇廣告、分析、當機報告或追蹤 SDK。開發者唔收集、出售或分享個人資料。

## 儲存喺你電腦嘅資料

### 設定

應用設定（Print Screen 熱鍵、隨系統啟動、OCR 引擎同語言、螢光筆顏色，以及喺授權或私隱視窗上次揀嘅語言）只儲存喺呢部電腦：

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### 專案同圖像

加咗註釋嘅專案同預覽圖儲存喺圖片資料夾：

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

你擷取嘅畫面、匯入嘅圖像同註釋文字留喺呢啲本機檔案（如果複製就亦會喺剪貼簿）。應用唔會上載。

### 日誌

診斷日誌可能會寫入：

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

如果應用當機，可能會喺桌面寫入本機錯誤報告檔案。嗰個檔案唔會傳去任何地方。

呢啲內容唔會上載俾開發者。

唔會用開發者伺服器儲存你嘅資料。

## 畫面擷取同 OCR

你用 Print Screen（或截圖命令）嗰陣，應用會擷取而家個畫面，方便裁剪同加註釋。擷取只喺呢部裝置進行。

OCR 喺本機執行：

- **Windows OCR** 用呢部 PC 作業系統嘅 `Windows.Media.Ocr` API。
- **Tesseract** 如果已安裝而且喺 PATH，就喺呢部電腦執行。

識別出嚟嘅文字會喺應用入面顯示，方便複製或修改。唔會傳俾開發者。

## 網絡使用

### 更新檢查

非 Store 版本可能會要求 GitHub 最新發行：

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub（Microsoft）會收到一般 HTTPS 請求（IP 地址、user-agent、時間）。開發者收唔到呢啲流量。

由 Microsoft Store 安裝嘅版本唔用呢項檢查；更新由 Store 提供。

### 你打開嘅連結

應用可以喺系統瀏覽器打開呢啲頁面。呢啲網站有自己嘅私隱政策：

- 專案主頁：[github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- 最新發行：[github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

授權喺應用入面顯示。唔會當網頁打開。

## 其他本機行為

你可以喺 Windows 或 Linux 登入嗰陣啟動應用。Windows 用啟動項目（Store 安裝就用 Microsoft Store 啟動工作）。Linux 用自動啟動項目。只會喺你部電腦啟動呢個應用。

可選嘅全域 Print Screen 熱鍵喺應用運行期間留喺記憶體，方便打開選擇器。

## 兒童

呢個應用係截圖註釋工具。唔係面向 13 歲以下兒童。

## 第三方

GitHub 按上文處理更新檢查同你打開嘅頁面。Microsoft Store 處理 Store 安裝同更新。Windows OCR 由作業系統提供。開發者收唔到呢啲流量。

## 變更

呢份政策嘅更新會發布喺專案倉庫呢個檔案。

## 聯絡

應用程式名稱：Screenshot Annotator
開發者名稱：Siarhei Kuchuk

問題：[github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
