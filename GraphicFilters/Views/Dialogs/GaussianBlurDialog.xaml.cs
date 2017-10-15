using System.Windows;

namespace GraphicFilters.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for GaussianBlurDialog.xaml
    /// </summary>
    public partial class GaussianBlurDialog : Window
    {
        public GaussianBlurDialog()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }
    }
}
