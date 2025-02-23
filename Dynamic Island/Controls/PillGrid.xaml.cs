namespace Dynamic_Island.Controls
{
    public sealed partial class PillGrid : Grid
    {
        public PillGrid()
        {
            this.InitializeComponent();
        }

        public InputCursor InputCursor
        {
            get => ProtectedCursor;
            set => ProtectedCursor = value;
        }
    }
}
