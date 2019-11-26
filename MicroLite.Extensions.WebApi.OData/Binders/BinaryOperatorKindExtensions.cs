// -----------------------------------------------------------------------
// <copyright file="BinaryOperatorKindExtensions.cs" company="Project Contributors">
// Copyright 2012 - 2019 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Extensions.WebApi.OData.Binders
{
    using System.Collections.Generic;
    using System.Net;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query.Expressions;

    internal static class BinaryOperatorKindExtensions
    {
        private static readonly Dictionary<BinaryOperatorKind, string> OperatorKindMap = new Dictionary<BinaryOperatorKind, string>
        {
            [BinaryOperatorKind.Add] = "+",
            [BinaryOperatorKind.And] = "AND",
            [BinaryOperatorKind.Divide] = "/",
            [BinaryOperatorKind.Equal] = "=",
            [BinaryOperatorKind.GreaterThan] = ">",
            [BinaryOperatorKind.GreaterThanOrEqual] = ">=",
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
            if (OperatorKindMap.TryGetValue(binaryOperatorKind, out string sqlOperator))
            {
                return sqlOperator;
            }

            throw new ODataException(HttpStatusCode.NotImplemented, $"The operator '{binaryOperatorKind}' is not implemented by this service");
        }
    }
}