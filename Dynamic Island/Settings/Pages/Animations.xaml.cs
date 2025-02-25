using Windows.Globalization.NumberFormatting;

namespace Dynamic_Island.Settings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Animations : Page
    {
        readonly Array easingTypes = Enum.GetValues(typeof(EasingType));

        public Animations()
        {
            this.InitializeComponent();

            InitializeMode(openMode, SettingValues.OpenMode);
            InitializeType(openType, SettingValues.OpenType);
            InitializeDuration(openDuration, SettingValues.OpenDuration);

            InitializeMode(closeMode, SettingValues.CloseMode);
            InitializeType(closeType, SettingValues.CloseType);
            InitializeDuration(closeDuration, SettingValues.CloseDuration);

            InitializeMode(sizingMode, SettingValues.SizingMode);
            InitializeType(sizingType, SettingValues.SizingType);
            InitializeDuration(sizingDuration, SettingValues.SizingDuration);
            InitializeDuration(sizingDelay, SettingValues.SizingResetDelay);

            openDuration.NumberFormatter = closeDuration.NumberFormatter = sizingDuration.NumberFormatter = sizingDelay.NumberFormatter = new DecimalFormatter()
            {
                IntegerDigits = 1,
                FractionDigits = 0,
                NumberRounder = new IncrementNumberRounder()
                {
                    Increment = 1,
                    RoundingAlgorithm = RoundingAlgorithm.RoundHalfUp
                }
            };
        }

        private void InitializeMode(ComboBox box, EnumSetting<EasingMode> setting)
        {
            box.SelectedIndex = (int)setting.Value;
            box.SelectionChanged += (s, e) => setting.Value = (EasingMode)box.SelectedIndex;
        }
        private void InitializeType(ComboBox box, EnumSetting<EasingType> setting)
        {
            box.SelectedIndex = (int)setting.Value;
            box.SelectionChanged += (s, e) => setting.Value = (EasingType)box.SelectedItem;
        }
        private void InitializeDuration(NumberBox box, Setting<int> setting)
        {
            box.Value = setting.Value;
            box.ValueChanged += (s, e) => setting.Value = (int)e.NewValue;
        }
    }
}
