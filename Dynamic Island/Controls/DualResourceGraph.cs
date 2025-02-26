using SkiaSharp;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.WinUI;
using LiveChartsCore.SkiaSharpView.Painting;

namespace Dynamic_Island.Controls
{
    public sealed partial class DualResourceGraph : ResourceGraph
    {
        static readonly LiveChartsCore.SkiaSharpView.Painting.Effects.DashEffect dash = new([3, 3]);

        LineSeries<ObservablePoint> line1;
        readonly ObservableCollection<ObservablePoint> values1 = [];

        LineSeries<ObservablePoint> line2;
        readonly ObservableCollection<ObservablePoint> values2 = [];

        protected override ISeries[] LinesRequested(CartesianChart chart) =>
        [
            line1 = new()
            {
                Values = values1,
                GeometryStroke = null,
                GeometryFill = null,
                Stroke = new SolidColorPaint(new(0, 0xFF, 0), StrokeThickness),
                Fill = new SolidColorPaint(new(0, 0xFF, 0, 0x32))
            },
            line2 = new()
            {
                Values = values2,
                GeometryStroke = null,
                GeometryFill = null,
                Stroke = new SolidColorPaint(new(0, 0xFF, 0), StrokeThickness) { PathEffect = dash },
                Fill = new SolidColorPaint(new(0, 0xFF, 0, 0x32)),

            }
        ];

        /// <summary>Gets or sets the color of the graph.</summary>
        public override SKColor Color
        {
            get => (line1.Stroke as SolidColorPaint).Color;
            set
            {
                line1.Stroke = new SolidColorPaint(value, StrokeThickness);
                line2.Stroke = new SolidColorPaint(value, StrokeThickness) { PathEffect = dash };
                line1.Fill = line2.Fill = new SolidColorPaint(new(value.Red, value.Green, value.Blue, 0x32));
            }
        }

        /// <summary>Adds a point to the graph. Only updates the limits of the X axis if adding to line 1.</summary>
        /// <param name="x">The x coordinate of the point.</param>
        /// <param name="y">The y coordinate of the point.</param>
        /// <param name="line1">A <see langword="bool"/> representing whether you're adding to line 1 or 2.</param>
        public void AddPoint(double x, double y, bool line1)
        {
            (line1 ? values1 : values2).Add(new(x, y));
            if (line1)
                XAxis.MinLimit = x - 10;
        }

        /// <summary>Adds a point to line 1.</summary>
        [Obsolete("Method not supported in this class. Use AddPoint(double, double, bool) overload instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void AddPoint(double x, double y) => AddPoint(x, y, true);

        /// <summary>Clears the graph, removing all points, from both lines 1 and 2.</summary>
        public override void Clear()
        {
            values1.Clear();
            values2.Clear();
        }
    }
}
