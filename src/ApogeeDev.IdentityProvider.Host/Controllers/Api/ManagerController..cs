using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApogeeDev.IdentityProvider.Host.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(
    AuthenticationSchemes = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
public class ManagerController : ControllerBase
{
    [HttpGet]
    [Route("check-authorization")]
    public IActionResult CheckAuthorization()
    {
        if(User.HasClaim(c => c.Type == CustomClaimTypes.Common.AppManager))
            return Ok();
        return Forbid();
    }
}
