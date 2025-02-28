namespace Dynamic_Island.Widgets
{
    public abstract partial class CoreWidget : Grid
    {
        public CoreWidget() => Height = Width = 150;

        /// <summary>The size of the widget.</summary>
        public WidgetSize Size
        {
            get => size;
            set
            {
                size = value;
                Height = value == WidgetSize.Large ? 304 : 150;
                Width = value == WidgetSize.Small ? 150 : 304;
                WidgetSizeChanged?.Invoke(value);
            }
        }
        private WidgetSize size = WidgetSize.Small;

        /// <summary>The index of the widget.</summary>
        public int Index { get; set; } = 1;

        /// <summary>Invoked after the size of the widget is changed.</summary>
        public event Action<WidgetSize> WidgetSizeChanged;

        /// <summary>Gets a <see cref="ReadOnlyDictionary{TKey, TValue}"/> of <see cref="Widgets.WidgetType"/>s and their respective <see cref="CoreWidget"/>.</summary>
        public static ReadOnlyDictionary<WidgetType, Type> WidgetTypes { get; } = new(new Dictionary<WidgetType, Type>()
        {
            {WidgetType.CPU, typeof(CPUWidget)},
            {WidgetType.GPU, typeof(GPUWidget)},
            {WidgetType.RAM, typeof(RAMWidget)},
            {WidgetType.Disk, typeof(DiskWidget)},
            {WidgetType.Network, typeof(NetworkWidget)},
            {WidgetType.NowPlaying, typeof(NowPlayingWidget)},
        });
    }
}
