[Languages](README.md)

# 개인정보 처리방침

최종 업데이트: 2026년 9월 20일

**Screenshot Annotator** by Siarhei Kuchuk

애플리케이션 이름: Screenshot Annotator
개발자 이름: Siarhei Kuchuk

이 소프트웨어는 이 컴퓨터에서 스크린샷을 찍고 주석을 달며, 선택한 영역의 텍스트를 OCR로 읽을 수 있습니다. 클라우드 계정을 만들지 않습니다. 개발자는 스크린샷, 프로젝트, 사용 데이터를 받는 서버를 운영하지 않습니다.

## 개발자가 수집하지 않는 데이터

앱에는 광고, 분석, 충돌 보고, 추적 SDK가 없습니다. 개발자는 개인 데이터를 수집, 판매, 공유하지 않습니다.

## 컴퓨터에 저장되는 데이터

### 설정

애플리케이션 설정(Print Screen 단축키, 시스템과 함께 시작, OCR 엔진 및 언어, 형광펜 색, 라이선스 또는 개인정보 창에서 마지막으로 선택한 언어)은 이 컴퓨터에만 저장됩니다.

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### 프로젝트와 이미지

주석이 달린 프로젝트와 미리보기는 사진 폴더에 저장됩니다.

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

찍은 스크린샷, 가져온 이미지, 주석 텍스트는 이러한 로컬 파일에 남습니다(복사하면 클립보드에도 남습니다). 앱은 업로드하지 않습니다.

### 로그

진단 로그는 다음에 기록될 수 있습니다.

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

앱이 충돌하면 바탕 화면에 로컬 오류 보고 파일을 쓸 수 있습니다. 그 파일은 어디에도 전송되지 않습니다.

이러한 값은 개발자에게 업로드되지 않습니다.

개발자 서버는 데이터를 저장하는 데 사용되지 않습니다.

## 화면 캡처와 OCR

Print Screen(또는 스크린샷 명령)을 사용하면 앱이 현재 화면을 캡처하여 자르고 주석을 달 수 있게 합니다. 캡처는 이 장치에서만 이루어집니다.

OCR은 로컬에서 실행됩니다.

- **Windows OCR**은 이 PC 운영 체제의 `Windows.Media.Ocr` API를 사용합니다.
- **Tesseract**는 설치되어 있고 PATH에 있으면 이 컴퓨터에서 실행됩니다.

인식된 텍스트는 앱에 표시되어 복사하거나 편집할 수 있습니다. 개발자에게 전송되지 않습니다.

## 네트워크 사용

### 업데이트 확인

Store가 아닌 빌드는 GitHub 최신 릴리스를 요청할 수 있습니다.

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub(Microsoft)는 일반 HTTPS 요청(IP 주소, user-agent, 시각)을 받습니다. 개발자는 그 트래픽을 받지 않습니다.

Microsoft Store 설치는 이 확인을 사용하지 않으며, Store가 업데이트를 제공합니다.

### 여는 링크

앱은 시스템 브라우저에서 다음 페이지를 열 수 있습니다. 해당 사이트에는 자체 개인정보 처리방침이 있습니다.

- 프로젝트 홈페이지: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- 최신 릴리스: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

라이선스는 앱 안에 표시됩니다. 웹 페이지로 열리지 않습니다.

## 기타 로컬 동작

Windows 또는 Linux에 로그인할 때 앱을 시작할 수 있습니다. Windows에서는 시작 항목(Store 설치는 Microsoft Store 시작 작업)을 사용합니다. Linux에서는 자동 시작 항목을 사용합니다. 이 컴퓨터에서 이 앱만 시작합니다.

선택적 전역 Print Screen 단축키는 선택 창을 열 수 있도록 앱이 실행되는 동안 메모리에 유지됩니다.

## 아동

이 앱은 스크린샷 주석 도구입니다. 13세 미만 아동을 대상으로 하지 않습니다.

## 제3자

GitHub는 위에서 설명한 대로 업데이트 확인과 연 페이지를 처리합니다. Microsoft Store는 Store 설치와 업데이트를 처리합니다. Windows OCR은 운영 체제가 제공합니다. 개발자는 그 트래픽을 받지 않습니다.

## 변경

이 정책의 업데이트는 프로젝트 저장소의 이 파일에 게시됩니다.

## 연락처

애플리케이션 이름: Screenshot Annotator
개발자 이름: Siarhei Kuchuk

질문: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
