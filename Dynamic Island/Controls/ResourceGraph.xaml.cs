using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

namespace Dynamic_Island.Controls
{
    public partial class ResourceGraph : UserControl
    {
        const float LineThickness = 3;
        static readonly LiveChartsCore.SkiaSharpView.Painting.Effects.DashEffect DashEffect = new([3, 3]);

        readonly BindableProperty<ObservableCollection<ISeries>> Series = new([]);

        public ResourceGraph()
        {
            this.InitializeComponent();

            Chart.XAxes = [XAxis = new LiveChartsCore.SkiaSharpView.Axis() { IsVisible = false }];
            Chart.YAxes = [YAxis = new LiveChartsCore.SkiaSharpView.Axis() { IsVisible = false, MinLimit = 0, MaxLimit = 100 }];
        }

        /// <summary>Gets or sets the color of the graph.</summary>
        public SKColor Color
        {
            get => color;
            set
            {
                color = value;
                foreach (var line in Series.Value)
                {
                    var series = line as LineSeries<ObservablePoint>;
                    bool applyEffect = (series.Stroke as SolidColorPaint).PathEffect is not null;
                    series.Stroke = new SolidColorPaint(value, LineThickness) { PathEffect = applyEffect ? DashEffect : null };
                    series.Fill = new SolidColorPaint(new(value.Red, value.Green, value.Blue, 0x32));
                }
            }
        }
        private SKColor color = new(0, 0xFF, 0);

        /// <summary>Gets or sets the margin from the view bounds to the graph itself.</summary>
        public LiveChartsCore.Measure.Margin GraphMargin
        {
            get => Chart.DrawMargin;
            set => Chart.DrawMargin = value;
        }

        /// <summary>The X axis of the graph.</summary>
        public LiveChartsCore.SkiaSharpView.Axis XAxis { get; private set; }
        /// <summary>The Y axis of the graph.</summary>
        public LiveChartsCore.SkiaSharpView.Axis YAxis { get; private set; }

        /// <summary>Adds a line to the graph.</summary>
        /// <param name="dashed">Whether the line should be dashed.</param>
        /// <returns>The index of the line in <see cref="Lines"/>.</returns>
        public int AddLine(bool dashed)
        {
            Series.Value.Add(new LineSeries<ObservablePoint>()
            {
                Values = new ObservableCollection<ObservablePoint>(),
                GeometryStroke = null,
                GeometryFill = null,
                Stroke = new SolidColorPaint(color, LineThickness) { PathEffect = dashed ? DashEffect : null },
                Fill = new SolidColorPaint(new(color.Red, color.Green, color.Blue, 0x32))
            });
            return Series.Value.Count - 1;
        }

        /// <summary>Adds a point to the graph. If the line is 0, also updates the bounds of the X axis.</summary>
        /// <param name="line">The line to add the point to.</param>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        public void AddPoint(int line, double x, double y)
        {
            (Series.Value[line].Values as ObservableCollection<ObservablePoint>).Add(new(x, y));
            if (line == 0)
                XAxis.MinLimit = x - 10;
        }

        /// <summary>Clears the graph, removing all points.</summary>
        public void Clear()
        {
            foreach (var line in Series.Value)
                (line.Values as ObservableCollection<ObservablePoint>).Clear();
        }
    }
}
