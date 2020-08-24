using BangazonAPI.Models;
using TestBangazonAPI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestBangazonAPI;
using Xunit;


namespace TestBangazonAPI
{
    public class CustomerTest : IClassFixture<DatabaseFixture>
    {
        DatabaseFixture fixture;
        public CustomerTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                var response = await client.GetAsync("/api/customer");
                // Act
                string responseBody = await response.Content.ReadAsStringAsync();
                var customerList = JsonConvert.DeserializeObject<List<Customer>>(responseBody);
                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customerList.Count > 0);
            }
        }
        [Fact]
        public async Task Create_One_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                Customer newCustomer = new Customer()
                {
                    FirstName = "Test name",
                    LastName = "Test name",
                    CreationDate = "2020-08-16",
                    LastActiveDate = "2020-08-16"
                };
                string jsonCustomer = JsonConvert.SerializeObject(newCustomer);
                // Act
                HttpResponseMessage response = await client.PostAsync("/api/customer",
                    new StringContent(jsonCustomer, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                Customer customerResponse = JsonConvert.DeserializeObject<Customer>(responseBody);
                // Assert
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(customerResponse.FirstName, newCustomer.FirstName);
                Assert.Equal(customerResponse.LastName, newCustomer.LastName);
                Assert.Equal(customerResponse.CreationDate, newCustomer.CreationDate);
                Assert.Equal(customerResponse.LastActiveDate, newCustomer.LastActiveDate);
            }
        }
        [Fact]
        public async Task Test_One_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {

                var response = await client.GetAsync($"/api/customer/{fixture.TestCustomer.Id}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Customer singleCustomer = JsonConvert.DeserializeObject<Customer>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                
            }
        }
        [Fact]
        public async Task Edit_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                Customer editedCustomer = new Customer()
                {
                    FirstName = "Test name",
                    LastName = "Edit Test name",
                    CreationDate = "2020-08-16",
                    LastActiveDate = "2020-08-16"
                };
                // Act
                string jsonCustomer = JsonConvert.SerializeObject(editedCustomer);
                HttpResponseMessage response = await client.PutAsync($"/api/customer/{fixture.TestCustomer.Id}",
                    new StringContent(jsonCustomer, Encoding.UTF8, "application/json"));
                // Assert
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                //get again to see if edit worked
                var getEdit = await client.GetAsync($"/api/customer/{fixture.TestCustomer.Id}");
                getEdit.EnsureSuccessStatusCode();

                string getEditBody = await getEdit.Content.ReadAsStringAsync();
                Customer newEdit = JsonConvert.DeserializeObject<Customer>(getEditBody);

                //TODO:: UNDERSTAND THIS STATEMENT AND IF IT WORKS

                Assert.Equal(HttpStatusCode.OK, getEdit.StatusCode);
                //  Assert.Equal(newEdit, newEdit.Title);

            }
        }
        [Fact]
        public async Task Edit_Nonexistent_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                Customer editedCustomer = new Customer()
                {
                    FirstName = "Test name",
                    LastName = "Edit Test name",
                    CreationDate = "2020-08-16",
                    LastActiveDate = "2020-08-16"
                };
                // Act
                string jsonCustomer = JsonConvert.SerializeObject(editedCustomer);
                HttpResponseMessage response = await client.PutAsync($"/api/customer/-1",
                    new StringContent(jsonCustomer, Encoding.UTF8, "application/json"));
                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }
      
    }
}
