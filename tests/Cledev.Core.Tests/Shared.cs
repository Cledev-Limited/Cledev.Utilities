using Cledev.Core.Domain.Store.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Cledev.Core.Tests;

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
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        httpContextAccessor.HttpContext.Returns(context);
        return httpContextAccessor;
    }
}
