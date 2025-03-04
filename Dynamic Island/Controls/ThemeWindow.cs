using Microsoft.UI.Xaml.Shapes;
using Windows.UI.ViewManagement;

namespace Dynamic_Island.Controls
{
    [Microsoft.UI.Xaml.Markup.ContentProperty(Name = nameof(Content))]
    public abstract class ThemeWindow : Window
    {
        private static class Brushes
        {
            public static SolidColorBrush IndianRed { get; } = new(Colors.IndianRed);
            public static SolidColorBrush DarkIndianRed { get; } = new(new() { A = 0xFF, R = 0x8C, G = 0x3F, B = 0x3F });
        }

        WindowManager manager;
        InputNonClientPointerSource input;
        readonly UISettings uiSettings = new();

        ContentPresenter presenter;

        /// <summary>Gets or sets the width of the window.</summary>
        public double Width
        {
            get => manager.Width;
            set => manager.Width = value;
        }
        /// <summary>Gets or sets the height of the window.</summary>
        public double Height
        {
            get => manager.Height;
            set => manager.Height = value;
        }

        /// <summary>Gets or sets the root visual of the window.</summary>
        public new object Content
        {
            get => presenter.Content;
            set => presenter.Content = value;
        }

        public ThemeWindow()
        {
            WindowHelper.ApplyOverlayProperties(this);
            manager = WindowManager.Get(this);
            base.Content = presenter = new();

            SetTheme();
            UpdateTitlebar();
        }

        private ElementTheme GetCurrentTheme() => uiSettings.GetColorValue(UIColorType.Background) == Colors.Black ? ElementTheme.Dark : ElementTheme.Light;
        private void SetTheme()
        {
            SystemBackdrop = new DesktopAcrylicBackdrop();
            var theme = GetCurrentTheme();
            (base.Content as FrameworkElement).RequestedTheme = theme;
            _ = PInvoke.SetPreferredAppMode(theme == ElementTheme.Dark ? PreferredAppMode.Dark : PreferredAppMode.Light);
            uiSettings.ColorValuesChanged += UpdateUIColor;
            Closed += (s, e) => uiSettings.ColorValuesChanged -= UpdateUIColor;
        }
        private void UpdateUIColor(UISettings sender, object e)
        {
            var theme = GetCurrentTheme();
            DispatcherQueue.TryEnqueue(() => (base.Content as FrameworkElement).RequestedTheme = theme);
            _ = PInvoke.SetPreferredAppMode(theme == ElementTheme.Dark ? PreferredAppMode.Dark : PreferredAppMode.Light);
        }

        private void UpdateTitlebar()
        {
            ExtendsContentIntoTitleBar = true;
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Collapsed;

            input = InputNonClientPointerSource.GetForWindowId(AppWindow.Id);
            input.ExitedMoveSize += (s, e) => DrawCaptionCutouts();
            Activated += (s, e) => DrawCaptionCutouts();

            var content = base.Content as FrameworkElement;
            content.Loaded += (s, e) => DrawCaptionCutouts();
            content.SizeChanged += (s, e) => DrawCaptionCutouts();
        }
        private void DrawCaptionCutouts()
        {
            if (AppWindow.IsVisible && base.Content.XamlRoot is XamlRoot root)
                input.SetRegionRects(NonClientRegionKind.Passthrough, CaptionCutoutsRequested(root.RasterizationScale));
        }
        /// <summary>Fired when the passthrough areas of the window title bar are requested.</summary>
        /// <param name="scale">The current <see cref="XamlRoot.RasterizationScale"/> of the window.</param>
        /// <returns>An array of <see cref="RectInt32"/> containing the passthrough areas for the title bar.</returns>
        protected abstract RectInt32[] CaptionCutoutsRequested(double scale);

        /// <summary>Sets the close button of the window.</summary>
        /// <param name="shape">The <see cref="Shape"/> to set the close button as.</param>
        public void SetCloseButton(Shape shape)
        {
            shape.PointerPressed += (s, e) => shape.Fill = Brushes.DarkIndianRed;
            shape.PointerExited += (s, e) => shape.Fill = Brushes.IndianRed;
            shape.PointerReleased += (s, e) =>
            {
                shape.Fill = Brushes.IndianRed;
                Close();
            };
        }
    }
}
