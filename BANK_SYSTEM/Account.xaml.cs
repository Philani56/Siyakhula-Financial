using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BANK_SYSTEM
{
    public partial class Account : Window
    {
        // Connection string to your database
        private string connectionString = "Data Source=labVMH8OX\\SQLEXPRESS;Initial Catalog=Banking;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public Account()
        {
            InitializeComponent();
            LoadRegisteredNames(); // Load registered names when the window opens
        }

        // Method to load registered names from the database into the ComboBox
        private void LoadRegisteredNames()
        {
            try
            {
                string query = "SELECT Name FROM People"; // Ensure table and column name matches your database

                List<string> registeredNames = new List<string>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                registeredNames.Add(reader["Name"].ToString());
                            }
                        }
                    }
                }

                RegisteredNamesComboBox.ItemsSource = registeredNames; // Set ComboBox's items
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading registered names: {ex.Message}");
            }
        }

        // Method to save new account information to the database
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get data from the UI
                string selectedName = RegisteredNamesComboBox.SelectedItem?.ToString();
                string accountType = (AccountTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
                string accountNumber = AccountNumber.Text;
                string initialDeposit = InitialDepositTextBox.Text;

                if (string.IsNullOrEmpty(selectedName) || string.IsNullOrEmpty(accountType) || string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(initialDeposit))
                {
                    // Create and show the custom alert dialog with a red background and error icon
                    CustomAlertDialog alertDialog = new CustomAlertDialog();
                    alertDialog.ShowDialog("Please fill in all required fields.", this, Colors.Red, "Images/alert.png");
                    return; // Exit the method if validation fails
                    
                }

                // Insert account data into the database
                string query = "INSERT INTO Accounts (Name, AccountType, AccountNumber, InitialDeposit) VALUES (@Name, @AccountType, @AccountNumber, @InitialDeposit)";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", selectedName);
                        cmd.Parameters.AddWithValue("@AccountType", accountType);
                        cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                        cmd.Parameters.AddWithValue("@InitialDeposit", initialDeposit);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            CustomAlertDialog successDialog = new CustomAlertDialog();
                            successDialog.ShowDialog("Account created successfully!", this, Colors.Green, "Images/checked.png");

                            // Clear all fields after saving
                            RegisteredNamesComboBox.SelectedItem = null;
                            AccountTypeComboBox.SelectedItem = null;
                            AccountNumber.Text = string.Empty;
                            InitialDepositTextBox.Text = string.Empty;

                        }
                        else
                        {
                            MessageBox.Show("Error creating account.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving account: {ex.Message}");
            }
        }

        // Cancel button event handler
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Open the Display Account window
            MainWindow displayWindow = new MainWindow();
            displayWindow.Show();
            this.Hide(); // Hide the current MainWindow
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
