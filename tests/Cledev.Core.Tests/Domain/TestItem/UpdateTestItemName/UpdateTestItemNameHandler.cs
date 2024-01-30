using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Core.Tests.Data;

namespace Cledev.Core.Tests.Domain.TestItem.UpdateTestItemName;

public class UpdateTestItemNameHandler(TestDbContext testDbContext) : IRequestHandler<UpdateTestItemName>
{
    public async Task<Result> Handle(UpdateTestItemName request, CancellationToken cancellationToken = default)
    {
        var aggregate = await testDbContext.GetAggregate<TestItem>(request.Id, ReadMode.Weak, cancellationToken: cancellationToken);
        if (aggregate.IsNotSuccess)
        {
            return aggregate.Failure!;
        }
        var testItem = aggregate.Value!;
        
        var expectedVersionNumber = testItem.Version;
        testItem.UpdateName(request.Name);
        var result = await testDbContext.SaveAggregate(testItem, expectedVersionNumber, cancellationToken);
        return result;
    }
}
