using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;

namespace Cledev.Core.Tests.Domain.TestItem.AddTestSubItem;

public class AddTestSubItemHandler(DomainDbContext dbContext) : IRequestHandler<AddTestSubItem>
{
    public async Task<Result> Handle(AddTestSubItem request, CancellationToken cancellationToken = default)
    {
        var aggregate = await dbContext.GetAggregate<TestItem>(request.Id, ReadMode.Strong, cancellationToken: cancellationToken);
        if (aggregate.IsNotSuccess)
        {
            return aggregate.Failure!;
        }
        var testItem = aggregate.Value!;
        
        var expectedVersionNumber = testItem.Version;
        testItem.AddSubItem(request.SubItemName);
        var result = await dbContext.SaveAggregate(testItem, expectedVersionNumber, cancellationToken);
        return result;
    }
}
