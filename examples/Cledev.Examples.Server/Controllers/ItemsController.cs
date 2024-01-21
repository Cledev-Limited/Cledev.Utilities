using Cledev.Examples.Shared;
using Cledev.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cledev.Examples.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ItemsController(
    IApplicationService applicationService,
    ICreateItemValidationRules createItemValidationRules,
    IUpdateItemValidationRules updateItemValidationRules)
    : ControllerBase
{
    [ProducesResponseType(typeof(GetAllItemsResponse), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> Get() => 
        await applicationService.ProcessRequest(new GetAllItems());

    [ProducesResponseType(typeof(GetItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Get([FromRoute] Guid id) =>
        await applicationService.ProcessRequest(new GetItem(id));

    [ProducesResponseType(typeof(CreateItem), StatusCodes.Status200OK)]
    [HttpGet("create-item")]
    public async Task<ActionResult> GetCreateItem() =>
        await applicationService.ProcessRequest(new GetCreateItem());

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateItem command) =>
        await applicationService.ProcessRequest(command);

    [ProducesResponseType(typeof(UpdateItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("update-item/{id:guid}")]
    public async Task<ActionResult> GetUpdateItem([FromRoute] Guid id) =>
        await applicationService.ProcessRequest(new GetUpdateItem(id));

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [HttpPut]
    public async Task<ActionResult> Put([FromBody] UpdateItem command) =>
        await applicationService.ProcessRequest(command);

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id) =>
        await applicationService.ProcessRequest(new DeleteItem(id));

    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("is-name-unique")]
    public async Task<ActionResult> IsNameUnique([FromQuery] string name, [FromQuery] Guid? id)
    {
        var result = id is null
            ? await createItemValidationRules.IsItemNameUnique(name)
            : await updateItemValidationRules.IsItemNameUnique(id.Value, name);

        return Ok(result);
    }
}