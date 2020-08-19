using BangazonAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBangazonAPI
{
    public class DatabaseFixture : IDisposable
    {
        private readonly string ConnectionString = @$"Server=localhost\SQLEXPRESS03;Database=BangazonAPI;Trusted_Connection=True;";
        public PaymentType TestPaymentType { get; set; }
        public DatabaseFixture()
        {
            PaymentType newpaymenttype = new PaymentType
            {
                
                AcctNumber = "test",
                Name = "hkkdkldnhjsk",
                CustomerId = 2
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
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

