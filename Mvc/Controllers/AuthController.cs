using Microsoft.AspNetCore.Mvc;

namespace Mvc.Controllers
{
  public class AuthController : Controller
  {
    public ActionResult AccessDenied()
    {
      return View();
    }

  }
}
