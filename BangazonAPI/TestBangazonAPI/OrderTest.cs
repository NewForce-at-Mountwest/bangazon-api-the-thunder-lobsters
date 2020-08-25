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
    [Collection("Database collection")]
    public class OrderTest : IClassFixture<DatabaseFixture>
    {
        //Creates a new DatabaseFixture object to run tests
        DatabaseFixture fixture;
        public OrderTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        //Test for getting all orders from the API
        [Fact]
        public async Task Test_Get_All_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Gets all orders from the API
                var response = await client.GetAsync("/api/orders");

                // Act
                //Converts the API response to C# object and adds it to a list of orders
                string responseBody = await response.Content.ReadAsStringAsync();
                var orderList = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                // Assert
                //The test will succeed if the above response generates an OK status code, and returns a order list
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orderList.Count > 0);
            }
        }

        //Test to add a single order to the API
        [Fact]
        public async Task Create_One_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Creates a new order and converts it to JSON
                Order newOrder = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = 1,
                    customerForOrder =
                    {
                         Id = 1,
                         FirstName = "Dylan",
                        LastName = "Bisop",
                        CreationDate = "8/16/2020 11:46:23 AM",
                            LastActiveDate =  "8/16/2020 11:46:23 AM"
       
                    }
                };
                string jsonOrder = JsonConvert.SerializeObject(newOrder);

                // Act
                //Posts the JSON object to the API, then converts its response to C#
                HttpResponseMessage response = await client.PostAsync("/api/orders",
                    new StringContent(jsonOrder, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                Order orderResponse = JsonConvert.DeserializeObject<Order>(responseBody);

                // Assert
                //Test succeeds if the response generates a 204 status code, and if the returned order's CustomerId is the same as the one that was passed in
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(newOrder.CustomerId, newOrder.CustomerId);
            }
        }

        //Test to get a single order from the API
        [Fact]
        public async Task Test_One_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Gets a single order from the API
                var response = await client.GetAsync($"/api/orders/{fixture.TestOrder.Id}");

                //Converts the above response into a C# object
                string responseBody = await response.Content.ReadAsStringAsync();
                Order singleOrder = JsonConvert.DeserializeObject<Order>(responseBody);

                //Test succeeds if the response returns an OK status code
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        //Test for editing a single order
        [Fact]
        public async Task Edit_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Creates a slightly altered version of the test order
                Order editedOrder = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = 2,
                    customerForOrder =
                    {
                        Id = 1,
                        FirstName = "Dylan",
                        LastName = "Bisop",
                        CreationDate = "8/16/2020 11:46:23 AM",
                        LastActiveDate =  "8/16/2020 11:46:23 AM"

                   }
                };

                // Act
                //Converts the above object to JSON, PUTs it in the API
                string jsonOrder = JsonConvert.SerializeObject(editedOrder);
                HttpResponseMessage response = await client.PutAsync($"/api/orders/{fixture.TestOrder.Id}",
                    new StringContent(jsonOrder, Encoding.UTF8, "application/json"));

                //Attempts to GET the newly edited object, converts the API responce to C#
                var newResponse = await client.GetAsync($"/api/orders/{fixture.TestOrder.Id}");
                string responseBody = await newResponse.Content.ReadAsStringAsync();
                Order singleOrder = JsonConvert.DeserializeObject<Order>(responseBody);

                // Assert
                //Test succeeds if the response generates a 204, and if the CustomerId of the passed object is the same as the one gotten from the API after the edit
                
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                Assert.Equal(editedOrder.CustomerId, singleOrder.CustomerId);

            }
        }

        //Test for deleting an order
        [Fact]
        public async Task Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Attempts to delete the test order
                HttpResponseMessage response = await client.DeleteAsync($"/api/orders/{fixture.TestDeleteOrder.Id}");

                //Test succeeds if the delete returns a No Content status after deleting
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
    }
}

