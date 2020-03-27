using MicroLite.Builder;
using MicroLite.Extensions.WebApi.OData.Binders;
using MicroLite.Extensions.WebApi.OData.Tests.TestEntities;
using Moq;
using Net.Http.OData.Model;
using Net.Http.OData.Query;
using Xunit;

namespace MicroLite.Extensions.WebApi.OData.Tests.Binders
{
    public class ODataQueryOptionExtensionsTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void CreateSqlQueryBindsSelectThenAddsFilterAndOrderBy()
        {
            TestHelper.EnsureEDM();

            var option = new ODataQueryOptions(
                "?$select=Forename,Surname&$filter=Forename eq 'John'&$orderby=Surname",
                EntityDataModel.Current.EntitySets["Customers"],
                Mock.Of<IODataQueryOptionsValidator>());

            SqlQuery sqlQuery = option.CreateSqlQuery();

            var expected = SqlBuilder.Select("Forename", "Surname").From(typeof(Customer)).Where("(Forename = ?)", "John").OrderByAscending("Surname").ToSqlQuery();

            Assert.Equal(expected, sqlQuery);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void CreateSqlQueryBindsSelectWildcardThenAddsFilterAndOrderBy()
        {
            TestHelper.EnsureEDM();

            var option = new ODataQueryOptions(
                "?$filter=Forename eq 'John'&$orderby=Surname",
                EntityDataModel.Current.EntitySets["Customers"],
                Mock.Of<IODataQueryOptionsValidator>());

            SqlQuery sqlQuery = option.CreateSqlQuery();

            var expected = SqlBuilder.Select("*").From(typeof(Customer)).Where("(Forename = ?)", "John").OrderByAscending("Surname").ToSqlQuery();

            Assert.Equal(expected, sqlQuery);
        }
    }
}
