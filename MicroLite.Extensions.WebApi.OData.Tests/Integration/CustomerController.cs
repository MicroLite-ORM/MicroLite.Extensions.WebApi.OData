using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MicroLite.Extensions.WebApi.OData.Tests.TestEntities;
using Net.Http.OData.Query;

namespace MicroLite.Extensions.WebApi.OData.Tests.Integration
{
    [RoutePrefix("odata")]
    public class CustomerController : MicroLiteODataApiController<Customer, int>
    {
        public CustomerController(ISession session)
            : base(session)
        {
        }

        public new ODataValidationSettings ValidationSettings => base.ValidationSettings;

        [HttpGet]
        [Route("Customers/$count")]
        public Task<IHttpActionResult> Count()
            => GetCountResponseAsync();

        [HttpDelete]
        [Route("Customers({id:int})")]
        public Task<IHttpActionResult> Delete(int id)
            => DeleteEntityResponseAsync(id);

        [HttpGet]
        [Route("Customers({id:int})")]
        public Task<IHttpActionResult> Get(int id)
            => GetEntityResponseAsync(id);

        [HttpGet]
        [Route("Customers")]
        public Task<IHttpActionResult> Get(ODataQueryOptions queryOptions)
            => GetEntityResponseAsync(queryOptions);

        [HttpGet]
        [Route("Customers({id:int})/{propertyName}")]
        public Task<IHttpActionResult> GetProperty(int id, string propertyName)
            => GetEntityPropertyResponseAsync(id, propertyName);

        [HttpGet]
        [Route("Customers({id:int})/{propertyName}/$value")]
        public Task<IHttpActionResult> GetPropertyValue(int id, string propertyName)
            => GetEntityPropertyValueResponseAsync(id, propertyName);

        [HttpPost]
        [Route("Customers")]
        public Task<HttpResponseMessage> Post(Customer entity)
            => PostEntityResponseAsync(entity);

        [HttpPut]
        [Route("Customers({id:int})")]
        public Task<IHttpActionResult> Put(int id, [FromBody]Customer entity)
            => PutEntityResponseAsync(id, entity);
    }
}
