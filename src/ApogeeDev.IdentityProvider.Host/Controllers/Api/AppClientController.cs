using System.Threading.Tasks;
using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApogeeDev.IdentityProvider.Host.Controllers.Api;

[ApiController]
[Route("api/app-client")]
[Authorize(
    AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
    Policy = "RequireAppManager")]
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
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> ClientDelete(string id)
    {
        mediator.Send(new AppClientDeleteRequest
        {
            ClientId = id,
        });

        return NoContent();
    }

    [HttpPost]
    [Route("")]
    public async Task<IActionResult> ClientAdd([FromBody] AppClientData model)
    {
        var response = await mediator.Send(new AppClientAddRequest
        {
            Data = model,
        });

        return Ok(response);
    }

    [HttpPost]
    [Route("{id}/client-secret")]
    public IActionResult CreateClientSecret(string id)
    {
        return Ok();
    }
}
