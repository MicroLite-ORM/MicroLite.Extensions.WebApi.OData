using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using MicroLite.Extensions.WebApi.OData;
using MicroLite.Extensions.WebApi.OData.Tests.Integration;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using Moq;
using Net.Http.WebApi.OData;
using Net.Http.WebApi.OData.Model;
using Net.Http.WebApi.OData.Query;
using Net.Http.WebApi.OData.Query.Validators;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests.OData
{
    public class MicroLiteODataApiControllerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenCallingGetEntityResponseTheODataQueryOptionsAreValidated()
        {
            TestHelper.EnsureEDM();

            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=-1"),
                EntityDataModel.Current.EntitySets["Customers"]);

            var controller = new CustomerController(Mock.Of<IAsyncSession>());

            ODataException exception = await Assert.ThrowsAsync<ODataException>(() => controller.Get(queryOptions));
            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        }

        public class TheDefaultValidatonSettings
        {
            private readonly CustomerController _controller = new CustomerController(Mock.Of<IAsyncSession>());

            [Fact]
            [Trait("Category", "Unit")]
            public void AllArithmeticOperatorsAreAllowed()
            {
                Assert.Equal(AllowedArithmeticOperators.All, _controller.ValidationSettings.AllowedArithmeticOperators & AllowedArithmeticOperators.All);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void AllLogicalOperatorsAreAllowed_ExceptHas()
            {
                Assert.Equal(AllowedLogicalOperators.And, _controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.And);
                Assert.Equal(AllowedLogicalOperators.Equal, _controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.Equal);
                Assert.Equal(AllowedLogicalOperators.GreaterThan, _controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.GreaterThan);
                Assert.Equal(AllowedLogicalOperators.GreaterThanOrEqual, _controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.GreaterThanOrEqual);
                Assert.NotEqual(AllowedLogicalOperators.Has, _controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.Has);
                Assert.Equal(AllowedLogicalOperators.LessThan, _controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.LessThan);
                Assert.Equal(AllowedLogicalOperators.LessThanOrEqual, _controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.LessThanOrEqual);
                Assert.Equal(AllowedLogicalOperators.NotEqual, _controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.NotEqual);
                Assert.Equal(AllowedLogicalOperators.Or, _controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.Or);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void CeilingFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Ceiling, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Ceiling);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ConcatFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Concat, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Concat);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ContainsFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Contains, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Contains);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void CountQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Count, _controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Count);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void DayFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Day, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Day);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void EndsWithFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.EndsWith, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.EndsWith);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ExpandQueryOptionIsNotAllowed()
            {
                Assert.NotEqual(AllowedQueryOptions.Expand, _controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Expand);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void FilterQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Filter, _controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Filter);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void FloorFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Floor, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Floor);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void FormatCountQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Format, _controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Format);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void HourFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Hour, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Hour);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void IndexOfFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.IndexOf, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.IndexOf);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void LengthFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Length, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Length);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void MaxTopIsSetTo50()
            {
                Assert.Equal(50, _controller.ValidationSettings.MaxTop);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void MinuteFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Minute, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Minute);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void MonthFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Month, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Month);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void OrderByQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.OrderBy, _controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.OrderBy);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ReplaceFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Replace, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Replace);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void RoundFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Round, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Round);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void SecondFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Second, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Second);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void SelectQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Select, _controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Select);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void SkipQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Skip, _controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Skip);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void SkipTokenQueryOptionIsNotAllowed()
            {
                Assert.NotEqual(AllowedQueryOptions.SkipToken, _controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.SkipToken);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void StartsWithFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.StartsWith, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.StartsWith);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void SubstringFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Substring, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Substring);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ToLowerFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.ToLower, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.ToLower);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TopQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Top, _controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Top);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ToUpperFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.ToUpper, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.ToUpper);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TrimFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Trim, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Trim);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void YearFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Year, _controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Year);
            }
        }

        public class WhenAValidSkipValueIsSpecified
        {
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;

            public WhenAValidSkipValueIsSpecified()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=15"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 50, 0)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _controller.Get(_queryOptions);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ItIsUsedInThePagedQuery()
            {
                _mockSession.Verify(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(_queryOptions.Skip.Value, 50)));
            }
        }

        public class WhenAValidTopValueIsSpecified
        {
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;

            public WhenAValidTopValueIsSpecified()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$top=15"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 15, 0)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _controller.Get(_queryOptions);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ItIsUsedInThePagedQuery()
            {
                _mockSession.Verify(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(0, _queryOptions.Top.Value)));
            }
        }

        public class WhenCalling_DeleteEntityResponseAsync_AndAnEntityIsDeleted
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCalling_DeleteEntityResponseAsync_AndAnEntityIsDeleted()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), _identifier)).Returns(Task.FromResult(true));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Delete, "http://services.microlite.org/odata/Customers(12345)")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Delete(_identifier).Result;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNoContent()
            {
                Assert.Equal(HttpStatusCode.NoContent, _response.StatusCode);
            }
        }

        public class WhenCalling_DeleteEntityResponseAsync_AndAnEntityIsNotDeleted
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCalling_DeleteEntityResponseAsync_AndAnEntityIsNotDeleted()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), _identifier)).Returns(Task.FromResult(false));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Delete, "http://services.microlite.org/odata/Customers(12345)")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Delete(_identifier).Result;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }
        }

        public class WhenCalling_GetCountResponseAsync
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetCountResponseAsync()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.Advanced.ExecuteScalarAsync<long>(It.IsAny<SqlQuery>())).Returns(Task.FromResult(150L));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers/$count")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Count().Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsStringContent()
            {
                Assert.IsType<StringContent>(_content);
                Assert.Equal("150", ((StringContent)_content).ReadAsStringAsync().Result);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContentType_IsTextPlain()
            {
                Assert.Equal("text/plain", _response.Content.Headers.ContentType.MediaType);
            }
        }

        public class WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsInvalid
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly string _propertyName = "Foo";
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsInvalid()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Foo")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.GetProperty(_identifier, _propertyName).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldContainTheErrorMessage()
            {
                Assert.Equal("{\"error\":{\"code\":\"400\",\"message\":\"The type 'MicroLite.Extensions.WebApi.Tests.OData.TestEntities.Customer' does not contain a property named 'Foo'.\"}}", Newtonsoft.Json.JsonConvert.SerializeObject(((ObjectContent)_content).Value));
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeBadRequest()
            {
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
            }
        }

        public class WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsValid
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly ExpandoObject _entity = new ExpandoObject();
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly string _propertyName = "Name";
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsValid()
            {
                TestHelper.EnsureEDM();

                ((IDictionary<string, object>)_entity)["Name"] = "Bob";

                _mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(_entity));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Name")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.GetProperty(_identifier, _propertyName).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContentShouldContainTheODataContext()
            {
                var responseContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers(12345)/Name", responseContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContentShouldContainThePropertyValue()
            {
                var responseContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("Bob", responseContent.Value);
            }
        }

        public class WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsValidButNoResults
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly string _propertyName = "Name";
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsValidButNoResults()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Name")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.GetProperty(_identifier, _propertyName).Result;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }
        }

        public class WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsInvalid
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly string _propertyName = "Foo";
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsInvalid()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Foo/$value")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.GetPropertyValue(_identifier, _propertyName).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldContainTheErrorMessage()
            {
                Assert.Equal("{\"error\":{\"code\":\"400\",\"message\":\"The type 'MicroLite.Extensions.WebApi.Tests.OData.TestEntities.Customer' does not contain a property named 'Foo'.\"}}", Newtonsoft.Json.JsonConvert.SerializeObject(((ObjectContent)_content).Value));
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeBadRequest()
            {
                Assert.Equal(HttpStatusCode.BadRequest, _response.StatusCode);
            }
        }

        public class WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsValid
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly ExpandoObject _entity = new ExpandoObject();
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly string _propertyName = "Name";
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsValid()
            {
                TestHelper.EnsureEDM();

                ((IDictionary<string, object>)_entity)["Name"] = "Bob";

                _mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(_entity));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Name")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.GetPropertyValue(_identifier, _propertyName).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsStringContent()
            {
                Assert.IsType<StringContent>(_response.Content);
                Assert.Equal("Bob", ((StringContent)_content).ReadAsStringAsync().Result);
            }
        }

        public class WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsValidButNoResults
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly string _propertyName = "Name";
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsValidButNoResults()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Name")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.GetPropertyValue(_identifier, _propertyName).Result;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }
        }

        public class WhenCalling_GetEntityResponseAsync_EntityKey_AndAnEntityIsNotReturned
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityResponseAsync_EntityKey_AndAnEntityIsNotReturned()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_identifier).Result;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(_response.Content);
            }
        }

        public class WhenCalling_GetEntityResponseAsync_EntityKey_AndAnEntityIsReturned
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly ExpandoObject _entity = new ExpandoObject();
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityResponseAsync_EntityKey_AndAnEntityIsReturned()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(_entity));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_identifier).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldContainTheEntity()
            {
                Assert.Equal(_entity, ((ObjectContent)_content).Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContentShouldContainTheODataContext()
            {
                var responseContent = (IDictionary<string, object>)_entity;

                Assert.True(responseContent.ContainsKey("@odata.context"));
                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers/$entity", ((Uri)responseContent["@odata.context"]).AbsoluteUri);
            }
        }

        public class WhenCalling_GetEntityResponseAsync_SelectProperties_AndAnEntityIsReturned
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityResponseAsync_SelectProperties_AndAnEntityIsReturned()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$select=Name,Reference"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 25, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers(Name,Reference)", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenCalling_GetEntityResponseAsync_SelectStar_AndAnEntityIsReturned
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCalling_GetEntityResponseAsync_SelectStar_AndAnEntityIsReturned()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$select=*"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 25, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers(*)", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenCalling_PostEntityResponseAsync
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Customer _customer = new Customer();
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCalling_PostEntityResponseAsync()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.InsertAsync(It.IsNotNull<Customer>()))
                    .Returns(Task.FromResult(0))
                    .Callback((object o) =>
                    {
                        ((Customer)o).Id = _identifier;
                    });

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Post, "http://services.microlite.org/odata/Customers")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Post(_customer).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldContainTheEntity()
            {
                Assert.Equal(_customer, ((ObjectContent)_content).Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldContainTheUriForTheEntity()
            {
                Assert.Equal("http://services.microlite.org/odata/Customers(12345)", _response.Headers.Location.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeCreated()
            {
                Assert.Equal(HttpStatusCode.Created, _response.StatusCode);
            }
        }

        public class WhenCalling_PutEntityResponseAsync_AndAnEntityIsNotReturned
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCalling_PutEntityResponseAsync_AndAnEntityIsNotReturned()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.SingleAsync<Customer>(_identifier)).Returns(Task.FromResult((Customer)null));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Put, "http://services.microlite.org/odata/Customers(12345)")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Put(_identifier, new Customer()).Result;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(_response.Content);
            }
        }

        public class WhenCalling_PutEntityResponseAsync_AndAnEntityIsNotUpdated
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            public WhenCalling_PutEntityResponseAsync_AndAnEntityIsNotUpdated()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.SingleAsync<Customer>(_identifier)).Returns(Task.FromResult(new Customer()));
                _mockSession.Setup(x => x.UpdateAsync(It.IsNotNull<Customer>())).Returns(Task.FromResult(false));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Put, "http://services.microlite.org/odata/Customers(12345)")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Put(_identifier, new Customer()).Result;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotModified()
            {
                Assert.Equal(HttpStatusCode.NotModified, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(_response.Content);
            }
        }

        public class WhenCalling_PutEntityResponseAsync_AndAnEntityIsUpdated
        {
            private readonly CustomerController _controller;
            private readonly int _identifier = 12345;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage _response;

            private readonly Customer _updatedCustomer = new Customer
            {
                Name = "Joe Bloggs"
            };

            public WhenCalling_PutEntityResponseAsync_AndAnEntityIsUpdated()
            {
                TestHelper.EnsureEDM();

                _mockSession.Setup(x => x.SingleAsync<Customer>(_identifier)).Returns(Task.FromResult(new Customer()));
                _mockSession.Setup(x => x.UpdateAsync(It.IsNotNull<Customer>())).Returns(Task.FromResult(true));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Put, "http://services.microlite.org/odata/Customers(12345)")
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Put(_identifier, _updatedCustomer).Result;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNoContent()
            {
                Assert.Equal(HttpStatusCode.NoContent, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(_response.Content);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheUpdatedCustomerShouldHaveTheIdentifierSet()
            {
                Assert.Equal(_identifier, _updatedCustomer.Id);
            }
        }

        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteODataApiController<Customer, int> _controller;
            private readonly IAsyncSession _session = new Mock<IAsyncSession>().Object;

            public WhenConstructedWithAnISession()
            {
                var mockController = new Mock<MicroLiteODataApiController<Customer, int>>(_session)
                {
                    CallBase = true
                };

                _controller = mockController.Object;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheSessionIsSet()
            {
                Assert.Equal(_session, _controller.Session);
            }
        }

        public class WhenCountIsNotSpecified_AndMoreResultsAreAvailable_ClientPaged
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCountIsNotSpecified_AndMoreResultsAreAvailable_ClientPaged()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$top=25"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 25, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_CountIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Null(odataContent.Count);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=25&$top=25", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenCountIsNotSpecified_AndMoreResultsAreAvailable_ServerPaged
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCountIsNotSpecified_AndMoreResultsAreAvailable_ServerPaged()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 50, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_CountIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Null(odataContent.Count);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=50", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenCountIsNotSpecified_AndNoMoreResultsAreAvailable
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCountIsNotSpecified_AndNoMoreResultsAreAvailable()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=75&$top=25"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(4, new object[0], 25, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_CountIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Null(odataContent.Count);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_NextLinkIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Null(odataContent.NextLink);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenCountIsNotSpecified_ForPage2_AndMoreResultsAreAvailable
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCountIsNotSpecified_ForPage2_AndMoreResultsAreAvailable()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=25&$top=25"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(2, new object[0], 25, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_CountIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Null(odataContent.Count);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=50&$top=25", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenCountTrueIsSpecified_AndMoreResultsAreAvailable_ClientPaged
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCountTrueIsSpecified_AndMoreResultsAreAvailable_ClientPaged()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$top=25&$count=true"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 25, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_CountIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.Count);
                Assert.Equal(100, odataContent.Count); // Matches PagedResult<T>.TotalResults
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=25&$count=true&$top=25", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenCountTrueIsSpecified_AndMoreResultsAreAvailable_ServerPaged
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCountTrueIsSpecified_AndMoreResultsAreAvailable_ServerPaged()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$count=true"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 50, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_CountIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.Count);
                Assert.Equal(100, odataContent.Count); // Matches PagedResult<T>.TotalResults
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=50&$count=true", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenCountTrueIsSpecified_AndNoMoreResultsAreAvailable
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCountTrueIsSpecified_AndNoMoreResultsAreAvailable()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=75&$top=25&$count=true"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(4, new object[0], 25, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_CountIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.Count);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_NextLinkIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Null(odataContent.NextLink);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenCountTrueIsSpecified_ForPage2_AndMoreResultsAreAvailable
        {
            private readonly HttpContent _content;
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenCountTrueIsSpecified_ForPage2_AndMoreResultsAreAvailable()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=25&$count=true&$top=25"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(2, new object[0], 25, 100)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
                _content = _response.Content;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_CountIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.Count);
                Assert.Equal(100, odataContent.Count); // Matches PagedResult<T>.TotalResults
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)_content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=50&$count=true&$top=25", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)_content).Value);
            }
        }

        public class WhenFormatQueryOptionIsSpecified
        {
            private readonly CustomerController _controller;
            private readonly Mock<IAsyncSession> _mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions _queryOptions;
            private readonly HttpResponseMessage _response;

            public WhenFormatQueryOptionIsSpecified()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$format=application/xml"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 50, 0)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = _queryOptions.Request
                };
                _controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                _response = _controller.Get(_queryOptions).Result;
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, _response.StatusCode);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheResponseContentTypeHeaderIsSet()
            {
                Assert.Equal(_queryOptions.Format.MediaTypeHeaderValue, _response.Content.Headers.ContentType);
            }
        }

        public class WhenNullQueryOptionsAreSupplied
        {
            [Fact]
            [Trait("Category", "Unit")]
            public void AnArgumentNullExceptionIsThrown()
            {
                var controller = new CustomerController(Mock.Of<IAsyncSession>());

                var queryOptions = default(ODataQueryOptions);

                AggregateException exception = Assert.Throws<AggregateException>(() => controller.Get(queryOptions).Result);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
                Assert.Contains("queryOptions", exception.InnerException.Message);
            }
        }
    }
}
