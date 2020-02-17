using System;
using System.Net.Http;
using MicroLite.Builder;
using MicroLite.Extensions.WebApi.OData.Binders;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using MicroLite.Mapping;
using Net.Http.WebApi.OData.Model;
using Net.Http.WebApi.OData.Query;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    public class SelectBinderTests
    {
        public SelectBinderTests()
        {
            TestHelper.EnsureEDM();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void BindBindSelectThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var queryOptions = new ODataQueryOptions(
                new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers"),
                EntityDataModel.Current.EntitySets["Customers"]);

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => SelectBinder.BindSelect(queryOptions.Select, null));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        public class WhenCallingBindSelectQueryOptionAndNoPropertiesHaveBeenSpecified
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindSelectQueryOptionAndNoPropertiesHaveBeenSpecified()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _sqlQuery = SelectBinder.BindSelect(queryOptions.Select, ObjectInfo.For(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void AllPropertiesOnTheMappedTypeShouldBeIncluded()
            {
                var expected = SqlBuilder.Select("*").From(typeof(Customer)).ToSqlQuery();

                Assert.Equal(expected, _sqlQuery);
            }
        }

        public class WhenCallingBindSelectQueryOptionAndSpecificPropertiesHaveBeenSpecified
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindSelectQueryOptionAndSpecificPropertiesHaveBeenSpecified()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        "http://services.microlite.org/odata/Customers?$select=Name,DateOfBirth,Status"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _sqlQuery = SelectBinder.BindSelect(queryOptions.Select, ObjectInfo.For(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheColumnNamesForTheSpecifiedPropertiesShouldBeTheOnlyOnesInTheSelectList()
            {
                var expected = SqlBuilder.Select("Name", "DateOfBirth", "CustomerStatusId").From(typeof(Customer)).ToSqlQuery();

                Assert.Equal(expected, _sqlQuery);
            }
        }

        public class WhenCallingBindSelectQueryOptionAndStarHasBeenSpecified
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindSelectQueryOptionAndStarHasBeenSpecified()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    new HttpRequestMessage(HttpMethod.Get, "http://services.microlite.org/odata/Customers?$select=*"),
                    EntityDataModel.Current.EntitySets["Customers"]);

                _sqlQuery = SelectBinder.BindSelect(queryOptions.Select, ObjectInfo.For(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void AllPropertiesOnTheMappedTypeShouldBeIncluded()
            {
                var expected = SqlBuilder.Select("*").From(typeof(Customer)).ToSqlQuery();

                Assert.Equal(expected, _sqlQuery);
            }
        }
    }
}
