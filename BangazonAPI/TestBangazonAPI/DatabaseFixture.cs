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

        private readonly string ConnectionString = @$"Server=localhost\SQLEXPRESS;Database=BangazonAPI;Trusted_Connection=True;";


        //creates two customer objects for GET, POST, PUT and another for DELETE
        public Customer TestCustomer
        {
            get; set;
        }
        
        //Creates two product objects for testing: one for the GET, POST, and PUT tests, and another for the DELETE method
       
        public PaymentType TestPaymentType { get; set; }
        public ProductType TestProductType { get; set; }

        //Creates two product objects for testing: one for the GET, POST, and PUT tests, and another for the DELETE method
        public Product TestProduct { get; set; }
        public Product TestDeleteProduct { get; set; }
        public Order TestOrder { get; set; }
        public Order TestDeleteOrder { get; set; }
        public DatabaseFixture()
        {

            Customer newCustomer = new Customer
            {
                FirstName = "Test name",
                LastName = "Test name",
                CreationDate = "2020-08-16",
                    LastActiveDate = "2020-08-16"
                    };

        





            PaymentType newpaymenttype = new PaymentType
            {
                
                AcctNumber = "test",
                Name = "hkkdkldnhjsk",
                CustomerId = 2
            };

            ProductType newProductType = new ProductType
            {
                Name = "Test Product Type",
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
            //CUSTOMER OBJECT

            //New Order object for GET, POST, PUT
            Order newOrder = new Order
            {
                CustomerId = 2,
                PaymentTypeId = 3
            };

            //New Order object for DELETE
            Order newDeleteOrder = new Order
            {
                CustomerId = 2,
                PaymentTypeId = 3
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
                    cmd.CommandText = @$"INSERT INTO ProductType (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES ( '{newProductType.Name}')";
                    int newId = (int)cmd.ExecuteScalar();
                    newProductType.Id = newId;
                    TestProductType = newProductType;
                }
            
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
                    //Inserts the first product object into the database
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

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Executes exactly like the product object creation
                    cmd.CommandText = $@"INSERT INTO [Order] (CustomerId, PaymentTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES ({newOrder.CustomerId}, {newOrder.PaymentTypeId})";
                    int newId = (int)cmd.ExecuteScalar();
                    newOrder.Id = newId;
                    TestOrder = newOrder;
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Executes exactly like the product object creation
                    cmd.CommandText = $@"INSERT INTO [Order] (CustomerId, PaymentTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES ({newDeleteOrder.CustomerId}, {newDeleteOrder.PaymentTypeId})";
                    int newId = (int)cmd.ExecuteScalar();
                    newDeleteOrder.Id = newId;
                    TestDeleteOrder = newDeleteOrder;
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
                    
                    cmd.CommandText = @$"DELETE FROM PaymentType WHERE AcctNumber='test'";
                     cmd.ExecuteNonQuery();
                    cmd.CommandText = @$"DELETE FROM ProductType WHERE Name='Test Product Type' ";
                    cmd.CommandText += @$"DELETE FROM PaymentType WHERE AcctNumber='test' ";
                    //Disposes of all test products when the tests finish
                    cmd.CommandText += @$"DELETE FROM Product WHERE Title='Integration Test Product' ";

                    //Disposes of all test orders when the tests finish
                    cmd.CommandText += @$"DELETE o FROM [Order] o 
                                        JOIN PaymentType pt ON o.PaymentTypeId = pt.Id
                                        WHERE pt.Name='INTEGRATION TEST' ";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

