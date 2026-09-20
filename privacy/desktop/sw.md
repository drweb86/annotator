[Languages](README.md)

# Sera ya faragha

Ilisasishwa mwisho: 20 Septemba 2026

**Screenshot Annotator** by Siarhei Kuchuk

Jina la programu: Screenshot Annotator
Jina la msanidi: Siarhei Kuchuk

Programu inachukua picha za skrini kwenye kompyuta hii, inaruhusu maelezo, na inaweza kusoma maandishi kutoka eneo lililochaguliwa kwa OCR. Haiumbi akaunti za wingu. Msanidi haendeshi seva inayopokea picha zako, miradi au data ya matumizi.

## Data ambayo msanidi hakusanyi

Programu haina matangazo, uchanganuzi, ripoti za kuharibika au SDK za ufuatiliaji. Msanidi hakusanyi, hauzi wala kushiriki data binafsi.

## Data iliyohifadhiwa kwenye kompyuta yako

### Mipangilio

Mipangilio ya programu (njia ya mkato ya Print Screen, kuanza na mfumo, injini ya OCR na lugha, rangi ya kionyeshi, na lugha ya mwisho katika dirisha la Leseni au Faragha) huhifadhiwa tu kwenye kompyuta hii:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator`

### Miradi na picha

Miradi iliyoelezwa na hakikisho huhifadhiwa katika folda ya Picha:

- Windows: `%USERPROFILE%\Pictures\ScreenshotAnnotator`
- Linux: `~/Pictures/ScreenshotAnnotator`

Picha za skrini, picha zilizoingizwa na maandishi ya maelezo hukaa katika faili hizo za ndani (au ubao wa kunakili ukinakili). Programu haipakii.

### Kumbukumbu

Kumbukumbu za uchunguzi zinaweza kuandikwa chini ya:

- Windows: `%AppData%\SiarheiKuchuk.ScreenshotAnnotator\Logs`
- Linux: `~/.config/SiarheiKuchuk.ScreenshotAnnotator/Logs`

Ikiwa programu itaharibika, inaweza kuandika faili ya ripoti ya kosa kwenye Eneo-kazi. Faili hiyo haitumwi popote.

Thamani hizo hazipakiwi kwa msanidi.

Hakuna seva ya msanidi inayotumiwa kuhifadhi data yako.

## Kukamata skrini na OCR

Unapotumia Print Screen (au amri ya picha), programu inakamata skrini ya sasa ili ukate na ueleze. Kukamata hutokea kwenye kifaa hiki pekee.

OCR inaendeshwa ndani:

- **Windows OCR** hutumia API za `Windows.Media.Ocr` za mfumo kwenye PC hii.
- **Tesseract** inaendeshwa kwenye kompyuta hii ikiwa imewekwa na iko kwenye PATH.

Maandishi yaliyotambuliwa yanaonyeshwa katika programu ili kunakili au kuhariri. Hayatumwi kwa msanidi.

## Matumizi ya mtandao

### Ukaguzi wa sasisho

Majengo yasiyo ya Store yanaweza kuomba toleo jipya zaidi la GitHub:

`https://api.github.com/repos/drweb86/annotator/releases/latest`

GitHub (Microsoft) hupokea ombi la kawaida la HTTPS (anwani ya IP, user-agent, wakati). Msanidi hapokei trafiki hiyo.

Usakinishaji wa Microsoft Store hautumii ukaguzi huu; Store hutoa sasisho.

### Viungo unavyofungua

Programu inaweza kufungua kurasa hizi katika kivinjari cha mfumo. Tovuti hizo zina sera zao za faragha:

- Ukurasa wa nyumbani wa mradi: [github.com/drweb86/annotator](https://github.com/drweb86/annotator)
- Toleo jipya zaidi: [github.com/drweb86/annotator/releases/latest](https://github.com/drweb86/annotator/releases/latest)

Leseni inaonyeshwa ndani ya programu. Haifunguliwi kama ukurasa wa wavuti.

## Tabia nyingine ya ndani

Unaweza kuanzisha programu unapoingia Windows au Linux. Kwenye Windows hutumia ingizo la kuanza (au kazi ya kuanza ya Microsoft Store kwa usakinishaji wa Store). Kwenye Linux ingizo la autostart. Hiyo inaanzisha tu programu hii kwenye kompyuta yako.

Njia ya mkato ya kimataifa ya Print Screen ya hiari inabaki kwenye kumbukumbu programu inapoendesha ili kufungua kichaguzi.

## Watoto

Programu ni zana ya kueleza picha za skrini. Hailengi watoto chini ya miaka 13.

## Wahusika wengine

GitHub huchakata ukaguzi wa sasisho na kurasa unazofungua, kama hapo juu. Microsoft Store huchakata usakinishaji na sasisho za Store. Windows OCR hutolewa na mfumo wa uendeshaji. Msanidi hapokei trafiki hiyo.

## Mabadiliko

Sasisho za sera hii zitachapishwa katika faili hii kwenye hifadhi ya mradi.

## Mawasiliano

Jina la programu: Screenshot Annotator
Jina la msanidi: Siarhei Kuchuk

Maswali: [github.com/drweb86/annotator/issues](https://github.com/drweb86/annotator/issues)
