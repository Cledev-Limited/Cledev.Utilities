using Cledev.Examples.Shared;
using Cledev.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cledev.Examples.Server.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IControllerService _controllerService;
    private readonly ICreateItemValidationRules _createItemValidationRules;
    private readonly IUpdateItemValidationRules _updateItemValidationRules;

    public ItemsController(IControllerService controllerService, ICreateItemValidationRules createItemValidationRules, IUpdateItemValidationRules updateItemValidationRules)
    {
        _controllerService = controllerService;
        _createItemValidationRules = createItemValidationRules;
        _updateItemValidationRules = updateItemValidationRules;
    }

    [ProducesResponseType(typeof(GetAllItemsResponse), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult> Get() => 
        await _controllerService.ProcessQuery(new GetAllItems());

    [ProducesResponseType(typeof(GetItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult> Get([FromRoute] Guid id) =>
        await _controllerService.ProcessQuery(new GetItem(id));

    [ProducesResponseType(typeof(CreateItem), StatusCodes.Status200OK)]
    [HttpGet("create-item")]
    public async Task<ActionResult> GetCreateItem() =>
        await _controllerService.ProcessQuery(new GetCreateItem());

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateItem command) =>
        await _controllerService.ProcessCommand(command);

    [ProducesResponseType(typeof(UpdateItem), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("update-item/{id:guid}")]
    public async Task<ActionResult> GetUpdateItem([FromRoute] Guid id) =>
        await _controllerService.ProcessQuery(new GetUpdateItem(id));

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [HttpPut]
    public async Task<ActionResult> Put([FromBody] UpdateItem command) =>
        await _controllerService.ProcessCommand(command);

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid id) =>
        await _controllerService.ProcessCommand(new DeleteItem(id));

    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [HttpGet("is-name-unique")]
    public async Task<ActionResult> IsNameUnique([FromQuery] string name, [FromQuery] Guid? id)
    {
        var result = id is null
            ? await _createItemValidationRules.IsItemNameUnique(name)
            : await _updateItemValidationRules.IsItemNameUnique(id.Value, name);

        return Ok(result);
    }
}