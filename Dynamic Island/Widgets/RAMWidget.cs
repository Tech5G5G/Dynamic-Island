namespace Dynamic_Island.Widgets
{
    public sealed partial class RAMWidget : ResourceWidget
    {
        MemoryStatus status;
        float percentUsage = 0;
        public RAMWidget()
        {
            Color = new(0x4F, 0x7E, 0xC2);
        }

        protected override Task<string> PrimaryTextRequested(TextBlock textBlock) => Task.Run(() => MemoryStatus.TryCreate(out status) ? $"{status.UsedMemory:0.##} GB" : "0 GB");
        protected override string SecondaryTextRequested(TextBlock textBlock) => (percentUsage = status.UsedMemory / status.TotalMemory).ToString("P0");
        protected override double DataRequested(ResourceGraph graph) => percentUsage * 100;
    }
}
