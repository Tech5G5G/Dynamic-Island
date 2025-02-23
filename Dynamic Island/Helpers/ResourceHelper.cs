namespace Dynamic_Island.Helpers;

/// <summary>Gets computer resource statistics.</summary>
public static class ResourceHelper
{
    static ResourceHelper()
    {
        gpuCounters = GetGPUCounters();
        networkCounters = GetNetworkCounters();
    }

    /// <summary>Gets the CPU usage.</summary>
    /// <value>The percent of the CPU used.</value>
    public static float CPUUsage => ProcessorCounters.Utility.NextValue();
    /// <summary>Gets the CPU clockage.</summary>
    /// <value>The speed of the CPU in GHz.</value>
    public static float CPUClockage => ProcessorCounters.Frequency.NextValue() * ProcessorCounters.Performance.NextValue() / 100000;
    private static class ProcessorCounters
    {
        public static readonly PerformanceCounter Utility = new("Processor Information", "% Processor Utility", "_Total");
        public static readonly PerformanceCounter Performance = new("Processor Information", "% Processor Performance", "_Total");
        public static readonly PerformanceCounter Frequency = new("Processor Information", "Processor Frequency", "_Total");
    }

    /// <summary>Gets the amount of GPUs available to the system.</summary>
    /// <value>The amount of GPUs that are available.</value>
    public static int GPUCount => gpuCounters.Count;
    /// <summary>Gets the usage for the specified <paramref name="gpu"/>.</summary>
    /// <param name="gpu">The GPU to retrieve the usage of. Vaild identifiers can be found using the <see cref="GPUCount"/> property.</param>
    /// <returns>The percent of <paramref name="gpu"/> used, asynchronously.</returns>
    public static async Task<float> GetGPUUsage(int gpu)
    {
        return await Task.Run(() =>
        {
            var counters = gpuCounters[gpu - 1];
            float sum = 0;
            List<PerformanceCounter> trunicate = [];
            lock (counters)
            {
                foreach (var counter in counters)
                {
                    sum += AssignerHelper.TryAssign(counter.NextValue, () =>
                    {
                        trunicate.Add(counter);
                        return 0;
                    });
                }
                foreach (var counter in trunicate)
                    counters.Remove(counter);
            }
            return sum;
        });
    }
    private static readonly Dictionary<int, List<PerformanceCounter>> gpuCounters;

    /// <summary>Gets the amount of networks available to the system.</summary>
    /// <value>The amount of networks that are available.</value>
    public static int NetworkCount => networkCounters.Count;
    /// <summary>Gets the usage for the specified <paramref name="network"/>.</summary>
    /// <param name="network">The network to retrieve the usage of. Vaild identifiers can be found using the <see cref="NetworkCount"/> property.</param>
    /// <returns>The percent of <paramref name="network"/> used.</returns>
    public static float GetNetworkUsage(int network)
    {
        var counters = networkCounters[network - 1];
        return 8 * (counters.Item1.NextValue() + counters.Item2.NextValue()) / counters.Item3.NextValue();
    }
    private static readonly Dictionary<int, (PerformanceCounter, PerformanceCounter, PerformanceCounter)> networkCounters;

    private static Dictionary<int, List<PerformanceCounter>> GetGPUCounters()
    {
        var category = new PerformanceCounterCategory("GPU Engine");
        var counterNames = category.GetInstanceNames();

        var counters = counterNames.Where(counterName => counterName.EndsWith("engtype_3D"))
            .SelectMany(category.GetCounters)
            .Where(counter => counter.CounterName == "Utilization Percentage");

        Dictionary<int, List<PerformanceCounter>> dictionary = [];
        foreach (var counter in counters)
        {
            var values = counter.InstanceName.Split('_');
            int index = Array.IndexOf(values, "phys") + 1;
            int key = int.Parse(values[index]);
            if (!dictionary.TryGetValue(key, out var value))
            {
                value = [];
                dictionary.Add(key, value);
            }
            value.Add(counter);
        }
        return dictionary;
    }
    private static Dictionary<int, (PerformanceCounter, PerformanceCounter, PerformanceCounter)> GetNetworkCounters()
    {
        var category = new PerformanceCounterCategory("Network Interface");
        var counterNames = category.GetInstanceNames();

        Dictionary<int, (PerformanceCounter, PerformanceCounter, PerformanceCounter)> dictionary = [];
        foreach (string name in counterNames)
            dictionary.Add(Array.IndexOf(counterNames, name), new
                (
                    new("Network Interface", "Bytes Sent/sec", name),
                    new("Network Interface", "Bytes Received/sec", name),
                    new("Network Interface", "Current Bandwidth", name)
                ));
        return dictionary;
    }
}

/// <summary>Wrapper for the <see cref="MEMORYSTATUSEX"/> structure.</summary>
public class MemoryStatus
{
    const float BytesInGigabyte = 1073741824;
    MEMORYSTATUSEX status;

    private MemoryStatus(MEMORYSTATUSEX status) => this.status = status;

    /// <summary>Tries to create the <see cref="MemoryStatus"/> class.</summary>
    /// <param name="status">If the creation succeeds, contains the created <see cref="MemoryStatus"/>. Otherwise, <see langword="null"/>.</param>
    /// <returns>Whether the <see cref="MemoryStatus"/> was created.</returns>
    public static bool TryCreate(out MemoryStatus status)
    {
        MEMORYSTATUSEX memStatus = new();
        bool created = PInvoke.GlobalMemoryStatusEx(ref memStatus);
        status = created ? new(memStatus) : null;
        return created;
    }

    /// <summary>Gets the total memory the system has in gigabytes.</summary>
    public float TotalMemory => status.ullTotalPhys / BytesInGigabyte;
    /// <summary>Gets the available memory the system has in gigabytes.</summary>
    public float AvailableMemory => status.ullAvailVirtual / BytesInGigabyte;
    /// <summary>Gets the memory the system is using in gigabytes.</summary>
    public float UsedMemory => (status.ullTotalPhys - status.ullAvailPhys) / BytesInGigabyte;
}
