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
    using System;
    using System.Collections.Generic;
    using Net.Http.WebApi.OData.Query.Expressions;

    internal static class BinaryOperatorKindExtensions
    {
        private static Dictionary<BinaryOperatorKind, string> operatorKindMap = new Dictionary<BinaryOperatorKind, string>
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
            string sqlOperator;

            if (operatorKindMap.TryGetValue(binaryOperatorKind, out sqlOperator))
            {
                return sqlOperator;
            }

            throw new NotImplementedException($"The operator '{binaryOperatorKind}' is not implemented by this service");
        }
    }
}