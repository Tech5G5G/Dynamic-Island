using Windows.Media.Control;

namespace Dynamic_Island.Helpers;

/// <summary>Contains events and methods for responding to and controlling media.</summary>
public static class MediaHelper
{
    /// <summary>Invoked when the current session's media properties are changed.</summary>
    public static event Action<GlobalSystemMediaTransportControlsSessionMediaProperties> MediaPropertiesChanged;
    /// <summary>Invoked when the current session's playback info is changed.</summary>
    public static event Action<GlobalSystemMediaTransportControlsSessionPlaybackInfo> PlaybackInfoChanged;
    /// <summary>Invoked when the current session's timeline properties are changed.</summary>
    public static event Action<GlobalSystemMediaTransportControlsSessionTimelineProperties> TimelinePropertiesChanged;

    /// <summary>Tries to toggle the playback of the current session; that is, play or pause it.</summary>
    public static void TogglePlayback() => session?.TryTogglePlayPauseAsync();
    /// <summary>Tries to skip to the previous track in the current session.</summary>
    public static void SkipPrevious() => session?.TrySkipPreviousAsync();
    /// <summary>Tries to skip to the next track in the current session.</summary>
    public static void SkipNext() => session?.TrySkipNextAsync();

    /// <summary>Gets a <see langword="bool"/> determining whether a session is active and existant.</summary>
    public static bool SessionExists => session is not null;

    /// <summary>Tries to get the playback info of the current session.</summary>
    /// <returns>If <see cref="SessionExists"/> is <see langword="true"/>, <see cref="GlobalSystemMediaTransportControlsSessionPlaybackInfo"/>. Otherwise, <see langword="null"/>.</returns>
    public static GlobalSystemMediaTransportControlsSessionPlaybackInfo GetPlaybackInfo() => session?.GetPlaybackInfo();
    /// <summary>Tries to get the timeline properties of the current session.</summary>
    /// <returns>If <see cref="SessionExists"/> is <see langword="true"/>, <see cref="GlobalSystemMediaTransportControlsSessionTimelineProperties"/>. Otherwise, <see langword="null"/>.</returns>
    public static GlobalSystemMediaTransportControlsSessionTimelineProperties GetTimelineProperties() => session?.GetTimelineProperties();
    /// <summary>Tries to get the media properties of the current session.</summary>
    /// <returns>If <see cref="SessionExists"/> is <see langword="true"/>, <see cref="GlobalSystemMediaTransportControlsSessionMediaProperties"/>; otherwise, <see langword="null"/>, asynchronously.</returns>
    public async static Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaPropertiesAsync()
    {
        if (session is null)
            return null;
        var props = await AssignerHelper.TryAssignAsync(() => session.TryGetMediaPropertiesAsync().AsTask());
        if (props is null && tries <= 10)
        {
            tries++;
            return await GetMediaPropertiesAsync();
        }
        tries = 0;
        return props;
    }
    static int tries = 0; //Avoids stack overflows :)

    private static GlobalSystemMediaTransportControlsSession session;
    private static GlobalSystemMediaTransportControlsSessionManager sessionManager;

    static MediaHelper() => CreateManager();
    private static async void CreateManager()
    {
        sessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        sessionManager.CurrentSessionChanged += (s, e) => GetSession();
        GetSession();
    }

    private static void GetSession()
    {
        session = sessionManager.GetCurrentSession();
        if (session is null)
            return;

        session.MediaPropertiesChanged += (s, e) => UpdateMediaProperties();
        session.PlaybackInfoChanged += (s, e) => UpdatePlaybackInfo();
        session.TimelinePropertiesChanged += (s, e) => UpdateTimeline();
        UpdateMediaProperties();
    }

    private static void UpdatePlaybackInfo() => PlaybackInfoChanged?.Invoke(GetPlaybackInfo());
    private static void UpdateTimeline() => TimelinePropertiesChanged?.Invoke(GetTimelineProperties());
    private static async void UpdateMediaProperties()
    {
        MediaPropertiesChanged?.Invoke(await GetMediaPropertiesAsync());
        UpdatePlaybackInfo();
    }
}
