using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BANK_SYSTEM
{
    public partial class Deposit : Window
    {
        private string connectionString = "Data Source=labVMH8OX\\SQLEXPRESS;Initial Catalog=Banking;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public Deposit()
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
                string initialDeposit = InitialDepositTextBox.Text;

                if (string.IsNullOrEmpty(accountNumber) || string.IsNullOrEmpty(initialDeposit))
                {
                    CustomAlertDialog alertDialog = new CustomAlertDialog();
                    alertDialog.ShowDialog("Please fill in all required fields.", this, Colors.Red, "Images/alert.png");
                    return; // Exit the method if validation fails
                }

                // Update deposit in the database
                string query = "UPDATE Accounts SET InitialDeposit = InitialDeposit + @InitialDeposit WHERE AccountNumber = @AccountNumber"; // Adjust the column name as necessary

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@InitialDeposit", decimal.Parse(initialDeposit)); // Make sure to parse the deposit
                        cmd.Parameters.AddWithValue("@AccountNumber", accountNumber);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            CustomAlertDialog successDialog = new CustomAlertDialog();
                            successDialog.ShowDialog("Deposit successfully updated!", this, Colors.Green, "Images/checked.png");

                            // Clear fields
                            RegisteredNamesComboBox.SelectedItem = null;
                            AccountNumberTextBox.Text = string.Empty;
                            InitialDepositTextBox.Text = string.Empty;
                        }
                        else
                        {
                            MessageBox.Show("Error updating deposit.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing deposit: {ex.Message}");
            }
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
