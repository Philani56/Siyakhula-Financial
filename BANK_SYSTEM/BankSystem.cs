using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace BANK_SYSTEM
{
    public class BankSystem
    {
        private string connectionString = "Data Source=labVMH8OX\\SQLEXPRESS;Initial Catalog=Banking;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";


        // Register a new person and save to the SQL database
        public void RegisterPerson(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO People (Name) VALUES (@Name)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name);
                    command.ExecuteNonQuery();
                }
            }
            Console.WriteLine($"Person '{name}' registered successfully in the database.");
        }

        // Get registered names from the database
        public List<string> GetRegisteredNames()
        {
            List<string> registeredNames = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Name FROM People";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            registeredNames.Add(reader.GetString(0)); // Read the Name column
                        }
                    }
                }
            }

            return registeredNames;
        }


    }
}
