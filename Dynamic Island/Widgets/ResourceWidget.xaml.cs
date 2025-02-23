namespace Dynamic_Island.Widgets
{
    public abstract partial class ResourceWidget : CoreWidget
    {
        int seconds = 0;
        public ResourceWidget()
        {
            this.InitializeComponent();

            WidgetSizeChanged += (size) =>
            {
                //TODO: Update size of graph
            };

            graph.GraphMargin = new(0, 0, -34, 0);
            button.Click += (s, e) => ButtonClicked?.Invoke(s, e);
            Tick += async () =>
            {
                primaryText.Text = await PrimaryTextRequested(primaryText);
                secondaryText.Text = SecondaryTextRequested(secondaryText);
                graph.AddPoint(seconds++, DataRequested(graph));
            };
        }
        /// <summary>Fired every second to update the text in the primary <see cref="TextBlock"/>.</summary>
        /// <param name="textBlock">The <see cref="TextBlock"/> to update.</param>
        /// <returns>The text to display in <paramref name="textBlock"/>, asynchronously.</returns>
        protected abstract Task<string> PrimaryTextRequested(TextBlock textBlock);
        /// <summary>Fired every second to update the text in the secondary <see cref="TextBlock"/>.</summary>
        /// <param name="textBlock">The <see cref="TextBlock"/> to update.</param>
        /// <returns>The text to display in <paramref name="textBlock"/>.</returns>
        protected abstract string SecondaryTextRequested(TextBlock textBlock);
        /// <summary>Fired every second to add a point to the <see cref="ResourceGraph"/>/</summary>
        /// <param name="graph">The <see cref="ResourceGraph"/> to add the point to.</param>
        /// <returns>A <see cref="double"/> representing the Y coordinate for the next point.</returns>
        protected abstract double DataRequested(ResourceGraph graph);

        /// <summary>Gets or sets the visibility of the displayable button.</summary>
        public Visibility ButtonVisibility
        {
            get => button.Visibility;
            set => button.Visibility = value;
        }
        /// <summary>Gets or sets the content of the displayable button.</summary>
        public object ButtonContent
        {
            get => button.Content;
            set => button.Content = value;
        }
        /// <summary>Invoked when the displayable button is clicked.</summary>
        public event RoutedEventHandler ButtonClicked;

        /// <summary>Gets or sets the color of the inner <see cref="ResourceGraph"/>.</summary>
        public SkiaSharp.SKColor Color
        {
            get => graph.Color;
            set => graph.Color = value;
        }

        /// <summary>Clears the inner <see cref="ResourceGraph"/>.</summary>
        public void ClearGraph() => graph.Clear();

        private static event Action Tick;
        private static readonly DispatcherTimer Timer = CreateTimer();
        private static DispatcherTimer CreateTimer()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, e) => Tick?.Invoke();
            timer.Start();
            return timer;
        }
    }
}
