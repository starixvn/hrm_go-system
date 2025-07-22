namespace GO.Letstalk.WebAPI.Controllers;

using GO.Letstalk.WebAPI.Extentions;
using Microsoft.AspNetCore.Mvc;

public class WhoamiController : Controller
{
    public IActionResult Index()
    {
        return Content(WhoamiHelper.GetContent(HttpContext));
    }
}