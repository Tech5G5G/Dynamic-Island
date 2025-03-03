using WinUIEx;

namespace Dynamic_Island.Widgets
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddWidgetWindow : ThemeWindow
    {
        public NavigationViewItem[] Items =
        [
            new() { Icon = new FontIcon { Glyph = "\uE950" }, Content = "CPU", Tag = WidgetType.CPU },
            new() { Icon = new FontIcon { Glyph = "\uE7F4" }, Content = "GPU", Tag = WidgetType.GPU },
            new() { Icon = new FontIcon { Glyph = "\uE964" }, Content = "RAM", Tag = WidgetType.RAM },
            new() { Icon = new FontIcon { Glyph = "\uEDA2" }, Content = "Disk", Tag = WidgetType.Disk },
            new() { Icon = new FontIcon { Glyph = "\uE968" }, Content = "Network", Tag = WidgetType.Network },
            new() { Icon = new FontIcon { Glyph = "\uE8D6" }, Content = "Now Playing", Tag = WidgetType.NowPlaying }
        ];

        /// <summary>The currently being dragged <see cref="CoreWidget"/> from the window.</summary>
        public CoreWidget DraggedWidget { get; private set; }
        /// <summary>Invoked when a <see cref="CoreWidget"/> from the window initiates a drag operation.</summary>
        public event Action<CoreWidget> DragWidget;

        public AddWidgetWindow()
        {
            this.InitializeComponent();
            this.CenterOnScreen();
            SetTitleBar(titleBar);

            navView.MenuItemsSource = Items;
            navView.SelectedItem = Items[0];
            Closed += (s, e) => App.AddWidgetWindow = null;
        }

        protected override RectInt32[] CaptionCutoutsRequested(double scale)
        {
            Rect closeRect = new(10 * scale, 11 * scale, 12 * scale, 12 * scale);
            Rect maxRect = new(30 * scale, 11 * scale, 12 * scale, 12 * scale);
            Rect minRect = new(50 * scale, 11 * scale, 12 * scale, 12 * scale);

            return [closeRect.ToRectInt32(), maxRect.ToRectInt32(), minRect.ToRectInt32()];
        }
        private void Close_PointerPressed(object sender, PointerRoutedEventArgs e) => (sender as Microsoft.UI.Xaml.Shapes.Ellipse).Fill = Brushes.DarkIndianRed;
        private void Close_PointerExited(object sender, PointerRoutedEventArgs e) => (sender as Microsoft.UI.Xaml.Shapes.Ellipse).Fill = Brushes.IndianRed;
        private void Close_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            (sender as Microsoft.UI.Xaml.Shapes.Ellipse).Fill = Brushes.IndianRed;
            Close();
        }

        /// <summary>Invoked when the user has picked a widget to add.</summary>
        public event Action<WidgetType, WidgetSize> WidgetPicked;

        private void SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = args.SelectedItem as NavigationViewItem;
            var page = new AddWidgetPage((string)item.Content, (WidgetType)item.Tag, WidgetPicked);

            widgetPreview.Content = page;
            page.DragWidget += (sender) => 
            {
                DragWidget?.Invoke(sender);
                DraggedWidget = sender;
                sender.DropCompleted += (s, e) => DraggedWidget = null;
            };
        }
    }

    public sealed partial class AddWidgetPage : Page
    {
        /// <summary>Invoked when the page's <see cref="CoreWidget"/> initiates a drag operation.</summary>
        public event Action<CoreWidget> DragWidget;

        /// <summary>Initializes a new instance of the <see cref="AddWidgetPage"/> class.</summary>
        /// <param name="title">The title of the page.</param>
        /// <param name="type">The type of widget to display.</param>
        public AddWidgetPage(string title, WidgetType type, Action<WidgetType, WidgetSize> picked)
        {
            var widget = (UIElement)Activator.CreateInstance(CoreWidget.WidgetTypes[type]);
            widget.CanDrag = true;
            widget.DragStarting += (s, e) => DragWidget?.Invoke((CoreWidget)s);

            Content = new StackPanel
            {
                Spacing = 24,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    new TextBlock
                    {
                        Text = title,
                        FontSize = 32,
                        Margin = new(0, 0, 0, -6),
                        FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    widget,
                    new Button
                    {
                        Content = "Add",
                        Style = (Style)Resources["AccentButtonStyle"],
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom
                    }.AddClick((s, e) =>
                    {
                        picked?.Invoke(type, WidgetSize.Small);
                        App.AddWidgetWindow.Close();
                    })
                }
            };
        }
    }
}
