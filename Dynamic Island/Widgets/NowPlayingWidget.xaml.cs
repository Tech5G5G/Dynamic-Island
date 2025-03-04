using Windows.Media.Control;
using Windows.Storage.Streams;

namespace Dynamic_Island.Widgets
{
    public sealed partial class NowPlayingWidget : CoreWidget
    {
        public NowPlayingWidget()
        {
            this.InitializeComponent();
            Size = WidgetSize.Wide;

            MediaHelper.MediaPropertiesChanged += (e) => DispatcherQueue.TryEnqueue(() => UpdateMediaUI(e));
            MediaHelper.TimelinePropertiesChanged += (e) => DispatcherQueue.TryEnqueue(() => UpdateTimelineUI(e));
            MediaHelper.PlaybackInfoChanged += (e) => DispatcherQueue.TryEnqueue(() => UpdatePlaybackUI(e));
            GetCurrentUI();

            title.RegisterPropertyChangedCallback(TextBlock.TextProperty, TextChanged);
            artist.RegisterPropertyChangedCallback(TextBlock.TextProperty, TextChanged);
            album.RegisterPropertyChangedCallback(TextBlock.TextProperty, TextChanged);

            WidgetSizeChanged += (size) =>
            {
                //TODO: Update height and width of album art
            };
        }

        public async void GetCurrentUI()
        {
            if (MediaHelper.SessionExists)
            {
                UpdateMediaUI(await MediaHelper.GetMediaPropertiesAsync());
                UpdateTimelineUI(MediaHelper.GetTimelineProperties());
                UpdatePlaybackUI(MediaHelper.GetPlaybackInfo());
            }
        }

        public async void UpdateMediaUI(GlobalSystemMediaTransportControlsSessionMediaProperties props)
        {
            bool propsUsable = props is not null;
            mediaThumbnail.Source = propsUsable && props.Thumbnail is IRandomAccessStreamReference stream ? new BitmapImage().AddSource(await stream.OpenReadAsync()) : new BitmapImage { UriSource = new(Assets.DefaultMedia) };
            title.Text = propsUsable ? props.Title : "Not playing";
            artist.Text = propsUsable ? string.IsNullOrWhiteSpace(props.Artist) ? props.AlbumArtist : props.Artist : string.Empty;
            album.Text = propsUsable ? props.AlbumTitle : string.Empty;
        }
        public void UpdateTimelineUI(GlobalSystemMediaTransportControlsSessionTimelineProperties props)
        {
            bool propsUsable = props is not null;
            mediaProgress.Minimum = propsUsable ? props.StartTime.TotalSeconds : 0;
            mediaProgress.Maximum = propsUsable ? props.EndTime.TotalSeconds : 1;
            mediaProgress.Value = propsUsable ? props.Position.TotalSeconds : 0;
        }
        public void UpdatePlaybackUI(GlobalSystemMediaTransportControlsSessionPlaybackInfo info)
        {
            bool infoUsable = info is not null;
            previous.IsEnabled = toggle.IsEnabled = next.IsEnabled = infoUsable;
            toggle.Content = new FontIcon { Glyph = infoUsable && info.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing ? "\uE769" : "\uE768" };

            if (!infoUsable)
            {
                UpdateMediaUI(null);
                UpdateTimelineUI(null);
            }
        }

        private void TextChanged(DependencyObject sender, DependencyProperty e)
        {
            if (sender is not TextBlock block)
                return;

            bool empty = string.IsNullOrWhiteSpace(block.Text);
            if (block.Name == nameof(title) && empty)
                block.Text = "Not playing";
            else
                block.Visibility = empty ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Toggle(object sender, RoutedEventArgs e) => MediaHelper.TogglePlayback();
        private void Previous(object sender, RoutedEventArgs e) => MediaHelper.SkipPrevious();
        private void Next(object sender, RoutedEventArgs e) => MediaHelper.SkipNext();
    }
}
