using System.Text.RegularExpressions;

namespace Dynamic_Island.Settings.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Animations : Page
    {
        Array easingTypes = Enum.GetValues(typeof(EasingType));

        public Animations()
        {
            this.InitializeComponent();

            openMode.SelectedIndex = (int)SettingValues.OpenMode.Value;
            openMode.SelectionChanged += (s, e) => SettingValues.OpenMode.Value = (string)openMode.SelectedItem switch
            {
                "Ease out" => EasingMode.EaseOut,
                "Ease in" => EasingMode.EaseIn,
                _ => EasingMode.EaseInOut
            };

            openType.SelectedIndex = (int)SettingValues.OpenType.Value;
            openType.SelectionChanged += (s, e) => SettingValues.OpenType.Value = (EasingType)openType.SelectedItem;

            openDuration.Text = SettingValues.OpenDuration.Value.ToString();
            openDuration.ValueChanged += (s, e) =>
            {
                if (int.TryParse(openDuration.Text, out int integer))
                    SettingValues.OpenDuration.Value = integer;
            };

            closeMode.SelectedIndex = (int)SettingValues.CloseMode.Value;
            closeMode.SelectionChanged += (s, e) => SettingValues.CloseMode.Value = (string)closeMode.SelectedItem switch
            {
                "Ease out" => EasingMode.EaseOut,
                "Ease in" => EasingMode.EaseIn,
                _ => EasingMode.EaseInOut
            };

            closeType.SelectedIndex = (int)SettingValues.CloseType.Value;
            closeType.SelectionChanged += (s, e) => SettingValues.CloseType.Value = (EasingType)closeType.SelectedItem;

            closeDuration.Text = SettingValues.CloseDuration.Value.ToString();
            closeDuration.ValueChanged += (s, e) => 
            {
                if (int.TryParse(closeDuration.Text, out int integer))
                    SettingValues.CloseDuration.Value = integer;
            };
        }

        private void Number_ValueChanging(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            //var currentPosition = sender.SelectionStart - 1;
            //var text = sender.Text;

            //var regex = new Regex("^[0-9]*$");

            //if (!regex.IsMatch(text))
            //{
            //    var foundChar = Regex.Match(sender.Text, @"[^0-9]");
            //    if (foundChar.Success)
            //        sender.Text = sender.Text.Remove(foundChar.Index, 1);
            //    sender.Select(currentPosition, 0);
            //}
            //else if (int.TryParse(sender.Text, out int integer) && (integer < 0 || integer > 10000))
            //{
            //    sender.Text = Math.Clamp(integer, 0, 10000).ToString();
            //    sender.Select(currentPosition, 0);
            //}
        }
    }
}
