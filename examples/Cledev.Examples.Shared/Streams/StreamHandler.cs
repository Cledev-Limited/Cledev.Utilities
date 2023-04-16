using System.Runtime.CompilerServices;
using Cledev.Core.Streams;

namespace Cledev.Examples.Shared.Streams;

public class StreamHandler : IStreamRequestHandler<StreamRequest, StreamResponse>
{
    public async IAsyncEnumerable<StreamResponse> Handle(StreamRequest request, [EnumeratorCancellation]CancellationToken cancellationToken)
    {
        yield return await Task.Run(() => new StreamResponse { Text = "Text 1" }, cancellationToken);
        yield return await Task.Run(() => new StreamResponse { Text = " and Text 2" }, cancellationToken);
        yield return await Task.Run(() => new StreamResponse { Text = " and Text 3" }, cancellationToken);
        yield return await Task.Run(() => new StreamResponse { Text = " and Text 4" }, cancellationToken);
        yield return await Task.Run(() => new StreamResponse { Text = " and Text 5" }, cancellationToken);
        yield return await Task.Run(() => new StreamResponse { Text = " and Text 6" }, cancellationToken);
        yield return await Task.Run(() => new StreamResponse { Text = " and Text 7" }, cancellationToken);
        yield return await Task.Run(() => new StreamResponse { Text = " and Text 8" }, cancellationToken);
        yield return await Task.Run(() => new StreamResponse { Text = " and Text 9" }, cancellationToken);
    }
}
