using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Net.Http.OData;
using Net.Http.WebApi.OData;
using Xunit;

namespace MicroLite.Extensions.WebApi.OData.Tests.Integration
{
    public class MicroLiteODataApiController_GetCountTests : IntegrationTest
    {
        private readonly HttpResponseMessage _httpResponseMessage;

        public MicroLiteODataApiController_GetCountTests()
        {
            MockSession.Setup(x => x.Advanced.ExecuteScalarAsync<long>(It.Is<SqlQuery>(s => s.CommandText == "SELECT COUNT(Id) AS Id FROM Customers"))).Returns(Task.FromResult(76235L));

            _httpResponseMessage = HttpClient.GetAsync("http://server/odata/Customers/$count").Result;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task Contains_Content_CountValue()
        {
            Assert.NotNull(_httpResponseMessage.Content);

            string result = await _httpResponseMessage.Content.ReadAsStringAsync();

            Assert.Equal("76235", result);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Contains_Header_ContentType_Parameter_ODataMetadata()
        {
            Assert.Equal("minimal", _httpResponseMessage.Content.Headers.ContentType.Parameters.Single(x => x.Name == ODataMetadataLevelExtensions.HeaderName).Value);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Contains_Header_ContentType_TextPlain()
        {
            Assert.Equal("text/plain", _httpResponseMessage.Content.Headers.ContentType.MediaType);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void Contains_Header_ODataVersion()
        {
            Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataResponseHeaderNames.ODataVersion).Single());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public void StatusCode_OK()
        {
            Assert.Equal(HttpStatusCode.OK, _httpResponseMessage.StatusCode);
        }
    }
}
