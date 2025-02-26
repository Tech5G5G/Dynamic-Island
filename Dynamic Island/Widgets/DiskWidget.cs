namespace Dynamic_Island.Widgets
{
    public sealed partial class DiskWidget : ResourceWidget
    {
        float usage = 0;
        public DiskWidget()
        {
            Color = new(0x21, 0x88, 0x88);
        }

        protected override Task<double> DataRequested(ResourceGraph graph) => Task.Run(() => (double)(usage = DiskHelper.DiskUsage));
        protected override string PrimaryTextRequested(TextBlock textBlock) => $"{(int)usage}%";
        protected override string SecondaryTextRequested(TextBlock textBlock) => /*$"{CPUHelper.CPUClockage:G3} GHz"*/string.Empty;
    }
}
