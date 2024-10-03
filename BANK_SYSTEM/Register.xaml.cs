using System;
using System.Windows;
using System.Windows.Media;

namespace BANK_SYSTEM
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim(); // Get the name from the TextBox and trim any whitespace

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a valid name.", "Registration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Exit the method if the name is invalid
            }

            // Create an instance of the BankSystem and register the person in the database
            BankSystem bankSystem = new BankSystem();
            bankSystem.RegisterPerson(name); // Register the new person

            CustomAlertDialog successDialog = new CustomAlertDialog();
            successDialog.ShowDialog("Registered successfully!", this, Colors.Green, "Images/checked.png");

            // Optionally, you can navigate to another window or reset the TextBox
            NameTextBox.Clear(); // Clear the input field after successful registration
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Open the MainWindow (or any other window)
            MainWindow displayWindow = new MainWindow();
            displayWindow.Show();
            this.Hide(); // Hide the current Register window
        }
    }
}
