using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MicroLite.Extensions.WebApi.OData.Tests.TestEntities;
using Moq;
using Net.Http.OData;
using Xunit;

namespace MicroLite.Extensions.WebApi.OData.Tests.Integration
{
    public class MicroLiteODataApiController_PostTests
    {
        public class Metadata_Minimal : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public Metadata_Minimal()
            {
                MockSession
                    .Setup(x => x.InsertAsync(It.IsAny<Customer>()))
                    .Callback((object o) => ((Customer)o).Id = 123)
                    .Returns(Task.CompletedTask);

                var content = new StringContent(
                    "{\"created\":\"2012-06-22T00:00:00\",\"dateOfBirth\":\"1978-11-18T00:00:00\",\"forename\":\"John\",\"name\":\"John Smith\",\"reference\":\"A/000122\",\"status\":1,\"surname\":\"Smith\"}",
                    Encoding.UTF8,
                    "application/json");

                _httpResponseMessage = HttpClient.PostAsync("http://server/odata/Customers", content).Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public async Task Contains_Content_PropertyValue()
            {
                Assert.NotNull(_httpResponseMessage.Content);

                string result = await _httpResponseMessage.Content.ReadAsStringAsync();

                // TODO: should the response contain metadata if it's not set to none in the request?
                Assert.Equal(
                    //"{\"created\":\"2012-06-22T00:00:00\",\"dateOfBirth\":\"1978-11-18T00:00:00\",\"forename\":\"John\",\"id\":123,\"name\":\"John Smith\",\"reference\":\"A/000122\",\"status\":1,\"surname\":\"Smith\",\"@odata.context\":\"http://server/odata/$metadata#Customers/$entity\"}",
                    "{\"created\":\"2012-06-22T00:00:00\",\"dateOfBirth\":\"1978-11-18T00:00:00\",\"forename\":\"John\",\"id\":123,\"name\":\"John Smith\",\"reference\":\"A/000122\",\"status\":1,\"surname\":\"Smith\"}",
                    result);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_ApplicationJson()
                => Assert.Equal("application/json", _httpResponseMessage.Content.Headers.ContentType.MediaType);

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_Parameter_ODataMetadata()
                => Assert.Equal("minimal", _httpResponseMessage.Content.Headers.ContentType.Parameters.Single(x => x.Name == "odata.metadata").Value);

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_Location()
                => Assert.Equal("http://server/odata/Customers(123)", _httpResponseMessage.Headers.Location.ToString());

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
                => Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataResponseHeaderNames.ODataVersion).Single());

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_Created()
                => Assert.Equal(HttpStatusCode.Created, _httpResponseMessage.StatusCode);
        }

        public class Metadata_None : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public Metadata_None()
            {
                MockSession
                    .Setup(x => x.InsertAsync(It.IsAny<Customer>()))
                    .Callback((object o) => ((Customer)o).Id = 123)
                    .Returns(Task.CompletedTask);

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://server/odata/Customers");
                httpRequestMessage.Headers.Add("Accept", "application/json;odata.metadata=none");
                httpRequestMessage.Content = new StringContent(
                    "{\"created\":\"2012-06-22T00:00:00\",\"dateOfBirth\":\"1978-11-18T00:00:00\",\"forename\":\"John\",\"name\":\"John Smith\",\"reference\":\"A/000122\",\"status\":1,\"surname\":\"Smith\"}",
                    Encoding.UTF8,
                    "application/json");

                _httpResponseMessage = HttpClient.SendAsync(httpRequestMessage).Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public async Task Contains_Content_PropertyValue()
            {
                Assert.NotNull(_httpResponseMessage.Content);

                string result = await _httpResponseMessage.Content.ReadAsStringAsync();

                Assert.Equal(
                    "{\"created\":\"2012-06-22T00:00:00\",\"dateOfBirth\":\"1978-11-18T00:00:00\",\"forename\":\"John\",\"id\":123,\"name\":\"John Smith\",\"reference\":\"A/000122\",\"status\":1,\"surname\":\"Smith\"}",
                    result);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_ApplicationJson()
                => Assert.Equal("application/json", _httpResponseMessage.Content.Headers.ContentType.MediaType);

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_Parameter_ODataMetadata()
                => Assert.Equal("none", _httpResponseMessage.Content.Headers.ContentType.Parameters.Single(x => x.Name == "odata.metadata").Value);

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
                => Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataResponseHeaderNames.ODataVersion).Single());

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_Created()
                => Assert.Equal(HttpStatusCode.Created, _httpResponseMessage.StatusCode);
        }
    }
}
