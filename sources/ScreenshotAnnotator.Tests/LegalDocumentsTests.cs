using ScreenshotAnnotator.Legal;
using Xunit;

namespace ScreenshotAnnotator.Tests;

public class LegalDocumentsTests
{
    [Fact]
    public void License_english_markdown_loads()
    {
        var markdown = LicenseDocuments.LoadMarkdown("en.md");
        Assert.Contains("Screenshot Annotator", markdown);
        Assert.Contains("CC0 1.0 Universal", markdown);
        Assert.NotEmpty(PrivacyMarkdown.Parse(markdown));
    }

    [Fact]
    public void License_unknown_language_falls_back_to_english()
    {
        var english = LicenseDocuments.LoadMarkdown("en.md");
        var missing = LicenseDocuments.LoadMarkdown("does-not-exist.md");
        Assert.Equal(english, missing);
    }

    [Fact]
    public void Privacy_english_markdown_loads()
    {
        var markdown = PrivacyDocuments.LoadMarkdown("en.md");
        Assert.Contains("Screenshot Annotator", markdown);
        Assert.Contains("does not collect", markdown, StringComparison.OrdinalIgnoreCase);
        Assert.NotEmpty(PrivacyMarkdown.Parse(markdown));
    }

    [Fact]
    public void License_languages_include_annotator_cultures()
    {
        Assert.Contains(DocumentLanguages.License, l => l.Code == "en");
        Assert.Contains(DocumentLanguages.License, l => l.Code == "zh-CN");
        Assert.Contains(DocumentLanguages.License, l => l.Code == "zh-HK");
        Assert.Contains(DocumentLanguages.License, l => l.Code == "pcm-NG");
        Assert.Contains(DocumentLanguages.License, l => l.Code == "he" && l.Rtl);
    }

    [Fact]
    public void Privacy_languages_match_license_languages()
    {
        Assert.Equal(DocumentLanguages.License.Count, DocumentLanguages.Privacy.Count);
        Assert.Equal("en", DocumentLanguages.Privacy[0].Code);
    }

    [Fact]
    public void All_license_languages_have_embedded_markdown()
    {
        foreach (var language in DocumentLanguages.License)
        {
            var markdown = LicenseDocuments.LoadMarkdown(language.AssetFile);
            Assert.False(string.IsNullOrWhiteSpace(markdown), language.AssetFile);
            Assert.Contains("Screenshot Annotator", markdown);
        }
    }

    [Fact]
    public void All_privacy_languages_have_embedded_markdown()
    {
        foreach (var language in DocumentLanguages.Privacy)
        {
            var markdown = PrivacyDocuments.LoadMarkdown(language.AssetFile);
            Assert.False(string.IsNullOrWhiteSpace(markdown), language.AssetFile);
            Assert.Contains("Screenshot Annotator", markdown);
        }
    }
}
