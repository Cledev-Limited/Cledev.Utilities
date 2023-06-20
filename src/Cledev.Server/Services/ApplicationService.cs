using Cledev.Core;
using Cledev.Core.Requests;
using Cledev.Server.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Cledev.Server.Services;

public interface IApplicationService
{
    Task<ActionResult> ProcessRequest<TRequest>(TRequest request, bool validateRequest = false, CancellationToken cancellationToken = default) where TRequest : IRequest;
    Task<ActionResult> ProcessRequest<TResponse>(IRequest<TResponse> request, bool validateRequest = false, CancellationToken cancellationToken = default);
}

public class ApplicationService : IApplicationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDispatcher _dispatcher;

    public ApplicationService(IServiceProvider serviceProvider, IDispatcher dispatcher)
    {
        _serviceProvider = serviceProvider;
        _dispatcher = dispatcher;
    }

    public async Task<ActionResult> ProcessRequest<TRequest>(TRequest request, bool validateRequest = false, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        if (validateRequest)
        {
            var validator = _serviceProvider.GetService<IValidator<TRequest>?>();
            if (validator is null)
            {
                throw new Exception("Request validator not found.");
            }

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsValid is false)
            {
                return validationResult.ToActionResult();
            }
        }
        
        var requestResult = await _dispatcher.Send(request, cancellationToken);

        requestResult.UpdateActivityIfNeeded();
        
        return requestResult.ToActionResult();
    }

    public async Task<ActionResult> ProcessRequest<TResponse>(IRequest<TResponse> request, bool validateRequest = false, CancellationToken cancellationToken = default)
    {
        if (validateRequest)
        {
            var validator = _serviceProvider.GetService<IValidator<IRequest<TResponse>>?>();
            if (validator is null)
            {
                throw new Exception("Request validator not found.");
            }
            
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.IsValid is false)
            {
                return validationResult.ToActionResult();
            }          
        }
        
        var requestResult = await _dispatcher.Send(request, cancellationToken);

        requestResult.UpdateActivityIfNeeded();
        
        return requestResult.ToActionResult();
    }
}
