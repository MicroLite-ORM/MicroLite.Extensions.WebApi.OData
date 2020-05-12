using System;
using System.Net;
using MicroLite.Builder;
using MicroLite.Extensions.WebApi.OData.Binders;
using MicroLite.Extensions.WebApi.OData.Tests.TestEntities;
using MicroLite.Mapping;
using Moq;
using Net.Http.OData;
using Net.Http.OData.Model;
using Net.Http.OData.Query;
using Xunit;

namespace MicroLite.Extensions.WebApi.OData.Tests.Binders
{
    public class SelectBinderTests
    {
        public SelectBinderTests() => TestHelper.EnsureEDM();

        [Fact]
        [Trait("Category", "Unit")]
        public void BindBindSelectThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var queryOptions = new ODataQueryOptions(
                "?",
                EntityDataModel.Current.EntitySets["Customers"],
                Mock.Of<IODataQueryOptionsValidator>());

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => SelectBinder.BindSelect(queryOptions.Select, null));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void BindBindSelectThrowsODataExceptionForPropertyPathWithNext()
        {
            var queryOptions = new ODataQueryOptions(
                "?$select=Name/Foo",
                EntityDataModel.Current.EntitySets["Customers"],
                Mock.Of<IODataQueryOptionsValidator>());

            ODataException exception = Assert.Throws<ODataException>(
                () => SelectBinder.BindSelect(queryOptions.Select, ObjectInfo.For(typeof(Customer))));
        }

        public class WhenCallingBindSelectQueryOptionAndNoPropertiesHaveBeenSpecified
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindSelectQueryOptionAndNoPropertiesHaveBeenSpecified()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

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
                    "?$select=Name,DateOfBirth,Status",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

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
                    "?$select=*",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

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
