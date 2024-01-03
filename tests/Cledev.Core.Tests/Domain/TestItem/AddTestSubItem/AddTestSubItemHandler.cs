using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Core.Tests.Data;

namespace Cledev.Core.Tests.Domain.TestItem.AddTestSubItem;

public class AddTestSubItemHandler : IRequestHandler<AddTestSubItem>
{
    private readonly TestDbContext _testDbContext;

    public AddTestSubItemHandler(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task<Result> Handle(AddTestSubItem request, CancellationToken cancellationToken = default)
    {
        var testItem = await _testDbContext.GetAggregate<TestItem>(request.Id, ReadMode.Strong);
        if (testItem.IsNotSuccess)
        {
            return testItem.Failure!;
        }
        var expectedVersionNumber = testItem.Value!.Version;
        testItem.Value!.AddSubItem(request.SubItemName);
        var result = await _testDbContext.SaveAggregate(testItem.Value!, expectedVersionNumber, cancellationToken);
        return result;
    }
}