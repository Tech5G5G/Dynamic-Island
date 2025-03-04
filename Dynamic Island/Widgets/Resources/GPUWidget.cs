namespace Dynamic_Island.Widgets.Resources
{
    public sealed partial class GPUWidget : ResourceWidget
    {
        private static readonly int[] gpus = Enumerable.Range(1, GPUHelper.GPUCount).ToArray();

        float usage = 0;
        public GPUWidget()
        {
            Color = new(0x98, 0x4F, 0xA4);

            ButtonContent = "Next GPU";
            ButtonVisibility = Visibility.Visible;
            ButtonClicked += (s, e) =>
            {
                ClearGraph();
                Index = Index == gpus.Length ? 1 : gpus[Index];
            };
        }

        protected override async Task<double> DataRequested(ResourceGraph graph) => usage = await GPUHelper.GetGPUUsage(Index);
        protected override string PrimaryTextRequested(TextBlock textBlock) => $"{(int)usage}%";
        protected override string SecondaryTextRequested(TextBlock textBlock) => string.Empty;
    }
}
