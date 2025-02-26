namespace Dynamic_Island.Widgets
{
    public sealed partial class NetworkWidget : ResourceWidget
    {
        private static readonly int[] networks = Enumerable.Range(1, NetworkHelper.NetworkCount).ToArray();
        int network = 1;

        float usage = 0;
        public NetworkWidget()
        {
            Color = new(0x84, 0x43, 0x54);

            ButtonContent = "Next Network";
            ButtonVisibility = Visibility.Visible;
            ButtonClicked += (s, e) =>
            {
                ClearGraph();
                network = network == networks.Length ? 1 : networks[network];
            };
        }

        protected override Task<double> DataRequested(ResourceGraph graph) => Task.Run(() => (double)(usage = NetworkHelper.GetNetworkUsage(network)) * 100);
        protected override string PrimaryTextRequested(TextBlock textBlock) => usage.ToString("P0");
        protected override string SecondaryTextRequested(TextBlock textBlock) => string.Empty;
    }
}
