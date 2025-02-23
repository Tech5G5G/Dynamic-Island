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

        /// <summary>Gets or sets a <see cref="IList{T}"/> of <see cref="CoreWidget"/> used to generate the content of the ItemsControl.</summary>
        /// <returns>The <see cref="IList{T}"/> of <see cref="CoreWidget"/> that is used to generate the content of the ItemsControl. The default is <see langword="null"/>.</returns>
        public new IList<CoreWidget> ItemsSource
        {
            get => base.ItemsSource as IList<CoreWidget>;
            set => base.ItemsSource = value;
        }

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
                var index = ItemsSource.IndexOf((s as GridViewItem).Content as CoreWidget);
                ItemsSource.Remove(draggedWidget);
                ItemsSource.Insert(index, draggedWidget);

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
    }
}
