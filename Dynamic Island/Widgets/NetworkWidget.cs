namespace Dynamic_Island.Widgets
{
    public sealed partial class NetworkWidget : DualResourceWidget
    {
        private static readonly int[] networks = Enumerable.Range(1, NetworkHelper.NetworkCount).ToArray();
        int network = 1;

        float receive = 0;
        float send = 0;
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

        protected override Task<double> DataRequested(ResourceGraph graph) => Task.Run(() => (double)(receive = NetworkHelper.GetNetworkReceive(network)));
        protected override double Data2Requested(ResourceGraph graph) => send = NetworkHelper.GetNetworkSend(network);
        protected override string PrimaryTextRequested(TextBlock textBlock) => ((receive + send) / NetworkHelper.GetNetworkBandwidth(network)).ToString("P0");
        protected override string SecondaryTextRequested(TextBlock textBlock) => string.Empty;
    }
}
