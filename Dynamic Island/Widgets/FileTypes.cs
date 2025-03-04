using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Storage;

namespace Dynamic_Island.Widgets;

/// <summary>Properties of a widget.</summary>
public class WidgetProperties
{
    /// <summary>The type of widget.</summary>
    [JsonPropertyName("type")]
    public WidgetType Type { get; set; }

    /// <summary>The size of the widget; that is, small, wide or large.</summary>
    [JsonPropertyName("size")]
    public WidgetSize Size { get; set; }

    /// <summary>The index of the widget.</summary>
    [JsonPropertyName("index")]
    public int Index { get; set; }
}

/// <summary>Contains data about widget boards and their widgets.</summary>
public class Board
{
    /// <summary>The file path to the icon of the board.</summary>
    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    /// <summary>The name of the board.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>The widgets contained within the board.</summary>
    [JsonPropertyName("widgets")]
    public WidgetProperties[] Widgets { get; set; }

    const string BoardsFileName = "boards.json";
    readonly static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

    static Board() => GetCurrent();
    private static async void GetCurrent()
    {
        if (await localFolder.TryGetItemAsync(BoardsFileName) is IStorageFile file)
            Current = JsonSerializer.Deserialize<Board[]>(await FileIO.ReadTextAsync(file));
        else
        {
            Board[] boards = [new() { Icon = string.Empty, Name = string.Empty, Widgets = [new() { Size = WidgetSize.Wide, Type = WidgetType.NowPlaying }] }];
            Current = boards;
            file = await localFolder.CreateFileAsync(BoardsFileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, JsonSerializer.Serialize(boards));
        }
        CurrentLoaded?.Invoke(Current);
    }

    /// <summary>Gets the current array of <see cref="Board"/> that are saved.</summary>
    public static Board[] Current { get; private set; }

    /// <summary>Invoked when the value for <see cref="Current"/> has been set.</summary>
    public static event Action<Board[]> CurrentLoaded;

    /// <summary>Updates the saved <see cref="Board"/> at <paramref name="index"/> with <paramref name="newValue"/>.</summary>
    /// <param name="index">The index of the <see cref="Board"/> to update.</param>
    /// <param name="newValue">The new value for the <see cref="Board"/>.</param>
    public static void UpdateBoard(int index, Board newValue)
    {
        Current[index] = newValue;
        Save();
    }

    /// <summary>Adds <paramref name="newValue"/> to the end of <see cref="Current"/>.</summary>
    /// <param name="newValue">The <see cref="Board"/> to add.</param>
    public static void AddBoard(Board newValue)
    {
        Current = [.. Current, newValue];
        Save();
    }

    private async static void Save()
    {
        var file = await localFolder.CreateFileAsync(BoardsFileName, CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteTextAsync(file, JsonSerializer.Serialize(Current));
    }
}

/// <summary>Sizes of a widget.</summary>
public enum WidgetSize
{
    Small,
    Wide,
    Large
}

/// <summary>Types of a widget.</summary>
public enum WidgetType
{
    CPU,
    GPU,
    RAM,
    Disk,
    Network,
    NowPlaying
}
