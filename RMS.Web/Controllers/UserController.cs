using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using RMS.Data.Services;
using RMS.Web.Models;

namespace RMS.Web.Controllers;
public class UserController : Controller
{
    private readonly IUserService _svc;

    public UserController()
    {
        _svc = new UserServiceDb();
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login([Bind("Email,Password")]UserLoginViewModel m)
    {        
        // call service to Authenticate User
        var user = _svc.Authenticate(m.Email, m.Password);

        // if user not authenticated manually add validation errors for email and password
        if (user == null)
        {
            ModelState.AddModelError("Email", "Invalid Login Credentials");
            ModelState.AddModelError("Password", "Invalid Login Credentials");
            return View(m);
        }
        
        // authenticated so sign user in using cookie authentication to store principal
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            AuthBuilder.BuildClaimsPrincipal(user)
        );
        return RedirectToAction("Index","Home");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register([Bind("Name,Email,Password,PasswordConfirm,Role")]UserRegisterViewModel m)
    {
        // check if email address is already in use
        if (_svc.GetUserByEmail(m.Email) != null) {
            ModelState.AddModelError(nameof(m.Email),"This email address is already in use. Choose another");
        }

        // check validation
        if (!ModelState.IsValid)
        {
            return View(m);
        }

        // register user
        var user = _svc.Register(m.Name, m.Email, m.Password, m.Role);               
        
        // registration successful now redirect to login page
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    public IActionResult ErrorNotAuthorised()
    {   
        return RedirectToAction("Index", "Home");
    }

    public IActionResult ErrorNotAuthenticated()
    {
        return RedirectToAction("Login", "User"); 
    }        

}

