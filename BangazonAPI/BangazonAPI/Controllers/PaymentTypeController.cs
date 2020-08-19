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
    public class PaymentTypeController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PaymentTypeController(IConfiguration config)
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

        // GET: api/<PaymentTypeController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, AcctNumber, Name, CustomerId FROM PaymentType";
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<PaymentType> paymenttypes = new List<PaymentType>();

                    while (reader.Read())
                    {
                        PaymentType paymenttype = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AcctNumber = reader.GetString(reader.GetOrdinal("AcctNumber")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))

                        };

                        paymenttypes.Add(paymenttype);
                    }
                    reader.Close();

                    return Ok(paymenttypes);
                }
            }
        }

        // GET api/<PaymentTypeController>/5
        
        [HttpGet("{id}", Name = "GetPaymentType")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, AcctNumber, Name, CustomerId
                        FROM PaymentType
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    PaymentType paymenttype = null;

                    if (reader.Read())
                    {
                        paymenttype = new PaymentType
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            AcctNumber = reader.GetString(reader.GetOrdinal("AcctNumber")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                    }
                    reader.Close();

                    return Ok(paymenttype);
                }
            }
        }

        // POST api/<PaymentTypeController>
        [HttpPost]

        public async Task<IActionResult> Post([FromBody] PaymentType paymenttype)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO PaymentType (AcctNumber, Name, CustomerId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@AcctNumber, @Name, @CustomerId)";
                    cmd.Parameters.Add(new SqlParameter("@AcctNumber", paymenttype.AcctNumber));
                    cmd.Parameters.Add(new SqlParameter("@Name", paymenttype.Name));
                    cmd.Parameters.Add(new SqlParameter("@CustomerId", paymenttype.CustomerId));

                    int newId = (int)cmd.ExecuteScalar();
                    paymenttype.Id = newId;
                    return CreatedAtRoute("GetPaymentType", new { id = newId }, paymenttype);
                }
            }
        }
        // PUT api/<PaymentTypeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] PaymentType paymenttype)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE PaymentType
                                            SET AcctNumber = @AcctNumber,
                                                Name = @Name,
                                                CustomerId = @CustomerId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@AcctNumber", paymenttype.AcctNumber));
                        cmd.Parameters.Add(new SqlParameter("@Name", paymenttype.Name));
                        cmd.Parameters.Add(new SqlParameter("@CustomerId", paymenttype.CustomerId));

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
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/<PaymentTypeController>/5
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
                        cmd.CommandText = @"DELETE FROM PaymentType WHERE Id = @id";
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
                if (!PaymentTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        private bool PaymentTypeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT
                        Id, AcctNumber, Name, CustomerId
                        FROM PaymentType
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}
