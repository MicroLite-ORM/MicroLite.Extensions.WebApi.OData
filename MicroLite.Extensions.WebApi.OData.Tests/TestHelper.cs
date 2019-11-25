namespace MicroLite.Extensions.WebApi.Tests.OData
{
    using System.Web.Http;
    using Net.Http.WebApi.OData.Model;
    using TestEntities;

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