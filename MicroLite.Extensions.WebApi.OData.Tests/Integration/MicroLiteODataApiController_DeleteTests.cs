﻿using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using Net.Http.WebApi.OData;
using Xunit;

namespace MicroLite.Extensions.WebApi.OData.Tests.Integration
{
    public class MicroLiteODataApiController_DeleteTests
    {
        public class InvalidEntityKey : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public InvalidEntityKey()
            {
                MockSession
                    .Setup(x => x.Advanced.DeleteAsync(typeof(Customer), 122))
                    .Returns(Task.FromResult(false));

                _httpResponseMessage = HttpClient.DeleteAsync("http://server/odata/Customers(122)").Result;
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

        public class ValidEntityKey : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public ValidEntityKey()
            {
                MockSession
                    .Setup(x => x.Advanced.DeleteAsync(typeof(Customer), 122))
                    .Returns(Task.FromResult(true));

                _httpResponseMessage = HttpClient.DeleteAsync("http://server/odata/Customers(122)").Result;
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