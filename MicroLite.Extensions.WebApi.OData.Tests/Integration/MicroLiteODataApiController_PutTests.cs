using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using Moq;
using Net.Http.OData;
using Xunit;

namespace MicroLite.Extensions.WebApi.OData.Tests.Integration
{
    public class MicroLiteODataApiController_PutTests
    {
        public class InvalidEntityKey : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public InvalidEntityKey()
            {
                MockSession
                    .Setup(x => x.SingleAsync<dynamic>(It.Is<SqlQuery>(s => s.CommandText == "SELECT Created,DateOfBirth,Forename,Id,Name,Reference,CustomerStatusId,Surname FROM Customers WHERE (Id = ?)")))
                    .Returns(Task.FromResult(default(object)));

                var content = new StringContent(
                    "{\"Created\":\"2012-06-22T00:00:00\",\"DateOfBirth\":\"1978-11-18T00:00:00\",\"Forename\":\"John\",\"Name\":\"John Smith\",\"Reference\":\"A/000122\",\"Status\":1,\"Surname\":\"Smith\"}",
                    Encoding.UTF8,
                    "application/json");

                _httpResponseMessage = HttpClient.PutAsync("http://server/odata/Customers(122)", content).Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
            {
                Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataHeaderNames.ODataVersion).Single());
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void DoesNotContain_Content()
            {
                Assert.Null(_httpResponseMessage.Content);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_NotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, _httpResponseMessage.StatusCode);
            }
        }

        public class ValidEntityKey_NotUpdated : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public ValidEntityKey_NotUpdated()
            {
                var entity = new Customer
                {
                    Created = new DateTime(2012, 6, 22),
                    DateOfBirth = new DateTime(1978, 11, 18),
                    Forename = "John",
                    Id = 122,
                    Name = "John Smith",
                    Reference = "A/000122",
                    Status = CustomerStatus.Active,
                    Surname = "Smith",
                };

                MockSession.Setup(x => x.SingleAsync<Customer>(122)).Returns(Task.FromResult(entity));
                MockSession.Setup(x => x.UpdateAsync(It.IsAny<object>())).Returns(Task.FromResult(false));

                var content = new StringContent(
                    "{\"Created\":\"2012-06-22T00:00:00\",\"DateOfBirth\":\"1978-11-18T00:00:00\",\"Forename\":\"John\",\"Name\":\"John Smith\",\"Reference\":\"A/000122\",\"Status\":1,\"Surname\":\"Smith\"}",
                    Encoding.UTF8,
                    "application/json");

                _httpResponseMessage = HttpClient.PutAsync("http://server/odata/Customers(122)", content).Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
            {
                Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataHeaderNames.ODataVersion).Single());
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void DoesNotContain_Content()
            {
                Assert.Null(_httpResponseMessage.Content);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_NotModified()
            {
                Assert.Equal(HttpStatusCode.NotModified, _httpResponseMessage.StatusCode);
            }
        }

        public class ValidEntityKey_Updated : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public ValidEntityKey_Updated()
            {
                var entity = new Customer
                {
                    Created = new DateTime(2012, 6, 22),
                    DateOfBirth = new DateTime(1978, 11, 18),
                    Forename = "John",
                    Id = 122,
                    Name = "John Smith",
                    Reference = "A/000122",
                    Status = CustomerStatus.Pending,
                    Surname = "Smith",
                };

                MockSession.Setup(x => x.SingleAsync<Customer>(122)).Returns(Task.FromResult(entity));
                MockSession.Setup(x => x.UpdateAsync(It.IsAny<object>())).Returns(Task.FromResult(true));

                var content = new StringContent(
                    "{\"Created\":\"2012-06-22T00:00:00\",\"DateOfBirth\":\"1978-11-18T00:00:00\",\"Forename\":\"John\",\"Name\":\"John Smith\",\"Reference\":\"A/000122\",\"Status\":1,\"Surname\":\"Smith\"}",
                    Encoding.UTF8,
                    "application/json");

                _httpResponseMessage = HttpClient.PutAsync("http://server/odata/Customers(122)", content).Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
            {
                Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataHeaderNames.ODataVersion).Single());
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void DoesNotContain_Content()
            {
                Assert.Null(_httpResponseMessage.Content);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_NoContent()
            {
                Assert.Equal(HttpStatusCode.NoContent, _httpResponseMessage.StatusCode);
            }
        }
    }
}
