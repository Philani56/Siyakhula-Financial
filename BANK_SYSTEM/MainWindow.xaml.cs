using System.Windows;

namespace BANK_SYSTEM
{
    public partial class MainWindow : Window
    {
        private BankSystem _bankSystem;

        public MainWindow()
        {
            InitializeComponent();
            _bankSystem = new BankSystem();
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Event handler for "Register" button
        private void AccountInfo_Click(object sender, RoutedEventArgs e)
        {
            // Open the Register window
            Register registerWindow = new Register();
            registerWindow.Show();
            this.Hide(); // Hide the current MainWindow
        }

        // Event handler for "Open an Account" button
        private void TransferFunds_Click(object sender, RoutedEventArgs e)
        {
            // Open the Open Account window
            // Open the Account window and pass the BankSystem instance
            Account accountWindow = new Account();
            accountWindow.ShowDialog();
            this.Hide(); // Hide the current MainWindow
        }

        // Event handler for "Deposit Money" button
        private void Deposit_Click(object sender, RoutedEventArgs e)
        {
            // Open the Deposit window
            Deposit depositWindow = new Deposit();
            depositWindow.Show();
            this.Hide(); // Hide the current MainWindow
        }

        // Event handler for "Withdraw Money" button
        private void Withdraw_Click(object sender, RoutedEventArgs e)
        {
            // Open the Withdraw Money window
            Withdraw_Money withdrawWindow = new Withdraw_Money();
            withdrawWindow.Show();
            this.Hide(); // Hide the current MainWindow
        }

        // Event handler for "Transfer Money" button
        private void Transfer_Click(object sender, RoutedEventArgs e)
        {
            // Open the Transfer Money window
            Transfer transferWindow = new Transfer();
            transferWindow.Show();
            this.Hide(); // Hide the current MainWindow
        }

        // Event handler for "Display Account" button
        private void Display_Click(object sender, RoutedEventArgs e)
        {
            // Open the Display Account window
            Display displayWindow = new Display();
            displayWindow.Show();
            this.Hide(); // Hide the current MainWindow
        }

        private void QRScan_Click(object sender, RoutedEventArgs e)
        {
            // Open the Display Account window
            QRScan displayWindow = new QRScan();
            displayWindow.Show();
            this.Hide(); // Hide the current MainWindow
        }
    }
}
