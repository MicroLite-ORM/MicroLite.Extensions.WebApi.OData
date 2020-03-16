// -----------------------------------------------------------------------
// <copyright file="MicroLiteODataApiController{TEntity,TEntityKey}.cs" company="Project Contributors">
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
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MicroLite.Builder;
using MicroLite.Extensions.WebApi.OData.Binders;
using MicroLite.Infrastructure;
using MicroLite.Mapping;
using Net.Http.OData;
using Net.Http.OData.Model;
using Net.Http.OData.Query;
using Net.Http.WebApi.OData;

namespace MicroLite.Extensions.WebApi.OData
{
    /// <summary>
    /// A controller which adds support for OData.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
    public abstract class MicroLiteODataApiController<TEntity, TEntityKey> : ODataController, IHaveSession
        where TEntity : class, new()
    {
        private static readonly IObjectInfo s_entityObjectInfo = Mapping.ObjectInfo.For(typeof(TEntity));

#pragma warning disable S2743 // Static fields should not be used in generic types
        private static SqlQuery s_entityCountQuery;
#pragma warning restore S2743 // Static fields should not be used in generic types

        /// <summary>
        /// Initialises a new instance of the <see cref="MicroLiteODataApiController{TEntity, TEntityKey}"/> class.
        /// </summary>
        /// <param name="session">The ISession for the current HTTP request.</param>
        /// <remarks>
        /// This constructor allows for an inheriting class to easily inject an ISession via an IOC container.
        /// </remarks>
        protected MicroLiteODataApiController(ISession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));

            ValidationSettings = new ODataValidationSettings
            {
                AllowedArithmeticOperators = AllowedArithmeticOperators.All,
                AllowedFunctions =
                    //// String functions
                    AllowedFunctions.Contains |
                    AllowedFunctions.EndsWith |
                    AllowedFunctions.StartsWith |
                    AllowedFunctions.Substring |
                    AllowedFunctions.ToLower |
                    AllowedFunctions.ToUpper |
                    AllowedFunctions.Trim |
                    //// Date functions
                    AllowedFunctions.Day |
                    AllowedFunctions.Month |
                    AllowedFunctions.Year |
                    //// Math functions
                    AllowedFunctions.Ceiling |
                    AllowedFunctions.Floor |
                    AllowedFunctions.Round,
                AllowedLogicalOperators = AllowedLogicalOperators.All & ~AllowedLogicalOperators.Has,
                AllowedQueryOptions =
                    AllowedQueryOptions.Count |
                    AllowedQueryOptions.Filter |
                    AllowedQueryOptions.Format |
                    AllowedQueryOptions.OrderBy |
                    AllowedQueryOptions.Select |
                    AllowedQueryOptions.Skip |
                    AllowedQueryOptions.Top,
                MaxTop = 50,
            };
        }

        /// <summary>
        /// Gets the <see cref="ISession"/> for the current HTTP request.
        /// </summary>
        public ISession Session { get; }

        /// <summary>
        /// Gets the object information for the entity.
        /// </summary>
        protected IObjectInfo ObjectInfo => s_entityObjectInfo;

        /// <summary>
        /// Gets or sets the Validation Settings for the OData query.
        /// </summary>
        protected ODataValidationSettings ValidationSettings { get; set; }

        /// <summary>
        /// Creates the SQL query to count the number of entities in the Entity Set.
        /// </summary>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateCountEntitiesSqlQuery()
        {
            EnsureEntityCountQuery();

            return s_entityCountQuery;
        }

        /// <summary>
        /// Creates the SQL query to query entities in the Entity Set based upon the values in the specified ODataQueryOptions.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateSelectEntitiesSqlQuery(ODataQueryOptions queryOptions)
        {
            if (queryOptions is null)
            {
                throw new ArgumentNullException(nameof(queryOptions));
            }

            return queryOptions.CreateSqlQuery();
        }

        /// <summary>
        /// Creates the SQL query to select an entity in the Entity Set based upon it's Entity Key.
        /// </summary>
        /// <param name="entityKey">The entity key.</param>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateSelectEntityByKeySqlQuery(TEntityKey entityKey)
            => SqlBuilder.Select("*")
                 .From(ObjectInfo.ForType)
                 .Where(ObjectInfo.TableInfo.IdentifierColumn.ColumnName).IsEqualTo(entityKey)
                 .ToSqlQuery();

        /// <summary>
        /// Creates the SQL query to select an individual property from an entity in the Entity Set based upon it's Entity Key.
        /// </summary>
        /// <param name="columnInfo">The column info.</param>
        /// <param name="entityKey">The entity key.</param>
        /// <returns>The SqlQuery to execute.</returns>
        protected virtual SqlQuery CreateSelectPropertySqlQuery(ColumnInfo columnInfo, TEntityKey entityKey)
        {
            if (columnInfo is null)
            {
                throw new ArgumentNullException(nameof(columnInfo));
            }

            return SqlBuilder.Select(columnInfo.ColumnName)
                .From(ObjectInfo.ForType)
                .Where(ObjectInfo.TableInfo.IdentifierColumn.ColumnName).IsEqualTo(entityKey)
                .ToSqlQuery();
        }

        /// <summary>
        /// Deletes the <typeparamref name="TEntity"/> with the specified id.
        /// </summary>
        /// <param name="entityKey">The Entity Key of the Entity to delete.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with
        /// 204 (No Content) if the entity is deleted successfully,
        /// or 404 (Not Found) if there is no entity with the specified Id.</returns>
        /// <remarks>Provides implementation for 'DELETE /odata/Entity(Key)'. <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#delete]]></remarks>
        protected virtual async Task<IHttpActionResult> DeleteEntityResponseAsync(TEntityKey entityKey)
        {
            bool deleted = await Session.Advanced.DeleteAsync(ObjectInfo.ForType, entityKey).ConfigureAwait(false);

            if (deleted)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return NotFound();
        }

        /// <summary>
        /// Gets the entity count response.
        /// </summary>
        /// <returns>The an <see cref="HttpResponseMessage"/> containing the entity count.</returns>
        /// <remarks>Provides implementation for 'GET /odata/Entity/$count'.</remarks>
        protected virtual async Task<IHttpActionResult> GetCountResponseAsync()
        {
            SqlQuery sqlQuery = CreateCountEntitiesSqlQuery();

            long count = await Session.Advanced.ExecuteScalarAsync<long>(sqlQuery).ConfigureAwait(false);

            return Content(count.ToString(CultureInfo.InvariantCulture), "text/plain", Encoding.UTF8);
        }

        /// <summary>
        /// Gets the entity property result for the entity with the specified Entity Key and property name.
        /// </summary>
        /// <param name="entityKey">The Entity Key for the entity to retrieve.</param>
        /// <param name="propertyName">The name of the property to retrieve.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        /// <remarks>Provides implementation for 'GET /odata/Entity(Key)/Property'. <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#property]]></remarks>
        protected virtual async Task<IHttpActionResult> GetEntityPropertyResponseAsync(TEntityKey entityKey, string propertyName)
        {
            EntitySet entitySet = Request.ODataEntitySet();
            ColumnInfo columnInfo = ObjectInfo.TableInfo.GetColumnInfoForProperty(propertyName);

            if (columnInfo is null)
            {
                return ODataError(HttpStatusCode.BadRequest, ODataErrorContent.Create(400, $"The type '{entitySet.EdmType.FullName}' does not contain a property named '{propertyName}'."));
            }

            SqlQuery sqlQuery = CreateSelectPropertySqlQuery(columnInfo, entityKey);

            dynamic result = await Session.SingleAsync<dynamic>(sqlQuery).ConfigureAwait(false);

            if (result is null)
            {
                return NotFound();
            }

            object value = ((IDictionary<string, object>)result)[columnInfo.ColumnName];

            string odataContext = Request.ODataContext(entitySet, entityKey, propertyName);

            return Ok(new ODataResponseContent { Context = odataContext, Value = value });
        }

        /// <summary>
        /// Gets the entity property value result for the entity with the specified Entity Key and property name.
        /// </summary>
        /// <param name="entityKey">The Entity Key for the entity to retrieve.</param>
        /// <param name="propertyName">The name of the property to retrieve the value of.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        /// <remarks>Provides implementation for 'GET /odata/Entity(Key)/Property/$value'. <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#propertyVal]]></remarks>
        protected virtual async Task<IHttpActionResult> GetEntityPropertyValueResponseAsync(TEntityKey entityKey, string propertyName)
        {
            EntitySet entitySet = Request.ODataEntitySet();
            ColumnInfo columnInfo = ObjectInfo.TableInfo.GetColumnInfoForProperty(propertyName);

            if (columnInfo is null)
            {
                return ODataError(HttpStatusCode.BadRequest, ODataErrorContent.Create(400, $"The type '{entitySet.EdmType.FullName}' does not contain a property named '{propertyName}'."));
            }

            SqlQuery sqlQuery = CreateSelectPropertySqlQuery(columnInfo, entityKey);

            dynamic result = await Session.SingleAsync<dynamic>(sqlQuery).ConfigureAwait(false);

            if (result is null)
            {
                return NotFound();
            }

            string value = ((IDictionary<string, object>)result)[columnInfo.ColumnName]?.ToString();

            return Content(value, "text/plain", Encoding.UTF8);
        }

        /// <summary>
        /// Gets the entity result for the entity with the specified Entity Key.
        /// </summary>
        /// <param name="entityKey">The Entity Key for the entity to retrieve.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        /// <remarks>Provides implementation for 'GET /odata/Entity(Key)'. <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#entityByID]]></remarks>
        protected virtual async Task<IHttpActionResult> GetEntityResponseAsync(TEntityKey entityKey)
        {
            SqlQuery sqlQuery = CreateSelectEntityByKeySqlQuery(entityKey);

            dynamic entity = await Session.SingleAsync<dynamic>(sqlQuery).ConfigureAwait(false);

            if (entity is null)
            {
                return NotFound();
            }

            EntitySet entitySet = Request.ODataEntitySet();
            string odataContext = Request.ODataContext<TEntityKey>(entitySet);

            var value = (IDictionary<string, object>)entity;

            if (odataContext != null)
            {
                value["@odata.context"] = odataContext;
            }

            return Ok(value);
        }

        /// <summary>
        /// Gets the entity results based upon the OData query.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result.</returns>
        /// <remarks>Provides implementation for GET /odata/Entity?$... <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#queryData]]></remarks>
        protected virtual Task<IHttpActionResult> GetEntityResponseAsync(ODataQueryOptions queryOptions)
        {
            if (queryOptions is null)
            {
                throw new ArgumentNullException(nameof(queryOptions));
            }

            return GetEntityResponseAsyncImpl(queryOptions);
        }

        /// <summary>
        /// Creates a new <typeparamref name="TEntity"/> based upon the values in the specified entity.
        /// </summary>
        /// <param name="entity">The entity containing the values to be created.</param>
        /// <returns>The an <see cref="HttpResponseMessage"/> with the execution result 201 (Created) if the entity is successfully created.</returns>
        /// <remarks>Provides implementation for 'POST /odata/Entity(Key)'. <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#create]]></remarks>
        protected virtual async Task<HttpResponseMessage> PostEntityResponseAsync(TEntity entity)
        {
            await Session.InsertAsync(entity).ConfigureAwait(false);

            var identifier = (TEntityKey)ObjectInfo.GetIdentifierValue(entity);

            EntitySet entitySet = Request.ODataEntitySet();
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, entity);
            response.Headers.Location = new Uri(Request.ODataId(entitySet, identifier));

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
        /// <remarks>Provides implementation for 'PUT /odata/Entity(Key)'. <![CDATA[http://www.odata.org/getting-started/basic-tutorial/#update]]></remarks>
        protected virtual async Task<IHttpActionResult> PutEntityResponseAsync(TEntityKey entityKey, TEntity entity)
        {
            TEntity existing = await Session.SingleAsync<TEntity>(entityKey).ConfigureAwait(false);

            if (existing is null)
            {
                return NotFound();
            }

            ObjectInfo.SetIdentifierValue(entity, entityKey);

            bool updated = await Session.UpdateAsync(entity).ConfigureAwait(false);

            if (updated)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return StatusCode(HttpStatusCode.NotModified);
        }

        private static void EnsureEntityCountQuery()
        {
            if (s_entityCountQuery is null)
            {
                s_entityCountQuery = SqlBuilder.Select()
                    .Count(s_entityObjectInfo.TableInfo.IdentifierColumn.ColumnName)
                    .From(s_entityObjectInfo.ForType)
                    .ToSqlQuery();
            }
        }

        private async Task<IHttpActionResult> GetEntityResponseAsyncImpl(ODataQueryOptions queryOptions)
        {
            queryOptions.Validate(ValidationSettings);

            SqlQuery sqlQuery = CreateSelectEntitiesSqlQuery(queryOptions);

            int skip = queryOptions.Skip ?? 0;
            int top = queryOptions.Top ?? ValidationSettings.MaxTop;

            PagedResult<dynamic> paged = await Session.PagedAsync<dynamic>(sqlQuery, PagingOptions.SkipTake(skip, top)).ConfigureAwait(false);

            string odataContext = Request.ODataContext(queryOptions.EntitySet, queryOptions.Select);
            int? count = queryOptions.Count ? paged.TotalResults : default(int?);
            string nextLink = paged.MoreResultsAvailable ? Request.ODataNextLink(queryOptions, skip, paged.ResultsPerPage) : null;

            return Ok(new ODataResponseContent { Context = odataContext, Count = count, NextLink = nextLink, Value = paged.Results });
        }
    }
}
