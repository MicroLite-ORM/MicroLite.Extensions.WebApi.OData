using System.Net;
using MicroLite.Extensions.WebApi.OData.Binders;
using Net.Http.WebApi.OData;
using Net.Http.WebApi.OData.Query.Expressions;
using Xunit;

namespace MicroLite.Extensions.WebApi.Tests.OData.Binders
{
    public class BinaryOperatorKindExtensionsTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsAndForBinaryOperatorKindAnd()
        {
            Assert.Equal("AND", BinaryOperatorKind.And.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsEqualsForBinaryOperatorKindEqual()
        {
            Assert.Equal("=", BinaryOperatorKind.Equal.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsForwardSlashForBinaryOperatorKindDivide()
        {
            Assert.Equal("/", BinaryOperatorKind.Divide.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsGreaterThanForBinaryOperatorKindGreaterThan()
        {
            Assert.Equal(">", BinaryOperatorKind.GreaterThan.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsGreaterThanOrEqualForBinaryOperatorKindGreaterThanOrEqual()
        {
            Assert.Equal(">=", BinaryOperatorKind.GreaterThanOrEqual.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsLessThanForBinaryOperatorKindLessThan()
        {
            Assert.Equal("<", BinaryOperatorKind.LessThan.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsLessThanOrEqualForBinaryOperatorKindLessThanOrEqual()
        {
            Assert.Equal("<=", BinaryOperatorKind.LessThanOrEqual.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsMinusForBinaryOperatorKindSubtract()
        {
            Assert.Equal("-", BinaryOperatorKind.Subtract.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsNotEqualForBinaryOperatorKindNotEqual()
        {
            Assert.Equal("<>", BinaryOperatorKind.NotEqual.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsOrForBinaryOperatorKindOr()
        {
            Assert.Equal("OR", BinaryOperatorKind.Or.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsPercentForBinaryOperatorKindModulo()
        {
            Assert.Equal("%", BinaryOperatorKind.Modulo.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsPlusForBinaryOperatorKindAdd()
        {
            Assert.Equal("+", BinaryOperatorKind.Add.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorReturnsStarForBinaryOperatorKindMultiply()
        {
            Assert.Equal("*", BinaryOperatorKind.Multiply.ToSqlOperator());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ToSqlOperatorThrowsODataExceptionForBinaryOperatorKindNone()
        {
            ODataException exception = Assert.Throws<ODataException>(() => BinaryOperatorKind.None.ToSqlOperator());

            Assert.Equal(HttpStatusCode.NotImplemented, exception.StatusCode);
            Assert.Equal("The operator 'None' is not implemented by this service", exception.Message);
        }
    }
}
