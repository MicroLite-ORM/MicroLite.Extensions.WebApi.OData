using System.Web.Http;
using MicroLite.Extensions.WebApi.OData.Tests.TestEntities;
using Net.Http.OData.Model;

namespace MicroLite.Extensions.WebApi.OData.Tests
{
    internal static class TestHelper
    {
        internal static void EnsureEDM()
        {
            if (EntityDataModel.Current is null)
            {
                var httpConfiguration = new HttpConfiguration();
                UseOData(httpConfiguration);
            }
        }

        internal static void UseOData(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.UseOData(entityDataModelBuilder =>
            {
                entityDataModelBuilder.RegisterEntitySet<Customer>("Customers", x => x.Id)
                    .RegisterEntitySet<Invoice>("Invoices", x => x.Id)
                    .RegisterEntitySet<User>("Users", x => x.Username);
            });
        }
    }
}
