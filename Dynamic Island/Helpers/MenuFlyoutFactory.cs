namespace Dynamic_Island.Helpers;

/// <summary>Provides methods for the creation of a <see cref="MenuFlyout"/>.</summary>
public static class MenuFlyoutFactory
{
    /// <summary>Creates a <see cref="MenuFlyout"/> for a tray item.</summary>
    /// <param name="opening"><see cref="EventHandler"/> for when the <see cref="MenuFlyout"/> is opening.</param>
    /// <param name="closed"><see cref="EventHandler"/> for when the <see cref="MenuFlyout"/> is closed.</param>
    /// <param name="addFirst"><see cref="bool"/> whether to add the first item in <paramref name="items"/>.</param>
    /// <param name="items">The <see cref="MenuFlyoutItemBase"/>s to add to the <see cref="MenuFlyout"/>.</param>
    /// <returns></returns>
    public static MenuFlyout Create(EventHandler<object> opening, EventHandler<object> closed, bool addFirst, params MenuFlyoutItemBase[] items)
    {
        var flyout = new MenuFlyout();
        flyout.Opening += opening;
        flyout.Closed += closed;
        foreach (var item in items)
        {
            if (items[0] != item || addFirst)
                flyout.Items.Add(item);
        }
        return flyout;
    }
}
