namespace Dynamic_Island.Settings
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsWindow : ThemeWindow
    {
        public SettingsWindow()
        {
            this.InitializeComponent();
            SetTitleBar(titleBarArea);

            SetCloseButton(close);
            Closed += (s, e) => App.SettingsWindow = null;
        }

        protected override RectInt32[] CaptionCutoutsRequested(double scale)
        {
            Rect closeRect = new(10 * scale, 11 * scale, 12 * scale, 12 * scale);
            Rect maxRect = new(30 * scale, 11 * scale, 12 * scale, 12 * scale);
            Rect minRect = new(50 * scale, 11 * scale, 12 * scale, 12 * scale);

            return [closeRect.ToRectInt32(), maxRect.ToRectInt32(), minRect.ToRectInt32()];
        }

        private void ChangeTab(SelectorBar sender, SelectorBarSelectionChangedEventArgs e) => view.Navigate(sender.SelectedItem.Text switch
        {
            "Animations" => typeof(Animations),
            "Volume" => typeof(Volume),
            _ => typeof(Appearance)
        });
    }
}
