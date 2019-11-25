namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    using System.Net;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query.Expressions;
    using Xunit;

    public class UnaryOperatorKindExtensionsTests
    {
        [Fact]
        public void ToSqlOperatorReturnsNotForUnaryOperatorKindNot()
        {
            Assert.Equal("NOT", UnaryOperatorKind.Not.ToSqlOperator());
        }

        [Fact]
        public void ToSqlOperatorThrowsODataExceptionForBinaryOperatorKindNone()
        {
            var exception = Assert.Throws<ODataException>(() => ((UnaryOperatorKind)(-1)).ToSqlOperator());

            Assert.Equal(HttpStatusCode.NotImplemented, exception.StatusCode);
            Assert.Equal("The operator '-1' is not implemented by this service", exception.Message);
        }
    }
}