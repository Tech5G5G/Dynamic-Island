namespace Dynamic_Island.Controls
{
    public sealed partial class URLDisplay : UserControl
    {
        public URLDisplay()
        {
            this.InitializeComponent();
        }

        public Uri URL
        {
            get { return (Uri)GetValue(URLProperty); }
            set { SetValue(URLProperty, value); }
        }
        public static readonly DependencyProperty URLProperty = DependencyProperty.Register("URL", typeof(Uri), typeof(URLDisplay), new PropertyMetadata(string.Empty));
    }
}
