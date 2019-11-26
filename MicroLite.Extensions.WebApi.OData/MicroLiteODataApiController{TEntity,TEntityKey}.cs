// -----------------------------------------------------------------------
// <copyright file="MicroLiteODataApiController{TEntity,TEntityKey}.cs" company="Project Contributors">
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
namespace MicroLite.Extensions.WebApi.OData
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using MicroLite.Builder;
    using MicroLite.Extensions.WebApi.OData.Binders;
    using MicroLite.Mapping;
    using Net.Http.WebApi.OData;
    using Net.Http.WebApi.OData.Query;
    using Net.Http.WebApi.OData.Query.Validators;

    /// <summary>
    /// A controller which adds support for OData.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
    public abstract class MicroLiteODataApiController<TEntity, TEntityKey> : MicroLiteApiController
        where TEntity : class, new()
    {
        private static readonly IObjectInfo EntityObjectInfo = Mapping.ObjectInfo.For(typeof(TEntity));

        private static SqlQuery entityCountQuery;

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteODataApiController{TEntity, TId}"/> class.
        /// </summary>
        /// <param name="session">The ISession for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an ISession via an IOC container.
        /// </remarks>
        protected MicroLiteODataApiController(IAsyncSession session)
            : base(session)
        {
            this.ValidationSettings = new ODataValidationSettings
            {
                AllowedArithmeticOperators = AllowedArithmeticOperators.All,
                AllowedFunctions = AllowedFunctions.Ceiling
                    | AllowedFunctions.Contains
                    | AllowedFunctions.Day
                    | AllowedFunctions.EndsWith
                    | AllowedFunctions.Floor
                    | AllowedFunctions.Month
                    | AllowedFunctions.Replace
                    | AllowedFunctions.Round
                    | AllowedFunctions.StartsWith
                    | AllowedFunctions.Substring
                    | AllowedFunctions.ToLower
                    | AllowedFunctions.ToUpper
                    | AllowedFunctions.Trim
                    | AllowedFunctions.Year,
                AllowedLogicalOperators = AllowedLogicalOperators.All & ~AllowedLogicalOperators.Has,
                AllowedQueryOptions = AllowedQueryOptions.Count
                    | AllowedQueryOptions.Filter
                    | AllowedQueryOptions.Format
                    | AllowedQueryOptions.OrderBy
                    | AllowedQueryOptions.Select
                    | AllowedQueryOptions.Skip
                    | AllowedQueryOptions.Top,
                MaxTop = 50,
            };
        }

        /// <summary>
        /// Gets the object information for the entity.
        /// </summary>
        protected IObjectInfo ObjectInfo => EntityObjectInfo;

        /// <summary>
        /// Gets or sets the Validation Settings for the OData query.
        /// </summary>
        protected ODataValidationSettings ValidationSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Creates the SQL query to count the number of entities in the Entity Set.
        /// </summary>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateCountEntitiesSqlQuery()
        {
            if (entityCountQuery is null)
            {
                entityCountQuery = SqlBuilder.Select()
                    .Count(this.ObjectInfo.TableInfo.IdentifierColumn.ColumnName)
                    .From(this.ObjectInfo.ForType)
                    .ToSqlQuery();
            }

            return entityCountQuery;
        }

        /// <summary>
        /// Creates the SQL query to query entities in the Entity Set based upon the values in the specified ODataQueryOptions.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateSelectEntitiesSqlQuery(ODataQueryOptions queryOptions)
            => queryOptions.CreateSqlQuery();

        /// <summary>
        /// Creates the SQL query to select an entity in the Entity Set based upon it's Entity Key.
        /// </summary>
        /// <param name="entityKey">The entity key.</param>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateSelectEntityByKeySqlQuery(TEntityKey entityKey)
            => SqlBuilder.Select("*")
                 .From(this.ObjectInfo.ForType)
                 .Where(this.ObjectInfo.TableInfo.IdentifierColumn.ColumnName).IsEqualTo(entityKey)
                 .ToSqlQuery();

        /// <summary>
        /// Creates the SQL query to select an individual property from an entity in the Entity Set based upon it's Entity Key.
        /// </summary>
        /// <param name="columnInfo">The column info.</param>
        /// <param name="entityKey">The entity key.</param>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateSelectPropertySqlQuery(ColumnInfo columnInfo, TEntityKey entityKey)
            => SqlBuilder.Select(columnInfo.ColumnName)
                .From(this.ObjectInfo.ForType)
                .Where(this.ObjectInfo.TableInfo.IdentifierColumn.ColumnName).IsEqualTo(entityKey)
                .ToSqlQuery();

        /// <summary>
        /// Deletes the <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <param name="entityKey">The Entity Key of the Entity to delete.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with
        /// 204 (No Content) if the entity is deleted successfully,
        /// or 404 (Not Found) if there is no entity with the specified Id.</returns>
        /// <remarks>Provides implementation for DELETE /odata/Entity(Key) <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#delete]]></remarks>
        protected virtual async Task<HttpResponseMessage> DeleteEntityResponseAsync(TEntityKey entityKey)
        {
            var deleted = await this.Session.Advanced.DeleteAsync(this.ObjectInfo.ForType, entityKey).ConfigureAwait(false);

            return this.Request.CreateODataResponse(deleted ? HttpStatusCode.NoContent : HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Gets the entity count response.
        /// </summary>
        /// <returns>The an <see cref="HttpResponseMessage"/> containing the entity count.</returns>
        /// <remarks>Provides implementation for GET /odata/Entity/$count</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It will be a Web API method")]
        protected virtual async Task<HttpResponseMessage> GetCountResponseAsync()
        {
            var sqlQuery = this.CreateCountEntitiesSqlQuery();

            var count = await this.Session.Advanced.ExecuteScalarAsync<long>(sqlQuery).ConfigureAwait(false);

            return this.Request.CreateODataResponse(count.ToString());
        }

        /// <summary>
        /// Gets the entity property result for the entity with the specified Entity Key and property name.
        /// </summary>
        /// <param name="entityKey">The Entity Key for the entity to retrieve.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        /// <remarks>Provides implementation for GET /odata/Entity(Key)/Property <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#property]]></remarks>
        protected virtual async Task<HttpResponseMessage> GetEntityPropertyResponseAsync(TEntityKey entityKey, string propertyName)
        {
            var entitySet = this.Request.ResolveEntitySet();
            var columnInfo = this.ObjectInfo.TableInfo.GetColumnInfoForProperty(propertyName);

            if (columnInfo is null)
            {
                return this.Request.CreateODataErrorResponse(HttpStatusCode.BadRequest, $"The type '{entitySet.EdmType.FullName}' does not contain a property named '{propertyName}'.");
            }

            var sqlQuery = this.CreateSelectPropertySqlQuery(columnInfo, entityKey);

            var result = await this.Session.SingleAsync<dynamic>(sqlQuery).ConfigureAwait(false);

            if (result is null)
            {
                return this.Request.CreateODataResponse(HttpStatusCode.NotFound, null);
            }

            var value = ((IDictionary<string, object>)result)[columnInfo.ColumnName];

            Uri context = this.Request.ResolveODataContextUri(entitySet, entityKey, propertyName);

            return this.Request.CreateODataResponse(HttpStatusCode.OK, new ODataResponseContent(context, value));
        }

        /// <summary>
        /// Gets the entity property value result for the entity with the specified Entity Key and property name.
        /// </summary>
        /// <param name="entityKey">The Entity Key for the entity to retrieve.</param>
        /// <param name="propertyName">The name of the property to retrieve the value of.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        /// <remarks>Provides implementation for GET /odata/Entity(Key)/Property/$value <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#propertyVal]]></remarks>
        protected virtual async Task<HttpResponseMessage> GetEntityPropertyValueResponseAsync(TEntityKey entityKey, string propertyName)
        {
            var entitySet = this.Request.ResolveEntitySet();
            var columnInfo = this.ObjectInfo.TableInfo.GetColumnInfoForProperty(propertyName);

            if (columnInfo is null)
            {
                return this.Request.CreateODataErrorResponse(HttpStatusCode.BadRequest, $"The type '{entitySet.EdmType.FullName}' does not contain a property named '{propertyName}'.");
            }

            var sqlQuery = this.CreateSelectPropertySqlQuery(columnInfo, entityKey);

            var result = await this.Session.SingleAsync<dynamic>(sqlQuery).ConfigureAwait(false);

            if (result is null)
            {
                return this.Request.CreateODataResponse(HttpStatusCode.NotFound, null);
            }

            var value = ((IDictionary<string, object>)result)[columnInfo.ColumnName]?.ToString();

            return this.Request.CreateODataResponse(value);
        }

        /// <summary>
        /// Gets the entity result for the entity with the specified Entity Key.
        /// </summary>
        /// <param name="entityKey">The Entity Key for the entity to retrieve.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        /// <remarks>Provides implementation for GET /odata/Entity(Key) <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#entityByID]]></remarks>
        protected virtual async Task<HttpResponseMessage> GetEntityResponseAsync(TEntityKey entityKey)
        {
            var sqlQuery = this.CreateSelectEntityByKeySqlQuery(entityKey);

            var entity = await this.Session.SingleAsync<dynamic>(sqlQuery).ConfigureAwait(false);

            if (entity is null)
            {
                return this.Request.CreateODataResponse(HttpStatusCode.NotFound, null);
            }

            var entitySet = this.Request.ResolveEntitySet();
            Uri context = this.Request.ResolveODataContextUri(entitySet, entityKey);

            var responseContent = (IDictionary<string, object>)entity;

            if (context != null)
            {
                responseContent["@odata.context"] = context;
            }

            return this.Request.CreateODataResponse(HttpStatusCode.OK, responseContent);
        }

        /// <summary>
        /// Gets the entity results based upon the OData query.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        /// <remarks>Provides implementation for GET /odata/Entity?$... <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#queryData]]></remarks>
        protected virtual async Task<HttpResponseMessage> GetEntityResponseAsync(ODataQueryOptions queryOptions)
        {
            if (queryOptions is null)
            {
                throw new ArgumentNullException(nameof(queryOptions));
            }

            queryOptions.Validate(this.ValidationSettings);

            var sqlQuery = this.CreateSelectEntitiesSqlQuery(queryOptions);

            var skip = queryOptions.Skip ?? 0;
            var top = queryOptions.Top ?? this.ValidationSettings.MaxTop;

            var paged = await this.Session.PagedAsync<dynamic>(sqlQuery, PagingOptions.SkipTake(skip, top)).ConfigureAwait(false);

            Uri context = this.Request.ResolveODataContextUri(queryOptions.EntitySet, queryOptions.Select);
            int? count = queryOptions.Count ? paged.TotalResults : default(int?);
            Uri nextLink = paged.MoreResultsAvailable ? queryOptions.NextLink(skip, paged.ResultsPerPage) : null;

            var responseContent = new ODataResponseContent(context, paged.Results, count, nextLink);

            var response = this.Request.CreateODataResponse(HttpStatusCode.OK, responseContent);

            if (queryOptions.Format != null)
            {
                response.Content.Headers.ContentType = queryOptions.Format.MediaTypeHeaderValue;
            }

            return response;
        }

        /// <summary>
        /// Creates a new <typeparamref name="TEntity"/> based upon the values in the specified entity.
        /// </summary>
        /// <param name="entity">The entity containing the values to be created.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result 201 (Created) if the entity is successfully created.</returns>
        /// <remarks>Provides implementation for POST /odata/Entity(Key) <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#create]]></remarks>
        protected virtual async Task<HttpResponseMessage> PostEntityResponseAsync(TEntity entity)
        {
            await this.Session.InsertAsync(entity).ConfigureAwait(false);

            var identifier = (TEntityKey)this.ObjectInfo.GetIdentifierValue(entity);

            var entitySet = this.Request.ResolveEntitySet();
            var response = this.Request.CreateODataResponse(HttpStatusCode.Created, entity);
            response.Headers.Location = this.Request.ResolveODataEntityUri(entitySet, identifier);

            return response;
        }

        /// <summary>
        /// Updates an existing <typeparamref name="TEntity"/> with the values in the specified entity.
        /// </summary>
        /// <param name="entityKey">The Entity Key of the Entity to update.</param>
        /// <param name="entity">The Entity containing the values to be updated.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result
        /// 404 (Not Found) if no entity was found with the specified Id to update,
        /// 304 (Not Modified) if there were no changes or
        /// 204 (NoContent) if the entity was updated successfully.</returns>
        /// <remarks>Provides implementation for PUT /odata/Entity(Key) <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#update]]></remarks>
        protected virtual async Task<HttpResponseMessage> PutEntityResponseAsync(TEntityKey entityKey, TEntity entity)
        {
            var existing = await this.Session.SingleAsync<TEntity>(entityKey).ConfigureAwait(false);

            if (existing is null)
            {
                return this.Request.CreateODataResponse(HttpStatusCode.NotFound);
            }

            this.ObjectInfo.SetIdentifierValue(entity, entityKey);

            var updated = await this.Session.UpdateAsync(entity).ConfigureAwait(false);

            return this.Request.CreateODataResponse(updated ? HttpStatusCode.NoContent : HttpStatusCode.NotModified);
        }
    }
}