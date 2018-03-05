namespace MicroLite.Extensions.WebApi.Tests.OData
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Hosting;
    using MicroLite.Extensions.WebApi.OData;
    using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
    using Moq;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Model;
    using Net.Http.WebApi.OData.Query;
    using Net.Http.WebApi.OData.Query.Validators;
    using Xunit;

    public class MicroLiteODataApiControllerTests
    {
        [Fact]
        public async Task WhenCallingGetEntityResponseTheODataQueryOptionsAreValidated()
        {
            TestHelper.EnsureEDM();

            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=-1"),
                EntityDataModel.Current.EntitySets["Customers"]);

            var controller = new CustomerController(Mock.Of<IAsyncSession>());

            var exception = await Assert.ThrowsAsync<ODataException>(() => controller.Get(queryOptions));
            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        }

        public class TheDefaultValidatonSettings
        {
            private readonly CustomerController controller = new CustomerController(Mock.Of<IAsyncSession>());

            [Fact]
            public void AllArithmeticOperatorsAreAllowed()
            {
                Assert.Equal(AllowedArithmeticOperators.All, controller.ValidationSettings.AllowedArithmeticOperators & AllowedArithmeticOperators.All);
            }

            [Fact]
            public void AllLogicalOperatorsAreAllowed_ExceptHas()
            {
                Assert.Equal(AllowedLogicalOperators.And, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.And);
                Assert.Equal(AllowedLogicalOperators.Equal, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.Equal);
                Assert.Equal(AllowedLogicalOperators.GreaterThan, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.GreaterThan);
                Assert.Equal(AllowedLogicalOperators.GreaterThanOrEqual, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.GreaterThanOrEqual);
                Assert.NotEqual(AllowedLogicalOperators.Has, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.Has);
                Assert.Equal(AllowedLogicalOperators.LessThan, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.LessThan);
                Assert.Equal(AllowedLogicalOperators.LessThanOrEqual, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.LessThanOrEqual);
                Assert.Equal(AllowedLogicalOperators.NotEqual, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.NotEqual);
                Assert.Equal(AllowedLogicalOperators.Or, controller.ValidationSettings.AllowedLogicalOperators & AllowedLogicalOperators.Or);
            }

            [Fact]
            public void CeilingFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Ceiling, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Ceiling);
            }

            [Fact]
            public void ConcatFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Concat, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Concat);
            }

            [Fact]
            public void ContainsFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Contains, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Contains);
            }

            [Fact]
            public void CountQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Count, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Count);
            }

            [Fact]
            public void DayFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Day, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Day);
            }

            [Fact]
            public void EndsWithFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.EndsWith, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.EndsWith);
            }

            [Fact]
            public void ExpandQueryOptionIsNotAllowed()
            {
                Assert.NotEqual(AllowedQueryOptions.Expand, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Expand);
            }

            [Fact]
            public void FilterQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Filter, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Filter);
            }

            [Fact]
            public void FloorFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Floor, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Floor);
            }

            [Fact]
            public void FormatCountQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Format, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Format);
            }

            [Fact]
            public void HourFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Hour, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Hour);
            }

            [Fact]
            public void IndexOfFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.IndexOf, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.IndexOf);
            }

            [Fact]
            public void LengthFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Length, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Length);
            }

            [Fact]
            public void MaxTopIsSetTo50()
            {
                Assert.Equal(50, controller.ValidationSettings.MaxTop);
            }

            [Fact]
            public void MinuteFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Minute, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Minute);
            }

            [Fact]
            public void MonthFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Month, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Month);
            }

            [Fact]
            public void OrderByQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.OrderBy, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.OrderBy);
            }

            [Fact]
            public void ReplaceFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Replace, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Replace);
            }

            [Fact]
            public void RoundFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Round, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Round);
            }

            [Fact]
            public void SecondFunctionIsNotAllowed()
            {
                Assert.NotEqual(AllowedFunctions.Second, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Second);
            }

            [Fact]
            public void SelectQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Select, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Select);
            }

            [Fact]
            public void SkipQueryOptionIsAllowed()
            {
                Assert.Equal(AllowedQueryOptions.Skip, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Skip);
            }

            [Fact]
            public void SkipTokenQueryOptionIsNotAllowed()
            {
                Assert.NotEqual(AllowedQueryOptions.SkipToken, controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.SkipToken);
            }

            [Fact]
            public void StartsWithFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.StartsWith, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.StartsWith);
            }

            [Fact]
            public void SubstringFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Substring, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Substring);
            }

            [Fact]
            public void ToLowerFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.ToLower, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.ToLower);
            }

            [Fact]
            public void TopQueryOptionIsAllowed()
            {
                Assert.Equal(controller.ValidationSettings.AllowedQueryOptions & AllowedQueryOptions.Top, AllowedQueryOptions.Top);
            }

            [Fact]
            public void ToUpperFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.ToUpper, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.ToUpper);
            }

            [Fact]
            public void TrimFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Trim, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Trim);
            }

            [Fact]
            public void YearFunctionIsAllowed()
            {
                Assert.Equal(AllowedFunctions.Year, controller.ValidationSettings.AllowedFunctions & AllowedFunctions.Year);
            }
        }

        public class WhenAValidSkipValueIsSpecified
        {
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;

            public WhenAValidSkipValueIsSpecified()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=15"),
                EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 50, 0)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void ItIsUsedInThePagedQuery()
            {
                this.mockSession.Verify(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(this.queryOptions.Skip.Value, 50)));
            }
        }

        public class WhenAValidTopValueIsSpecified
        {
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;

            public WhenAValidTopValueIsSpecified()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$top=15"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 15, 0)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.controller.Get(this.queryOptions);
            }

            [Fact]
            public void ItIsUsedInThePagedQuery()
            {
                this.mockSession.Verify(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), PagingOptions.SkipTake(0, this.queryOptions.Top.Value)));
            }
        }

        public class WhenCalling_DeleteEntityResponseAsync_AndAnEntityIsDeleted
        {
            private readonly CustomerController controller;
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCalling_DeleteEntityResponseAsync_AndAnEntityIsDeleted()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), this.identifier)).Returns(Task.FromResult(true));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Delete, "http://services.microlite.org/odata/Customers(12345)");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.Delete(this.identifier).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNoContent()
            {
                Assert.Equal(HttpStatusCode.NoContent, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_DeleteEntityResponseAsync_AndAnEntityIsNotDeleted
        {
            private readonly CustomerController controller;
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCalling_DeleteEntityResponseAsync_AndAnEntityIsNotDeleted()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.Advanced.DeleteAsync(typeof(Customer), this.identifier)).Returns(Task.FromResult(false));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Delete, "http://services.microlite.org/odata/Customers(12345)");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.Delete(this.identifier).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_GetCountResponseAsync
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCalling_GetCountResponseAsync()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.Advanced.ExecuteScalarAsync<long>(It.IsAny<SqlQuery>())).Returns(Task.FromResult(150L));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Get, "http://services.microlite.org/odata/Customers/$count");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.Count().Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsStringContent()
            {
                Assert.IsType<StringContent>(this.response.Content);
                Assert.Equal("150", ((StringContent)this.response.Content).ReadAsStringAsync().Result);
            }

            [Fact]
            public void TheResponseContentType_IsTextPlain()
            {
                Assert.Equal("text/plain", response.Content.Headers.ContentType.MediaType);
            }
        }

        public class WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsInvalid
        {
            private readonly CustomerController controller;
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly string propertyName = "Foo";
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsInvalid()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Foo");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.GetProperty(this.identifier, this.propertyName).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheErrorMessage()
            {
                Assert.Equal("{\"error\":{\"code\":\"400\",\"message\":\"The type 'MicroLite.Extensions.WebApi.Tests.OData.TestEntities.Customer' does not contain a property named 'Foo'.\"}}", Newtonsoft.Json.JsonConvert.SerializeObject(((ObjectContent)this.response.Content).Value));
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeBadRequest()
            {
                Assert.Equal(HttpStatusCode.BadRequest, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsValid
        {
            private readonly CustomerController controller;
            private readonly ExpandoObject entity = new ExpandoObject();
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly string propertyName = "Name";
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsValid()
            {
                TestHelper.EnsureEDM();

                ((IDictionary<string, object>)this.entity)["Name"] = "Bob";

                this.mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(entity));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Name");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.GetProperty(this.identifier, this.propertyName).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }

            [Fact]
            public void TheResponseContentShouldContainTheODataContext()
            {
                var responseContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers(12345)/Name", responseContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheResponseContentShouldContainThePropertyValue()
            {
                var responseContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("Bob", responseContent.Value);
            }
        }

        public class WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsValidButNoResults
        {
            private readonly CustomerController controller;
            private readonly ExpandoObject entity = new ExpandoObject();
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly string propertyName = "Name";
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityPropertyResponseAsync_AndThePropertyNameIsValidButNoResults()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Name");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.GetProperty(this.identifier, this.propertyName).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsInvalid
        {
            private readonly CustomerController controller;
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly string propertyName = "Foo";
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsInvalid()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Foo/$value");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.GetPropertyValue(this.identifier, propertyName).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheErrorMessage()
            {
                Assert.Equal("{\"error\":{\"code\":\"400\",\"message\":\"The type 'MicroLite.Extensions.WebApi.Tests.OData.TestEntities.Customer' does not contain a property named 'Foo'.\"}}", Newtonsoft.Json.JsonConvert.SerializeObject(((ObjectContent)this.response.Content).Value));
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeBadRequest()
            {
                Assert.Equal(HttpStatusCode.BadRequest, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsValid
        {
            private readonly CustomerController controller;
            private readonly ExpandoObject entity = new ExpandoObject();
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly string propertyName = "Name";
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsValid()
            {
                TestHelper.EnsureEDM();

                ((IDictionary<string, object>)this.entity)["Name"] = "Bob";

                this.mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(entity));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Name");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.GetPropertyValue(this.identifier, this.propertyName).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsStringContent()
            {
                Assert.IsType<StringContent>(this.response.Content);
                Assert.Equal("Bob", ((StringContent)this.response.Content).ReadAsStringAsync().Result);
            }
        }

        public class WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsValidButNoResults
        {
            private readonly CustomerController controller;
            private readonly ExpandoObject entity = new ExpandoObject();
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly string propertyName = "Name";
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityPropertyValueResponseAsync_AndThePropertyNameIsValidButNoResults()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)/Name");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.GetPropertyValue(this.identifier, this.propertyName).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_GetEntityResponseAsync_EntityKey_AndAnEntityIsNotReturned
        {
            private readonly CustomerController controller;
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityResponseAsync_EntityKey_AndAnEntityIsNotReturned()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(null));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.Get(this.identifier).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, this.response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(this.response.Content);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_GetEntityResponseAsync_EntityKey_AndAnEntityIsReturned
        {
            private readonly CustomerController controller;
            private readonly ExpandoObject entity = new ExpandoObject();
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityResponseAsync_EntityKey_AndAnEntityIsReturned()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.SingleAsync<dynamic>(It.IsAny<SqlQuery>())).Returns(Task.FromResult<dynamic>(entity));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Get, "http://services.microlite.org/odata/Customers(12345)");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.Get(this.identifier).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheEntity()
            {
                Assert.Equal(this.entity, ((ObjectContent)this.response.Content).Value);
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContentShouldContainTheODataContext()
            {
                var responseContent = (IDictionary<string, object>)this.entity;

                Assert.True(responseContent.ContainsKey("@odata.context"));
                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers/$entity", ((Uri)responseContent["@odata.context"]).AbsoluteUri);
            }
        }

        public class WhenCalling_GetEntityResponseAsync_SelectProperties_AndAnEntityIsReturned
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityResponseAsync_SelectProperties_AndAnEntityIsReturned()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$select=Name,Reference"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 25, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers(Name,Reference)", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenCalling_GetEntityResponseAsync_SelectStar_AndAnEntityIsReturned
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCalling_GetEntityResponseAsync_SelectStar_AndAnEntityIsReturned()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$select=*"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 25, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers(*)", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenCalling_PostEntityResponseAsync
        {
            private readonly CustomerController controller;
            private readonly Customer customer = new Customer();
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCalling_PostEntityResponseAsync()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.InsertAsync(It.IsNotNull<Customer>()))
                    .Returns(Task.FromResult(0))
                    .Callback((object o) =>
                    {
                        ((Customer)o).Id = this.identifier;
                    });

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Post, "http://services.microlite.org/odata/Customers");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.Post(this.customer).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheEntity()
            {
                Assert.Equal(this.customer, ((ObjectContent)this.response.Content).Value);
            }

            [Fact]
            public void TheHttpResponseMessageShouldContainTheUriForTheEntity()
            {
                Assert.Equal("http://services.microlite.org/odata/Customers(12345)", this.response.Headers.Location.AbsoluteUri);
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeCreated()
            {
                Assert.Equal(HttpStatusCode.Created, this.response.StatusCode);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_PutEntityResponseAsync_AndAnEntityIsNotReturned
        {
            private readonly CustomerController controller;
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCalling_PutEntityResponseAsync_AndAnEntityIsNotReturned()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(Task.FromResult((Customer)null));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Put, "http://services.microlite.org/odata/Customers(12345)");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.Put(this.identifier, new Customer()).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotFound()
            {
                Assert.Equal(HttpStatusCode.NotFound, this.response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(this.response.Content);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_PutEntityResponseAsync_AndAnEntityIsNotUpdated
        {
            private readonly CustomerController controller;
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            public WhenCalling_PutEntityResponseAsync_AndAnEntityIsNotUpdated()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(Task.FromResult(new Customer()));
                this.mockSession.Setup(x => x.UpdateAsync(It.IsNotNull<Customer>())).Returns(Task.FromResult(false));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Put, "http://services.microlite.org/odata/Customers(12345)");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.Put(this.identifier, new Customer()).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNotModified()
            {
                Assert.Equal(HttpStatusCode.NotModified, this.response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(this.response.Content);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }
        }

        public class WhenCalling_PutEntityResponseAsync_AndAnEntityIsUpdated
        {
            private readonly CustomerController controller;
            private readonly int identifier = 12345;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly HttpResponseMessage response;

            private readonly Customer updatedCustomer = new Customer
            {
                Name = "Joe Bloggs"
            };

            public WhenCalling_PutEntityResponseAsync_AndAnEntityIsUpdated()
            {
                TestHelper.EnsureEDM();

                this.mockSession.Setup(x => x.SingleAsync<Customer>(this.identifier)).Returns(Task.FromResult(new Customer()));
                this.mockSession.Setup(x => x.UpdateAsync(It.IsNotNull<Customer>())).Returns(Task.FromResult(true));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = new HttpRequestMessage(
                    HttpMethod.Put, "http://services.microlite.org/odata/Customers(12345)");
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());

                this.response = this.controller.Put(this.identifier, this.updatedCustomer).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeNoContent()
            {
                Assert.Equal(HttpStatusCode.NoContent, this.response.StatusCode);
            }

            [Fact]
            public void TheHttpResponseMessageShouldNotContainAnyContent()
            {
                Assert.Null(this.response.Content);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheUpdatedCustomerShouldHaveTheIdentifierSet()
            {
                Assert.Equal(this.identifier, this.updatedCustomer.Id);
            }
        }

        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteODataApiController<Customer, int> controller;
            private readonly IAsyncSession session = new Mock<IAsyncSession>().Object;

            public WhenConstructedWithAnISession()
            {
                var mockController = new Mock<MicroLiteODataApiController<Customer, int>>(this.session);
                mockController.CallBase = true;

                this.controller = mockController.Object;
            }

            [Fact]
            public void TheSessionIsSet()
            {
                Assert.Equal(this.session, this.controller.Session);
            }
        }

        public class WhenCountIsNotSpecified_AndMoreResultsAreAvailable_ClientPaged
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCountIsNotSpecified_AndMoreResultsAreAvailable_ClientPaged()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$top=25"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 25, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataResponseContent_CountIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Null(odataContent.Count);
            }

            [Fact]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=25&$top=25", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenCountIsNotSpecified_AndMoreResultsAreAvailable_ServerPaged
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCountIsNotSpecified_AndMoreResultsAreAvailable_ServerPaged()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 50, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataResponseContent_CountIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Null(odataContent.Count);
            }

            [Fact]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=50", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenCountIsNotSpecified_AndNoMoreResultsAreAvailable
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCountIsNotSpecified_AndNoMoreResultsAreAvailable()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=75&$top=25"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(4, new object[0], 25, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataResponseContent_CountIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Null(odataContent.Count);
            }

            [Fact]
            public void TheODataResponseContent_NextLinkIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Null(odataContent.NextLink);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenCountIsNotSpecified_ForPage2_AndMoreResultsAreAvailable
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCountIsNotSpecified_ForPage2_AndMoreResultsAreAvailable()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=25&$top=25"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(2, new object[0], 25, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataResponseContent_CountIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Null(odataContent.Count);
            }

            [Fact]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=50&$top=25", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenCountTrueIsSpecified_AndMoreResultsAreAvailable_ClientPaged
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCountTrueIsSpecified_AndMoreResultsAreAvailable_ClientPaged()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$top=25&$count=true"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 25, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataResponseContent_CountIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.Count);
                Assert.Equal(100, odataContent.Count); // Matches PagedResult<T>.TotalResults
            }

            [Fact]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=25&$count=true&$top=25", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenCountTrueIsSpecified_AndMoreResultsAreAvailable_ServerPaged
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCountTrueIsSpecified_AndMoreResultsAreAvailable_ServerPaged()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$count=true"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 50, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataResponseContent_CountIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.Count);
                Assert.Equal(100, odataContent.Count); // Matches PagedResult<T>.TotalResults
            }

            [Fact]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=50&$count=true", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenCountTrueIsSpecified_AndNoMoreResultsAreAvailable
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCountTrueIsSpecified_AndNoMoreResultsAreAvailable()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=75&$top=25&$count=true"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(4, new object[0], 25, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataResponseContent_CountIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.Count);
            }

            [Fact]
            public void TheODataResponseContent_NextLinkIsNotSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Null(odataContent.NextLink);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenCountTrueIsSpecified_ForPage2_AndMoreResultsAreAvailable
        {
            private readonly HttpContent content;
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenCountTrueIsSpecified_ForPage2_AndMoreResultsAreAvailable()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$skip=25&$count=true&$top=25"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(2, new object[0], 25, 100)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;

                this.response = this.controller.Get(this.queryOptions).Result;
                this.content = this.response.Content;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheODataResponseContent_ContextIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.Equal("http://services.microlite.org/odata/$metadata#Customers", odataContent.Context.AbsoluteUri);
            }

            [Fact]
            public void TheODataResponseContent_CountIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.Count);
                Assert.Equal(100, odataContent.Count); // Matches PagedResult<T>.TotalResults
            }

            [Fact]
            public void TheODataResponseContent_NextLinkIsSet()
            {
                var odataContent = (ODataResponseContent)((ObjectContent)this.response.Content).Value;

                Assert.NotNull(odataContent.NextLink);
                Assert.Equal("http://services.microlite.org/odata/Customers?$skip=50&$count=true&$top=25", odataContent.NextLink.AbsoluteUri);
            }

            [Fact]
            public void TheODataVersionHeaderIsSet()
            {
                Assert.True(response.Headers.Contains("OData-Version"));
                Assert.Equal("4.0", response.Headers.GetValues("OData-Version").Single());
            }

            [Fact]
            public void TheResponseContent_IsObjectContent_WithODataResponseContentValue()
            {
                Assert.IsType<ODataResponseContent>(((ObjectContent)this.response.Content).Value);
            }
        }

        public class WhenFormatQueryOptionIsSpecified
        {
            private readonly CustomerController controller;
            private readonly Mock<IAsyncSession> mockSession = new Mock<IAsyncSession>();
            private readonly ODataQueryOptions queryOptions;
            private readonly HttpResponseMessage response;

            public WhenFormatQueryOptionIsSpecified()
            {
                TestHelper.EnsureEDM();

                this.queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$format=application/xml"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                this.mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 50, 0)));

                this.controller = new CustomerController(this.mockSession.Object);
                this.controller.Request = this.queryOptions.Request;
                this.controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration());
                this.controller.Session = this.mockSession.Object;
                this.response = this.controller.Get(this.queryOptions).Result;
            }

            [Fact]
            public void TheHttpResponseMessageShouldHaveHttpStatusCodeOK()
            {
                Assert.Equal(HttpStatusCode.OK, this.response.StatusCode);
            }

            [Fact]
            public void TheResponseContentTypeHeaderIsSet()
            {
                Assert.Equal(this.queryOptions.Format.MediaTypeHeaderValue, response.Content.Headers.ContentType);
            }
        }

        public class WhenNullQueryOptionsAreSupplied
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var controller = new CustomerController(Mock.Of<IAsyncSession>());

                var queryOptions = default(ODataQueryOptions);

                var exception = Assert.Throws<AggregateException>(() => controller.Get(queryOptions).Result);
                Assert.IsType<ArgumentNullException>(exception.InnerException);
                Assert.Contains("queryOptions", exception.InnerException.Message);
            }
        }

        private class CustomerController : MicroLiteODataApiController<Customer, int>
        {
            public CustomerController(IAsyncSession session)
                : base(session)
            {
            }

            public new ODataValidationSettings ValidationSettings => base.ValidationSettings;

            public System.Threading.Tasks.Task<HttpResponseMessage> Count()
                => this.GetCountResponseAsync();

            public System.Threading.Tasks.Task<HttpResponseMessage> Delete(int entityKey)
                => this.DeleteEntityResponseAsync(entityKey);

            public System.Threading.Tasks.Task<HttpResponseMessage> Get(int entityKey)
                => this.GetEntityResponseAsync(entityKey);

            public System.Threading.Tasks.Task<HttpResponseMessage> Get(ODataQueryOptions queryOptions)
                => this.GetEntityResponseAsync(queryOptions);

            public System.Threading.Tasks.Task<HttpResponseMessage> GetProperty(int entityKey, string propertyName)
                => this.GetEntityPropertyResponseAsync(entityKey, propertyName);

            public System.Threading.Tasks.Task<HttpResponseMessage> GetPropertyValue(int entityKey, string propertyName)
                => this.GetEntityPropertyValueResponseAsync(entityKey, propertyName);

            public System.Threading.Tasks.Task<HttpResponseMessage> Post(Customer entity)
                => this.PostEntityResponseAsync(entity);

            public System.Threading.Tasks.Task<HttpResponseMessage> Put(int entityKey, Customer entity)
                => this.PutEntityResponseAsync(entityKey, entity);
        }
    }
}