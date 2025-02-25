using Windows.Storage;
using Windows.ApplicationModel.DataTransfer;

namespace Dynamic_Island.Extensions;

/// <summary>Contains extension methods for <see cref="DataPackageView"/>.</summary>
public static class DataPackageExtensions
{
    /// <summary>Gets the data from a <see cref="DataPackageView"/> and puts it into a <see cref="DataPackage"/>.</summary>
    /// <param name="view">The <see cref="DataPackageView"/> to convert to a <see cref="DataPackage"/>.</param>
    /// <returns>A <see cref="DataPackage"/> containing data from <paramref name="view"/> or <see langword="null"/> if the data couldn't be identified asynchronously.</returns>
    public async static Task<DataPackage> ToDataPackage(this DataPackageView view) => (await view.GetData())?.ToDataPackage();

    /// <summary>Gets the data from <paramref name="view"/>.</summary>
    /// <param name="view">The <see cref="DataPackageView"/> to get data from.</param>
    /// <returns>A <see cref="DataContainer"/> containing the format ID and data asynchronously.</returns>
    public async static Task<DataContainer> GetData(this DataPackageView view)
    {
        var formats = view.GetFormats();
        foreach (string format in formats)
        {
            try
            { 
                var data = await view.GetDataAsync(format);
                return new(format, data);
            }
            catch { continue; }
        }
        return null;
    }

    /// <summary>Gets the <see cref="StandardDataFormats"/> contained in <paramref name="view"/>.</summary>
    /// <param name="view">The <see cref="DataPackageView"/> to get the formats from.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> with the <see cref="StandardDataFormats"/> contained in <paramref name="view"/>.</returns>
    public static IEnumerable<string> GetFormats(this DataPackageView view) => view.AvailableFormats.Intersect([StandardDataFormats.ApplicationLink,
            StandardDataFormats.Bitmap,
            StandardDataFormats.Html,
            StandardDataFormats.Rtf,
            StandardDataFormats.StorageItems,
            StandardDataFormats.Text,
            StandardDataFormats.Uri,
            StandardDataFormats.UserActivityJsonArray,
            StandardDataFormats.WebLink]);
}

/// <summary>Contains the data from the <see cref="DataPackageExtensions.GetData(DataPackageView)"/> extension.</summary>
public class DataContainer
{
    /// <summary>Initializes a new <see cref="DataContainer"/> using the specified <paramref name="formatId"/> and <paramref name="data"/>.</summary>
    /// <param name="formatId">The format of the data.</param>
    /// <param name="data">The data in <see cref="object"/> form.</param>
    public DataContainer(string formatId, object data)
    {
        FormatID = formatId;
        Data = data;
    }
    /// <summary>Initializes a new <see cref="DataContainer"/> using an <see cref="IStorageItem"/>.</summary>
    public DataContainer(IStorageItem item)
    {
        FormatID = StandardDataFormats.StorageItems;
        Data = new List<IStorageItem> { item };
    }

    /// <summary>The format of the data.</summary>
    public string FormatID { get; set; }
    /// <summary>The data in <see cref="object"/> form.</summary>
    public object Data { get; set; }

    /// <summary>Converts the <see cref="DataContainer"/> to a <see cref="DataPackage"/>, using its <see cref="FormatID"/>.</summary>
    /// <returns>A <see cref="DataPackage"/> with its format as <see cref="FormatID"/> and data as <see cref="Data"/>.</returns>
    public DataPackage ToDataPackage()
    {
        DataPackage package = new();
        package.SetData(FormatID, Data);
        return package;
    }

    public MainWindow MainWindow { get; } = App.MainWindow;
}
