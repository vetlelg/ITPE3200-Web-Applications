using ITPE3200ExamProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ITPE3200ExamProject.Models;
using ITPE3200ExamProject.DAL;
using ITPE3200ExamProject.ControllerHelper;
using Microsoft.AspNetCore.Authorization;

namespace ITPE3200ExamProject.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<Account> _signInManager;
    private readonly IAccountRepository _accountRepository;
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<Account> _userManager;


    public AccountController(SignInManager<Account> signInManager, ILogger<AccountController> logger, IAccountRepository accountRepository, UserManager<Account> userManager)
    {
        _signInManager = signInManager;
        _logger = logger;
        _accountRepository = accountRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Manage() => View();

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdatePassword(PasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var accountId = AccountHelper.GetAccountId(User);
            var user = await _accountRepository.GetByAccountId(accountId);
            if (user == null)
            {
                _logger.LogError("[AccountController] Account not found");
                return NotFound("Account not found.");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("[AccountController] Password updated successfully.");
                ViewBag.PasswordSuccess = "Password changed successfully";
                return View("Manage");
            }
            _logger.LogError("[AccountController] Password was incorrect");
            ModelState.AddModelError(nameof(model.OldPassword), $"Password was incorrect");
            return View("Manage");
        }
        _logger.LogError("[AccountController] Invalid model state.");
        return BadRequest("Invalid model state.");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateEmail(EmailViewModel model)
    {
        if (ModelState.IsValid)
        {
            var accountId = AccountHelper.GetAccountId(User);
            var user = await _accountRepository.GetByAccountId(accountId);
            if (user == null)
            {
                _logger.LogError("[AccountController] Account not found");
                return NotFound("Account not found.");
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            var result = await _accountRepository.Update(user);
            if (result)
            {
                await _signInManager.RefreshSignInAsync(user);
                _logger.LogInformation("[AccountController] Email updated successfully.");
                ViewBag.EmailSuccess = "Email changed successfully";
                return View("Manage");
            }
            _logger.LogError("[AccountController] Failed to update email.");
            ModelState.AddModelError(nameof(model.Email), $"Account with e-mail already exists.");
            return View("Manage");
        }
        _logger.LogError("[AccountController] Invalid model state.");
        return BadRequest("Invalid model state.");
    }
    
    // Shows the login page
    [HttpGet]
    public IActionResult Login() => View();

    // Logs in the user
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("[AccountController] Logged in successfully.");
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user == null)
            {
                ModelState.AddModelError(nameof(model.Email), $"Account with e-mail: {model.Email} doesn't exist");
            }
            else
            {
                ModelState.AddModelError(nameof(model.Password), $"Password was incorrect");
            }
        }
        _logger.LogError("[AccountController] Failed to log in.");
        return View();
    }

    // Shows the register page
    [HttpGet]
    public IActionResult Register() => View();

    // Registers a new user
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new Account { UserName = model.Email, Email = model.Email };
            IdentityResult result = await _accountRepository.Create(user, model.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("[AccountController] Account created successfully.");
                return RedirectToAction("Login", "Account");
            }
            foreach (var error in result.Errors) 
            { 
                if (error.Code == "DuplicateEmail")
                {
                    ModelState.AddModelError(nameof(model.Email), $"Account with e-mail: {model.Email} e-mail already exists");
                }
            }
        }
        _logger.LogError("[AccountController] Failed to create account.");
        return View();
    }

    // Logs out the user
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return RedirectToAction("Index", "Home");
    }
}