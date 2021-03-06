﻿// -----------------------------------------------------------------------
// <copyright file="TableInfoExtensions.cs" company="Project Contributors">
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
using MicroLite.Mapping;

namespace MicroLite.Extensions.WebApi.OData
{
    internal static class TableInfoExtensions
    {
        internal static ColumnInfo GetColumnInfoForProperty(this TableInfo tableInfo, string propertyName)
        {
            for (int i = 0; i < tableInfo.Columns.Count; i++)
            {
                ColumnInfo column = tableInfo.Columns[i];

                if (column.PropertyInfo.Name.Equals(propertyName, StringComparison.Ordinal))
                {
                    return column;
                }
            }

            return null;
        }
    }
}
