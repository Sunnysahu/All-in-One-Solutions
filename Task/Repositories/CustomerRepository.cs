using Microsoft.Data.SqlClient;
using System;
using System.Data;
using Task.Data;
using Task.Models;

namespace Task.Repositories
{
    public class CustomerRepository
    {
        private readonly DbConnection _db;

        public CustomerRepository(DbConnection db) => _db = db;

        public void AddCustomer(Customers customer)
        {
            string spName = "sp_InsertCustomer";
            SqlConnection connection = null;
            try
            {
                connection = _db.CreateConnection();
                using (SqlCommand cmd = new SqlCommand(spName, connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", customer.LastName);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@PhoneNumber", customer.PhoneNumber);
                    cmd.Parameters.AddWithValue("@City", customer.City);
                    cmd.Parameters.AddWithValue("@State", customer.State);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@PostalCode", customer.PostalCode);

                    connection.Open();
                    var result = cmd.ExecuteNonQuery();

                    Console.WriteLine(result > 0 ? "Customer added successfully." : "Failed to add customer.");
                }
            }
            catch (Exception ex)
            {
                if (ex is SqlException)
                {
                    Console.WriteLine("Database error: " + ex.Message);
                }
                else
                {
                    Console.WriteLine("Unexpected error: " + ex.Message);
                }

                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public void GetAllCustomers()
        {
            string query = "Select * From Customer";
            SqlConnection connection = null;
            try
            {
                connection = _db.CreateConnection();
                connection.Open();
                
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("Customer List:");

                    while (reader.Read())
                    {
                        int id = Convert.ToInt32(reader["CustomerId"]);
                        string firstName = reader["FirstName"].ToString();
                        string lastName = reader["LastName"].ToString();
                        string address = reader["Address"].ToString();
                        string phoneNumber = reader["PhoneNumber"].ToString();
                        string city = reader["City"].ToString();
                        string state = reader["State"].ToString();
                        string email = reader["Email"].ToString();
                        string postalCode = reader["PostalCode"].ToString();

                        Console.WriteLine($"{id} | {firstName} {lastName} | {address} | {phoneNumber} | {city} | {state} | {email} | {postalCode}");
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("An error occurred while retrieving customers.");
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
