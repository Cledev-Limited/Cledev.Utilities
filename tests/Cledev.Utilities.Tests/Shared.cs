using Cledev.Core.Domain.Store.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Cledev.Utilities.Tests;

public static class Shared
{
    public static DbContextOptions<DomainDbContext> CreateContextOptions()
    {
        var builder = new DbContextOptionsBuilder<DomainDbContext>();
        builder.UseInMemoryDatabase("Cledev");
        return builder.Options;
    }

    public static IHttpContextAccessor CreateHttpContextAccessor()
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
        return mockHttpContextAccessor.Object;
    }
}
