using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
        var response = await mediator.Send(new AppClientListRequest());

        var vm = new ClientListViewModel
        {
            Clients = response?.Select(c => new ClientListItem(c)).ToList()
                ?? new List<ClientListItem>(),
        };

        if (TempData.Get<ViewMessageState>(GetTempDataKey()) is ViewMessageState state)
        {
            vm.ViewState = state;
        }

        return View(vm);
    }
    [Route("Client/Edit/{id}")]
    public async Task<IActionResult> ClientEdit(string id)
    {
        var response = await mediator.Send(new AppClientListRequest
        {
            ClientId = id,
        });

        if (response.Count == 0)
        {
            TempData.Put(GetTempDataKey(), ViewMessageState.Error("Client not found"));
            return RedirectToAction("Client");
        }

        var vm = new ModifyClientViewModel(response.First());

        return View(vm);
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
    private string GetTempDataKey() => $"Client.{ViewMessageState.TempDataKey}";
}
