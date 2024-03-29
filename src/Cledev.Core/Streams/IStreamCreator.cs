﻿namespace Cledev.Core.Streams;

public interface IStreamCreator
{
    IAsyncEnumerable<TResponse> Create<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default);
}