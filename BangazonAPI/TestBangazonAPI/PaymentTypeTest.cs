using BangazonAPI.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace TestBangazonAPI
{
    public class PaymentTypeTest : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture fixture;
        public PaymentTypeTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        public async Task Test_Get_All_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                var response = await client.GetAsync("/api/PaymentType");
                // Act
                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttypeList = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);
                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymenttypeList.Count > 0);
            }
        }
        [Fact]
        public async Task Create_One_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                PaymentType newPaymentType = new PaymentType()
                {
                    AcctNumber = "test",
                    Name = "hkkdkldnhjsk",
                    CustomerId = 2
                };
                string jsonPaymentType = JsonConvert.SerializeObject(newPaymentType);
                // Act
                HttpResponseMessage response = await client.PostAsync("/api/PaymentType",
                    new StringContent(jsonPaymentType, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                PaymentType paymenttypeResponse = JsonConvert.DeserializeObject<PaymentType>(responseBody);
                // Assert
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(paymenttypeResponse.AcctNumber, newPaymentType.AcctNumber);
                Assert.Equal(paymenttypeResponse.Name, newPaymentType.Name);
                Assert.Equal(paymenttypeResponse.CustomerId, newPaymentType.CustomerId);
            }
        }
        [Fact]
        public async Task Test_One_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {

                var response = await client.GetAsync($"/api/PaymentType/{fixture.TestPaymentType.Id}");
                string responseBody = await response.Content.ReadAsStringAsync();
                PaymentType singlePaymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //
            }
        }
        [Fact]
        public async Task Edit_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                PaymentType editedPaymentType = new PaymentType()
                {
                    AcctNumber = "test",
                    Name = "hkkdkldnhjsk",
                    CustomerId = 2
                };
                // Act
                string jsonPaymentType = JsonConvert.SerializeObject(editedPaymentType);
                HttpResponseMessage response = await client.PutAsync($"/api/PaymentType/{fixture.TestPaymentType.Id}",
                    new StringContent(jsonPaymentType, Encoding.UTF8, "application/json"));
                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }
        [Fact]
        public async Task Delete_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                HttpResponseMessage response = await client.DeleteAsync($"/api/PaymentType/{fixture.TestPaymentType.Id}");
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }

        [Fact]
        public async Task Delete_PaymentType_NonExistent()
        {
            using (var client = new APIClientProvider().Client)
            {
                HttpResponseMessage response = await client.DeleteAsync($"/api/PaymentType/{fixture.TestPaymentType.Id}");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);


            }
        }
    }
}