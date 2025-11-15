using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
using Microsoft.AspNetCore.Mvc;

namespace ApogeeDev.IdentityProvider.Host.Controllers;

[ApiController]
[Route("api/app-client")]
public class AppClientController : ControllerBase
{
    private readonly IMediator mediator;

    public AppClientController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Client()
    {
        var response = await mediator.Send(new AppClientListRequest());

        return Ok(response);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ClientDetails(string id)
    {
        var response = await mediator.Send(new AppClientListRequest
        {
            ClientId = id,
        });

        return Ok(response.FirstOrDefault());
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> ClientEdit(string id, [FromBody] AppClientData model)
    {
        return await Task.FromResult(NoContent());
    }

    [HttpPost]
    [Route("")]
    public IActionResult ClientAdd([FromBody] AppClientData model)
    {
        return Ok();
    }

    [HttpPost]
    [Route("{id}/client-secret")]
    public IActionResult CreateClientSecret(string id)
    {
        return Ok();
    }
}
