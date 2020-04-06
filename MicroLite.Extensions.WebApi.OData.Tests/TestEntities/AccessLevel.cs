using System;

namespace MicroLite.Extensions.WebApi.OData.Tests.TestEntities
{
    [Flags]
    public enum AccessLevel
    {
        None = 0,
        Read = 1,
        Write = 2,
        Delete = 4
    }
}
