using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
using Microsoft.AspNetCore.Mvc;

namespace ApogeeDev.IdentityProvider.Host.Controllers;

[Controller]
[Route("Manage")]
public class ManageController : Controller
{
    private readonly IMediator mediator;

    public ManageController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [Route("Client")]
    public async Task<IActionResult> Client()
    {
        ClientListViewModel vm = await mediator.Send(new AppClientListRequest());
        return View(vm);
    }
    [Route("Client/Edit/{id}")]
    public IActionResult ClientEdit(string id)
    {
        return View();
    }
    [Route("Client/Add")]
    public IActionResult ClientAdd()
    {
        return View();
    }
    [HttpPost]
    [Route("Client/Register")]
    public IActionResult ClientRegister(ModifyClientViewModel model)
    {
        return RedirectToAction("Client");
    }
}
