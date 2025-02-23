namespace Dynamic_Island.Widgets
{
    public sealed partial class NetworkWidget : ResourceWidget
    {
        private static readonly int[] networks = Enumerable.Range(1, ResourceHelper.NetworkCount).ToArray();
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

        //TODO: Fix slow startup
        //Fix Now Playing widget image corner radius binding
        //Fix WidgetRadius startup binding
        //Fix settings window weird behavior with window sizing
        protected override Task<string> PrimaryTextRequested(TextBlock textBlock) => Task.Run(() => $"{(int)(usage = ResourceHelper.GetNetworkUsage(network))}%");
        protected override string SecondaryTextRequested(TextBlock textBlock) => string.Empty;
        protected override double DataRequested(ResourceGraph graph) => usage;
    }
}
