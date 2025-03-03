using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.FileProperties;
using Windows.ApplicationModel.DataTransfer;
using CoreAudio;

namespace Dynamic_Island
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        const string StorageItem = "StorageItem";
        public static class CornerRadii
        {
            public static CornerRadius NotchRadius { get; } = new(0, 0, 10, 10);
            public static CornerRadius NotchButtonRadius { get; } = new(6);
        }
        public static class Brushes
        {
            public static SolidColorBrush Black { get; } = new(Colors.Black);
            public static SolidColorBrush Transparent { get; } = new(Colors.Transparent);
            public static SolidColorBrush White { get; } = new(Colors.White);
        }
        public static class VolumeKeys
        {
            public const int VolumeMute = 173;
            public const int VolumeDown = 174;
            public const int VolumeUp = 175;
            public static int[] All { get; } = [VolumeMute, VolumeDown, VolumeUp];
        }
        readonly KeyboardHelper KeyboardHelper = new();

        UIElement elementToHide = null;
        readonly DispatcherTimer ResetAnimationDelayer = new() { Interval = TimeSpan.FromMilliseconds(SettingValues.SizingResetDelay) };

        public nint Handle { get; set; }
        public BindableProperty<CornerRadius> UIRadius { get; set; } = new(new(16));

        public MainWindow()
        {
            this.InitializeComponent();
            WindowHelper.ApplyOverlayProperties(this);
            WindowHelper.HideTitleBar(this);
            Handle = WindowHelper.RemoveBorderElements(this);

            ResetAnimationDelayer.Tick += (s, e) =>
            {
                AnimationBuilder.Create().Size(new((float)SettingValues.PillWidth, (float)SettingValues.PillHeight),
                    duration: TimeSpan.FromMilliseconds(SettingValues.SizingDuration),
                    easingMode: SettingValues.SizingMode,
                    easingType: SettingValues.SizingType,
                    layer: FrameworkLayer.Xaml).Start(pill);
                ResetAnimationDelayer.Stop();

                if (elementToHide is not null)
                {
                    elementToHide.Visibility = Visibility.Collapsed;
                    elementToHide = null;
                }
            };

            Board.CurrentLoaded += (s) => widgetsPanel.ItemsSource = s;
            widgetsPanel.ItemsSourceUpdated += (s, e) => Board.UpdateBoard(s.ToList().IndexOf(e), e);

            ApplySettings();
            AddDropHandlers();
            TrackVolume();
        }

        private void ApplySettings()
        {
            volumeLabel.Visibility = SettingValues.VolumeMode == VolumeMode.Slider ? Visibility.Collapsed : Visibility.Visible;
            pill.Width = SettingValues.PillWidth;
            pill.Height = SettingValues.PillHeight;
            moveIcon.FontSize = Math.Clamp(pill.Height - 20, 10, 30);
            bool notchMode = SettingValues.PillMode == PillMode.Notch;
            pill.CornerRadius = notchMode ? CornerRadii.NotchRadius : new(SettingValues.PillHeight / 2);
            UIRadius.Value = notchMode ? CornerRadii.NotchButtonRadius : pill.CornerRadius.Subtract(new(4));

            SettingValues.VolumeMode.ValueChanged += (s, e) => volumeLabel.Visibility = e == VolumeMode.Slider ? Visibility.Collapsed : Visibility.Visible;
            SettingValues.PillWidth.ValueChanged += (s, e) => pill.Width = e;
            SettingValues.PillHeight.ValueChanged += (s, e) =>
            {
                moveIcon.FontSize = Math.Clamp((pill.Height = e) - 20, 10, 30);
                if (SettingValues.PillMode == PillMode.Island)
                    UIRadius.Value = (pill.CornerRadius = new(e / 2)).Subtract(new(4));
            };
            SettingValues.PillMode.ValueChanged += (s, e) =>
            {
                bool notchMode = SettingValues.PillMode == PillMode.Notch;
                pill.CornerRadius = notchMode ? CornerRadii.NotchRadius : new(SettingValues.PillHeight / 2);
                UIRadius.Value = notchMode ? CornerRadii.NotchButtonRadius : pill.CornerRadius.Subtract(new(4));
                AppWindow.Move(new(AppWindow.Position.X, e == PillMode.Notch ? -4 : 2));
            };
            CenterOnScreen(null, null); //TODO: Replace with setting for saved window position
        }

        MMDevice device;
        private void TrackVolume()
        {
            KeyboardHelper.KeyPressed += (s, e) =>
            {
                int keycode = (int)e.Key;
                if (SettingValues.VolumeMode == VolumeMode.Disabled || !VolumeKeys.All.Contains(keycode) || WindowHelper.CursorInWindow(this) || windowMoving)
                    return;
                e.Handled = true;

                (elementToHide = volumeContainer).Visibility = Visibility.Visible;
                AnimationBuilder.Create().Size(new((float)SettingValues.PillWidth + 90, Math.Clamp((float)SettingValues.PillHeight + 10, 40, int.MaxValue)),
                    duration: TimeSpan.FromMilliseconds(SettingValues.SizingDuration),
                    easingMode: SettingValues.SizingMode,
                    easingType: SettingValues.SizingType,
                    layer: FrameworkLayer.Xaml).Start(pill);
                ResetAnimationDelayer.Restart(TimeSpan.FromMilliseconds(SettingValues.SizingResetDelay));

                if (keycode == VolumeKeys.VolumeMute)
                {
                    device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
                    return;
                }
                bool up = keycode == VolumeKeys.VolumeUp;
                float changer = up ? 0.02f : -0.02f;
                float volume = device.AudioEndpointVolume.MasterVolumeLevelScalar;
                float remainder = volume % 0.02f;
                if (remainder > 0 && remainder < 0.012f)
                    changer -= up ? remainder : -remainder;
                device.AudioEndpointVolume.MasterVolumeLevelScalar = Math.Clamp(device.AudioEndpointVolume.MasterVolumeLevelScalar + changer, 0, 1);
            };

            MMDeviceEnumerator enumerator = new(Guid.NewGuid());
            device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            device.AudioEndpointVolume.OnVolumeNotification += (data) => DispatcherQueue.TryEnqueue(() =>
            {
                float value = (int)Math.Round(data.MasterVolume * 100);
                volume.Value = value;
                volumeLabel.Text = value.ToString();
                volumeIcon.Glyph = data.Muted ? "\uE74F" : value switch
                {
                    > 66 => "\uE995",
                    > 33 => "\uE994",
                    > 0 => "\uE993",
                    _ => "\uE992"
                };
                volumeIconBackground.Visibility = data.Muted ? Visibility.Collapsed : Visibility.Visible;
            });
            float value = (int)Math.Round(device.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            volume.Value = value;
            volumeLabel.Text = value.ToString();
            volumeIcon.Glyph = device.AudioEndpointVolume.Mute ? "\uE74F" : "\uE767";
        }

        object currentShare;
        ObservableCollection<DataContainer> tray = [];
        private void InvokeShareDialog(object shareItem)
        {
            currentShare = shareItem;
            Pill_Toggle(false, () => dragOptions.Visibility = Visibility.Collapsed, force: true);
            PInvoke.SetForegroundWindow(Handle);
            DataTransferManagerInterop.ShowShareUIForWindow(Handle);
        }
        private void AddDropHandlers()
        {
            pill.AllowDrop = trayOption.AllowDrop = shareOption.AllowDrop = true;
            pill.DragEnter += (s, e) => Pill_Toggle(true, () => dragOptions.Visibility = Visibility.Visible, addCheck: widgetsPanel.DraggingItem, toggleView: false);
            pill.DragLeave += (s, e) => Pill_Toggle(false, () => dragOptions.Visibility = Visibility.Collapsed, addCheck: widgetsPanel.DraggingItem);
            DataTransferManagerInterop.GetForWindow(Handle).DataRequested += async (s, e) =>
            {
                var request = e.Request;
                if (currentShare is null)
                    request.FailWithDisplayText("Unable to find item");
                else if (currentShare is DataContainer container)
                {
                    var package = container.ToDataPackage();
                    package.Properties.Title = "Share";
                    request.Data = package;
                }
                else if (currentShare is DataPackageView view)
                {
                    var deferral = request.GetDeferral();
                    var package = await view.ToDataPackage();
                    if (package is null)
                    {
                        request.FailWithDisplayText("Unable to get type of item");
                        return;
                    }
                    package.Properties.Title = "Share";
                    request.Data = package;
                    deferral.Complete();
                }
                currentShare = null;
            };

            trayOption.DragOver += AcceptDrag;
            trayOption.DragEnter += HighlightButton;
            trayOption.DragLeave += LowlightButton;
            trayOption.Drop += async (s, e) =>
            {
                Pill_Toggle(false, () => dragOptions.Visibility = Visibility.Collapsed, force: true);

                var item = await e.DataView.GetData();
                if (item is null)
                    return;
                if (item.Data is IReadOnlyList<IStorageItem> files)
                {
                    foreach (var file in files)
                        tray.Add(new(StorageItem, file));
                    return;
                }
                tray.Add(item);
            };

            shareOption.DragOver += AcceptDrag;
            shareOption.DragEnter += HighlightButton;
            shareOption.DragLeave += LowlightButton;
            shareOption.Drop += (s, e) => InvokeShareDialog(e.DataView);
        }
        private void HighlightButton(object sender, DragEventArgs e) => (sender as Button).Background = Application.Current.Resources["ControlFillColorSecondaryBrush"] as SolidColorBrush;
        private void LowlightButton(object sender, DragEventArgs e) => (sender as Button).Background = Application.Current.Resources["ControlFillColorDefaultBrush"] as SolidColorBrush;
        private void AcceptDrag(object sender, DragEventArgs e)
        {
            if (!windowMoving && e.AllowedOperations.HasFlag(DataPackageOperation.Copy))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.IsCaptionVisible = e.DragUIOverride.IsGlyphVisible = false;
            }
        }

        private void TrayItem_Remove(object sender, RoutedEventArgs e) => tray.Remove((sender as FrameworkElement).DataContext as DataContainer);
        private void TrayItem_Share(object sender, RoutedEventArgs e)
        {
            var data = (sender as FrameworkElement).DataContext as DataContainer;
            InvokeShareDialog(data.FormatID == StorageItem ? new((IStorageItem)data.Data) : data);
        }
        private async void TrayItem_Open(object sender, RoutedEventArgs e)
        {
            var container = (sender as FrameworkElement).DataContext as DataContainer;
            if (container.Data is Uri uri)
                await Windows.System.Launcher.LaunchUriAsync(uri);
            else if (container.Data is IStorageItem item)
                Process.Start(new ProcessStartInfo { FileName = item.Path, UseShellExecute = true });
        }
        private async void TrayItem_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: Fix items unintentially repeating
            bool addOpen = false;
            var viewBox = sender as Viewbox;
            var container = viewBox.DataContext as DataContainer;
            UIElement visual = new TextBlock { Text = container.FormatID };

            if (container.Data is Uri uri)
            {
                addOpen = true;
                visual = new URLDisplay { URL = uri };
            }
            else if (container.Data is RandomAccessStreamReference bitmapStream)
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(await bitmapStream.OpenReadAsync());
                visual = new Image { Source = bitmap };
            }
            else if (container.Data is IStorageItem item)
            {
                addOpen = true;
                BitmapImage source = new();
                IRandomAccessStream thumbnail = null;

                if (item is StorageFile file)
                    thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem);
                else if (item is StorageFolder folder)
                    thumbnail = await folder.GetThumbnailAsync(ThumbnailMode.SingleItem, 256);

                if (thumbnail is not null) source.SetSource(thumbnail);
                else source.UriSource = new(Assets.DefaultFile);
                visual = new Image { Source = source };
            }
            else if (container.Data is string text)
            {
                var grid = (Grid)(visual = new Grid { Background = Brushes.White, Width = 150, Height = 200 });
                if (container.FormatID == StandardDataFormats.Text || container.FormatID == StandardDataFormats.Html)
                    grid.Children.Add(new TextBlock { Text = text, Foreground = Brushes.Black });
                else if (container.FormatID == StandardDataFormats.Rtf)
                {
                    var edit = new RichEditBox { Padding = new(), IsColorFontEnabled = false, Foreground = Brushes.Black };
                    edit.Document.SetText(Microsoft.UI.Text.TextSetOptions.FormatRtf, text);
                    grid.Children.Add(edit);
                    grid.Children.Add(new Grid { Background = Brushes.Transparent });
                }
            }

            viewBox.Child = visual;
            viewBox.ContextFlyout = MenuFlyoutFactory.Create(Flyout_Opening, Flyout_Closed, addOpen,
                new MenuFlyoutItem { Icon = new FontIcon { Glyph = "\uE8A7" }, Text = "Open" }.AddClick(TrayItem_Open),
                new MenuFlyoutItem { Icon = new FontIcon { Glyph = "\uE72D" }, Text = "Share" }.AddClick(TrayItem_Share),
                new MenuFlyoutItem { Icon = new FontIcon { Glyph = "\uE74D" }, Text = "Remove" }.AddClick(TrayItem_Remove));

            viewBox.CanDrag = true;
            viewBox.DragStarting += (s, e) =>
            {
                e.Data.SetData(container.FormatID == StorageItem ? StandardDataFormats.StorageItems : container.FormatID,
                    container.FormatID == StorageItem ? new List<IStorageItem> { (IStorageItem)container.Data } : container.Data);
                e.AllowedOperations = DataPackageOperation.Copy | DataPackageOperation.Move;
                pill.AllowDrop = false;
            };
            viewBox.DropCompleted += (s, e) =>
            {
                //TODO: Fix items not being removed (might be because of previous issue)
                pill.AllowDrop = true;
                if (e.DropResult != DataPackageOperation.None)
                    tray.Remove(container);
            };
        }

        private void CenterOnScreen(object sender, RoutedEventArgs e) => AppWindow.Move(new((DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest).WorkArea.Width - AppWindow.Size.Width) / 2, SettingValues.PillMode == PillMode.Notch ? -4 : 2));
        private void Exit(object sender, RoutedEventArgs e) => Environment.Exit(0);
        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            if (App.SettingsWindow is null)
                (App.SettingsWindow = new()).Activate();
            else
                PInvoke.SetForegroundWindow(WinRT.Interop.WindowNative.GetWindowHandle(App.SettingsWindow));
        }

        private void Pill_Toggle(bool open, Action addAction = null, bool addCheck = false, bool force = false, bool toggleView = true)
        {
            if ((!windowMoving && !addCheck) || force)
            {
                AnimationBuilder.Create().Size(open ? new(620, 201) : new((float)SettingValues.PillWidth, (float)SettingValues.PillHeight),
                    duration: TimeSpan.FromMilliseconds(open ? SettingValues.OpenDuration : SettingValues.CloseDuration),
                    easingMode: open ? SettingValues.OpenMode : SettingValues.CloseMode,
                    easingType: open ? SettingValues.OpenType : SettingValues.CloseType,
                    layer: FrameworkLayer.Xaml).Start(pill);
                addAction?.Invoke();
                if (toggleView)
                    mainView.Visibility = open ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void Window_SizeToPill(object sender, SizeChangedEventArgs e)
        {
            var prevWid = AppWindow.Size.Width / 2;
            double scale = Content.XamlRoot.RasterizationScale;
            AppWindow.Resize(new((int)((e.NewSize.Width + 7) * scale), (int)((e.NewSize.Height + 7) * scale)));
            AppWindow.Move(new(AppWindow.Position.X - (AppWindow.Size.Width / 2 - prevWid), AppWindow.Position.Y));
        }

        bool pointerInMenu = false;
        private void Flyout_Opening(object sender, object e) => pointerInMenu = true;
        private void Flyout_Closed(object sender, object e)
        {
            pointerInMenu = false;
            Pill_Toggle(false, addCheck: WindowHelper.CursorInWindow(this));
        }

        private void Pill_PointerExited(object sender, PointerRoutedEventArgs e) => Pill_Toggle(false, addCheck: pointerInMenu || widgetsPanel.DraggingItem);
        private void Pill_PointerEntered(object sender, PointerRoutedEventArgs e) => Pill_Toggle(true, () =>
        {
            if (elementToHide is not null)
            {
                elementToHide.Visibility = Visibility.Collapsed;
                elementToHide = null;
                ResetAnimationDelayer.Stop();
            }
        });

        bool windowMoving = false, movingEnabled = false;
        int cursorStart, windowStart;
        private void StartMoving(object sender, RoutedEventArgs e)
        {
            windowMoving = true;
            Pill_Toggle(false, force: true, toggleView: true);
            moveIcon.Visibility = Visibility.Visible;
            pill.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.SizeAll);
        }
        private void Pill_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!movingEnabled || !windowMoving)
                return;

            var area = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest);
            PInvoke.GetCursorPos(out PointInt32 point);
            AppWindow.Move(new(Math.Clamp(windowStart + (point.X - cursorStart), 0, area.WorkArea.Width - AppWindow.Size.Width), AppWindow.Position.Y));
        }
        private void Pill_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!windowMoving)
                return;

            windowStart = AppWindow.Position.X;
            PInvoke.GetCursorPos(out PointInt32 point);
            cursorStart = point.X;

            moveIcon.Visibility = Visibility.Collapsed;
            pill.CapturePointer(e.Pointer);
            movingEnabled = true;
        }
        private void Pill_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!windowMoving)
                return;

            pill.InputCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
            pill.ReleasePointerCaptures();

            windowMoving = movingEnabled = false;
        }
    }
}
