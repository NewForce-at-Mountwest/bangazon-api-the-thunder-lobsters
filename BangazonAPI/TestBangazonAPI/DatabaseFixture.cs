using BangazonAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace BangazonAPITests
{
    public class DatabaseFixture : IDisposable
    {

        private readonly string ConnectionString = @$"Server=localhost\SQLEXPRESS;Database=BangazonAPI;Trusted_Connection=True;";



        public Customer TestCustomer
        {
            get; set;
        }

        public DatabaseFixture()
        {

            Customer newCustomer = new Customer
            {
                FirstName = "Test name",
                LastName = "Test name",
                CreationDate = "2020-08-16",
                LastActiveDate = "2020-08-16"
            };

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @$"INSERT INTO Customer (FirstName, LastName, CreationDate, LastActiveDate)
                                        OUTPUT INSERTED.Id
                                        VALUES ('{newCustomer.FirstName}', '{newCustomer.LastName}', '{newCustomer.CreationDate}', '{newCustomer.LastActiveDate}')";


                    int newId = (int)cmd.ExecuteScalar();

                    newCustomer.Id = newId;

                    TestCustomer = newCustomer;
                }
            }

        }

        public void Dispose()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @$"DELETE FROM Customer WHERE FirstName ='Test name'";

                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
