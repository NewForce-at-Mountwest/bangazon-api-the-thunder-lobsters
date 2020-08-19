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
    public class ProductTest : IClassFixture<DatabaseFixture>
    {
        //Creates a new DatabaseFixture object to run tests
        DatabaseFixture fixture;
        public ProductTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }

        //Test for getting all products from the API
        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Gets all products from the API
                var response = await client.GetAsync("/api/products");

                // Act
                //Converts the API response to C# object and adds it to a list of products
                string responseBody = await response.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                // Assert
                //The test will succeed if the above response generates an OK status code, and returns a product list
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productList.Count > 0);
            }
        }

        //Test to add a single product to the API
        [Fact]
        public async Task Create_One_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Creates a new product and converts it to JSON
                Product newProduct = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    Price = Convert.ToDecimal(1.80),
                    Title = "Integration Test Product",
                    Description = "Integration Test Product",
                    Quantity = 1
                };
                string jsonProduct = JsonConvert.SerializeObject(newProduct);

                // Act
                //Posts the JSON object to the API, then converts its response to C#
                HttpResponseMessage response = await client.PostAsync("/api/products",
                    new StringContent(jsonProduct, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                Product productResponse = JsonConvert.DeserializeObject<Product>(responseBody);

                // Assert
                //Test succeeds if the response generates a 204 status code, and if the returned product's title is the same as the one that was passed in
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(productResponse.Title, newProduct.Title);
            }
        }

        //Test to get a single product from the API
        [Fact]
        public async Task Test_One_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Gets a single product from the API
                var response = await client.GetAsync($"/api/products/{fixture.TestProduct.Id}");

                //Converts the above response into a C# object
                string responseBody = await response.Content.ReadAsStringAsync();
                Product singleProduct = JsonConvert.DeserializeObject<Product>(responseBody);

                //Test succeeds if the response returns an OK status code
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        //Test for editing a single product
        [Fact]
        public async Task Edit_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Creates a slightly altered version of the test product
                Product editedProduct = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    Price = Convert.ToDecimal(1.80),
                    Title = "Integration Test Product",
                    Description = "Integration TEST Product",
                    Quantity = 1
                };

                // Act
                //Converts the above object to JSON, PUTs it in the API
                string jsonProduct = JsonConvert.SerializeObject(editedProduct);
                HttpResponseMessage response = await client.PutAsync($"/api/products/{fixture.TestProduct.Id}",
                    new StringContent(jsonProduct, Encoding.UTF8, "application/json"));

                //Attempts to GET the newly edited object, converts the API responce to C#
                var newResponse = await client.GetAsync($"/api/products/{fixture.TestProduct.Id}");
                string responseBody = await newResponse.Content.ReadAsStringAsync();
                Product singleProduct = JsonConvert.DeserializeObject<Product>(responseBody);

                // Assert
                //Test succeeds if the response generates a 204, and if the description of the passed object is the same as the one gotten from the API after the edit
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                Assert.Equal(editedProduct.Description, singleProduct.Description);

            }
        }

        //Test for deleting a product
        [Fact]
        public async Task Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Attempts to delete the test product
                HttpResponseMessage response = await client.DeleteAsync($"/api/products/{fixture.TestDeleteProduct.Id}");

                //Test succeeds if the delete returns a No Content status after deleting
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
    }
}

