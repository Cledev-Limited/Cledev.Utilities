using Cledev.Core.Domain.Store.EF;
using Cledev.Core.Requests;
using Cledev.Core.Results;

namespace Cledev.Core.Tests.Domain.TestItem.UpdateTestItemName;

public class UpdateTestItemNameHandler(DomainDbContext dbContext) : IRequestHandler<UpdateTestItemName>
{
    public async Task<Result> Handle(UpdateTestItemName request, CancellationToken cancellationToken = default)
    {
        var aggregate = await dbContext.GetAggregate<TestItem>(request.Id, cancellationToken: cancellationToken);
        if (aggregate.IsNotSuccess)
        {
            return aggregate.Failure!;
        }
        var testItem = aggregate.Value!;
        
        var expectedVersionNumber = testItem.Version;
        testItem.UpdateName(request.Name);
        var result = await dbContext.SaveAggregate(testItem, expectedVersionNumber, cancellationToken);
        return result;
    }
}
