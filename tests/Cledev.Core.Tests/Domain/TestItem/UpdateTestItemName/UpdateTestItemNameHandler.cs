using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;
using Cledev.Core.Tests.Data;

namespace Cledev.Core.Tests.Domain.TestItem.UpdateTestItemName;

public class UpdateTestItemNameHandler : IRequestHandler<UpdateTestItemName>
{
    private readonly TestDbContext _testDbContext;

    public UpdateTestItemNameHandler(TestDbContext testDbContext)
    {
        _testDbContext = testDbContext;
    }

    public async Task<Result> Handle(UpdateTestItemName request, CancellationToken cancellationToken = default)
    {
        var aggregate = await _testDbContext.GetAggregate<TestItem>(request.Id, ReadMode.Strong);
        if (aggregate.IsNotSuccess)
        {
            return aggregate.Failure!;
        }
        var testItem = aggregate.Value!;
        
        var expectedVersionNumber = testItem.Version;
        testItem.UpdateName(request.Name);
        var result = await _testDbContext.SaveAggregate(testItem, expectedVersionNumber, cancellationToken);
        return result;
    }
}