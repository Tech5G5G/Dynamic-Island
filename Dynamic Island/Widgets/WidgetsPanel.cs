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
            set
            {
                widgetRadius.Value = value;
                WidgetRadiusChanged?.Invoke(value);
            }
        }
        private readonly BindableProperty<CornerRadius> widgetRadius = new(new());
        private event Action<CornerRadius> WidgetRadiusChanged;

        /// <summary>Gets or sets a <see cref="IEnumerable{T}"/> of <see cref="Board"/> used to generate the content of the ItemsControl.</summary>
        /// <returns>The <see cref="IEnumerable{T}"/> of <see cref="Board"/> that is used to generate the content of the ItemsControl. The default is <see langword="null"/>.</returns>
        public new IEnumerable<Board> ItemsSource
        {
            get => boards;
            set
            {
                boards = value;
                var board = CurrentBoard = value.First();
                var widgets = GetCoreWidgets(board);
                base.ItemsSource = widgets;
                cachedWidgets.Add(0, widgets);
            }
        }
        private IEnumerable<Board> boards;
        /// <summary>Invoked when the data within <see cref="ItemsSource"/> has been changed.</summary>
        public event TypedEventHandler<WidgetsPanel, ItemsSourceUpdatedEventArgs> ItemsSourceUpdated;

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
                    coll = GetCoreWidgets(CurrentBoard);
                    base.ItemsSource = coll;
                    cachedWidgets.Add(value, coll);
                }
            }
        }
        private int index = 0;
        private readonly Dictionary<int, ObservableCollection<CoreWidget>> cachedWidgets = [];

        /// <summary>Invoked when a <see cref="CoreWidget"/> initiates a drag operation.</summary>
        public new event Action<CoreWidget> DragItemsStarting;
        /// <summary>Invoked when a <see cref="CoreWidget"/> receives a drop request, thus ending the drag operation.</summary>
        public new event Action<CoreWidget> DragItemsCompleted;

        /// <summary>Gets a <see langword="bool"/> determining whether a <see cref="CoreWidget"/> from the view is currently being dragged.</summary>
        /// <returns><see langword="true"/> if a <see cref="CoreWidget"/> is currently being dragged. Otherwise, <see langword="false"/>.</returns>
        public bool DraggingItem => draggedWidget is not null;

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
            WidgetRadiusChanged += (radius) => widget.CornerRadius = radius;
            container.DragStarting += (s, e) =>
            {
                draggedWidget = widget;
                DragItemsStarting?.Invoke(widget);
            };
            container.DragOver += (s, e) => e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move;
            container.Drop += (s, e) =>
            {
                if (draggedWidget is null)
                    return;

                var coll = base.ItemsSource as ObservableCollection<CoreWidget>;
                var index = coll.IndexOf((s as GridViewItem).Content as CoreWidget);
                coll.Remove(draggedWidget);
                coll.Insert(index, draggedWidget);

                CurrentBoard.Widgets = GetProperties(coll);
                ItemsSourceUpdated?.Invoke(this, new(boards, CurrentBoard));

                draggedWidget = null;
                DragItemsCompleted?.Invoke(widget);
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

    /// <summary>Event args for the <see cref="WidgetsPanel.ItemsSourceUpdated"/> event.</summary>
    /// <param name="source">The current source of the <see cref="WidgetsPanel"/>.</param>
    /// <param name="board">The <see cref="Board"/> that was updated.</param>
    public class ItemsSourceUpdatedEventArgs(IEnumerable<Board> source, Board board) : EventArgs
    {
        /// <summary>The current source of the <see cref="WidgetsPanel"/>.</summary>
        public IEnumerable<Board> ItemsSource { get; } = source;

        /// <summary>The <see cref="Board"/> that was updated.</summary>
        public Board ChangedBoard { get; } = board;
    }
}
