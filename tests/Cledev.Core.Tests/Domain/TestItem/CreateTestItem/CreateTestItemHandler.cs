using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;

namespace Cledev.Core.Tests.Domain.TestItem.CreateTestItem;

public class CreateTestItemHandler(DomainDbContext dbContext) : IRequestHandler<CreateTestItem>
{
    public async Task<Result> Handle(CreateTestItem request, CancellationToken cancellationToken = default)
    {
        var testItem = new TestItem(request.Id, request.Name, request.Description);
        var result = await dbContext.SaveAggregate(testItem, expectedVersionNumber: 0, cancellationToken);
        return result;
    }
}
