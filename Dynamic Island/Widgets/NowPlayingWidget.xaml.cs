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

            MediaHelper.MediaPropertiesChanged += MediaPropertiesChanged;
            MediaHelper.TimelinePropertiesChanged += TimelinePropertiesChanged;
            MediaHelper.PlaybackInfoChanged += PlaybackInfoChanged;

            title.RegisterPropertyChangedCallback(TextBlock.TextProperty, TextChanged);
            artist.RegisterPropertyChangedCallback(TextBlock.TextProperty, TextChanged);
            album.RegisterPropertyChangedCallback(TextBlock.TextProperty, TextChanged);

            WidgetSizeChanged += (size) =>
            {
                //TODO: Update height and width of album art
            };
        }

        private void MediaPropertiesChanged(GlobalSystemMediaTransportControlsSessionMediaProperties props)
        {
            if (props is null)
                ResetUI(0);
            else
                DispatcherQueue.TryEnqueue(async () =>
                {
                    mediaThumbnail.Source = props.Thumbnail is IRandomAccessStreamReference stream ? new BitmapImage().AddSource(await stream.OpenReadAsync()) : null;
                    title.Text = props.Title;
                    artist.Text = string.IsNullOrWhiteSpace(props.Artist) ? props.AlbumArtist : props.Artist;
                    album.Text = props.AlbumTitle;
                });
        }
        private void TimelinePropertiesChanged(GlobalSystemMediaTransportControlsSessionTimelineProperties props)
        {
            if (props is null)
                ResetUI(1);
            else
                DispatcherQueue.TryEnqueue(() =>
                {
                    mediaProgress.Minimum = props.StartTime.TotalSeconds;
                    mediaProgress.Maximum = props.EndTime.TotalSeconds;
                    mediaProgress.Value = props.Position.TotalSeconds;
                });
        }
        private void PlaybackInfoChanged(GlobalSystemMediaTransportControlsSessionPlaybackInfo info)
        {
            if (info is null)
                ResetUI(2);
            else
                DispatcherQueue.TryEnqueue(() =>
                {
                    previous.IsEnabled = toggle.IsEnabled = next.IsEnabled = true;
                    toggle.Content = new FontIcon { Glyph = info.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing ? "\uE769" : "\uE768" };
                });
        }

        private void ResetUI(int ui) => DispatcherQueue.TryEnqueue(() =>
        {
            if (ui == 0 || ui == 2)
            {
                mediaThumbnail.Source = new BitmapImage { UriSource = new(Assets.DefaultMedia) };
                title.Text = "Not playing";
                artist.Text = album.Text = string.Empty;
            }
            if (ui == 1 || ui == 2)
            {
                mediaProgress.Maximum = 1;
                mediaProgress.Value = mediaProgress.Minimum = 0;
            }
            if (ui == 2)
            {
                previous.IsEnabled = toggle.IsEnabled = next.IsEnabled = false;
                toggle.Content = new FontIcon { Glyph = "\uE768" };
            }
        });

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
