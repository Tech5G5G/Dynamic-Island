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

        protected override Task<double> DataRequested(ResourceGraph graph) => Task.Run(() => MemoryStatus.TryCreate(out status) ? (double)(percentUsage = status.UsedMemory / status.TotalMemory) * 100 : percentUsage * 100);
        protected override string PrimaryTextRequested(TextBlock textBlock) => $"{status.UsedMemory:0.##} GB";
        protected override string SecondaryTextRequested(TextBlock textBlock) => percentUsage.ToString("P0");
    }
}
