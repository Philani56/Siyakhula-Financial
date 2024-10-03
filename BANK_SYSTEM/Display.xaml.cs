using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // For SaveFileDialog
using OfficeOpenXml; // For Excel generation
using iTextSharp.text; // For PDF generation
using iTextSharp.text.pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;



namespace BANK_SYSTEM
{
    public partial class Display : Window
    {
        private string connectionString = "Data Source=labVMH8OX\\SQLEXPRESS;Initial Catalog=Banking;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        // Store bank details in a list for easy access when generating files
        private List<BankDetail> bankDetails = new List<BankDetail>();

        public Display()
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
            if (RegisteredNamesComboBox.SelectedItem != null)
            {
                string selectedName = RegisteredNamesComboBox.SelectedItem.ToString();
                LoadBankDetails(selectedName);
            }
        }

        private void LoadBankDetails(string name)
        {
            // Clear previous bank details
            BankDetailsListView.Items.Clear();
            bankDetails.Clear(); // Clear the list for new entries

            // Fetch bank details from the database based on the selected name
            string query = "SELECT AccountType, AccountNumber, InitialDeposit FROM Accounts WHERE Name = @Name"; // Adjust table name and field name as necessary
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Store details in a list for later use
                            bankDetails.Add(new BankDetail
                            {
                                Field = "Account Type",
                                Value = reader["AccountType"].ToString()
                            });
                            bankDetails.Add(new BankDetail
                            {
                                Field = "Account Number",
                                Value = reader["AccountNumber"].ToString()
                            });
                            bankDetails.Add(new BankDetail
                            {
                                Field = "Initial Deposit",
                                Value = reader["InitialDeposit"].ToString()
                            });
                        }
                    }
                }
            }

            // Bind the bank details to the ListView
            BankDetailsListView.ItemsSource = bankDetails;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the window
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Minimize the window
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal; // Restore the window to normal size
            }
            else
            {
                this.WindowState = WindowState.Maximized; // Maximize the window
            }
        }




        private void DownloadPDF_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                Title = "Save a PDF File"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var pdfDoc = new iText.Kernel.Pdf.PdfDocument(new iText.Kernel.Pdf.PdfWriter(saveFileDialog.FileName)))
                {
                    var document = new iText.Layout.Document(pdfDoc);

                    // Create a title
                    document.Add(new iText.Layout.Element.Paragraph("Bank Details")
                        .SetFontSize(18)
                        .SetBold());

                    // Add bank details to the document
                    foreach (var detail in BankDetailsListView.Items)
                    {
                        if (detail is BankDetail bankDetail)
                        {
                            document.Add(new iText.Layout.Element.Paragraph($"{bankDetail.Field}: {bankDetail.Value}"));
                        }
                    }
                }

                MessageBox.Show("PDF file created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Open the MainWindow (or any other window)
            MainWindow displayWindow = new MainWindow();
            displayWindow.Show();
            this.Hide(); // Hide the current Register window
        }
    }

    public class BankDetail
    {
        public string Field { get; set; }
        public string Value { get; set; }
    }
}
