namespace Dynamic_Island.Widgets
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddBoardWindow : ThemeWindow
    {
        static readonly Symbol[] symbolIcons = (Symbol[])Enum.GetValues(typeof(Symbol));

        public AddBoardWindow()
        {
            this.InitializeComponent();
            this.CenterOnScreen();
            SetTitleBar(titleBar);

            SetCloseButton(close);
            Closed += (s, e) => App.AddBoardWindow = null;

            icons.SelectionChanged += (s, e) => icon.Glyph = (string)icons.SelectedItem;
            PopulateIcons();
        }

        private void PopulateIcons()
        {
            icons.ItemsSource = symbolIcons.Select(icon => Convert.ToChar(Convert.ToInt32(((int)icon).ToString("X"), 16)).ToString()).ToArray();
            icons.SelectedIndex = 15;
        }

        protected override RectInt32[] CaptionCutoutsRequested(double scale)
        {
            Rect closeRect = new(10 * scale, 11 * scale, 12 * scale, 12 * scale);
            Rect maxRect = new(30 * scale, 11 * scale, 12 * scale, 12 * scale);
            Rect minRect = new(50 * scale, 11 * scale, 12 * scale, 12 * scale);

            return [closeRect.ToRectInt32(), maxRect.ToRectInt32(), minRect.ToRectInt32()];
        }

        /// <summary>Invoked when the user has created a board to add.</summary>
        public event Action<Board> BoardCreated;

        private void AddBoard(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(name.Text))
            {
                BoardCreated?.Invoke(new() { Name = name.Text, Icon = symbolIcons[icons.SelectedIndex], Widgets = [] });
                Close();
            }
            else
            {
                ContentDialog dialog = new ContentDialog()
                {
                    XamlRoot = (Content as UIElement).XamlRoot,
                    DefaultButton = ContentDialogButton.Close,
                    CloseButtonText = "OK",
                    Title = "Error",
                    Content = "All details of the new board need to be filled out before continuing, including its name."
                };
                dialog.ShowAsync().AsTask();
            }

        }
    }
}
