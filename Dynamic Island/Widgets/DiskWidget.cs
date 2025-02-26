namespace Dynamic_Island.Widgets
{
    public sealed partial class DiskWidget : DualResourceWidget
    {
        float read = 0;
        float write = 0;
        public DiskWidget()
        {
            Color = new(0x21, 0x88, 0x88);
        }

        protected override Task<double> DataRequested(ResourceGraph graph) => Task.Run(() => (double)(read = DiskHelper.DiskRead));
        protected override double Data2Requested(ResourceGraph graph) => write = DiskHelper.DiskWrite;
        protected override string PrimaryTextRequested(TextBlock textBlock) => $"{(int)read} KB/s";
        protected override string SecondaryTextRequested(TextBlock textBlock) => $"{(int)write} KB/s";
    }
}
