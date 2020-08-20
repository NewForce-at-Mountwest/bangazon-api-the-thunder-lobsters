using BangazonAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Linq;

namespace TestBangazonAPI
{
    public class DatabaseFixture : IDisposable
    {
        private readonly string ConnectionString = @$"Server=localhost\SQLEXPRESS03;Database=BangazonAPI;Trusted_Connection=True;";
        public PaymentType TestPaymentType { get; set; }
   
 //Creates two product objects for testing: one for the GET, POST, and PUT tests, and another for the DELETE method
        public Product TestProduct { get; set; }
        public Product TestDeleteProduct { get; set; }
        public DatabaseFixture()
        {
              PaymentType newpaymenttype = new PaymentType
            {
                
                AcctNumber = "test",
                Name = "hkkdkldnhjsk",
                CustomerId = 2
            };
            //Object for GET, POST, PUT
            Product newProduct = new Product
            {
                ProductTypeId = 1,
                CustomerId = 2,
                Price = Convert.ToDecimal(1.80),
                Title = "Integration Test Product",
                Description = "Integration Test Product",
                Quantity = 1
            };

            //Object for DELETE
            Product newDeleteProduct = new Product
            {
                ProductTypeId = 1,
                CustomerId = 2,
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
                    cmd.CommandText = @$"INSERT INTO PaymentType (AcctNumber, Name, CustomerId)
                                        OUTPUT INSERTED.Id
                                        VALUES ('{newpaymenttype.AcctNumber}', '{newpaymenttype.Name}', '{newpaymenttype.CustomerId}')";
                    int newId = (int)cmd.ExecuteScalar();
                    newpaymenttype.Id = newId;
                    TestPaymentType = newpaymenttype;
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
                    cmd.CommandText = @$"DELETE FROM PaymentType WHERE AcctNumber='test'";
                    //Disposes of all test products when the tests finish
                    cmd.CommandText = @$"DELETE FROM Product WHERE Title='Integration Test Product'";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

