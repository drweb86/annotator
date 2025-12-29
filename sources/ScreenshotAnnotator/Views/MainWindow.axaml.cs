using Avalonia.Controls;
using System;
using System.Runtime.InteropServices;

namespace ScreenshotAnnotator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public static bool CanExtendClientAreaToDecorationsHint => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}