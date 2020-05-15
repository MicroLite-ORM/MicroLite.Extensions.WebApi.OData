using System.Net;
using MicroLite.Extensions.WebApi.OData.Binders;
using Net.Http.OData;
using Net.Http.OData.Query.Expressions;
using Xunit;

namespace MicroLite.Extensions.WebApi.OData.Tests.Binders
{
    public class UnaryOperatorKindExtensionsTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsNotForUnaryOperatorKindNot()
            => Assert.Equal("NOT", UnaryOperatorKind.Not.ToSqlOperator());

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorThrowsODataExceptionForBinaryOperatorKindNone()
        {
            ODataException exception = Assert.Throws<ODataException>(() => ((UnaryOperatorKind)(-1)).ToSqlOperator());

            Assert.Equal(HttpStatusCode.NotImplemented, exception.StatusCode);
            Assert.Equal("The operator '-1' is not implemented by this service", exception.Message);
            Assert.Equal("$filter", exception.Target);
        }
    }
}
