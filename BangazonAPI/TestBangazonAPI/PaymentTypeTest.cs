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
    public class PaymentTypeTest : IClassFixture<DatabaseFixture>
    {
        //Creates a new DatabaseFixture object to run tests
        DatabaseFixture fixture;
        public PaymentTypeTest(DatabaseFixture fixture)
        {
            this.fixture = fixture;
        }
        [Fact]
        //Test for getting all payment types from the API
        public async Task Test_Get_All_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Gets all payment types from the API
                var response = await client.GetAsync("/api/PaymentType");
                // Act
                //Converts the API response to C# object and adds it to a list of payment types
                string responseBody = await response.Content.ReadAsStringAsync();
                var paymenttypeList = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);
                // Assert
                //The test will succeed if the above response generates an OK status code, and returns a payment type
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymenttypeList.Count > 0);
            }
        }
        [Fact]
        //Test for getting one payment type from the API
        public async Task Create_One_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Creates a new payment type and converts it to JSON
                PaymentType newPaymentType = new PaymentType()
                {
                    AcctNumber = "test",
                    Name = "hkkdkldnhjsk",
                    CustomerId = 2
                };
                string jsonPaymentType = JsonConvert.SerializeObject(newPaymentType);
                // Act
                //Posts the JSON object to the API, then converts its response to C#
                HttpResponseMessage response = await client.PostAsync("/api/PaymentType",
                    new StringContent(jsonPaymentType, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                PaymentType paymenttypeResponse = JsonConvert.DeserializeObject<PaymentType>(responseBody);
                // Assert
                //Test succeeds if the response generates a 204 status code, and if the returned payment type's title is the same as the one that was passed in
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(paymenttypeResponse.AcctNumber, newPaymentType.AcctNumber);
                Assert.Equal(paymenttypeResponse.Name, newPaymentType.Name);
                Assert.Equal(paymenttypeResponse.CustomerId, newPaymentType.CustomerId);
            }
        }
        [Fact]
        //Test to get a single payment type from the API
        public async Task Test_One_PaymentType()
        {

            using (var client = new APIClientProvider().Client)
            {
                //Gets a single payment type from the API
                var response = await client.GetAsync($"/api/PaymentType/{fixture.TestPaymentType.Id}");
                //Converts the above response into a C# object
                string responseBody = await response.Content.ReadAsStringAsync();
                PaymentType singlePaymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);
                //Test succeeds if the response returns an OK status code
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //
            }
        }
        [Fact]
        //Test for editing a single payment type
        public async Task Edit_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                // Arrange
                //Creates an edited version of the test payment type
                PaymentType editedPaymentType = new PaymentType()
                {
                    AcctNumber = "test",
                    Name = "test",
                    CustomerId = 2
                };
                // Act
                //Converts the above object to JSON, PUTs it in the API
                string jsonPaymentType = JsonConvert.SerializeObject(editedPaymentType);
                HttpResponseMessage response = await client.PutAsync($"/api/PaymentType/{fixture.TestPaymentType.Id}",
                    new StringContent(jsonPaymentType, Encoding.UTF8, "application/json"));
                // Assert
                //Test succeeds if the response generates a 204, and if the description of the passed object is the same as the one gotten from the API after the edit
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            }
        }
        [Fact]
        //Test for deleting a payment type
        public async Task Delete_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                //Attempts to delete the test payment type
                HttpResponseMessage response = await client.DeleteAsync($"/api/PaymentType/{fixture.TestPaymentType.Id}");
                //Test succeeds if the delete returns a No Content status after deleting
                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }

     
    }
}