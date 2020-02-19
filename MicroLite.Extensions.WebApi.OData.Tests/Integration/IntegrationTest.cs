﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dependencies;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using Moq;

namespace MicroLite.Extensions.WebApi.OData.Tests.Integration
{
    public abstract class IntegrationTest : IDisposable
    {
        private HttpConfiguration _httpConfiguration;
        private HttpServer _httpServer;

        protected IntegrationTest()
        {
            _httpConfiguration = new HttpConfiguration();
            _httpConfiguration.DependencyResolver = new TestDependencyResolver(MockSession);
            _httpConfiguration.UseOData(entityDataModelBuilder =>
            {
                entityDataModelBuilder.RegisterEntitySet<Customer>("Customers", x => x.Id);
                entityDataModelBuilder.RegisterEntitySet<Invoice>("Invoices", x => x.Id);
            });

            _httpConfiguration.MapHttpAttributeRoutes();

            _httpServer = new HttpServer(_httpConfiguration);
            HttpClient = new HttpClient(_httpServer);
        }

        protected HttpClient HttpClient { get; }

        protected Mock<IAsyncSession> MockSession { get; } = new Mock<IAsyncSession>();

        public void Dispose()
        {
            HttpClient.Dispose();
            _httpServer.Dispose();
            _httpConfiguration.Dispose();
        }

        private class TestDependencyResolver : IDependencyResolver
        {
            private readonly Mock<IAsyncSession> _mockSession;

            public TestDependencyResolver(Mock<IAsyncSession> mockSession)
            {
                _mockSession = mockSession;
            }

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

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return Enumerable.Empty<object>();
            }
        }
    }
}