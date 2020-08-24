using BangazonAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace TestBangazonAPI
{
    [Collection("Database collection")]
    public class ProductTypeTest : IClassFixture<DatabaseFixture>
    {
        //Creates a new DatabaseFixture object to run tests
        DatabaseFixture fixture;
        public ProductTypeTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        //Test for getting all product types from the API
        public async Task Test_Get_All_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Gets all product types from the API
                var response = await client.GetAsync("/api/ProductType");
                // Act
                //Converts the API response to C# object and adds it to a list of product types
                string responseBody = await response.Content.ReadAsStringAsync();
                var producttypeList = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);
                // Assert
                //The test will succeed if the above response generates an OK status code, and returns a product type
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(producttypeList.Count > 0);
            }
        }
        [Fact]
        //Test for getting one product type from the API
        public async Task Create_One_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Creates a new product type and converts it to JSON
                ProductType newProductType = new ProductType()
                {

                    Name = "Test Product Type",

                };
                string jsonProductType = JsonConvert.SerializeObject(newProductType);
                // Act
                //Posts the JSON object to the API, then converts its response to C#
                HttpResponseMessage response = await client.PostAsync("/api/ProductType",
                    new StringContent(jsonProductType, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                ProductType producttypeResponse = JsonConvert.DeserializeObject<ProductType>(responseBody);
                // Assert
                //Test succeeds if the response generates a 204 status code, and if the returned product type's Name is the same as the one that was passed in
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(producttypeResponse.Name, newProductType.Name);
            }
        }
        [Fact]
        //Test to get a single product type from the API
        public async Task Test_One_ProductType()
        {

            using (var client = new APIClientProvider().Client)
            {
                //Gets a single product type from the API
                var response = await client.GetAsync($"/api/ProductType/{fixture.TestProductType.Id}");
                //Converts the above response into a C# object
                string responseBody = await response.Content.ReadAsStringAsync();
                ProductType singleProductType = JsonConvert.DeserializeObject<ProductType>(responseBody);
                //Test succeeds if the response returns an OK status code
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //
            }
        }
        [Fact]
        //Test for editing a single product type
        public async Task Edit_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Creates an edited version of the test product type
                ProductType editedProductType = new ProductType()
                {

                    Name = "test",

                };
                // Act
                //Converts the above object to JSON, PUTs it in the API
                string jsonProductType = JsonConvert.SerializeObject(editedProductType);
                HttpResponseMessage response = await client.PutAsync($"/api/ProductType/{fixture.TestProductType.Id}",
                    new StringContent(jsonProductType, Encoding.UTF8, "application/json"));
                // Assert
                //Test succeeds if the response generates a 204, and if the description of the passed object is the same as the one gotten from the API after the edit
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            }
        }
        [Fact]
        //Test for deleting a product type
        public async Task Delete_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Attempts to delete the test product type
                HttpResponseMessage response = await client.DeleteAsync($"/api/ProductType/{fixture.TestProductType.Id}");
                //Test succeeds if the delete returns a No Content status after deleting
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }


    }
}