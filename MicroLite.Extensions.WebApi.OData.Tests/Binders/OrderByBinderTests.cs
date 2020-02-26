using System;
using MicroLite.Builder;
using MicroLite.Extensions.WebApi.OData.Binders;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using MicroLite.Mapping;
using Moq;
using Net.Http.OData.Model;
using Net.Http.OData.Query;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    public class OrderByBinderTests
    {
        public OrderByBinderTests()
        {
            TestHelper.EnsureEDM();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void BindOrderByThrowsArgumentNullExceptionForNullObjectInfo()
        {
            var queryOptions = new ODataQueryOptions(
                "?$orderby=Name",
                EntityDataModel.Current.EntitySets["Customers"],
                Mock.Of<IODataQueryOptionsValidator>());

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => OrderByBinder.BindOrderBy(queryOptions.OrderBy, null, SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("objectInfo", exception.ParamName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void BindOrderByThrowsArgumentNullExceptionForNullOrderBySqlBuilder()
        {
            var queryOptions = new ODataQueryOptions(
                "?$orderby=Name",
                EntityDataModel.Current.EntitySets["Customers"],
                Mock.Of<IODataQueryOptionsValidator>());

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
                () => OrderByBinder.BindOrderBy(queryOptions.OrderBy, ObjectInfo.For(typeof(Customer)), null));

            Assert.Equal("orderBySqlBuilder", exception.ParamName);
        }

        public class WhenCallingBindOrderBy_WithAnOrderByQueryOption
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindOrderBy_WithAnOrderByQueryOption()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$orderby=Status desc,Name",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = OrderByBinder.BindOrderBy(
                    queryOptions.OrderBy,
                    ObjectInfo.For(typeof(Customer)),
                    SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheColumnNamesForTheSpecifiedPropertiesShouldBeSetInTheOrderByClause()
            {
                var expected = SqlBuilder
                    .Select("*")
                    .From(typeof(Customer))
                    .OrderByDescending("CustomerStatusId")
                    .OrderByAscending("Name")
                    .ToSqlQuery();

                Assert.Equal(expected, _sqlQuery);
            }
        }

        public class WhenCallingBindOrderBy_WithoutAnOrderByQueryOption
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindOrderBy_WithoutAnOrderByQueryOption()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = OrderByBinder.BindOrderBy(
                    queryOptions.OrderBy,
                    ObjectInfo.For(typeof(Customer)),
                    SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheQueryShouldBeSortedByTheIdAscending()
            {
                var expected = SqlBuilder
                    .Select("*")
                    .From(typeof(Customer))
                    .OrderByAscending("Id")
                    .ToSqlQuery();

                Assert.Equal(expected, _sqlQuery);
            }
        }
    }
}
