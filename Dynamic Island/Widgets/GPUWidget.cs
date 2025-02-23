namespace Dynamic_Island.Widgets
{
    public sealed partial class GPUWidget : ResourceWidget
    {
        private static readonly int[] gpus = Enumerable.Range(1, ResourceHelper.GPUCount).ToArray();
        int gpu = 1;

        float usage = 0;
        public GPUWidget()
        {
            Color = new(0x98, 0x4F, 0xA4);

            ButtonContent = "Next GPU";
            ButtonVisibility = Visibility.Visible;
            ButtonClicked += (s, e) =>
            {
                ClearGraph();
                gpu = gpu == gpus.Length ? 1 : gpus[gpu];
            };
        }

        protected override async Task<string> PrimaryTextRequested(TextBlock textBlock) => $"{(int)(usage = await ResourceHelper.GetGPUUsage(gpu))}%";
        protected override string SecondaryTextRequested(TextBlock textBlock) => string.Empty;
        protected override double DataRequested(ResourceGraph graph) => usage;
    }
}
