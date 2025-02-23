namespace Dynamic_Island.Settings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Appearance : Page
    {
        public Appearance()
        {
            this.InitializeComponent();

            notchToggle.IsOn = SettingValues.PillMode == PillMode.Notch;
            height.Value = SettingValues.PillHeight;
            width.Value = SettingValues.PillWidth;
        }

        private void ToggleNotch(object sender, RoutedEventArgs e)
        {
            if (IsLoaded)
                SettingValues.PillMode.Value = (sender as ToggleSwitch).IsOn ? PillMode.Notch : PillMode.Island;
        }
        private void Width_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsLoaded)
                SettingValues.PillWidth.Value = e.NewValue;
        }
        private void Height_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (IsLoaded)
                SettingValues.PillHeight.Value = e.NewValue;
        }
    }
}
