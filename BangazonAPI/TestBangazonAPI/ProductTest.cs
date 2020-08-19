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
        DatabaseFixture fixture;
        public ProductTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                var response = await client.GetAsync("/api/products");
                // Act
                string responseBody = await response.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<List<Product>>(responseBody);
                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productList.Count > 0);
            }
        }
        [Fact]
        public async Task Create_One_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
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
                HttpResponseMessage response = await client.PostAsync("/api/products",
                    new StringContent(jsonProduct, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                Product productResponse = JsonConvert.DeserializeObject<Product>(responseBody);
                // Assert
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(productResponse.Title, newProduct.Title);
            }
        }
        [Fact]
        public async Task Test_One_Product()
        {
            using (var client = new APIClientProvider().Client)
            {

                var response = await client.GetAsync($"/api/products/{fixture.TestProduct.Id}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Product singleProduct = JsonConvert.DeserializeObject<Product>(responseBody);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
        [Fact]
        public async Task Edit_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
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
                string jsonProduct = JsonConvert.SerializeObject(editedProduct);
                HttpResponseMessage response = await client.PutAsync($"/api/products/{fixture.TestProduct.Id}",
                    new StringContent(jsonProduct, Encoding.UTF8, "application/json"));

                var newResponse = await client.GetAsync($"/api/products/{fixture.TestProduct.Id}");
                string responseBody = await newResponse.Content.ReadAsStringAsync();
                Product singleProduct = JsonConvert.DeserializeObject<Product>(responseBody);

                // Assert
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
                Assert.Equal(editedProduct.Description, singleProduct.Description);

            }
        }
        [Fact]
        public async Task Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                HttpResponseMessage response = await client.DeleteAsync($"/api/products/{fixture.TestDeleteProduct.Id}");
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
    }
}

