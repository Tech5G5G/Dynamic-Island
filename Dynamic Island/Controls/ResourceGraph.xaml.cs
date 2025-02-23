using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace Dynamic_Island.Controls
{
    public sealed partial class ResourceGraph : UserControl
    {
        const float StrokeThickness = 3;

        BindableProperty<ISeries[]> Series { get; set; } = new([]);
        readonly ObservableCollection<ObservablePoint> values = [];

        readonly LineSeries<ObservablePoint> line;
        readonly LiveChartsCore.SkiaSharpView.Axis xAxis;

        public ResourceGraph()
        {
            this.InitializeComponent();

            Chart.XAxes = [xAxis = new LiveChartsCore.SkiaSharpView.Axis() { IsVisible = false }];
            Chart.YAxes = [new LiveChartsCore.SkiaSharpView.Axis() { IsVisible = false, MinLimit = 0, MaxLimit = 100 }];
            Series.Value =
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
        }

        /// <summary>Gets or sets the color of the graph.</summary>
        public SKColor Color
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
        public void AddPoint(double x, double y)
        {
            values.Add(new(x, y));
            xAxis.MinLimit = x - 10;
        }

        /// <summary>Clears the graph, removing all points.</summary>
        public void Clear() => values.Clear();
    }
}
