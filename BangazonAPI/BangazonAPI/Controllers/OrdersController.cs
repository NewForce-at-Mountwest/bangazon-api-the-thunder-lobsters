using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;

//TODO: Handle PaymentTypeId if there is no payment

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrdersController(IConfiguration config)
        {
            _config = config;
        }

        //Creates a new SQL connection to edit the database
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        //A method to get all objects in the selected table in the database. Enables GET in the API
        [HttpGet]
        public async Task<IActionResult> Get(string _completed, string _include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string query = @"SELECT o.Id AS 'OrderKey', o.CustomerId AS 'OrderCustomerId', o.PaymentTypeId AS 'OrderPaymentTypeId'
                                     FROM [Order] o ";

                    if (_include == "products")
                    {
                        query = @"SELECT o.Id AS 'OrderKey', o.CustomerId AS 'OrderCustomerId', o.PaymentTypeId AS 'OrderPaymentTypeId', p.Id AS 'ProductKey', p.ProductTypeId, p.CustomerId as 'ProductCustomerId', p.Price, p.Title, p.Description, p.Quantity
                                  FROM [Order] o 
                                  LEFT JOIN OrderProduct op ON o.Id = op.OrderId
                                  LEFT JOIN Product p ON op.ProductId = p.Id ";
                    }

                    if (_include == "customer")
                    {
                        query = @"SELECT o.Id AS 'OrderKey', o.CustomerId AS 'OrderCustomerId', o.PaymentTypeId AS 'OrderPaymentTypeId', c.Id AS 'CustomerKey', c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate
                                  FROM [Order] o 
                                  LEFT JOIN Customer c ON o.CustomerId = c.Id";
                    }

                    if (_completed == "false")
                    {
                        query += @"WHERE PaymentTypeId IS NULL";
                    }
                    else if (_completed == "true")
                    {
                        query += @"WHERE PaymentTypeId IS NOT NULL";
                    }

                    //Executes the command to get all objects
                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader();

                    //A list to hold the C# version of the SQL objects
                    List<Order> orderList = new List<Order>();

                    while (reader.Read())
                    {
                        Order orderFromRepo = null;
                        //Creates a new object from the data found in the corresponding columns of the database
                        if (!reader.IsDBNull("OrderPaymentTypeId"))
                        {
                            orderFromRepo = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderKey")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("OrderCustomerId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("OrderPaymentTypeId"))
                            };
                        }
                        else
                        {
                            orderFromRepo = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderKey")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("OrderCustomerId")),
                                PaymentTypeId = null
                            };
                        }

                        if (!orderList.Any(orderFromList => orderFromList.Id == orderFromRepo.Id))
                        {
                            orderList.Add(orderFromRepo);
                        }

                        if (_include == "products")
                        {
                            Order orderToAddTo = orderList.FirstOrDefault(orderFromList => orderFromList.Id == orderFromRepo.Id);

                            if (!reader.IsDBNull("ProductKey"))
                            {
                                Product productFromRepo = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductKey")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("ProductCustomerId")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                };

                                if (!orderToAddTo.productsInOrder.Any(productFromList => productFromList.Id == productFromRepo.Id))
                                {
                                    orderToAddTo.productsInOrder.Add(productFromRepo);
                                }
                            }
                        }

                        if (_include == "customer")
                        {
                            Order orderToAddTo = orderList.FirstOrDefault(orderFromList => orderFromList.Id == orderFromRepo.Id);

                            if (!reader.IsDBNull("CustomerKey"))
                            {
                                Customer customerFromRepo = new Customer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CustomerKey")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    AccountCreated = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                    LastActive = reader.GetDateTime(reader.GetOrdinal("LastActiveDate"))
                                };

                                orderToAddTo.customerForOrder = customerFromRepo; 
                            }
                        }
                    }
                    //Closes the SQL reader
                    reader.Close();

                    //Returns a 200 status code and the newly created object list
                    return Ok(orderList);
                }
            }
        }

        //A method to get a single object in the selected table in the database. Enables GET in the API
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get([FromRoute] int id, string _include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string query = @"SELECT o.Id AS 'OrderKey', o.CustomerId AS 'OrderCustomerId', o.PaymentTypeId AS 'OrderPaymentTypeId'
                                     FROM [Order] o 
                                     WHERE o.Id = @id ";

                    if (_include == "products")
                    {
                        query = @"SELECT o.Id AS 'OrderKey', o.CustomerId AS 'OrderCustomerId', o.PaymentTypeId AS 'OrderPaymentTypeId', p.Id AS 'ProductKey', p.ProductTypeId, p.CustomerId as 'ProductCustomerId', p.Price, p.Title, p.Description, p.Quantity
                                  FROM [Order] o 
                                  LEFT JOIN OrderProduct op ON o.Id = op.OrderId
                                  LEFT JOIN Product p ON op.ProductId = p.Id 
                                  WHERE o.Id = @id ";
                    }

                    if (_include == "customer")
                    {
                        query = @"SELECT o.Id AS 'OrderKey', o.CustomerId AS 'OrderCustomerId', o.PaymentTypeId AS 'OrderPaymentTypeId', c.Id AS 'CustomerKey', c.FirstName, c.LastName, c.CreationDate, c.LastActiveDate
                                  FROM [Order] o 
                                  LEFT JOIN Customer c ON o.CustomerId = c.Id
                                  WHERE o.Id = @id ";
                    }

                    //Executes the command to get one object
                    cmd.CommandText = query;
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Order orderFromRepo = null;

                    while (reader.Read())
                    {
                        //Creates a new object from the data found in the corresponding columns of the database
                        if (!reader.IsDBNull("OrderPaymentTypeId"))
                        {
                            orderFromRepo = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderKey")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("OrderCustomerId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("OrderPaymentTypeId"))
                            };
                        }
                        else
                        {
                            orderFromRepo = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OrderKey")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("OrderCustomerId")),
                                PaymentTypeId = null
                            };
                        }

                        if (_include == "products")
                        {
                            if (!reader.IsDBNull("ProductKey"))
                            {
                                Product productFromRepo = new Product
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ProductKey")),
                                    ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                    CustomerId = reader.GetInt32(reader.GetOrdinal("ProductCustomerId")),
                                    Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                };

                                orderFromRepo.productsInOrder.Add(productFromRepo);
                            }
                        }

                        if (_include == "customer")
                        {
                            if (!reader.IsDBNull("CustomerKey"))
                            {
                                Customer customerFromRepo = new Customer
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CustomerKey")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    AccountCreated = reader.GetDateTime(reader.GetOrdinal("CreationDate")),
                                    LastActive = reader.GetDateTime(reader.GetOrdinal("LastActiveDate"))
                                };

                                orderFromRepo.customerForOrder = customerFromRepo;
                            }
                        }
                    }
                    //Closes the SQL reader
                    reader.Close();

                    //Returns a 200 status code and the newly created object
                    return Ok(orderFromRepo);
                }
            }
        }

        //A method to create a new object and add it to the database. Enables use of the POST command. Accepts a C# object as a parameter -- this object will be added to the database.
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order orderToAdd)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (orderToAdd.PaymentTypeId != null)
                    {
                        //SQL query to insert the object parameter into the database. Properties of the object are passed into the query as SQL parameters
                        cmd.CommandText = @"INSERT INTO [Order] (CustomerId, PaymentTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@customerId, @paymentTypeId)";

                        cmd.Parameters.Add(new SqlParameter("@customerId", orderToAdd.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@paymentTypeId", orderToAdd.PaymentTypeId));
                    }
                    else
                    {
                        cmd.CommandText = @"INSERT INTO [Order] (CustomerId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@customerId)";

                        cmd.Parameters.Add(new SqlParameter("@customerId", orderToAdd.CustomerId));
                    }

                    int newId = (int)cmd.ExecuteScalar();
                    orderToAdd.Id = newId;

                    //Returns a 201 status code and the newly created object
                    return CreatedAtRoute("GetOrder", new { id = newId }, orderToAdd);
                }
            }
        }

        //Allows the user to edit any given object in the database. Enables usage of the PUT command. Accepts two parameters: the id of the object to edit, and an object containing the new properties for the edit object
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Order orderToAdd)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        //SQL query to update the object at the id in the database. Properties of the object parameter are passed into the query as SQL parameters
                        cmd.CommandText = @"UPDATE [Order]
                                            SET CustomerId = @customerId,
                                                PaymentTypeId = @paymentTypeId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@customerId", orderToAdd.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@paymentTypeId", orderToAdd.PaymentTypeId));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        //If any rows are affected, returns a 204 status. Otherwise, outputs "no rows affected"
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }

            //Catches the above exception, if thrown
            catch (Exception)
            {
                //Returns a 404 if no object exists at the passed-in id
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //Allows the user to delete objects from the database. Enables use of the DELETE command. Accepts a parameter for the id of the object to be deleted
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        //Performs a SQL query to delete the object at the id parameter
                        cmd.CommandText = @"DELETE FROM [Order] WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        cmd.CommandText = @"DELETE FROM OrderProduct WHERE OrderId = @id";
                        cmd.ExecuteNonQuery();

                        //If any rows are affected, returns a 204 status. Otherwise, outputs "no rows affected"
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }

            //Catches the above exception, if thrown
            catch (Exception)
            {
                //Returns a 404 if no object exists at the passed-in id
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //A method to check if an object exists in the database, used by the Delete and Edit methods. Accepts an id parameter to know which object to check the existence of
        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Executes SQL query to select the desired object
                    cmd.CommandText = @"SELECT Id, CustomerId, PaymentTypeId
                                        FROM [Order] 
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    //Returns true if an object exists and the reader has something to read, else is false
                    return reader.Read();
                }
            }
        }
    }
}