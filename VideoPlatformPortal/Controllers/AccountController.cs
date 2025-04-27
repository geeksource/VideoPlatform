using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VideoPlatformPortal.Services;
using VideoPlatformPortal.ViewModels;
using VideoPlatformPortal.Helper;
using System.Text.Json.Serialization;
using VideoPlatformPortal.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace VideoPlatformPortal.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ISubscriptionService _subService;

        public AccountController(IAuthService authService,ISubscriptionService subService)
        {
            _authService = authService;
            _subService = subService;
        }

        // GET: /Account/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            RegisterModel registerModel = new RegisterModel();
            return View(registerModel);
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                bool registrationSuccess = await _authService.RegisterAsync(model);

                if (registrationSuccess)
                {
                    // Optionally, you can redirect to the login page after successful registration
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Registration failed. Please try again.");
            }

            return View(model);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _authService.LoginAsync(model);
                var jwtToken =JsonSerializer.Deserialize<JwtToken>(response);

                if (jwtToken!=null && jwtToken.token != null)
                {
                    var jobj = Json(jwtToken);
                    var userid  =JwtHelper.GetUserIdFromToken(jwtToken.token);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email.ToString(),model.Email) ,
                        new Claim("JwtToken",jwtToken.token),
                        new Claim("UserId",userid)
                    };

                    var activeSubscriptions = await _subService.GetActivePlansAsync(userid);
                    if (activeSubscriptions != null && activeSubscriptions.Count > 0)
                    {
                        string subscriptions = string.Join(",", activeSubscriptions.Select(s => s.Name).ToList());
                        claims.Add(new Claim("ActiveSubscriptions", subscriptions));
                    }

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                    // Store the token in session or cookies
                    HttpContext.Session.SetString("JwtToken", jwtToken.token);
                    return RedirectToAction("Index", "Home"); // Redirect to subscription page
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(model);
        }
    }
}
