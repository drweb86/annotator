using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services.Ocr;

public class TesseractOcrEngine : IOcrEngine
{
    public string Id => "tesseract";
    public string DisplayName => "Tesseract OCR";
    public bool SupportsLanguageSelection => true;

    // Resolved once: full path on Windows installs that skip PATH, or plain "tesseract" elsewhere.
    private static readonly string _exe = FindExecutable();

    private static string FindExecutable()
    {
        // Well-known Windows installer locations (setup does not add to PATH by default)
        if (OperatingSystem.IsWindows())
        {
            var candidates = new[]
            {
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                    "Tesseract-OCR", "tesseract.exe"),
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Programs", "Tesseract-OCR", "tesseract.exe"),
            };

            foreach (var path in candidates)
                if (File.Exists(path))
                    return path;
        }

        return "tesseract"; // rely on PATH (Linux / manual Windows install)
    }

    public static bool IsAvailable()
    {
        try
        {
            using var p = Process.Start(new ProcessStartInfo(_exe, "--version")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            return p != null && p.WaitForExit(4000) && p.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public IReadOnlyList<OcrLanguage> GetAvailableLanguages()
    {
        try
        {
            using var p = Process.Start(new ProcessStartInfo(_exe, "--list-langs")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            if (p == null) return Array.Empty<OcrLanguage>();
            p.WaitForExit(5000);
            // tesseract outputs lang list to stderr
            var output = p.StandardError.ReadToEnd() + p.StandardOutput.ReadToEnd();
            return output
                .Split('\n')
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith("List of", StringComparison.Ordinal) && !l.StartsWith("Error", StringComparison.Ordinal))
                .Select(tag => new OcrLanguage(tag, GetLangDisplayName(tag)))
                .ToList();
        }
        catch
        {
            return Array.Empty<OcrLanguage>();
        }
    }

    public async Task<string> RecognizeTextAsync(byte[] pngData, IReadOnlyList<string> languageTags)
    {
        var tmpInput = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
        var tmpOutputBase = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            await File.WriteAllBytesAsync(tmpInput, pngData);
            var langs = languageTags.Count > 0 ? string.Join("+", languageTags) : "eng";
            using var p = Process.Start(new ProcessStartInfo(
                _exe, $"\"{tmpInput}\" \"{tmpOutputBase}\" -l {langs}")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            });
            if (p != null)
                await p.WaitForExitAsync();

            var resultFile = tmpOutputBase + ".txt";
            return File.Exists(resultFile) ? (await File.ReadAllTextAsync(resultFile)).Trim() : string.Empty;
        }
        finally
        {
            TryDelete(tmpInput);
            TryDelete(tmpOutputBase + ".txt");
        }
    }

    private static void TryDelete(string path)
    {
        try { if (File.Exists(path)) File.Delete(path); } catch { }
    }

    private static readonly Dictionary<string, string> KnownLangNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["afr"] = "Afrikaans", ["amh"] = "Amharic", ["ara"] = "Arabic",
        ["bel"] = "Belarusian", ["ben"] = "Bengali", ["bul"] = "Bulgarian",
        ["cat"] = "Catalan", ["ces"] = "Czech", ["chi_sim"] = "Chinese (Simplified)",
        ["chi_tra"] = "Chinese (Traditional)", ["chr"] = "Cherokee", ["cos"] = "Corsican",
        ["cym"] = "Welsh", ["dan"] = "Danish", ["deu"] = "German",
        ["ell"] = "Greek", ["eng"] = "English", ["enm"] = "English (Middle)",
        ["epo"] = "Esperanto", ["est"] = "Estonian", ["eus"] = "Basque",
        ["fas"] = "Persian", ["fin"] = "Finnish", ["fra"] = "French",
        ["frk"] = "German (Fraktur)", ["frm"] = "French (Middle)", ["gle"] = "Irish",
        ["glg"] = "Galician", ["grc"] = "Greek (Ancient)", ["guj"] = "Gujarati",
        ["hat"] = "Haitian Creole", ["heb"] = "Hebrew", ["hin"] = "Hindi",
        ["hrv"] = "Croatian", ["hun"] = "Hungarian", ["hye"] = "Armenian",
        ["ind"] = "Indonesian", ["isl"] = "Icelandic", ["ita"] = "Italian",
        ["jav"] = "Javanese", ["jpn"] = "Japanese", ["kan"] = "Kannada",
        ["kat"] = "Georgian", ["kaz"] = "Kazakh", ["khm"] = "Khmer",
        ["kir"] = "Kyrgyz", ["kor"] = "Korean", ["lao"] = "Lao",
        ["lat"] = "Latin", ["lav"] = "Latvian", ["lit"] = "Lithuanian",
        ["ltz"] = "Luxembourgish", ["mal"] = "Malayalam", ["mar"] = "Marathi",
        ["mkd"] = "Macedonian", ["mlt"] = "Maltese", ["mon"] = "Mongolian",
        ["msa"] = "Malay", ["mya"] = "Burmese", ["nep"] = "Nepali",
        ["nld"] = "Dutch", ["nor"] = "Norwegian", ["oci"] = "Occitan",
        ["ori"] = "Odia", ["pan"] = "Punjabi", ["pol"] = "Polish",
        ["por"] = "Portuguese", ["pus"] = "Pashto", ["que"] = "Quechua",
        ["ron"] = "Romanian", ["rus"] = "Russian", ["san"] = "Sanskrit",
        ["sin"] = "Sinhala", ["slk"] = "Slovak", ["slv"] = "Slovenian",
        ["snd"] = "Sindhi", ["som"] = "Somali", ["spa"] = "Spanish",
        ["sqi"] = "Albanian", ["srp"] = "Serbian", ["swa"] = "Swahili",
        ["swe"] = "Swedish", ["syr"] = "Syriac", ["tam"] = "Tamil",
        ["tat"] = "Tatar", ["tel"] = "Telugu", ["tgk"] = "Tajik",
        ["tha"] = "Thai", ["tir"] = "Tigrinya", ["tur"] = "Turkish",
        ["uig"] = "Uyghur", ["ukr"] = "Ukrainian", ["urd"] = "Urdu",
        ["uzb"] = "Uzbek", ["vie"] = "Vietnamese", ["yid"] = "Yiddish",
        ["yor"] = "Yoruba"
    };

    public static string GetLangDisplayName(string tag) =>
        KnownLangNames.TryGetValue(tag, out var name) ? $"{name} ({tag})" : tag;
}
