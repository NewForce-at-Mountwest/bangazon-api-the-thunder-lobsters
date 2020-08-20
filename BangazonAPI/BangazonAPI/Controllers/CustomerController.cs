using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomerController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        //GET ALL with query string parameters to enable the user to view products and payment types with customer.
        //GET ALL also allows user to see if any properties contain a string typed in by the user in the form of 'q'
        // '%{ q }%' allows string query to check if q is contained anywhere in the property
        [HttpGet]
        public async Task<IActionResult> Get(string _include, string q)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string query = "SELECT Id, FirstName, LastName, CreationDate, LastActiveDate FROM Customer ";
                    if (_include == "products")
                    {
                        query = @$"SELECT c.Id AS 'Id', c.FirstName AS 'FirstName', c.LastName AS 'LastName', 
                            c.CreationDate AS 'CreationDate', c.LastActiveDate AS 'LastActiveDate', 
                            p.Id AS 'ProductId', p.ProductTypeId AS 'ProductTypeId', p.price AS 'Price', 
                            p.title AS 'title', p.quantity AS 'Quantity', p.description AS 'Description' FROM Customer c
                            LEFT JOIN product p ON p.customerId = c.Id ";
                    }
                    if (_include == "payments")
                    {
                        query = $"SELECT c.Id AS 'Id', c.FirstName AS 'FirstName', c.LastName AS 'LastName', " +
                            $"c.CreationDate AS 'CreationDate', c.LastActiveDate AS 'LastActiveDate', " +
                            $"t.AcctNumber AS 'Account#', t.Id AS 'PaymentTypeId', t.Name AS 'PaymentName' " +
                            $"FROM Customer c FULL JOIN PaymentType t ON t.CustomerId = c.Id ";
                    }
                    if (q != null)
                    {
                        query += $"WHERE FirstName LIKE '%{q}%' OR LastName LIKE '%{q}%'";
                    }
                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Customer> customers = new List<Customer>();
                   

                    while (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")).ToString(),
                            LastActiveDate = reader.GetDateTime(reader.GetOrdinal("LastActiveDate")).ToString(),
                          
                            
                        };

                        if (_include == "payments")
                        {
                            PaymentType paymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                AcctNumber = reader.GetInt32(reader.GetOrdinal("Account#")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Name = reader.GetString(reader.GetOrdinal("PaymentName"))
                            };

                            if (customers.FirstOrDefault(customer => customer.Id == paymentType.CustomerId) == null)
                            {
                                customer.payments.Add(paymentType);
                                customers.Add(customer);
                            }
                            else
                            {
                                Customer customerWithPayment = customers.FirstOrDefault(customer => customer.Id == paymentType.CustomerId);
                                customerWithPayment.payments.Add(paymentType);
                            }
                        }
                        else if (_include == "products")
                        {
                            Product product = new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                            };

                            if (customers.FirstOrDefault(customer => customer.Id == product.CustomerId) == null)
                            {
                                customer.products.Add(product);
                                customers.Add(customer);
                            }
                            else
                            {
                                Customer customerWithProducts = customers.FirstOrDefault(customer => customer.Id == product.CustomerId);
                                customerWithProducts.products.Add(product);
                            }

                            if (customers.FirstOrDefault(customer => customer.Id == product.CustomerId) == null)
                            {
                                customers.Add(customer);
                            }

                        }
                        else
                        {
                            customers.Add(customer);
                        }

                        
                    }
                    reader.Close();

                    return Ok(customers);
                }
            }
        }

        [HttpGet("{id}", Name = "GetCustomer")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, FirstName, LastName, CreationDate, LastActiveDate
                        FROM Customer
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Customer customer = null;

                    if (reader.Read())
                    {
                        customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CreationDate = reader.GetDateTime(reader.GetOrdinal("CreationDate")).ToString(),
                            LastActiveDate = reader.GetDateTime(reader.GetOrdinal("LastActiveDate")).ToString()
                            
                        };
                    }
                    reader.Close();

                    return Ok(customer);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Customer (FirstName, LastName, CreationDate, LastActiveDate)
                                        OUTPUT INSERTED.Id
                                        VALUES (@FirstName, @LastName, @CreationDate, @LastActiveDate)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", customer.LastName));
                    cmd.Parameters.Add(new SqlParameter("@CreationDate", customer.CreationDate));
                    cmd.Parameters.Add(new SqlParameter("@LastActiveDate", customer.LastActiveDate));
                   

                    int newId = (int)cmd.ExecuteScalar();
                    customer.Id = newId;
                    return CreatedAtRoute("GetCustomer", new
                    {
                        id = newId
                    }, customer);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Customer
                                            SET FirstName = @FirstName,
                                                LastName = @LastName,
                                                CreationDate = @CreationDate,
                                                LastActiveDate = @LastActiveDate
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@FirstName", customer.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", customer.LastName));
                        cmd.Parameters.Add(new SqlParameter("@CreationDate", customer.CreationDate));
                        cmd.Parameters.Add(new SqlParameter("@LastActiveDate", customer.LastActiveDate));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, CreationDate, LastActiveDate
                        FROM Customer
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    }
}
