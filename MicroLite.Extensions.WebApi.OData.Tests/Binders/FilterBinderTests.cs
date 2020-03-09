using System;
using System.Net;
using MicroLite.Builder;
using MicroLite.Extensions.WebApi.OData.Binders;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using MicroLite.Mapping;
using Moq;
using Net.Http.OData;
using Net.Http.OData.Model;
using Net.Http.OData.Query;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    public class FilterBinderTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void BindFilterThrowsODataExceptionForUnspportedFunctionName()
        {
            TestHelper.EnsureEDM();

            var queryOptions = new ODataQueryOptions(
                "?$filter=indexof(Name, 'ayes') eq 1",
                EntityDataModel.Current.EntitySets["Customers"],
                Mock.Of<IODataQueryOptionsValidator>());

            ODataException exception = Assert.Throws<ODataException>(
                () => FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))));

            Assert.Equal("The function 'indexof' is not implemented by this service.", exception.Message);
            Assert.Equal(HttpStatusCode.NotImplemented, exception.StatusCode);
        }

        public class WhenCallingApplyToWithAComplexQuery
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingApplyToWithAComplexQuery()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Created ge 2013-05-01 and Created le 2013-06-12 and Reference eq 'A0113334' and startswith(Name, 'Hayes') eq true",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(new DateTime(2013, 5, 1), _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFourthQueryValue()
            {
                Assert.Equal("Hayes%", _sqlQuery.Arguments[3].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 6, 12), _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheThirdQueryValue()
            {
                Assert.Equal("A0113334", _sqlQuery.Arguments[2].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("((((Created >= ?) AND (Created <= ?)) AND (Reference = ?)) AND (Name LIKE ?))", new DateTime(2013, 5, 1), new DateTime(2013, 6, 12), "A0113334", "Hayes%")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe4ArgumentValues()
            {
                Assert.Equal(4, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingApplyToWithAGroupedFunctionAndFunction
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingApplyToWithAGroupedFunctionAndFunction()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=(endswith(Name, 'son') and endswith(Name, 'nes'))",
                    EntityDataModel.Current.EntitySets["Customers"],
                Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("%son", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal("%nes", _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name LIKE ? AND Name LIKE ?)", "%son", "%nes")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingApplyToWithAGroupedFunctionOrFunction
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingApplyToWithAGroupedFunctionOrFunction()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=(endswith(Name, 'son') or endswith(Name, 'nes'))",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("%son", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal("%nes", _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name LIKE ? OR Name LIKE ?)", "%son", "%nes")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanAndLessThan
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanAndLessThan()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Name eq 'Fred Bloggs' and Created gt 2013-04-01 and Created lt 2013-04-30",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("Fred Bloggs", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheThirdQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), _sqlQuery.Arguments[2].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(((Name = ?) AND (Created > ?)) AND (Created < ?))", "Fred Bloggs", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe3ArgumentValues()
            {
                Assert.Equal(3, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanOrLessThan
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithAPropertyEqualsAndGreaterThanOrLessThan()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Name eq 'Fred Bloggs' and Created gt 2013-04-01 or Created lt 2013-04-30",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("Fred Bloggs", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheThirdQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), _sqlQuery.Arguments[2].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(((Name = ?) AND (Created > ?)) OR (Created < ?))", "Fred Bloggs", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe3ArgumentValues()
            {
                Assert.Equal(3, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithAPropertyGreaterThanAndLessThan
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithAPropertyGreaterThanAndLessThan()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Created gt 2013-04-01 and Created lt 2013-04-30",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("((Created > ?) AND (Created < ?))", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithAPropertyGreaterThanOrLessThan
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithAPropertyGreaterThanOrLessThan()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Created gt 2013-04-01 or Created lt 2013-04-30",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 30), _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("((Created > ?) OR (Created < ?))", new DateTime(2013, 4, 1), new DateTime(2013, 4, 30))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyCeiling
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyCeiling()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=ceiling(Id) eq 32",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(32, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(CEILING(Id) = ?)", 32)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyContains
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyContains()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=contains(Name, 'Bloggs')",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("%Bloggs%", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name LIKE ?", "%Bloggs%")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyDay
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyDay()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=day(DateOfBirth) eq 22",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(22, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(DAY(DateOfBirth) = ?)", 22)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWith
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWith()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=endswith(Name, 'Bloggs')",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("%Bloggs", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name LIKE ?", "%Bloggs")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWithEqTrue
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEndsWithEqTrue()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=endswith(Name, 'Bloggs') eq true",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("%Bloggs", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name LIKE ?)", "%Bloggs")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEqualEnum
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEqualEnum()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Status eq MicroLite.Extensions.WebApi.Tests.OData.TestEntities.CustomerStatus'Active'",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(CustomerStatus.Active, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(CustomerStatusId = ?)", CustomerStatus.Active)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEqualNull
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEqualNull()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Name eq null",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name").IsNull()
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBeNoArgumentValues()
            {
                Assert.Equal(0, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyEqualString
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyEqualString()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Name eq 'Fred Bloggs'",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred Bloggs", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name = ?)", "Fred Bloggs")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyFloor
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyFloor()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=floor(Id) eq 32",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(32, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(FLOOR(Id) = ?)", 32)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyGreaterThan
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyGreaterThan()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Created gt 2013-04-01",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Created > ?)", new DateTime(2013, 4, 1))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyGreaterThanOrEqual
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyGreaterThanOrEqual()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Created ge 2013-04-01",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Created >= ?)", new DateTime(2013, 4, 1))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyLessThan
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyLessThan()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Created lt 2013-04-01",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Created < ?)", new DateTime(2013, 4, 1))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyLessThanOrEqual
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyLessThanOrEqual()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Created le 2013-04-01",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal(new DateTime(2013, 4, 1), _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Created <= ?)", new DateTime(2013, 4, 1))
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyMonth
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyMonth()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=month(DateOfBirth) eq 6",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(6, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(MONTH(DateOfBirth) = ?)", 6)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyNotEqual
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyNotEqual()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Name ne 'Fred Bloggs'",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred Bloggs", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name <> ?)", "Fred Bloggs")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyNotEqualNull
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyNotEqualNull()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=Name ne null",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name").IsNotNull()
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBeNoArgumentValues()
            {
                Assert.Equal(0, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyRound
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyRound()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=round(Id) eq 32",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(32, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(ROUND(Id) = ?)", 32)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyStartsWith
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyStartsWith()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=startswith(Name, 'Fred')",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred%", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("Name LIKE ?", "Fred%")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyStartsWithEqTrue
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyStartsWithEqTrue()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=startswith(Name, 'Fred') eq true",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred%", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(Name LIKE ?)", "Fred%")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertySubStringWithStartAndLength
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertySubStringWithStartAndLength()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=substring(Name, 1, 2) eq 'oh'",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ArgumentOneShouldBeTheValueToBeLength()
            {
                Assert.Equal(2, _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ArgumentTwoShouldBeTheValueToFind()
            {
                Assert.Equal("oh", _sqlQuery.Arguments[2].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ArgumentZeroShouldBeTheValueToBeStartIndex()
            {
                Assert.Equal(1, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(SUBSTRING(Name, ?, ?) = ?)", 1, 2, "oh")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe3ArgumentValue()
            {
                Assert.Equal(3, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertySubStringWithStartOnly
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertySubStringWithStartOnly()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=substring(Name, 1) eq 'ohnSmith'",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ArgumentOneShouldBeTheValueToFind()
            {
                Assert.Equal("ohnSmith", _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ArgumentZeroShouldBeTheValueToBeStartIndex()
            {
                Assert.Equal(1, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(SUBSTRING(Name, ?) = ?)", 1, "ohnSmith")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValue()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyToLower
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyToLower()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=tolower(Name) eq 'fred bloggs'",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("fred bloggs", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(LOWER(Name) = ?)", "fred bloggs")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyToUpper
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyToUpper()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=toupper(Name) eq 'FRED BLOGGS'",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("FRED BLOGGS", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(UPPER(Name) = ?)", "FRED BLOGGS")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyTrim
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyTrim()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=trim(Name) eq 'FRED BLOGGS'",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal("FRED BLOGGS", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(LTRIM(RTRIM(Name)) = ?)", "FRED BLOGGS")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithASinglePropertyYear
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithASinglePropertyYear()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=year(DateOfBirth) eq 1971",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(1971, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("(YEAR(DateOfBirth) = ?)", 1971)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithNotSinglePropertyEqual
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithNotSinglePropertyEqual()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "?$filter=not Name eq 'Fred Bloggs'",
                    EntityDataModel.Current.EntitySets["Customers"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Customer)), SqlBuilder.Select("*").From(typeof(Customer))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheQueryValue()
            {
                Assert.Equal("Fred Bloggs", _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Customer))
                    .Where("NOT (Name = ?)", "Fred Bloggs")
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe1ArgumentValue()
            {
                Assert.Equal(1, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithPropertyAddValueEquals
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertyAddValueEquals()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "Invoices?$filter=Quantity add 10 eq 15",
                    EntityDataModel.Current.EntitySets["Invoices"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity + ?) = ?)", 10, 15)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithPropertyDivideValueEquals
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertyDivideValueEquals()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "Invoices?$filter=Quantity div 10 eq 15",
                    EntityDataModel.Current.EntitySets["Invoices"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity / ?) = ?)", 10, 15)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithPropertyModuloValueEquals
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertyModuloValueEquals()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "Invoices?$filter=Quantity mod 10 eq 15",
                    EntityDataModel.Current.EntitySets["Invoices"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity % ?) = ?)", 10, 15)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithPropertyMultiplyValueEquals
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertyMultiplyValueEquals()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "Invoices?$filter=Quantity mul 10 eq 15",
                    EntityDataModel.Current.EntitySets["Invoices"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity * ?) = ?)", 10, 15)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }

        public class WhenCallingBindFilterQueryOptionWithPropertySubtractValueEquals
        {
            private readonly SqlQuery _sqlQuery;

            public WhenCallingBindFilterQueryOptionWithPropertySubtractValueEquals()
            {
                TestHelper.EnsureEDM();

                var queryOptions = new ODataQueryOptions(
                    "Invoices?$filter=Quantity sub 10 eq 15",
                    EntityDataModel.Current.EntitySets["Invoices"],
                    Mock.Of<IODataQueryOptionsValidator>());

                _sqlQuery = FilterBinder.BindFilter(queryOptions.Filter, ObjectInfo.For(typeof(Invoice)), SqlBuilder.Select("*").From(typeof(Invoice))).ToSqlQuery();
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheFirstQueryValue()
            {
                Assert.Equal(10, _sqlQuery.Arguments[0].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheArgumentsShouldContainTheSecondQueryValue()
            {
                Assert.Equal(15, _sqlQuery.Arguments[1].Value);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void TheCommandTextShouldContainTheWhereClause()
            {
                string expected = SqlBuilder.Select("*")
                    .From(typeof(Invoice))
                    .Where("((Quantity - ?) = ?)", 10, 15)
                    .ToSqlQuery()
                    .CommandText;

                Assert.Equal(expected, _sqlQuery.CommandText);
            }

            [Fact]
            [Trait("Category", "Unit")]
            public void ThereShouldBe2ArgumentValues()
            {
                Assert.Equal(2, _sqlQuery.Arguments.Count);
            }
        }
    }
}
