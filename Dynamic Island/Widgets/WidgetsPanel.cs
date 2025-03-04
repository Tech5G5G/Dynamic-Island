namespace Dynamic_Island.Widgets
{
    public sealed partial class WidgetsPanel : GridView
    {
        public WidgetsPanel()
        {
            this.DefaultStyleKey = typeof(WidgetsPanel);
            SelectionMode = ListViewSelectionMode.None;
        }

        /// <summary>Gets or sets a <see cref="CornerRadius"/> that changes the <see cref="Grid.CornerRadius"/> of all the <see cref="CoreWidget"/>s in the panel.</summary>
        /// <returns>The <see cref="CornerRadius"/> of all the <see cref="CoreWidget"/>s in the panel.</returns>
        public CornerRadius WidgetRadius
        {
            get => widgetRadius.Value;
            set => widgetRadius.Value = value;
        }
        private readonly BindableProperty<CornerRadius> widgetRadius = new(new());

        /// <summary>Gets or sets a <see cref="IEnumerable{T}"/> of <see cref="Board"/> used to generate the content of the ItemsControl.</summary>
        /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="Board"/> that is used to generate the content of the ItemsControl. The default is <see langword="null"/>.</returns>
        public new IEnumerable<Board> ItemsSource
        {
            get => boards;
            set
            {
                boards = value;
                var board = CurrentBoard = value.FirstOrDefault(new Board { Widgets = [] });
                var widgets = GetCoreWidgets(board);
                base.ItemsSource = widgets;
                cachedWidgets.Clear();
                cachedWidgets.Add(0, widgets);
                BoardIndex = index;
            }
        }
        private IEnumerable<Board> boards;
        /// <summary>Invoked when the data within <see cref="ItemsSource"/> has been changed.</summary>
        public event TypedEventHandler<IEnumerable<Board>, Board> ItemsSourceUpdated;

        /// <summary>Gets the current <see cref="Board"/> displayed. This can be changed using the <see cref="BoardIndex"/> property.</summary>
        public Board CurrentBoard { get; private set; }
        /// <summary>Gets or sets the displayed <see cref="Board"/> using its index in <see cref="ItemsSource"/>.</summary>
        public int BoardIndex
        {
            get => index;
            set
            {
                index = value;
                CurrentBoard = boards.ElementAt(value);
                if (cachedWidgets.TryGetValue(value, out var coll))
                    base.ItemsSource = coll;
                else
                {
                    base.ItemsSource = coll = GetCoreWidgets(CurrentBoard);
                    cachedWidgets.Add(value, coll);
                }
            }
        }
        private int index = 0;
        private readonly Dictionary<int, ObservableCollection<CoreWidget>> cachedWidgets = [];

        /// <summary>Invoked when a <see cref="CoreWidget"/> initiates a drag operation.</summary>
        public new event Action<CoreWidget> DragItemsStarting;

        /// <summary>Invoked when a <see cref="CoreWidget"/> receives a drop request, thus ending the drag operation.</summary>
        public new event TypedEventHandler<WidgetsPanel, DragWidgetCompletedEventArgs> DragItemsCompleted;
        public class DragWidgetCompletedEventArgs(CoreWidget widget)
        {
            /// <summary>The <see cref="CoreWidget"/> being dragged.</summary>
            public CoreWidget Widget { get; set; } = widget;
            /// <summary>Set this to <see langword="true"/> when overriding <see cref="Widget"/>.</summary>
            public bool OverridingWidget { get; set; } = false;
        }

        /// <summary>Gets or sets a value that indicates whether the <see cref="CoreWidget"/>s within the view can be reordered through user interaction.</summary>
        /// <returns><see langword="true"/> if <see cref="CoreWidget"/>s in the view can be reordered through user interaction; otherwise, <see langword="false"/>. The default is <see langword="false"/>.</returns>
        public new bool CanReorderItems
        {
            get => canReorderItems.Value;
            set => canReorderItems.Value = value;
        }
        private readonly BindableProperty<bool> canReorderItems = new(false);
        CoreWidget draggedWidget;

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var container = element as UIElement;
            var size = item is CoreWidget widget ? widget.Size : throw new InvalidDataException($"{GetType()} contains item that should be of type {typeof(CoreWidget)}, not {item.GetType()}.");

            BindingOperations.SetBinding(container, CanDragProperty, new Binding { Source = canReorderItems.Value, Mode = BindingMode.OneWay });
            widget.CornerRadius = WidgetRadius;
            widgetRadius.PropertyChanged += (s, e) => widget.CornerRadius = widgetRadius.Value;
            container.DragStarting += (s, e) =>
            {
                draggedWidget = widget;
                DragItemsStarting?.Invoke(widget);
            };
            container.DragOver += (s, e) => e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            container.Drop += (s, e) =>
            {
                var args = new DragWidgetCompletedEventArgs(draggedWidget);
                DragItemsCompleted?.Invoke(this, args);

                if (args.Widget is not CoreWidget widget)
                    return;

                var coll = base.ItemsSource as ObservableCollection<CoreWidget>;
                var index = coll.IndexOf((s as GridViewItem).Content as CoreWidget);
                if (!args.OverridingWidget)
                    coll.Remove(widget);
                coll.Insert(index, widget);

                draggedWidget = null;
                CurrentBoard.Widgets = GetProperties(coll);
                ItemsSourceUpdated?.Invoke(boards, CurrentBoard);
            };

            UpdateContainerSpans(container, size);
            widget.WidgetSizeChanged += (size) => UpdateContainerSpans(container, size);
            base.PrepareContainerForItemOverride(element, item);
        }

        private static void UpdateContainerSpans(UIElement container, WidgetSize size)
        {
            VariableSizedWrapGrid.SetColumnSpan(container, size == WidgetSize.Small ? 1 : 2);
            VariableSizedWrapGrid.SetRowSpan(container, size == WidgetSize.Large ? 2 : 1);
        }

        private static ObservableCollection<CoreWidget> GetCoreWidgets(Board board) => new(board.Widgets.Select(i => (CoreWidget)Activator.CreateInstance(CoreWidget.WidgetTypes[i.Type])));
        private static WidgetProperties[] GetProperties(ObservableCollection<CoreWidget> widgets) => widgets.Select(i => new WidgetProperties()
        {
            Type = CoreWidget.WidgetTypes.First(x => x.Value == i.GetType()).Key,
            Size = i.Size,
            Index = i.Index
        }).ToArray();
    }
}
