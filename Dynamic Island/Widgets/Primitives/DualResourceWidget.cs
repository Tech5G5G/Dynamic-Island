namespace Dynamic_Island.Widgets.Primitives
{
    public abstract class DualResourceWidget : ResourceWidget
    {
        /// <summary>Fired every second to add a point to the second line on the <see cref="ResourceGraph"/>.</summary>
        /// <param name="graph">The <see cref="ResourceGraph"/> to add the point to.</param>
        /// <returns>A <see cref="double"/> representing the Y coordinate for the next point.</returns>
        protected abstract double Data2Requested(ResourceGraph graph);

        protected override bool[] LinesRequested(ResourceGraph graph)
        {
            graph.YAxis.MaxLimit = null;
            return [false, true];
        }

        protected override Task UpdateGraph(ResourceGraph graph, int seconds)
        {
            graph.AddPoint(1, seconds, Data2Requested(graph));
            return base.UpdateGraph(graph, seconds);
        }
    }
}
