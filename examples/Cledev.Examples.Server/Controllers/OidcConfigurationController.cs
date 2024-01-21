using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;

namespace Cledev.Examples.Server.Controllers;

public class OidcConfigurationController(
    IClientRequestParametersProvider clientRequestParametersProvider,
    ILogger<OidcConfigurationController> logger)
    : Controller
{
    private readonly ILogger<OidcConfigurationController> _logger = logger;

    public IClientRequestParametersProvider ClientRequestParametersProvider { get; } = clientRequestParametersProvider;

    [HttpGet("_configuration/{clientId}")]
    public IActionResult GetClientRequestParameters([FromRoute] string clientId)
    {
        var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
        return Ok(parameters);
    }
}