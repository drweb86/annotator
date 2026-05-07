using System.Text;
using System.Xml;

// =============================================================================
// TECHNICAL KEYS IN RESX FILES (DO NOT TRANSLATE)
// =============================================================================
// This tool reads special "_Technical_*" keys from .resx files to generate
// language-specific output files. These keys should NOT be translated - they
// contain technical identifiers used by external systems.
//
// Available technical keys:
//
// 1. _Technical_WingetLocale
//    - Purpose: Winget package manager locale identifier
//    - Example values: "en-US", "de-DE", "zh-CN", "pt-BR"
//    - Used by: WingetLocaleGenerator to create locale.*.yaml files
//    - Required for: All languages that should have winget locale files
//
// 2. _Technical_NsisLanguage
//    - Purpose: NSIS installer language name (must match NSIS built-in names)
//    - Example values: "English", "German", "SimpChinese", "PortugueseBR"
//    - Used by: NsisLanguageGenerator to create setup-languages.nsh
//    - Required for: Only languages supported by NSIS installer
//    - Reference: https://nsis.sourceforge.io/docs/Chapter5.html#langsinst
//
// Adding a new language:
//   1. Create Strings.{culture}.resx file
//   2. Add _Technical_WingetLocale with the appropriate locale code
//   3. Add _Technical_NsisLanguage ONLY if NSIS supports that language
//   4. Add Winget_ShortDescription and Winget_Description translations
//   5. Add Installer_* translations if NSIS language is supported
//   6. Run ResxSorter to generate output files
// =============================================================================

namespace Codice.SortResX
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            var sourceDir = Directory.GetCurrentDirectory();
            while (Path.GetFileName(sourceDir) != "sources")
                sourceDir = Directory.GetParent(sourceDir)!.FullName;
            var repoRoot = Directory.GetParent(sourceDir)!.FullName;

            AppIconUpdater.Update(repoRoot);

            var localizationDir = Path.Combine(sourceDir, "ScreenshotAnnotator", "Resources");

            var dictionary = new Dictionary<string, int>();
            var allResx = Directory
                .GetFiles(localizationDir, "*.resx")
                .OrderBy(x => x.Length);
            var mainFile = allResx.First();

            foreach (var resx in Directory
                .GetFiles(localizationDir, "*.resx")
                .OrderBy(x => x.Length))
            {
                Console.WriteLine($"Sorting {resx}");
                int countKeys = new FileProcessor(resx)
                    .Process();

                dictionary.Add(resx, countKeys);

                if (countKeys != dictionary[mainFile])
                {
                    var percent = (countKeys * 100.0) / (dictionary[mainFile] * 1.0);
                    if (percent < 95)
                    {
                        throw new Exception($"{Path.GetFileNameWithoutExtension(resx)} needs attention {percent}.");
                    }
                }
            }

            NsisLanguageGenerator.Generate(sourceDir, repoRoot);
            WingetLocaleGenerator.Generate(sourceDir);
        }
    }

    public class FileProcessor
    {
        public FileProcessor(string path)
        {
            mPath = path;
            mResourceNameList = new List<string>();
            mResourceNodes = new Dictionary<string, XmlNode>();
            mDoc = new XmlDocument();
            mDoc.Load(mPath);
        }

        public int Process()
        {
            ExtractResources("data/@name");
            var sortedNames = SortResourceList();
            WriteOrderedResources(sortedNames);
            return sortedNames.Count();
        }

        void ExtractResources(string query)
        {
            var nodesFileNames = Array.Empty<string>();

            foreach (XmlAttribute attribute in mDoc.DocumentElement!.SelectNodes(query)!)
            {
                var element = attribute.OwnerElement!;
                if (nodesFileNames.Contains(attribute.Value))
                {
                    foreach (XmlNode child in element.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Element)
                        {
                            var value = child.InnerText;

                            if (Path.GetInvalidPathChars().Any(x => value.Contains(x)) ||
                                Path.GetInvalidFileNameChars().Any(x => value.Contains(x)))
                                throw new Exception($"{attribute.Name} contains invalid path chars");
                        }
                    }

                }
                AddXmlNode(element, attribute);
                element.ParentNode!.RemoveChild(element);
            }
        }

        void AddXmlNode(XmlNode node, XmlAttribute attribute)
        {
            if (mResourceNodes.ContainsKey(attribute.Value.ToString()))
                return;

            mResourceNodes.Add(attribute.Value.ToString(), node);
            mResourceNameList.Add(attribute.Value.ToString());
        }

        string[] SortResourceList()
        {
            string[] names = new string[mResourceNameList.Count];

            for (int i = 0; i < mResourceNameList.Count; i++)
                names[i] = mResourceNameList[i];

            Array.Sort(names);
            return names;
        }

        void WriteOrderedResources(string[] names)
        {
            foreach (string key in names)
            {
                mDoc.DocumentElement!.AppendChild(mResourceNodes[key]);
            }

            mDoc.Save(mPath);
        }

        private List<string> mResourceNameList = null!;
        private Dictionary<string, XmlNode> mResourceNodes = null!;
        private XmlDocument mDoc = null!;
        private string mPath = null!;
    }

    public static class NsisLanguageGenerator
    {
        public static void Generate(string sourceDir, string repoRoot)
        {
            var localizationDir = Path.Combine(sourceDir, "ScreenshotAnnotator", "Resources");
            var outputPath = Path.Combine(repoRoot, "scripts", "setup-languages.nsh");

            var allResx = Directory.GetFiles(localizationDir, "*.resx")
                .OrderBy(x => x.Length);

            var languageData = new Dictionary<string, (string NsisLanguage, Dictionary<string, string> Entries)>();

            foreach (var resxPath in allResx)
            {
                var doc = new XmlDocument();
                doc.Load(resxPath);

                string? nsisLanguage = null;
                var entries = new Dictionary<string, string>();

                foreach (XmlNode node in doc.SelectNodes("//data")!)
                {
                    var name = node.Attributes?["name"]?.Value;
                    if (name == "_Technical_NsisLanguage")
                        nsisLanguage = node.SelectSingleNode("value")?.InnerText;
                    else if (name != null && (name.StartsWith("Installer_") || name == "App_DisplayName"))
                    {
                        var value = node.SelectSingleNode("value")?.InnerText ?? "";
                        entries[name] = value;
                    }
                }

                if (string.IsNullOrEmpty(nsisLanguage))
                    continue;

                var culture = ExtractCulture(resxPath);
                languageData[culture] = (nsisLanguage, entries);
            }

            if (!languageData.TryGetValue("", out var mainData) || mainData.Entries.Count == 0)
            {
                Console.WriteLine("No Installer_ keys found in main resources, skipping NSH generation.");
                return;
            }

            if (!mainData.Entries.ContainsKey("App_DisplayName"))
                throw new Exception("App_DisplayName is required in main resources for NSIS generation.");

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            using var writer = new StreamWriter(outputPath, false, new UTF8Encoding(false));
            writer.WriteLine("; Auto-generated by ResxSorter from .resx resources - do not edit manually");
            writer.WriteLine();

            writer.WriteLine($"!insertmacro MUI_LANGUAGE \"English\"");
            foreach (var kvp in languageData.Where(x => x.Key != "").OrderBy(x => x.Value.NsisLanguage))
            {
                writer.WriteLine($"!insertmacro MUI_LANGUAGE \"{kvp.Value.NsisLanguage}\"");
            }
            writer.WriteLine();

            foreach (var key in mainData.Entries.Keys.OrderBy(x => x))
            {
                foreach (var kvp in languageData.OrderBy(x => x.Value.NsisLanguage == "English" ? "" : x.Value.NsisLanguage))
                {
                    string? value = null;
                    kvp.Value.Entries.TryGetValue(key, out value);
                    value ??= mainData.Entries[key];
                    if (key == "App_DisplayName")
                        ValidateShortcutDisplayName(value, kvp.Key == "" ? "default" : kvp.Key);

                    var escapedValue = EscapeForNsis(value);
                    writer.WriteLine($"LangString {key} ${{LANG_{kvp.Value.NsisLanguage.ToUpperInvariant()}}} \"{escapedValue}\"");
                }
                writer.WriteLine();
            }

            Console.WriteLine($"Generated {outputPath}");
        }

        private static string ExtractCulture(string resxPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(resxPath);
            var dotIndex = fileName.IndexOf('.');
            return dotIndex >= 0 ? fileName[(dotIndex + 1)..] : "";
        }

        private static string EscapeForNsis(string value)
        {
            var sb = new StringBuilder();
            foreach (var ch in value)
            {
                switch (ch)
                {
                    case '$': sb.Append("$$"); break;
                    case '"': sb.Append("$\\\""); break;
                    case '\n': sb.Append("$\\n"); break;
                    case '\r': break;
                    case '\t': sb.Append("$\\t"); break;
                    case '`': sb.Append("$\\`"); break;
                    default: sb.Append(ch); break;
                }
            }
            return sb.ToString();
        }

        private static void ValidateShortcutDisplayName(string value, string culture)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"App_DisplayName is empty for culture '{culture}'.");

            if (value.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                throw new Exception($"App_DisplayName contains invalid file name characters for culture '{culture}'.");
        }
    }

    public static class AppIconUpdater
    {
        public static void Update(string repoRoot)
        {
            var scriptsDir = Path.Combine(repoRoot, "scripts");
            if (!Directory.Exists(scriptsDir))
                return;

            var catFiles = Directory.GetFiles(scriptsDir, "Cat-*.png");
            if (catFiles.Length == 0)
                return;

            var selected = catFiles[new Random().Next(catFiles.Length)];
            Console.WriteLine($"Selected cat icon: {Path.GetFileName(selected)}");

            string[] pngTargets =
            [
                Path.Combine(scriptsDir, "App.png"),
                Path.Combine(repoRoot, "sources", "ScreenshotAnnotator", "Assets", "App.png"),
            ];

            foreach (var target in pngTargets)
            {
                File.Copy(selected, target, overwrite: true);
                Console.WriteLine($"Updated {target}");
            }

            string[] icoTargets =
            [
                Path.Combine(scriptsDir, "App.ico"),
                Path.Combine(repoRoot, "sources", "ScreenshotAnnotator.Desktop", "App.ico"),
            ];

            foreach (var target in icoTargets)
            {
                WritePngAsIco(selected, target);
                Console.WriteLine($"Updated {target}");
            }
        }

        static void WritePngAsIco(string pngPath, string icoPath)
        {
            var pngData = File.ReadAllBytes(pngPath);
            // PNG IHDR chunk starts at byte 16: 4-byte width, 4-byte height (big-endian)
            int width  = (pngData[16] << 24) | (pngData[17] << 16) | (pngData[18] << 8) | pngData[19];
            int height = (pngData[20] << 24) | (pngData[21] << 16) | (pngData[22] << 8) | pngData[23];

            using var stream = new FileStream(icoPath, FileMode.Create, FileAccess.Write);
            using var writer = new BinaryWriter(stream);

            // ICONDIR (6 bytes)
            writer.Write((short)0);  // reserved
            writer.Write((short)1);  // type: ICO
            writer.Write((short)1);  // image count

            // ICONDIRENTRY (16 bytes) — offset 22 = 6 + 16
            writer.Write((byte)(width  >= 256 ? 0 : width));
            writer.Write((byte)(height >= 256 ? 0 : height));
            writer.Write((byte)0);   // color count (0 = true color)
            writer.Write((byte)0);   // reserved
            writer.Write((short)1);  // planes
            writer.Write((short)32); // bit count
            writer.Write(pngData.Length);
            writer.Write(22);        // image data offset

            writer.Write(pngData);
        }
    }

    public static class WingetLocaleGenerator
    {
        public static void Generate(string sourceDir)
        {
            var localizationDir = Path.Combine(sourceDir, "ScreenshotAnnotator", "Resources");
            var wingetPkgsDir = Path.Combine(sourceDir, "tools", "winget-pkgs");

            var allResx = Directory.GetFiles(localizationDir, "*.resx")
                .OrderBy(x => x.Length);

            foreach (var resxPath in allResx)
            {
                var doc = new XmlDocument();
                doc.Load(resxPath);

                string? wingetLocale = null;
                string? shortDescription = null;
                string? description = null;

                foreach (XmlNode node in doc.SelectNodes("//data")!)
                {
                    var name = node.Attributes?["name"]?.Value;
                    if (name == "_Technical_WingetLocale")
                        wingetLocale = node.SelectSingleNode("value")?.InnerText;
                    else if (name == "Winget_ShortDescription")
                        shortDescription = node.SelectSingleNode("value")?.InnerText;
                    else if (name == "Winget_Description")
                        description = node.SelectSingleNode("value")?.InnerText;
                }

                if (string.IsNullOrWhiteSpace(wingetLocale))
                    continue;

                if (shortDescription == null || description == null)
                {
                    Console.WriteLine($"Missing Winget_ keys in {resxPath}, skipping locale generation.");
                    continue;
                }

                var culture = ExtractCulture(resxPath);
                var isDefaultLocale = culture == "";
                var schemaType = isDefaultLocale ? "defaultLocale" : "locale";
                var manifestType = isDefaultLocale ? "defaultLocale" : "locale";
                var outputFileName = $"SiarheiKuchuk.ScreenshotAnnotator.locale.{wingetLocale}.yaml";
                var outputPath = Path.Combine(wingetPkgsDir, outputFileName);

                using var writer = new StreamWriter(outputPath, false, new UTF8Encoding(false));
                writer.WriteLine($"# yaml-language-server: $schema=https://aka.ms/winget-manifest.{schemaType}.1.12.0.schema.json");
                writer.WriteLine();
                writer.WriteLine("PackageIdentifier: SiarheiKuchuk.ScreenshotAnnotator");
                writer.WriteLine("PackageVersion: APP_VERSION_STRING");
                writer.WriteLine($"PackageLocale: {wingetLocale}");
                writer.WriteLine("Publisher: Siarhei Kuchuk");
                writer.WriteLine("PublisherUrl: https://github.com/drweb86");
                writer.WriteLine("PublisherSupportUrl: https://github.com/drweb86/annotator/issues");
                writer.WriteLine("PrivacyUrl: https://raw.githubusercontent.com/drweb86/annotator/refs/heads/main/PRIVACY_POLICY.md");
                writer.WriteLine("Author: Siarhei Kuchuk");
                writer.WriteLine("PackageName: ScreenshotAnnotator");
                writer.WriteLine("PackageUrl: https://github.com/drweb86/annotator");
                writer.WriteLine("License: MIT, GPL, MSPL");
                writer.WriteLine("LicenseUrl: https://raw.githubusercontent.com/drweb86/annotator/refs/heads/main/LICENSE.md");
                writer.WriteLine("Copyright: 2025-CURRENT_YEAR Siarhei Kuchuk");
                writer.WriteLine("CopyrightUrl: https://raw.githubusercontent.com/drweb86/annotator/refs/heads/main/LICENSE.md");
                writer.WriteLine($"ShortDescription: {YamlDoubleQuoted(shortDescription)}");
                writer.WriteLine("Description: |");
                foreach (var line in description.Split('\n'))
                {
                    var trimmedLine = line.TrimEnd('\r');
                    writer.WriteLine($"  {trimmedLine}");
                }
                if (isDefaultLocale)
                    writer.WriteLine("Moniker: screenshotannotator");
                writer.WriteLine("Tags:");
                writer.WriteLine("- screenshot");
                writer.WriteLine("- annotate");
                writer.WriteLine("ReleaseNotesUrl: https://raw.githubusercontent.com/drweb86/annotator/refs/heads/main/CHANGELOG.md");
                writer.WriteLine($"ManifestType: {manifestType}");
                writer.WriteLine("ManifestVersion: 1.12.0");

                Console.WriteLine($"Generated {outputPath}");
            }
        }

        private static string ExtractCulture(string resxPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(resxPath);
            var dotIndex = fileName.IndexOf('.');
            return dotIndex >= 0 ? fileName[(dotIndex + 1)..] : "";
        }

        // Double-quoted: plain YAML scalars fail on embedded ':' (e.g. trailing ':' in translations).
        private static string YamlDoubleQuoted(string value)
        {
            var sb = new StringBuilder(value.Length + 2);
            sb.Append('"');
            foreach (var ch in value)
            {
                switch (ch)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '"': sb.Append("\\\""); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': break;
                    case '\t': sb.Append("\\t"); break;
                    default: sb.Append(ch); break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }
    }
}
