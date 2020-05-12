// -----------------------------------------------------------------------
// <copyright file="BinaryOperatorKindExtensions.cs" company="Project Contributors">
// Copyright Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
using System.Collections.Generic;
using Net.Http.OData;
using Net.Http.OData.Query.Expressions;

namespace MicroLite.Extensions.WebApi.OData.Binders
{
    internal static class BinaryOperatorKindExtensions
    {
        private static readonly Dictionary<BinaryOperatorKind, string> s_operatorKindMap = new Dictionary<BinaryOperatorKind, string>
        {
            [BinaryOperatorKind.Add] = "+",
            [BinaryOperatorKind.And] = "AND",
            [BinaryOperatorKind.Divide] = "/",
            [BinaryOperatorKind.Equal] = "=",
            [BinaryOperatorKind.GreaterThan] = ">",
            [BinaryOperatorKind.GreaterThanOrEqual] = ">=",
            [BinaryOperatorKind.Has] = "=",
            [BinaryOperatorKind.LessThan] = "<",
            [BinaryOperatorKind.LessThanOrEqual] = "<=",
            [BinaryOperatorKind.Modulo] = "%",
            [BinaryOperatorKind.Multiply] = "*",
            [BinaryOperatorKind.NotEqual] = "<>",
            [BinaryOperatorKind.Or] = "OR",
            [BinaryOperatorKind.Subtract] = "-",
        };

        internal static string ToSqlOperator(this BinaryOperatorKind binaryOperatorKind)
        {
            if (s_operatorKindMap.TryGetValue(binaryOperatorKind, out string sqlOperator))
            {
                return sqlOperator;
            }

            throw ODataException.NotImplemented($"The operator '{binaryOperatorKind}' is not implemented by this service.", "$filter");
        }
    }
}
