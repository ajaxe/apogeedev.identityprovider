using Microsoft.AspNetCore.Mvc;

namespace ApogeeDev.IdentityProvider.Host.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}