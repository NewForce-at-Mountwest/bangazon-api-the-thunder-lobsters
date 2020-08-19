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

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
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
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    string query = @"SELECT Id, ProductTypeId, CustomerId, Price, Title, Description, Quantity 
                                    FROM Product ";
                    
                    //Executes the command to get all objects
                    cmd.CommandText = query;
                    SqlDataReader reader = cmd.ExecuteReader();

                    //A list to hold the C# version of the SQL objects
                    List<Product> productList = new List<Product>();

                    while (reader.Read())
                    {
                        //Creates a new object from the data found in the corresponding columns of the database
                        Product productFromRepo = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                        };

                        productList.Add(productFromRepo);
                    }
                    //Closes the SQL reader
                    reader.Close();

                    //Returns a 200 status code and the newly created object list
                    return Ok(productList);
                }
            }
        }

        //A method to get a single object in the selected table in the database. Enables GET in the API
        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Executes the command to get an object
                    cmd.CommandText = @"SELECT Id, ProductTypeId, CustomerId, Price, Title, Description, Quantity 
                                        FROM Product 
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Product productFromRepo = null;

                    if (reader.Read())
                    {
                        //Creates a new object from the data found in the corresponding columns of the database
                        productFromRepo = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                        };
                    }
                    //Closes the SQL reader
                    reader.Close();

                    //Returns a 200 status code and the newly created object
                    return Ok(productFromRepo);
                }
            }
        }

        //A method to create a new object and add it to the database. Enables use of the POST command. Accepts a C# object as a parameter -- this object will be added to the database.
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product productToAdd)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //SQL query to insert the object parameter into the database. Properties of the object are passed into the query as SQL parameters
                    cmd.CommandText = @"INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, Description, Quantity)
                                        OUTPUT INSERTED.Id
                                        VALUES (@productTypeId, @customerId, @price, @title, @description, @quantity)";
                    cmd.Parameters.Add(new SqlParameter("@productTypeId", productToAdd.ProductTypeId));
                    cmd.Parameters.Add(new SqlParameter("@customerId", productToAdd.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@price", productToAdd.Price));
                    cmd.Parameters.Add(new SqlParameter("@title", productToAdd.Title));
                    cmd.Parameters.Add(new SqlParameter("@description", productToAdd.Description));
                    cmd.Parameters.Add(new SqlParameter("@quantity", productToAdd.Quantity));

                    int newId = (int)cmd.ExecuteScalar();
                    productToAdd.Id = newId;

                    //Returns a 201 status code and the newly created object
                    return CreatedAtRoute("GetProduct", new { id = newId }, productToAdd);
                }
            }
        }

        //Allows the user to edit any given object in the database. Enables usage of the PUT command. Accepts two parameters: the id of the object to edit, and an object containing the new properties for the edit object
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Product productToAdd)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        //SQL query to update the object at the id in the database. Properties of the object parameter are passed into the query as SQL parameters
                        cmd.CommandText = @"UPDATE Product
                                            SET ProductTypeId = @productTypeId,
                                                CustomerId = @customerId,
                                                Price = @price,
                                                Title = @title,
                                                Description = @description,
                                                Quantity = @quantity
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@productTypeId", productToAdd.ProductTypeId));
                        cmd.Parameters.Add(new SqlParameter("@customerId", productToAdd.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@price", productToAdd.Price));
                        cmd.Parameters.Add(new SqlParameter("@title", productToAdd.Title));
                        cmd.Parameters.Add(new SqlParameter("@description", productToAdd.Description));
                        cmd.Parameters.Add(new SqlParameter("@quantity", productToAdd.Quantity));
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
                if (!ProductExists(id))
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
                        cmd.CommandText = @"DELETE FROM Product WHERE Id = @id";
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
                if (!ProductExists(id))
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
        private bool ProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Executes SQL query to select the desired object
                    cmd.CommandText = @"SELECT Id, ProductTypeId, CustomerId, Price, Title, Description, Quantity 
                                        FROM Product 
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