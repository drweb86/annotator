[Languages](README.md)

# プライバシーポリシー

最終更新: 2026年9月20日

**Screenshot Annotator** by Siarhei Kuchuk

アプリケーション名: Screenshot Annotator
開発者名: Siarhei Kuchuk

本ソフトウェアはこのコンピューター上でスクリーンショットを撮影し、注釈を付け、選択した領域の文字をOCRで読み取ります。クラウドアカウントは作成しません。開発者は、スクリーンショット、プロジェクト、利用データを受け取るサーバーを運用していません。

## 開発者が収集しないデータ

アプリに広告、分析、クラッシュ報告、追跡SDKは含まれません。開発者は個人データを収集、販売、共有しません。

## このコンピューターに保存されるデータ

### 設定

アプリの設定（Print Screen ホットキー、システム起動時の開始、OCRエンジンと言語、蛍光ペンの色、ライセンスまたはプライバシーウィンドウで最後に選んだ言語）はこのコンピューターにのみ保存されます。

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### プロジェクトと画像

注釈付きプロジェクトとプレビューはピクチャフォルダーに保存されます。

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

撮影したスクリーンショット、読み込んだ画像、注釈の文字はこれらのローカルファイルに残ります（コピーした場合はクリップボードにも残ります）。アプリはアップロードしません。

### ログ

診断ログは次の場所に書き込まれることがあります。

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

アプリが異常終了した場合、デスクトップにローカルの不具合報告ファイルを書くことがあります。そのファイルはどこにも送信されません。

これらの値は開発者にアップロードされません。

開発者のサーバーはデータの保存に使われません。

## 画面キャプチャとOCR

Print Screen（またはスクリーンショットコマンド）を使うと、アプリは現在の画面を取り込み、切り抜いて注釈できるようにします。取り込みはこの端末上でのみ行われます。

OCRはローカルで実行されます。

- **Windows OCR** はこのPCの OS の `Windows.Media.Ocr` API を使います。
- **Tesseract** はインストールされ PATH にある場合、このコンピューターで実行されます。

認識された文字はアプリ内に表示され、コピーや編集ができます。開発者には送信されません。

## ネットワークの利用

### 更新確認

Store 以外のビルドは GitHub の最新リリースを要求することがあります。

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub（Microsoft）は通常の HTTPS リクエスト（IPアドレス、user-agent、時刻）を受け取ります。開発者はその通信を受け取りません。

Microsoft Store からのインストールはこの確認を使いません。更新は Store が配信します。

### 開くリンク

アプリはシステムのブラウザーで次のページを開けます。これらのサイトには独自のプライバシーポリシーがあります。

- プロジェクトのホームページ: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- 最新リリース: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

ライセンスはアプリ内に表示されます。ウェブページとしては開きません。

## その他のローカル動作

Windows または Linux のサインイン時にアプリを起動できます。Windows ではスタートアップ項目（Store インストールでは Microsoft Store のスタートアップタスク）を使います。Linux では自動起動項目を使います。起動するのはこのコンピューター上の本アプリだけです。

任意のグローバル Print Screen ホットキーは、セレクターを開けるよう、アプリの実行中メモリに残ります。

## 子ども

本アプリはスクリーンショット注釈ツールです。13歳未満の子どもを対象としていません。

## 第三者

GitHub は上記のとおり更新確認と開いたページを処理します。Microsoft Store は Store のインストールと更新を処理します。Windows OCR は OS が提供します。開発者はその通信を受け取りません。

## 変更

このポリシーの更新は、プロジェクトリポジトリ内のこのファイルに掲載されます。

## 連絡先

アプリケーション名: Screenshot Annotator
開発者名: Siarhei Kuchuk

質問: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
