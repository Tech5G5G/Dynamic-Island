namespace Dynamic_Island.Extensions;

/// <summary>Contains extension methods for various types.</summary>
public static class OtherExtensions
{
    /// <summary>Assigns the <see cref="ButtonBase.Click"/> in the fluent style.</summary>
    /// <param name="item">The <see cref="ButtonBase"/> to assign the click handler to.</param>
    /// <param name="handler">The handler for the <see cref="ButtonBase.Click"/> event.</param>
    /// <returns><paramref name="item"/> with the <see cref="ButtonBase.Click"/> assigned to <paramref name="handler"/>.</returns>
    public static ButtonBase AddClick(this ButtonBase item, RoutedEventHandler handler)
    {
        item.Click += handler;
        return item;
    }

    /// <summary>Assigns the <see cref="MenuFlyoutItem.Click"/> in the fluent style.</summary>
    /// <param name="item">The <see cref="MenuFlyoutItem"/> to assign the click handler to.</param>
    /// <param name="handler">The handler for the <see cref="MenuFlyoutItem.Click"/> event.</param>
    /// <returns><paramref name="item"/> with the <see cref="MenuFlyoutItem.Click"/> assigned to <paramref name="handler"/>.</returns>
    public static MenuFlyoutItem AddClick(this MenuFlyoutItem item, RoutedEventHandler handler)
    {
        item.Click += handler;
        return item;
    }

    /// <summary>Assigns the source of <paramref name="image"/> to <paramref name="source"/> in the fluent style.</summary>
    /// <param name="image">The <see cref="BitmapImage"/> to assign the source to.</param>
    /// <param name="source">The source for the <paramref name="image"/>.</param>
    /// <returns><paramref name="image"/> with its source assigned to <paramref name="source"/>.</returns>
    public static BitmapImage AddSource(this BitmapImage image, Windows.Storage.Streams.IRandomAccessStream source)
    {
        ArgumentNullException.ThrowIfNull(source);
        image.SetSource(source);
        return image;
    }

    /// <summary>Restarts <paramref name="timer"/> by invoking <see cref="DispatcherTimer.Stop"/> and <see cref="DispatcherTimer.Start"/>, whilst setting its <see cref="DispatcherTimer.Interval"/>.</summary>
    /// <param name="timer">The <see cref="DispatcherTimer"/> to restart.</param>
    /// <param name="interval">The interval to set the <see cref="DispatcherTimer.Interval"/> to.</param>
    public static void Restart(this DispatcherTimer timer, TimeSpan interval)
    {
        timer.Stop();
        timer.Interval = interval;
        timer.Start();
    }
}
