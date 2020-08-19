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
        
        //Creates two product objects for testing: one for the GET, POST, and PUT tests, and another for the DELETE method
        public Product TestProduct { get; set; }
        public Product TestDeleteProduct { get; set; }
        public DatabaseFixture()
        {

            Customer newCustomer = new Customer
            {
                FirstName = "Test name",
                LastName = "Test name",
                CreationDate = "2020-08-16",
                    LastActiveDate = "2020-08-16"
                    };
       

        

        
            //Object for GET, POST, PUT
            Product newProduct = new Product
            {
                ProductTypeId = 1,
                CustomerId = 1,
                Price = Convert.ToDecimal(1.80),
                Title = "Integration Test Product",
                Description = "Integration Test Product",
                Quantity = 1
            };

            //Object for DELETE
            Product newDeleteProduct = new Product
            {
                ProductTypeId = 1,
                CustomerId = 1,
                Price = Convert.ToDecimal(1.80),
                Title = "Integration Test Product",
                Description = "Integration Delete Test Product",
                Quantity = 1
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
            
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Inserts the first object into the database
                    cmd.CommandText = $@"INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
                                        OUTPUT INSERTED.Id
                                        VALUES ({newProduct.ProductTypeId}, {newProduct.CustomerId}, {newProduct.Price}, '{newProduct.Title}', '{newProduct.Description}', {newProduct.Quantity})";

                    //Executes the above query, returns the id of the newly created object, assigns the id to the newProduct, then sets it to the global TestProduct variable
                    int newId = (int)cmd.ExecuteScalar();
                    newProduct.Id = newId;
                    TestProduct = newProduct;
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Executes exactly like the first object creation
                    cmd.CommandText = $@"INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
                                        OUTPUT INSERTED.Id
                                        VALUES ({newDeleteProduct.ProductTypeId}, {newDeleteProduct.CustomerId}, {newDeleteProduct.Price}, '{newDeleteProduct.Title}', '{newDeleteProduct.Description}', {newDeleteProduct.Quantity})";
                    int newId = (int)cmd.ExecuteScalar();
                    newDeleteProduct.Id = newId;
                    TestDeleteProduct = newDeleteProduct;
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
                    
                    //Disposes of all test products when the tests finish
                    cmd.CommandText = @$"DELETE FROM Product WHERE Title='Integration Test Product'";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}