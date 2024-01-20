using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Core.Tests.Data;

namespace Cledev.Core.Tests.Domain.TestItem.CreateTestItem;

public class CreateTestItemHandler : IRequestHandler<CreateTestItem>
{
    private readonly TestDbContext _testDbContext;

    public CreateTestItemHandler(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task<Result> Handle(CreateTestItem request, CancellationToken cancellationToken = default)
    {
        var testItem = new TestItem(request.Id, request.Name, request.Description);
        var result = await _testDbContext.SaveAggregate(testItem, expectedVersionNumber: 0, cancellationToken);
        return result;
    }
}
