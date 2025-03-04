namespace Dynamic_Island.Widgets.Resources
{
    public sealed partial class NetworkWidget : DualResourceWidget
    {
        private static readonly int[] networks = Enumerable.Range(1, NetworkHelper.NetworkCount).ToArray();

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
                Index = Index == networks.Length ? 1 : networks[Index];
            };
        }

        protected override Task<double> DataRequested(ResourceGraph graph) => Task.Run(() => (double)(receive = NetworkHelper.GetNetworkReceive(Index)));
        protected override double Data2Requested(ResourceGraph graph) => send = NetworkHelper.GetNetworkSend(Index);
        protected override string PrimaryTextRequested(TextBlock textBlock) => ((receive + send) / NetworkHelper.GetNetworkBandwidth(Index)).ToString("P0");
        protected override string SecondaryTextRequested(TextBlock textBlock) => string.Empty;
    }
}
