using System.Web.Http;
using MicroLite.Extensions.WebApi.Tests.OData.TestEntities;
using Net.Http.WebApi.OData.Model;

namespace MicroLite.Extensions.WebApi.Tests.OData
{
    internal static class TestHelper
    {
        internal static void EnsureEDM()
        {
            if (EntityDataModel.Current is null)
            {
                var httpConfiguration = new HttpConfiguration();
                httpConfiguration.UseOData(entityDataModelBuilder =>
                {
                    entityDataModelBuilder.RegisterEntitySet<Customer>("Customers", x => x.Id);
                    entityDataModelBuilder.RegisterEntitySet<Invoice>("Invoices", x => x.Id);
                });
            }
        }
    }
}
