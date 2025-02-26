using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinUI;
using LiveChartsCore.SkiaSharpView.Painting;

namespace Dynamic_Island.Controls
{
    public partial class ResourceGraph : UserControl
    {
        /// <summary>The default thickness of the line(s) on the graph.</summary>
        protected const float StrokeThickness = 3;
        /// <summary>The X axis of the graph.</summary>
        protected LiveChartsCore.SkiaSharpView.Axis XAxis { get; private set; }

        BindableProperty<ISeries[]> Series { get; } = new([]);

        LineSeries<ObservablePoint> line;
        readonly ObservableCollection<ObservablePoint> values = [];

        public ResourceGraph()
        {
            this.InitializeComponent();

            Chart.XAxes = [XAxis = new LiveChartsCore.SkiaSharpView.Axis() { IsVisible = false }];
            Chart.YAxes = [new LiveChartsCore.SkiaSharpView.Axis() { IsVisible = false, MinLimit = 0, MaxLimit = 100 }];
            Series.Value = LinesRequested(Chart);
        }

        /// <summary>Fired when the lines of the graph are requested.</summary>
        /// <param name="chart">The <see cref="CartesianChart"/> that uses the lines.</param>
        /// <returns>An array of <see cref="ISeries"/> that contains lines for <paramref name="chart"/>.</returns>
        protected virtual ISeries[] LinesRequested(CartesianChart chart) =>
        [
            line = new()
            {
                Values = values,
                GeometryStroke = null,
                GeometryFill = null,
                Stroke = new SolidColorPaint(new(0, 0xFF, 0), StrokeThickness),
                Fill = new SolidColorPaint(new(0, 0xFF, 0, 0x32))
            }
        ];

        /// <summary>Gets or sets the color of the graph.</summary>
        public virtual SKColor Color
        {
            get => (line.Stroke as SolidColorPaint).Color;
            set
            {
                line.Stroke = new SolidColorPaint(value, StrokeThickness);
                line.Fill = new SolidColorPaint(new(value.Red, value.Green, value.Blue, 0x32));
            }
        }

        /// <summary>Gets or sets the margin from the view bounds and the graph itself.</summary>
        public LiveChartsCore.Measure.Margin GraphMargin
        {
            get => Chart.DrawMargin;
            set => Chart.DrawMargin = value;
        }

        /// <summary>Adds a point to the graph.</summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        public virtual void AddPoint(double x, double y)
        {
            values.Add(new(x, y));
            XAxis.MinLimit = x - 10;
        }

        /// <summary>Clears the graph, removing all points.</summary>
        public virtual void Clear() => values.Clear();
    }
}
