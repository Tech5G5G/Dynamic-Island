namespace Dynamic_Island.Controls
{
    public sealed partial class PillGrid : Grid
    {
        public InputCursor InputCursor
        {
            get => ProtectedCursor;
            set => ProtectedCursor = value;
        }
    }
}
