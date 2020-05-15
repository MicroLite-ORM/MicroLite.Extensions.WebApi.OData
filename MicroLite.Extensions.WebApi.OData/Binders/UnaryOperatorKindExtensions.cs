// -----------------------------------------------------------------------
// <copyright file="UnaryOperatorKindExtensions.cs" company="Project Contributors">
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
using Net.Http.OData;
using Net.Http.OData.Query.Expressions;

namespace MicroLite.Extensions.WebApi.OData.Binders
{
    internal static class UnaryOperatorKindExtensions
    {
        internal static string ToSqlOperator(this UnaryOperatorKind unaryOperatorKind)
        {
            switch (unaryOperatorKind)
            {
                case UnaryOperatorKind.Not:
                    return "NOT";

                default:
                    throw ODataException.NotImplemented($"The operator '{unaryOperatorKind}' is not implemented by this service", "$filter");
            }
        }
    }
}
