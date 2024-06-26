using System.Text;
using System.Text.Encodings.Web;
using ecommerce_temp.Data;
using ecommerce_temp.Data.Models;
using ecommerce_temp.Areas.Account.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authorization;


namespace ecommerce_temp.Areas.Account.Controllers
{
    [Area("Account")]
    [Route("[area]/[action]")]
    [AllowAnonymous]
    public class RegisterController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<RegisterController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly ecommerce_tempContext _context;

        public RegisterController(
          UserManager<User> userManager,
          IUserStore<User> userStore,
          SignInManager<User> signInManager,
          ILogger<RegisterController> logger,
          IEmailSender emailSender,
          ecommerce_tempContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/DashBoard");
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Register",
                        new { userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // Create a new cart for the user
                    var cart = new Cart { UserId = user.Id };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToAction("RegisterConfirmation", new { email = model.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterConfirmation(string email, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Email"] = email;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string returnUrl = null)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(Register));
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load user with ID '{userId}'.");
            }
            var decodedCode = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, decodedCode);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl ?? Url.Content("~/"));
            }
            return View("Error");
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor.");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
