using AuthService.DTOs;
using AuthService.Models;
using Common.Messaging;
using Common.Messaging.Events;
using Common.Messaging.Queues;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly IMessagePublisher _publisher;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration config, IMessagePublisher publisher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _publisher = publisher;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // Generate email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);

            var confirmationLink = $"https://localhost:7021/api/Auth/verify-email?token={encodedToken}&email={model.Email}";

            // Send event to RabbitMQ or any messaging system
            var message = new
            {
                user.Email,
                ConfirmationLink = confirmationLink
            };

            // Send to NotificationService (we’ll write the publisher shortly)
            await _publisher.PublishAsync(new UserRegisteredEvent
            {
                ConfirmationLink = confirmationLink,
                Email = user.Email
            }, RabbitMqQueues.User_Registered);

            return Ok("User registered. Please check your email to confirm.");
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound("User not found.");

            //var decodedToken = WebUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return Ok("Email confirmed successfully.");
            else
                return BadRequest("Invalid token.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            // ✅ Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Invalid email or password.");

            // ✅ Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized("Email is not confirmed. Please verify your email.");

            // ✅ Generate JWT token or session token
            //var token = await _jwtTokenService.GenerateTokenAsync(user); // assuming you're using JWT
            //return Ok(new { token });
            var token = new JwtSecurityToken(
                claims: new[] 
                {
                    new Claim(ClaimTypes.Name, user.UserName), 
                    new Claim("IsSubscribed", user.IsSubscribed.ToString()), 
                    new Claim("UserId",user.Id)
                },
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes( _config.GetSection("AppSettings")["JWTSecret"])), SecurityAlgorithms.HmacSha256));
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Invalid email address.");

            if (await _userManager.IsEmailConfirmedAsync(user))
                return BadRequest("Email is already confirmed.");

            // Generate a new email confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Create confirmation link
            var confirmationLink = Url.Action(
                "ConfirmEmail", // <-- Name of your confirmation endpoint
                "Auth",         // <-- Controller
                new { userId = user.Id, token = WebUtility.UrlEncode(token) },
                protocol: HttpContext.Request.Scheme
            );

            // Send to NotificationService (we’ll write the publisher shortly)
            await _publisher.PublishAsync(new UserRegisteredEvent
            {
                ConfirmationLink = confirmationLink,
                Email = user.Email
            }, RabbitMqQueues.User_Registered);

            return Ok("Confirmation link has been resent.");
        }



    }
}
