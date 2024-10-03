using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BANK_SYSTEM
{
    public partial class CustomAlertDialog : UserControl
    {
        public CustomAlertDialog()
        {
            InitializeComponent();
        }

        public void ShowDialog(string message, Window owner, Color backgroundColor, string iconUri)
        {
            MessageTextBlock.Text = message;
            DialogBorder.Background = new SolidColorBrush(backgroundColor); // Set the background color
            AlertIcon.Source = new BitmapImage(new Uri(iconUri, UriKind.RelativeOrAbsolute)); // Set the icon

            Window dialogWindow = new Window
            {
                Title = "Alert",
                Content = this,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = owner,
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.Transparent
            };
            dialogWindow.ShowDialog();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window)?.Close();
        }
    }
}
