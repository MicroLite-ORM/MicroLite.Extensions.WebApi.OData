// -----------------------------------------------------------------------
// <copyright file="ODataQueryOptionsExtensions.cs" company="Project Contributors">
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
using MicroLite.Builder.Syntax.Read;
using MicroLite.Mapping;
using Net.Http.OData.Query;

namespace MicroLite.Extensions.WebApi.OData.Binders
{
    internal static class ODataQueryOptionsExtensions
    {
        internal static SqlQuery CreateSqlQuery(this ODataQueryOptions queryOptions)
        {
            IObjectInfo objectInfo = ObjectInfo.For(queryOptions.EntitySet.EdmType.ClrType);

            IWhereOrOrderBy whereSqlBuilder = SelectBinder.BindSelect(queryOptions.Select, objectInfo);
            IOrderBy orderBySqlBuilder = FilterBinder.BindFilter(queryOptions.Filter, objectInfo, whereSqlBuilder);
            orderBySqlBuilder = OrderByBinder.BindOrderBy(queryOptions.OrderBy, objectInfo, orderBySqlBuilder);

            return orderBySqlBuilder.ToSqlQuery();
        }
    }
}
