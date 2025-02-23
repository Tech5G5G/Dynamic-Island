namespace Dynamic_Island.Settings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Volume : Page
    {
        public Volume()
        {
            this.InitializeComponent();

            var mode = SettingValues.VolumeMode.Value;
            volumeMode.SelectedIndex = (int)mode;
            smoke.Visibility = mode == VolumeMode.Disabled ? Visibility.Visible : Visibility.Collapsed;
            volumeLabel.Visibility = mode == VolumeMode.Slider ? Visibility.Collapsed : Visibility.Visible;
        }

        private void VolumeMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;

            var mode = (VolumeMode)volumeMode.SelectedIndex;
            SettingValues.VolumeMode.Value = mode;
            smoke.Visibility = mode == VolumeMode.Disabled ? Visibility.Visible : Visibility.Collapsed;
            volumeLabel.Visibility = mode == VolumeMode.Slider ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
