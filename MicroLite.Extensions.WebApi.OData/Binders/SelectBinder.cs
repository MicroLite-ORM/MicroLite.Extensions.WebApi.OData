// -----------------------------------------------------------------------
// <copyright file="SelectBinder.cs" company="Project Contributors">
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
using System;
using MicroLite.Builder;
using MicroLite.Builder.Syntax.Read;
using MicroLite.Mapping;
using Net.Http.WebApi.OData.Model;
using Net.Http.WebApi.OData.Query;
using Net.Http.WebApi.OData.Query.Binders;

namespace MicroLite.Extensions.WebApi.OData.Binders
{
    /// <summary>
    /// The binder class which can append the $select query option.
    /// </summary>
    public sealed class SelectBinder : AbstractSelectExpandBinder
    {
        private readonly string[] _columnNames;
        private readonly IObjectInfo _objectInfo;
        private int _columnCount;

        private SelectBinder(IObjectInfo objectInfo, string[] columnNames)
        {
            _objectInfo = objectInfo;
            _columnNames = columnNames;
        }

        /// <summary>
        /// Binds the select query option to the SqlBuilder.
        /// </summary>
        /// <param name="selectQueryOption">The select query option.</param>
        /// <param name="objectInfo">The IObjectInfo for the type to bind the select list for.</param>
        /// <returns>The SqlBuilder after the select and from clauses have been added.</returns>
        public static IWhereOrOrderBy BindSelect(SelectExpandQueryOption selectQueryOption, IObjectInfo objectInfo)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            if (selectQueryOption is null)
            {
                return SqlBuilder.Select("*").From(objectInfo.ForType);
            }

            string[] columnNames = new string[selectQueryOption.Properties.Count];

            var selectBinder = new SelectBinder(objectInfo, columnNames);
            selectBinder.Bind(selectQueryOption);

            return SqlBuilder.Select(columnNames).From(objectInfo.ForType);
        }

        /// <inheritdoc/>
        protected override void Bind(EdmProperty edmProperty)
        {
            if (edmProperty is null)
            {
                throw new ArgumentNullException(nameof(edmProperty));
            }

            ColumnInfo column = _objectInfo.TableInfo.GetColumnInfoForProperty(edmProperty.Name);

            _columnNames[_columnCount++] = column.ColumnName;
        }
    }
}
