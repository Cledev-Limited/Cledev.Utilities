using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Core.Tests.Data;

namespace Cledev.Core.Tests.Domain.TestItem.AddTestSubItem;

public class AddTestSubItemHandler(TestDbContext testDbContext) : IRequestHandler<AddTestSubItem>
{
    public async Task<Result> Handle(AddTestSubItem request, CancellationToken cancellationToken = default)
    {
        var aggregate = await testDbContext.GetAggregate<TestItem>(request.Id, ReadMode.Strong, cancellationToken: cancellationToken);
        if (aggregate.IsNotSuccess)
        {
            return aggregate.Failure!;
        }
        var testItem = aggregate.Value!;
        
        var expectedVersionNumber = testItem.Version;
        testItem.AddSubItem(request.SubItemName);
        var result = await testDbContext.SaveAggregate(testItem, expectedVersionNumber, cancellationToken);
        return result;
    }
}
