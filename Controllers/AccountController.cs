using Microsoft.AspNetCore.Mvc;
using StoredProcedureAPI.Models;

namespace StoredProcedureAPI.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Add your authentication logic here
                // For now, this is just a placeholder
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}