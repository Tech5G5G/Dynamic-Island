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
            foreach (bool dashed in LinesRequested(graph))
                graph.AddLine(dashed);

            button.Click += (s, e) => ButtonClicked?.Invoke(s, e);
            Tick += async () =>
            {
                await UpdateGraph(graph, seconds++);
                primaryText.Text = PrimaryTextRequested(primaryText);
                secondaryText.Text = SecondaryTextRequested(secondaryText);
            };
        }

        /// <summary>Fired every second to add a point to the primary line on the <see cref="ResourceGraph"/>.</summary>
        /// <param name="graph">The <see cref="ResourceGraph"/> to add the point to.</param>
        /// <returns>A <see cref="double"/> representing the Y coordinate for the next point, asynchronously.</returns>
        protected abstract Task<double> DataRequested(ResourceGraph graph);
        /// <summary>Fired every second to update the text in the primary <see cref="TextBlock"/>.</summary>
        /// <param name="textBlock">The <see cref="TextBlock"/> to update.</param>
        /// <returns>The text to display in <paramref name="textBlock"/>.</returns>
        protected abstract string PrimaryTextRequested(TextBlock textBlock);
        /// <summary>Fired every second to update the text in the secondary <see cref="TextBlock"/>.</summary>
        /// <param name="textBlock">The <see cref="TextBlock"/> to update.</param>
        /// <returns>The text to display in <paramref name="textBlock"/>.</returns>
        protected abstract string SecondaryTextRequested(TextBlock textBlock);

        /// <summary>Fired to get the lines for the <see cref="ResourceGraph"/>.</summary>
        /// <param name="graph">The <see cref="ResourceGraph"/> to add the lines to.</param>
        /// <returns>An array of <see cref="bool"/> representing whether the lines should be dashed or not.</returns>
        protected virtual bool[] LinesRequested(ResourceGraph graph) => [false];
        /// <summary>Fired every second to update the values for the <see cref="ResourceGraph"/>.</summary>
        /// <param name="graph">The <see cref="ResourceGraph"/> to update.</param>
        /// <param name="seconds">The amount of seconds passed since the widget has been created.</param>
        protected virtual async Task UpdateGraph(ResourceGraph graph, int seconds) => graph.AddPoint(0, seconds, await DataRequested(graph));

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
