using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Gumo.Web.Controllers;

public class AccountController(SignInManager<IdentityUser> signInManager) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (signInManager.IsSignedIn(User))
            return RedirectToAction("Orders", "Admin");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: true);

        if (result.Succeeded)
            return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl) : RedirectToAction("Orders", "Admin");

        ViewBag.ReturnUrl = returnUrl;
        ViewBag.Error = result.IsLockedOut
            ? "Too many failed attempts. Try again later."
            : "Invalid email or password.";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}