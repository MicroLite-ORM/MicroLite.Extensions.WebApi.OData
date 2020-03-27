using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dependencies;
using MicroLite.Extensions.WebApi.OData.Tests.TestEntities;
using Moq;
using Net.Http.OData;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MicroLite.Extensions.WebApi.OData.Tests.Integration
{
    public abstract class IntegrationTest : IDisposable
    {
        private readonly HttpConfiguration _httpConfiguration;
        private readonly HttpServer _httpServer;

        protected IntegrationTest()
        {
            _httpConfiguration = new HttpConfiguration
            {
                DependencyResolver = new TestDependencyResolver(MockSession)
            };

            _httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            _httpConfiguration.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            _httpConfiguration.UseOData(entityDataModelBuilder =>
            {
                entityDataModelBuilder
                    .RegisterEntitySet<Customer>("Customers", x => x.Id)
                    .RegisterEntitySet<Invoice>("Invoices", x => x.Id);
            });

            _httpConfiguration.MapHttpAttributeRoutes();

            _httpServer = new HttpServer(_httpConfiguration);

            HttpClient = new HttpClient(_httpServer);
            HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpClient.DefaultRequestHeaders.Add(ODataRequestHeaderNames.ODataVersion, "4.0");
            HttpClient.DefaultRequestHeaders.Add(ODataRequestHeaderNames.ODataMaxVersion, "4.0");
        }

        protected HttpClient HttpClient { get; }

        protected Mock<ISession> MockSession { get; } = new Mock<ISession>();

        public void Dispose()
        {
            HttpClient.Dispose();

            _httpServer.Dispose();
            _httpConfiguration.Dispose();
        }

        private class TestDependencyResolver : IDependencyResolver
        {
            private readonly Mock<ISession> _mockSession;

            public TestDependencyResolver(Mock<ISession> mockSession) => _mockSession = mockSession;

            public IDependencyScope BeginScope() => this;

            public void Dispose()
            {
            }

            public object GetService(Type serviceType)
            {
                if (serviceType == typeof(CustomerController))
                {
                    return new CustomerController(_mockSession.Object);
                }

                return null;
            }

            public IEnumerable<object> GetServices(Type serviceType) => Enumerable.Empty<object>();
        }
    }
}
