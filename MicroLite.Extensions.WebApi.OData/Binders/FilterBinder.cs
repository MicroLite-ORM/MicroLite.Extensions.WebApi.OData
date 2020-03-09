// -----------------------------------------------------------------------
// <copyright file="FilterBinder.cs" company="Project Contributors">
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
using System.Collections.Generic;
using MicroLite.Builder;
using MicroLite.Builder.Syntax.Read;
using MicroLite.Characters;
using MicroLite.Mapping;
using Net.Http.OData;
using Net.Http.OData.Model;
using Net.Http.OData.Query;
using Net.Http.OData.Query.Binders;
using Net.Http.OData.Query.Expressions;

namespace MicroLite.Extensions.WebApi.OData.Binders
{
    /// <summary>
    /// The binder class which can append the $filter by query option.
    /// </summary>
    public sealed class FilterBinder : AbstractFilterBinder
    {
        private readonly IObjectInfo _objectInfo;
        private readonly RawWhereBuilder _predicateBuilder = new RawWhereBuilder();
        private readonly SqlCharacters _sqlCharacters = SqlCharacters.Current;

        private FilterBinder(IObjectInfo objectInfo) => _objectInfo = objectInfo;

        /// <summary>
        /// Binds the filter query option to the sql builder.
        /// </summary>
        /// <param name="filterQueryOption">The filter query.</param>
        /// <param name="objectInfo">The IObjectInfo for the type to bind the filter list for.</param>
        /// <param name="whereSqlBuilder">The select from SQL builder.</param>
        /// <returns>The SqlBuilder after the where clause has been added.</returns>
        public static IOrderBy BindFilter(FilterQueryOption filterQueryOption, IObjectInfo objectInfo, IWhereOrOrderBy whereSqlBuilder)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            if (whereSqlBuilder is null)
            {
                throw new ArgumentNullException(nameof(whereSqlBuilder));
            }

            if (filterQueryOption != null)
            {
                var filterBinder = new FilterBinder(objectInfo);
                filterBinder.Bind(filterQueryOption);
                filterBinder._predicateBuilder.ApplyTo(whereSqlBuilder);
            }

            return whereSqlBuilder;
        }

        /// <inheritdoc/>
        protected override void Bind(BinaryOperatorNode binaryOperatorNode)
        {
            if (binaryOperatorNode is null)
            {
                throw new ArgumentNullException(nameof(binaryOperatorNode));
            }

            _predicateBuilder.Append("(");

            Bind(binaryOperatorNode.Left);

            // ignore 'eq true' or 'eq false' for method calls
            if (!(binaryOperatorNode.Left.Kind == QueryNodeKind.FunctionCall
                && binaryOperatorNode.OperatorKind == BinaryOperatorKind.Equal
                && binaryOperatorNode.Right.Kind == QueryNodeKind.Constant
                && ((ConstantNode)binaryOperatorNode.Right).EdmType == EdmPrimitiveType.Boolean))
            {
                if (binaryOperatorNode.Right.Kind == QueryNodeKind.Constant
                    && ((ConstantNode)binaryOperatorNode.Right).EdmType is null)
                {
                    if (binaryOperatorNode.OperatorKind == BinaryOperatorKind.Equal)
                    {
                        _predicateBuilder.Append(" IS ");
                    }
                    else if (binaryOperatorNode.OperatorKind == BinaryOperatorKind.NotEqual)
                    {
                        _predicateBuilder.Append(" IS NOT ");
                    }
                }
                else
                {
                    _predicateBuilder.Append(" ").Append(binaryOperatorNode.OperatorKind.ToSqlOperator()).Append(" ");
                }

                Bind(binaryOperatorNode.Right);
            }

            _predicateBuilder.Append(")");
        }

        /// <inheritdoc/>
        protected override void Bind(ConstantNode constantNode)
        {
            if (constantNode is null)
            {
                throw new ArgumentNullException(nameof(constantNode));
            }

            if (constantNode.EdmType is null)
            {
                _predicateBuilder.Append("NULL");
            }
            else
            {
                _predicateBuilder.Append(_sqlCharacters.GetParameterName(0), constantNode.Value);
            }
        }

        /// <inheritdoc/>
        protected override void Bind(FunctionCallNode functionCallNode)
        {
            if (functionCallNode is null)
            {
                throw new ArgumentNullException(nameof(functionCallNode));
            }

            IReadOnlyList<QueryNode> parameters = functionCallNode.Parameters;

            switch (functionCallNode.Name)
            {
                // String functions
                case "substring":
                case "tolower":
                case "toupper":
                // Date functions
                case "day":
                case "month":
                case "year":
                // Math functions
                case "ceiling":
                case "floor":
                case "round":
                    string name = functionCallNode.Name.StartsWith("to", StringComparison.Ordinal)
                        ? functionCallNode.Name.Substring(2)
                        : functionCallNode.Name;

                    _predicateBuilder.Append(name.ToUpperInvariant()).Append("(");

                    for (int i = 0; i < parameters.Count; i++)
                    {
                        Bind(parameters[i]);

                        if (i < parameters.Count - 1)
                        {
                            _predicateBuilder.Append(", ");
                        }
                    }

                    _predicateBuilder.Append(")");
                    break;

                case "contains":
                    Bind(parameters[0]);
                    _predicateBuilder.Append(" LIKE ")
                        .Append(_sqlCharacters.GetParameterName(0), _sqlCharacters.LikeWildcard + ((ConstantNode)parameters[1]).Value + _sqlCharacters.LikeWildcard);
                    break;

                case "endswith":
                    Bind(parameters[0]);
                    _predicateBuilder.Append(" LIKE ")
                        .Append(_sqlCharacters.GetParameterName(0), _sqlCharacters.LikeWildcard + ((ConstantNode)parameters[1]).Value);
                    break;

                case "startswith":
                    Bind(parameters[0]);
                    _predicateBuilder.Append(" LIKE ")
                        .Append(_sqlCharacters.GetParameterName(0), ((ConstantNode)parameters[1]).Value + _sqlCharacters.LikeWildcard);
                    break;

                case "trim":
                    _predicateBuilder.Append("LTRIM(RTRIM(");
                    Bind(parameters[0]);
                    _predicateBuilder.Append("))");
                    break;

                default:
                    throw ODataException.NotImplemented($"The function '{functionCallNode.Name}' is not implemented by this service.");
            }
        }

        /// <inheritdoc/>
        protected override void Bind(PropertyAccessNode propertyAccessNode)
        {
            if (propertyAccessNode is null)
            {
                throw new ArgumentNullException(nameof(propertyAccessNode));
            }

            if (propertyAccessNode.PropertyPath.Next != null)
            {
                throw ODataException.NotImplemented("This service does not support nested property paths.");
            }

            ColumnInfo column = _objectInfo.TableInfo.GetColumnInfoForProperty(propertyAccessNode.PropertyPath.Property.Name);

            if (column is null)
            {
                throw ODataException.BadRequest(
                    $"The type '{_objectInfo.ForType.Name}' does not contain a property named '{propertyAccessNode.PropertyPath.Property.Name}'.");
            }

            _predicateBuilder.Append(column.ColumnName);
        }

        /// <inheritdoc/>
        protected override void Bind(UnaryOperatorNode unaryOperatorNode)
        {
            if (unaryOperatorNode is null)
            {
                throw new ArgumentNullException(nameof(unaryOperatorNode));
            }

            _predicateBuilder.Append(unaryOperatorNode.OperatorKind.ToSqlOperator()).Append(" ");
            Bind(unaryOperatorNode.Operand);
        }
    }
}
