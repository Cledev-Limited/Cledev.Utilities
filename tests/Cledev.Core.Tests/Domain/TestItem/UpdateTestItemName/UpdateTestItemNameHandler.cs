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
        var testItem = await _testDbContext.GetAggregate<TestItem>(request.Id, ReadMode.Strong);
        if (testItem.IsNotSuccess)
        {
            return testItem.Failure!;
        }
        var expectedVersionNumber = testItem.Value!.Version;
        testItem.Value!.UpdateName(request.Name);
        var result = await _testDbContext.SaveAggregate(testItem.Value!, expectedVersionNumber, cancellationToken);
        return result;
    }
}