using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

using RMS.Data.Entities;
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
    public IActionResult Register([Bind("Name,Email,Password,PasswordConfirm")]UserRegisterViewModel m)
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

        // register user — role is always guest for self-registration
        var user = _svc.Register(m.Name, m.Email, m.Password, Role.guest);               
        
        // registration successful now redirect to login page
        return RedirectToAction(nameof(Login));
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    // GET /User/Index — admin and owner: list all users
    [HttpGet]
    [Authorize(Roles = "admin,owner")]
    public IActionResult Index()
    {
        var users = _svc.GetAllUsers();
        var vms = users.Select(UserViewModel.FromUser).ToList();
        return View(vms);
    }

    // GET /User/EditRole/{id} — admin and owner: change a user's role
    [HttpGet]
    [Authorize(Roles = "admin,owner")]
    public IActionResult EditRole(int id)
    {
        var user = _svc.GetUserById(id);
        if (user is null)
        {
            return RedirectToAction(nameof(Index));
        }
        return View(UserViewModel.FromUser(user));
    }

    // POST /User/EditRole/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "admin,owner")]
    public IActionResult EditRole(int id, UserViewModel vm)
    {
        var updated = _svc.UpdateUserRole(id, vm.Role);
        if (updated is not null)
        {
            TempData["Alert.Message"] = $"Role updated for {updated.Name}.";
            TempData["Alert.Type"] = "success";
        }
        return RedirectToAction(nameof(Index));
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

