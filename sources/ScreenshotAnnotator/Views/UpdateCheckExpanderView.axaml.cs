using Avalonia.Controls;
using ScreenshotAnnotator.Services;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Views;

public partial class UpdateCheckExpanderView : UserControl
{
    public UpdateCheckExpanderView()
    {
        InitializeComponent();
        IsVisible = false;
#if RELEASE
        _ = CheckForUpdates();
#endif
    }

    private async Task CheckForUpdates()
    {
        var update = await UpdateChecker.CheckForUpdateGithub();

        if (update.HasUpdate)
        {
            this.updateNews.Text = update.Changes;
            this.updateNewsTitle.Text = string.Format("New {0} version is available.", update.Version);
            IsVisible = true;
        }
    }
}
