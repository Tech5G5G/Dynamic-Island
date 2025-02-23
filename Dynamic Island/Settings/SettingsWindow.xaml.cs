using Windows.UI.ViewManagement;

namespace Dynamic_Island.Settings
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsWindow : Window
    {
        public static class Brushes
        {
            public static SolidColorBrush IndianRed { get; } = new(Colors.IndianRed);
            public static SolidColorBrush DarkIndianRed { get; } = new(new() { A = 0xFF, R = 0x8C, G = 0x3F, B = 0x3F });
        }

        readonly UISettings uiSettings = new();
        readonly InputNonClientPointerSource input;

        public SettingsWindow()
        {
            this.InitializeComponent();
            input = InputNonClientPointerSource.GetForWindowId(AppWindow.Id);

            ExtendsContentIntoTitleBar = true;
            AppWindow.SetPresenter(AppWindowPresenterKind.CompactOverlay);
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;
            SetTitleBar(titleBarArea);

            SetTheme();
            SetCaptionCutouts();
        }

        private ElementTheme GetCurrentTheme() => uiSettings.GetColorValue(UIColorType.Background) == Colors.Black ? ElementTheme.Dark : ElementTheme.Light;
        private void UpdateUIColor(UISettings sender, object e)
        {
            var theme = GetCurrentTheme();
            DispatcherQueue.TryEnqueue(() => (Content as FrameworkElement).RequestedTheme = theme);
            _ = PInvoke.SetPreferredAppMode(theme == ElementTheme.Dark ? PreferredAppMode.Dark : PreferredAppMode.Light);
        }
        private void SetTheme()
        {
            var theme = GetCurrentTheme();
            (Content as FrameworkElement).RequestedTheme = theme;
            _ = PInvoke.SetPreferredAppMode(theme == ElementTheme.Dark ? PreferredAppMode.Dark : PreferredAppMode.Light);
            uiSettings.ColorValuesChanged += UpdateUIColor;
            Closed += (s, e) =>
            {
                App.SettingsWindow = null;
                uiSettings.ColorValuesChanged -= UpdateUIColor;
            };
        }

        private static Windows.Graphics.RectInt32 ToRectInt32(Rect rect) => new((int)Math.Round(rect.X), (int)Math.Round(rect.Y), (int)Math.Round(rect.Width), (int)Math.Round(rect.Height));
        private void DrawCaptionCutouts()
        {
            if (!AppWindow.IsVisible || Content.XamlRoot is not XamlRoot root)
                return;

            double scale = root.RasterizationScale;
            Rect closeRect = new(10 * scale, 10 * scale, 12 * scale, 12 * scale);
            Rect maxRect = new(30 * scale, 10 * scale, 12 * scale, 12 * scale);
            Rect minRect = new(50 * scale, 10 * scale, 12 * scale, 12 * scale);

            input.SetRegionRects(NonClientRegionKind.Passthrough, [ToRectInt32(closeRect), ToRectInt32(maxRect), ToRectInt32(minRect)]);
        }
        private void SetCaptionCutouts()
        {
            Activated += (s, e) => DrawCaptionCutouts();
            input.ExitedMoveSize += (s, e) => DrawCaptionCutouts();

            var content = Content as FrameworkElement;
            content.Loaded += (s, e) => DrawCaptionCutouts();
            content.SizeChanged += (s, e) => DrawCaptionCutouts();
        }

        private void Close_PointerPressed(object sender, PointerRoutedEventArgs e) => (sender as Microsoft.UI.Xaml.Shapes.Ellipse).Fill = Brushes.DarkIndianRed;
        private void Close_PointerExited(object sender, PointerRoutedEventArgs e) => (sender as Microsoft.UI.Xaml.Shapes.Ellipse).Fill = Brushes.IndianRed;
        private void Close_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            (sender as Microsoft.UI.Xaml.Shapes.Ellipse).Fill = Brushes.IndianRed;
            Close();
        }

        private void ChangeTab(SelectorBar sender, SelectorBarSelectionChangedEventArgs e) => view.Navigate(sender.SelectedItem.Text switch
        {
            "Animations" => typeof(Animations),
            "Volume" => typeof(Volume),
            _ => typeof(Appearance)
        });
    }
}
