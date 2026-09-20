[Languages](README.md)

# Política de privacidade

Última atualização: 20 de setembro de 2026

**Screenshot Annotator** by Siarhei Kuchuk

Nome do aplicativo: Screenshot Annotator
Nome do desenvolvedor: Siarhei Kuchuk

O software captura capturas de tela neste computador, permite anotá-las e pode ler texto de uma área selecionada com OCR. Ele não cria contas na nuvem. O desenvolvedor não opera um servidor que recebe suas capturas, projetos ou dados de uso.

## Dados que o desenvolvedor não coleta

O aplicativo não inclui anúncios nem SDKs de análise, relatório de falhas ou rastreamento. O desenvolvedor não coleta, vende nem compartilha dados pessoais.

## Dados armazenados no seu computador

### Configurações

As configurações do aplicativo (atalho Print Screen, iniciar com o sistema, mecanismo OCR e idiomas, cor do marca-texto e o último idioma escolhido nas janelas Licença ou Privacidade) são armazenadas apenas neste computador:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Projetos e imagens

Projetos anotados e miniaturas são armazenados na pasta Imagens:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Capturas, imagens importadas e texto das anotações permanecem nesses arquivos locais (ou na área de transferência se você os copiar). O aplicativo não os envia.

### Registros

Registros de diagnóstico podem ser gravados em:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator\Logs`

Se o aplicativo falhar, ele pode gravar um relatório de erro local na Área de trabalho. Esse arquivo não é enviado a lugar nenhum.

Esses valores não são enviados ao desenvolvedor.

Nenhum servidor do desenvolvedor é usado para armazenar seus dados.

## Captura de tela e OCR

Quando você usa Print Screen (ou o comando de captura), o aplicativo captura a tela atual para que você possa recortar e anotar. A captura ocorre apenas neste dispositivo.

O OCR é executado localmente:

- **Windows OCR** usa as APIs `Windows.Media.Ocr` do sistema neste PC.
- **Tesseract** é executado neste computador se estiver instalado e no PATH.

O texto reconhecido é mostrado no aplicativo para copiar ou editar. Não é enviado ao desenvolvedor.

## Uso da rede

### Verificação de atualizações

Compilações fora da Store podem solicitar a última versão do GitHub:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

O GitHub (Microsoft) recebe uma solicitação HTTPS normal (endereço IP, user-agent, horário). O desenvolvedor não recebe esse tráfego.

Instalações da Microsoft Store não usam essa verificação; a Store entrega as atualizações.

### Links que você abre

O aplicativo pode abrir estas páginas no navegador do sistema. Esses sites têm suas próprias políticas de privacidade:

- Página do projeto: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Última versão: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

A licença é mostrada no aplicativo. Não é aberta como página da web.

## Outro comportamento local

Você pode iniciar o aplicativo ao entrar no Windows ou Linux. No Windows isso usa uma entrada de inicialização (ou uma tarefa de inicialização da Microsoft Store nas instalações da Store). No Linux usa uma entrada de autostart. Isso inicia apenas este aplicativo no seu computador.

O atalho global opcional Print Screen permanece na memória enquanto o aplicativo está em execução para abrir o seletor.

## Crianças

O aplicativo é uma ferramenta de anotação de capturas. Não é destinado a crianças menores de 13 anos.

## Terceiros

O GitHub processa a verificação de atualizações e as páginas que você abre, conforme acima. A Microsoft Store processa instalações e atualizações da Store. O Windows OCR é fornecido pelo sistema operacional. O desenvolvedor não recebe esse tráfego.

## Alterações

Atualizações desta política serão publicadas neste arquivo no repositório do projeto.

## Contato

Nome do aplicativo: Screenshot Annotator
Nome do desenvolvedor: Siarhei Kuchuk

Perguntas: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
