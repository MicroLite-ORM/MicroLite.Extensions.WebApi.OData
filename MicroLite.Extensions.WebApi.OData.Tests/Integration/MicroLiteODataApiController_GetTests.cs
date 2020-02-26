using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using Moq;
using Net.Http.OData;
using Xunit;

namespace MicroLite.Extensions.WebApi.OData.Tests.Integration
{
    public class MicroLiteODataApiController_GetTests
    {
        public class InvalidEntityKey_ValidProperty : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public InvalidEntityKey_ValidProperty()
            {
                MockSession
                    .Setup(x => x.SingleAsync<dynamic>(It.Is<SqlQuery>(s => s.CommandText == "SELECT Created,DateOfBirth,Forename,Id,Name,Reference,CustomerStatusId,Surname FROM Customers WHERE (Id = ?)")))
                    .Returns(Task.FromResult(default(object)));

                _httpResponseMessage = HttpClient.GetAsync("http://server/odata/Customers(122)").Result;
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

        public class QueryOptions_Metadata_Minimal_NoResults : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public QueryOptions_Metadata_Minimal_NoResults()
            {
                var entities = new dynamic[0];

                MockSession
                    .Setup(x => x.PagedAsync<dynamic>(It.Is<SqlQuery>(s => s.CommandText == "SELECT Forename,Surname FROM Customers WHERE (Surname = ?) ORDER BY Id ASC"), It.IsAny<PagingOptions>()))
                    .Returns((SqlQuery s, PagingOptions p) => Task.FromResult(new PagedResult<dynamic>(1, entities, p.Count, entities.Length)));

                _httpResponseMessage = HttpClient.GetAsync("http://server/odata/Customers?$select=Forename,Surname&$filter=Surname eq 'Stark'").Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public async Task Contains_Content_PropertyValue()
            {
                Assert.NotNull(_httpResponseMessage.Content);

                string result = await _httpResponseMessage.Content.ReadAsStringAsync();

                Assert.Equal(
                    "{\"@odata.context\":\"http://server/odata/$metadata#Customers(Forename,Surname)\",\"value\":[]}",
                    result);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_ApplicationJson()
            {
                Assert.Equal("application/json", _httpResponseMessage.Content.Headers.ContentType.MediaType);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_Parameter_ODataMetadata()
            {
                Assert.Equal("minimal", _httpResponseMessage.Content.Headers.ContentType.Parameters.Single(x => x.Name == "odata.metadata").Value);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
            {
                Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataHeaderNames.ODataVersion).Single());
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_OK()
            {
                Assert.Equal(HttpStatusCode.OK, _httpResponseMessage.StatusCode);
            }
        }

        public class QueryOptions_Metadata_Minimal_Results : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public QueryOptions_Metadata_Minimal_Results()
            {
                dynamic entity = new ExpandoObject();
                entity.Forename = "Tony";
                entity.Surname = "Stark";

                var entities = new dynamic[]
                {
                    entity
                };

                MockSession
                    .Setup(x => x.PagedAsync<dynamic>(It.Is<SqlQuery>(s => s.CommandText == "SELECT Forename,Surname FROM Customers WHERE (Surname = ?) ORDER BY Id ASC"), It.IsAny<PagingOptions>()))
                    .Returns((SqlQuery s, PagingOptions p) => Task.FromResult(new PagedResult<dynamic>(1, entities, p.Count, entities.Length)));

                _httpResponseMessage = HttpClient.GetAsync("http://server/odata/Customers?$select=Forename,Surname&$filter=Surname eq 'Stark'").Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public async Task Contains_Content_PropertyValue()
            {
                Assert.NotNull(_httpResponseMessage.Content);

                string result = await _httpResponseMessage.Content.ReadAsStringAsync();

                Assert.Equal(
                    "{\"@odata.context\":\"http://server/odata/$metadata#Customers(Forename,Surname)\",\"value\":[{\"Forename\":\"Tony\",\"Surname\":\"Stark\"}]}",
                    result);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_ApplicationJson()
            {
                Assert.Equal("application/json", _httpResponseMessage.Content.Headers.ContentType.MediaType);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_Parameter_ODataMetadata()
            {
                Assert.Equal("minimal", _httpResponseMessage.Content.Headers.ContentType.Parameters.Single(x => x.Name == "odata.metadata").Value);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
            {
                Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataHeaderNames.ODataVersion).Single());
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_OK()
            {
                Assert.Equal(HttpStatusCode.OK, _httpResponseMessage.StatusCode);
            }
        }

        public class QueryOptions_Metadata_None_NoResults : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public QueryOptions_Metadata_None_NoResults()
            {
                var entities = new dynamic[0];

                MockSession
                    .Setup(x => x.PagedAsync<dynamic>(It.Is<SqlQuery>(s => s.CommandText == "SELECT Forename,Surname FROM Customers WHERE (Surname = ?) ORDER BY Id ASC"), It.IsAny<PagingOptions>()))
                    .Returns((SqlQuery s, PagingOptions p) => Task.FromResult(new PagedResult<dynamic>(1, entities, p.Count, entities.Length)));

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://server/odata/Customers?$select=Forename,Surname&$filter=Surname eq 'Stark'");
                httpRequestMessage.Headers.Add("Accept", "application/json;odata.metadata=none");

                _httpResponseMessage = HttpClient.SendAsync(httpRequestMessage).Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public async Task Contains_Content_PropertyValue()
            {
                Assert.NotNull(_httpResponseMessage.Content);

                string result = await _httpResponseMessage.Content.ReadAsStringAsync();

                Assert.Equal(
                    "{\"value\":[]}",
                    result);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_ApplicationJson()
            {
                Assert.Equal("application/json", _httpResponseMessage.Content.Headers.ContentType.MediaType);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_Parameter_ODataMetadata()
            {
                Assert.Equal("none", _httpResponseMessage.Content.Headers.ContentType.Parameters.Single(x => x.Name == "odata.metadata").Value);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
            {
                Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataHeaderNames.ODataVersion).Single());
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_OK()
            {
                Assert.Equal(HttpStatusCode.OK, _httpResponseMessage.StatusCode);
            }
        }

        public class QueryOptions_Metadata_None_Results : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public QueryOptions_Metadata_None_Results()
            {
                dynamic entity = new ExpandoObject();
                entity.Forename = "Tony";
                entity.Surname = "Stark";

                var entities = new dynamic[]
                {
                    entity
                };

                MockSession
                    .Setup(x => x.PagedAsync<dynamic>(It.Is<SqlQuery>(s => s.CommandText == "SELECT Forename,Surname FROM Customers WHERE (Surname = ?) ORDER BY Id ASC"), It.IsAny<PagingOptions>()))
                    .Returns((SqlQuery s, PagingOptions p) => Task.FromResult(new PagedResult<dynamic>(1, entities, p.Count, entities.Length)));

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://server/odata/Customers?$select=Forename,Surname&$filter=Surname eq 'Stark'");
                httpRequestMessage.Headers.Add("Accept", "application/json;odata.metadata=none");

                _httpResponseMessage = HttpClient.SendAsync(httpRequestMessage).Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public async Task Contains_Content_PropertyValue()
            {
                Assert.NotNull(_httpResponseMessage.Content);

                string result = await _httpResponseMessage.Content.ReadAsStringAsync();

                Assert.Equal(
                    "{\"value\":[{\"Forename\":\"Tony\",\"Surname\":\"Stark\"}]}",
                    result);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_ApplicationJson()
            {
                Assert.Equal("application/json", _httpResponseMessage.Content.Headers.ContentType.MediaType);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_Parameter_ODataMetadata()
            {
                Assert.Equal("none", _httpResponseMessage.Content.Headers.ContentType.Parameters.Single(x => x.Name == "odata.metadata").Value);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
            {
                Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataHeaderNames.ODataVersion).Single());
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_OK()
            {
                Assert.Equal(HttpStatusCode.OK, _httpResponseMessage.StatusCode);
            }
        }

        public class ValidEntityKey_Metadata_Minimal : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public ValidEntityKey_Metadata_Minimal()
            {
                dynamic entity = new ExpandoObject();
                entity.Created = new DateTime(2012, 6, 22);
                entity.DateOfBirth = new DateTime(1978, 11, 18);
                entity.Forename = "John";
                entity.Id = 122;
                entity.Name = "John Smith";
                entity.Reference = "A/000122";
                entity.Status = CustomerStatus.Active;
                entity.Surname = "Smith";

                MockSession
                    .Setup(x => x.SingleAsync<dynamic>(It.Is<SqlQuery>(s => s.CommandText == "SELECT Created,DateOfBirth,Forename,Id,Name,Reference,CustomerStatusId,Surname FROM Customers WHERE (Id = ?)")))
                    .Returns(Task.FromResult((object)entity));

                _httpResponseMessage = HttpClient.GetAsync("http://server/odata/Customers(122)").Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public async Task Contains_Content_PropertyValue()
            {
                Assert.NotNull(_httpResponseMessage.Content);

                string result = await _httpResponseMessage.Content.ReadAsStringAsync();

                Assert.Equal(
                    "{\"Created\":\"2012-06-22T00:00:00\",\"DateOfBirth\":\"1978-11-18T00:00:00\",\"Forename\":\"John\",\"Id\":122,\"Name\":\"John Smith\",\"Reference\":\"A/000122\",\"Status\":1,\"Surname\":\"Smith\",\"@odata.context\":\"http://server/odata/$metadata#Customers/$entity\"}",
                    result);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_ApplicationJson()
            {
                Assert.Equal("application/json", _httpResponseMessage.Content.Headers.ContentType.MediaType);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_Parameter_ODataMetadata()
            {
                Assert.Equal("minimal", _httpResponseMessage.Content.Headers.ContentType.Parameters.Single(x => x.Name == "odata.metadata").Value);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
            {
                Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataHeaderNames.ODataVersion).Single());
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_OK()
            {
                Assert.Equal(HttpStatusCode.OK, _httpResponseMessage.StatusCode);
            }
        }

        public class ValidEntityKey_Metadata_None : IntegrationTest
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public ValidEntityKey_Metadata_None()
            {
                dynamic entity = new ExpandoObject();
                entity.Created = new DateTime(2012, 6, 22);
                entity.DateOfBirth = new DateTime(1978, 11, 18);
                entity.Forename = "John";
                entity.Id = 122;
                entity.Name = "John Smith";
                entity.Reference = "A/000122";
                entity.Status = CustomerStatus.Active;
                entity.Surname = "Smith";

                MockSession
                    .Setup(x => x.SingleAsync<dynamic>(It.Is<SqlQuery>(s => s.CommandText == "SELECT Created,DateOfBirth,Forename,Id,Name,Reference,CustomerStatusId,Surname FROM Customers WHERE (Id = ?)")))
                    .Returns(Task.FromResult((object)entity));

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://server/odata/Customers(122)");
                httpRequestMessage.Headers.Add("Accept", "application/json;odata.metadata=none");

                _httpResponseMessage = HttpClient.SendAsync(httpRequestMessage).Result;
            }

            [Fact]
            [Trait("Category", "Integration")]
            public async Task Contains_Content_PropertyValue()
            {
                Assert.NotNull(_httpResponseMessage.Content);

                string result = await _httpResponseMessage.Content.ReadAsStringAsync();

                Assert.Equal(
                    "{\"Created\":\"2012-06-22T00:00:00\",\"DateOfBirth\":\"1978-11-18T00:00:00\",\"Forename\":\"John\",\"Id\":122,\"Name\":\"John Smith\",\"Reference\":\"A/000122\",\"Status\":1,\"Surname\":\"Smith\"}",
                    result);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_ApplicationJson()
            {
                Assert.Equal("application/json", _httpResponseMessage.Content.Headers.ContentType.MediaType);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ContentType_Parameter_ODataMetadata()
            {
                Assert.Equal("none", _httpResponseMessage.Content.Headers.ContentType.Parameters.Single(x => x.Name == "odata.metadata").Value);
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void Contains_Header_ODataVersion()
            {
                Assert.Equal("4.0", _httpResponseMessage.Headers.GetValues(ODataHeaderNames.ODataVersion).Single());
            }

            [Fact]
            [Trait("Category", "Integration")]
            public void StatusCode_OK()
            {
                Assert.Equal(HttpStatusCode.OK, _httpResponseMessage.StatusCode);
            }
        }
    }
}
