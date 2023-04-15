using Cledev.Core.Streams;

namespace Cledev.Examples.Shared.Streams;

public class StreamRequest : IStreamRequest<StreamResponse>
{
    public string? Text { get; set; }
}