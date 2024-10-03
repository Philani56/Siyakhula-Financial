using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BANK_SYSTEM
{
    public partial class Withdraw_Money : Window
    {
        private string connectionString = "Data Source=labVMH8OX\\SQLEXPRESS;Initial Catalog=Banking;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public Withdraw_Money()
        {
            InitializeComponent();
            LoadRegisteredNames();
        }

        private void LoadRegisteredNames()
        {
            // Load registered names from the database into the ComboBox
            string query = "SELECT Name FROM People"; // Adjust table name as necessary
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RegisteredNamesComboBox.Items.Add(reader["Name"].ToString());
                        }
                    }
                }
            }
        }

        private void RegisteredNamesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get account number based on selected name
            string selectedName = RegisteredNamesComboBox.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedName))
            {
                string query = "SELECT AccountNumber FROM Accounts WHERE Name = @Name"; // Adjust table name and field name as necessary
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", selectedName);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            AccountNumberTextBox.Text = result.ToString(); // Set the account number in the TextBox
                        }
                    }
                }
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string accountNumber = AccountNumberTextBox.Text;
                string withdrawalAmountText = WithdrawalAmountTextBox.Text;

                if (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(withdrawalAmountText))
                {
                    // Create and show the custom alert dialog with a red background and error icon
                    CustomAlertDialog alertDialog = new CustomAlertDialog();
                    alertDialog.ShowDialog("Please fill in all required fields.", this, Colors.Red, "Images/alert.png");
                    return; // Exit the method if validation fails
                }

                // Parse the withdrawal amount
                decimal withdrawalAmount = decimal.Parse(withdrawalAmountText);

                // Check if the withdrawal amount is valid (i.e., less than or equal to the current balance)
                decimal currentBalance;
                string balanceQuery = "SELECT InitialDeposit FROM Accounts WHERE AccountNumber = @AccountNumber"; // Adjust table name as necessary
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(balanceQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);
                        currentBalance = (decimal)cmd.ExecuteScalar(); // Get the current balance

                        // Check if there are sufficient funds
                        if (withdrawalAmount > currentBalance)
                        {
                            // Create and show the custom alert dialog with a red background and error icon
                            CustomAlertDialog alertDialog = new CustomAlertDialog();
                            alertDialog.ShowDialog("Insufficient funds for this withdrawal.", this, Colors.Red, "Images/alert.png");
                            return; // Exit the method if validation fails
                        }
                    }
                }

                // Update the deposit in the database
                string query = "UPDATE Accounts SET InitialDeposit = InitialDeposit - @InitialDeposit WHERE AccountNumber = @AccountNumber"; // Adjust column name as necessary

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InitialDeposit", withdrawalAmount);
                        cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            CustomAlertDialog successDialog = new CustomAlertDialog();
                            successDialog.ShowDialog("Withdrawal successful!! OTP sent to your mobile. Use it to complete your transaction.", this, Colors.Green, "Images/checked.png");
                            // Clear fields
                            RegisteredNamesComboBox.SelectedItem = null;
                            AccountNumberTextBox.Text = string.Empty;
                            WithdrawalAmountTextBox.Text = string.Empty;
                        }
                        else
                        {
                            // Create and show the custom alert dialog with a red background and error icon
                            CustomAlertDialog alertDialog = new CustomAlertDialog();
                            alertDialog.ShowDialog("Withdrawal failed. Please try again.", this, Colors.Red, "Images/alert.png");
                            return; // Exit the method if validation fails
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid withdrawal amount.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Open the MainWindow (or any other window)
            MainWindow displayWindow = new MainWindow();
            displayWindow.Show();
            this.Hide(); // Hide the current Register window
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Minimize the window
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized; // Toggle maximize/restore
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the application
        }
    }
}
