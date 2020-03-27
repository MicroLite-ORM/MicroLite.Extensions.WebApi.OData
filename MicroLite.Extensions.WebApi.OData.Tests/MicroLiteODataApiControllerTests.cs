using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using MicroLite.Extensions.WebApi.OData;
using MicroLite.Extensions.WebApi.OData.Tests.Integration;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using Moq;
using Net.Http.OData;
using Net.Http.OData.Model;
using Net.Http.OData.Query;
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
                "?$skip=-1",
                EntityDataModel.Current.EntitySets["Customers"],
                ODataQueryOptionsValidator.GetValidator(ODataVersion.OData40));

            var controller = new CustomerController(Mock.Of<ISession>());

            ODataException exception = await Assert.ThrowsAsync<ODataException>(() => controller.Get(queryOptions));
            Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
        }

        public class TheDefaultValidatonSettings
        {
            private readonly CustomerController _controller = new CustomerController(Mock.Of<ISession>());

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
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly ODataQueryOptions _queryOptions;

            public WhenAValidSkipValueIsSpecified()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    "?$skip=15",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 50, 0)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers" + _queryOptions.RawValues.ToString()),
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
            private readonly Mock<ISession> _mockSession = new Mock<ISession>();
            private readonly ODataQueryOptions _queryOptions;

            public WhenAValidTopValueIsSpecified()
            {
                TestHelper.EnsureEDM();

                _queryOptions = new ODataQueryOptions(
                    "?$top=15",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _mockSession.Setup(x => x.PagedAsync<dynamic>(It.IsAny<SqlQuery>(), It.IsAny<PagingOptions>())).Returns(Task.FromResult(new PagedResult<dynamic>(1, new object[0], 15, 0)));

                _controller = new CustomerController(_mockSession.Object)
                {
                    Request = new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers" + _queryOptions.RawValues.ToString()),
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

        public class WhenConstructedWithAnISession
        {
            private readonly MicroLiteODataApiController<Customer, int> _controller;
            private readonly ISession _session = new Mock<ISession>().Object;

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

        public class WhenNullQueryOptionsAreSupplied
        {
            [Fact]
            [Trait("Category", "Unit")]
            public async Task AnArgumentNullExceptionIsThrown()
            {
                var controller = new CustomerController(Mock.Of<ISession>());

                var queryOptions = default(ODataQueryOptions);

                ArgumentNullException exception = await Assert.ThrowsAsync<ArgumentNullException>(() => controller.Get(queryOptions));
                Assert.Contains("queryOptions", exception.Message);
            }
        }
    }
}
