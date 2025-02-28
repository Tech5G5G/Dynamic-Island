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

        /// <summary>Creates a new <see cref="WidgetProperties"/> depending on the type of <see cref="CoreWidget"/>.</summary>
        /// <returns>A new instance of <see cref="WidgetProperties"/> containing this <see cref="CoreWidget"/>s properties.</returns>
        /// <exception cref="NotImplementedException"/>
        public WidgetProperties GetProperties() => new() { Size = size, Type = GetType().ToString() switch
        {
            nameof(CPUWidget) => WidgetType.CPU,
            nameof(GPUWidget) => WidgetType.GPU,
            nameof(RAMWidget) => WidgetType.RAM,
            nameof(NowPlayingWidget) => WidgetType.NowPlaying,
            _ => throw new NotImplementedException($"Widget of type {GetType()} is not implemented.")
        } };
    }

    /// <summary>Properties of a widget.</summary>
    public class WidgetProperties
    {
        /// <summary>The type of widget.</summary>
        //Insert JSON property name here
        public WidgetType Type { get; set; }
        /// <summary>The size of the widget; that is, small, wide or large.</summary>
        //Insert JSON property name here
        public WidgetSize Size { get; set; }

        /// <summary>Creates a new <see cref="CoreWidget"/> depending on <see cref="Type"/>.</summary>
        /// <returns>A new instance of a derived class of <see cref="CoreWidget"/>.</returns>
        /// <exception cref="NotImplementedException"/>
        public CoreWidget ToCoreWidget() => Type switch
        {
            WidgetType.CPU => new CPUWidget() { Size = Size },
            WidgetType.GPU => new GPUWidget() { Size = Size },
            WidgetType.RAM => new RAMWidget() { Size = Size },
            WidgetType.NowPlaying => new NowPlayingWidget() { Size = Size },
            _ => throw new NotImplementedException($"Widget of type {Type} is not implemented.")
        };
    }

    /// <summary>Sizes of a widget.</summary>
    public enum WidgetSize
    {
        Small,
        Wide,
        Large
    }

    /// <summary>Types of a widget.</summary>
    public enum WidgetType
    {
        CPU,
        GPU,
        RAM,
        NowPlaying
    }
}
