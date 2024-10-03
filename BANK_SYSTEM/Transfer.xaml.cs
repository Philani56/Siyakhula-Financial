using System;
using System.Data.SqlClient; // For SQL connection
using System.Windows;
using System.Windows.Media;

namespace BANK_SYSTEM
{
    public partial class Transfer : Window
    {
        private string connectionString = "Data Source=labVMH8OX\\SQLEXPRESS;Initial Catalog=Banking;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
        private decimal currentBalance = 0;

        public Transfer()
        {
            InitializeComponent();
            LoadRegisteredNames();
        }

        // Method to load registered names into the ComboBox
        private void LoadRegisteredNames()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Name FROM [Banking].[dbo].[Accounts]";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        RegisteredNamesComboBox.Items.Add(reader["Name"].ToString());
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading registered names: " + ex.Message);
            }
        }

        // Event when user selects their name from ComboBox
        private void RegisteredNamesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Ensure that the selected item is not null
            if (RegisteredNamesComboBox.SelectedItem != null)
            {
                // Safely convert the selected item to a string (or handle accordingly if it's an object)
                string selectedName = RegisteredNamesComboBox.SelectedItem.ToString();

                // Load account details based on the selected name
                LoadAccountDetails(selectedName);
            }
            else
            {
                // Optional: Clear the fields or display a message if no valid selection
                AccountNumberTextBox.Clear();
                
            }
        }

        // Method to load account details (Account Number and Balance) based on selected name
        private void LoadAccountDetails(string name)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT AccountNumber, InitialDeposit FROM [Banking].[dbo].[Accounts] WHERE Name = @Name";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", name);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        AccountNumberTextBox.Text = reader["AccountNumber"].ToString();
                        currentBalance = Convert.ToDecimal(reader["InitialDeposit"]);
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading account details: " + ex.Message);
            }
        }

        // Method to handle the Submit button click event
        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(AccountNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(RecipientNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(RecipientAccountNumberTextBox.Text) ||
                string.IsNullOrWhiteSpace(TransferAmountTextBox.Text) ||
                 BankSelectionComboBox.SelectedItem == null ||
                RegisteredNamesComboBox.SelectedItem == null)
            {
                CustomAlertDialog alertDialog = new CustomAlertDialog();
                alertDialog.ShowDialog("Please fill in all required fields.", this, Colors.Red, "Images/alert.png");
                return;
            }

            // Parse transfer amount
            if (!decimal.TryParse(TransferAmountTextBox.Text, out decimal transferAmount))
            {
                MessageBox.Show("Please enter a valid transfer amount.");
                return;
            }

            if (transferAmount > currentBalance)
            {
                // Create and show the custom alert dialog with a red background and error icon
                CustomAlertDialog alertDialog = new CustomAlertDialog();
                alertDialog.ShowDialog("Insufficient funds for this transaction.", this, Colors.Red, "Images/alert.png");
                return; // Exit the method if validation fails
            }

            // Proceed with the money transfer logic
            TransferMoney(transferAmount);
        }

        // Method to handle the money transfer logic and update the database
        private void TransferMoney(decimal transferAmount)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Update the user's account with the new balance
                    string updateQuery = "UPDATE [Banking].[dbo].[Accounts] SET InitialDeposit = InitialDeposit - @TransferAmount WHERE AccountNumber = @AccountNumber";
                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@TransferAmount", transferAmount);
                    updateCommand.Parameters.AddWithValue("@AccountNumber", AccountNumberTextBox.Text);
                    updateCommand.ExecuteNonQuery();

                    // (Optional) Insert transaction record in a transactions table

                    connection.Close();
                }

                CustomAlertDialog successDialog = new CustomAlertDialog();
                string message = $"Money transfer successful!\n" +
                                 $"Transferred Amount: {transferAmount:C}\n" +
                                 $"Remaining Balance: {currentBalance - transferAmount:C}";
                successDialog.ShowDialog(message, this, Colors.Green, "Images/success_icon.png");


                // Clear fields after successful transfer
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during money transfer: " + ex.Message);
            }
        }

        // Method to clear fields after a successful transfer
        private void ClearFields()
        {
            RegisteredNamesComboBox.SelectedItem = null;
            AccountNumberTextBox.Clear();
            RecipientNameTextBox.Clear();
            RecipientAccountNumberTextBox.Clear();
            TransferAmountTextBox.Clear();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Open the Display Account window
            MainWindow displayWindow = new MainWindow();
            displayWindow.Show();
            this.Hide(); // Hide the current MainWindow
        }
    }

}