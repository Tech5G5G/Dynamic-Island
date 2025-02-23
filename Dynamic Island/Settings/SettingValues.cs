using Windows.Storage;

namespace Dynamic_Island.Settings
{
    public static class SettingValues
    {
        public static Setting<double> PillHeight { get; } = new("PillHeight", 40);
        public static Setting<double> PillWidth { get; } = new("PillWidth", 150);
        public static EnumSetting<PillMode> PillMode { get; } = new("PillMode", Settings.PillMode.Island);

        public static EnumSetting<EasingMode> OpenMode { get; } = new("OpenMode", EasingMode.EaseOut);
        public static EnumSetting<EasingType> OpenType { get; } = new("OpenType", EasingType.Default);
        public static Setting<int> OpenDuration { get; } = new("OpenDuration", 300);

        public static EnumSetting<EasingMode> CloseMode { get; } = new("CloseMode", EasingMode.EaseOut);
        public static EnumSetting<EasingType> CloseType { get; } = new("CloseType", EasingType.Default);
        public static Setting<int> CloseDuration { get; } = new("CloseDuration", 300);

        //TODO: Implement UI elements for changing these values
        public static EnumSetting<EasingMode> SizingMode { get; } = new("SizingMode", EasingMode.EaseOut);
        public static EnumSetting<EasingType> SizingType { get; } = new("SizingType", EasingType.Default);
        public static Setting<int> SizingDuration { get; } = new("SizingDuration", 300);
        public static Setting<int> SizingResetDuration { get; } = new("SizingResetDuration", 3000);

        public static EnumSetting<VolumeMode> VolumeMode { get; } = new("VolumeMode", Settings.VolumeMode.SliderLabel);
    }

    public partial class BindableProperty<T>(T defaultValue) : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
        private T _value = defaultValue;
    }

    public class Setting<T>(string key, T defaultValue)
    {
        static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public T Value
        {
            get
            {
                if (localSettings.Values.TryGetValue(key, out object value))
                    return (T)value;
                else
                {
                    localSettings.Values[key] = defaultValue;
                    return defaultValue;
                }
            }
            set
            {
                localSettings.Values[key] = value;
                ValueChanged?.Invoke(this, value);
            }
        }

        public static implicit operator T(Setting<T> setting) => setting.Value;

        public event TypedEventHandler<Setting<T>, T> ValueChanged;
    }

    public class EnumSetting<T>(string key, T defaultValue) where T : Enum
    {
        static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public T Value
        {
            get
            {
                if (localSettings.Values.TryGetValue(key, out object value))
                    return (T)value;
                else
                {
                    localSettings.Values[key] = (int)(object)defaultValue;
                    return defaultValue;
                }
            }
            set
            {
                localSettings.Values[key] = (int)(object)value;
                ValueChanged?.Invoke(this, value);
            }
        }

        public static implicit operator T(EnumSetting<T> setting) => setting.Value;

        public event TypedEventHandler<EnumSetting<T>, T> ValueChanged;
    }

    public enum PillMode
    {
        Island,
        Notch
    }

    public enum VolumeMode
    {
        SliderLabel,
        Slider,
        Disabled
    }
}
