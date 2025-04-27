using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VideoPlatformPortal.Helper;
using VideoPlatformPortal.Services;

namespace VideoPlatformPortal.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        public async Task<IActionResult> Index()
        {
            var plans = await _subscriptionService.GetPlansAsync();
            return View(plans);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(string planId)
        {
            var userId = User.Claims.Where(c => c.Type == "UserId").FirstOrDefault();
            var userEmail = User.Claims.Where(c =>c.Type ==  ClaimTypes.Email.ToString()).FirstOrDefault();


            //var checkoutUrl = await _subscriptionService.GeneratePaymentLinkAsync(planId, User.Identity.Name);
            var result = await _subscriptionService.GeneratePaymentLinkAsync(userId?.Value, userEmail?.Value, planId);
            return View();
        }
        public async Task<IActionResult> Success()
        {
            //Lets check if there are any active subscription
            var activeSubscriptions = await _subscriptionService.GetActivePlansAsync(User.GetUserId());
            if(activeSubscriptions!=null && activeSubscriptions.Count > 0)
            {
                string subscriptions = string.Join(",", activeSubscriptions.Select(s => s.Name).ToList());
                //User.UpdateClaims("ActiveSubscriptions",subscriptions);
                User.UpdateClaims("ActiveSubscriptions", subscriptions, Request.HttpContext); // Pass HttpContext to update it

                var identity = new ClaimsIdentity(User.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
            var req = Request.HttpContext;
            return Redirect("/Home");
        }
        public ActionResult cancel()
        {
            return Redirect("/Home");
        }
    }
}
