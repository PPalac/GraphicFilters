using System.Windows;
using System.Windows.Input;

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

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                KernelInput.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}
