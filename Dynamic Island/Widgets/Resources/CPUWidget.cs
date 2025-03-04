namespace Dynamic_Island.Widgets.Resources
{
    public sealed partial class CPUWidget : ResourceWidget
    {
        float usage = 0;
        public CPUWidget()
        {
            Color = new(0x37, 0xA9, 0xCF);
        }

        protected override Task<double> DataRequested(ResourceGraph graph) => Task.Run(() => (double)(usage = CPUHelper.CPUUsage));
        protected override string PrimaryTextRequested(TextBlock textBlock) => $"{(int)usage}%";
        protected override string SecondaryTextRequested(TextBlock textBlock) => $"{CPUHelper.CPUClockage:G3} GHz";
    }
}
