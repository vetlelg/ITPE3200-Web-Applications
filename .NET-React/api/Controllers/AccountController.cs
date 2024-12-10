using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ITPE3200ExamProject.Models;
using ITPE3200ExamProject.DAL;
using ITPE3200ExamProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ITPE3200ExamProject.ControllerHelper;

namespace ITPE3200ExamProject.Controllers;

[ApiController]
[Route("api/accounts")]
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {

        var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation("[AccountController] Logged in successfully");
            string accountId = AccountHelper.GetAccountId(User);
            return Ok(new { accountId });
        }
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user == null)
        {
            _logger.LogError("[AccountController] Account doesn't exist");
            return Unauthorized(new { message = "Account doesn't exist" });
        }
        else
        {
            _logger.LogError("[AccountController] Invalid password");
            return Unauthorized(new { message = "Invalid password" });
        }

    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto accountDto)
    {
        var user = new Account { UserName = accountDto.Email, Email = accountDto.Email };
        IdentityResult? result = await _accountRepository.Create(user, accountDto.Password);
        if (result == null)
        {
            return BadRequest(new { message = "Unexpected error occurred when creating account in database" });
        }

        if (result.Succeeded)
        {
            _logger.LogInformation("[AccountController] Account created successfully.");
            await Login(new LoginDto { Email = accountDto.Email, Password = accountDto.Password, RememberMe = true });
            return Ok(new { accountId = user.Id });
        }

        foreach (var error in result.Errors)
        {
            if (error.Code == "DuplicateEmail")
            {
                _logger.LogError($"[AccountController] Account with e-mail: {accountDto.Email} e-mail already exists");
                return BadRequest(new { message = "Account with e-mail already exists" });
            }
        }
        return BadRequest(new { message = "Failed to create account" });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");
        return Ok(new { message = "User logged out." });
    }

    [HttpGet("checkAuth")]
    public async Task<IActionResult> CheckAuth()
    {
        string? accountId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (accountId == null)
            return Ok(new { isAuthenticated = false });

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Ok(new { isAuthenticated = false });

        return Ok(new { isAuthenticated = true, accountId = user.Id, email = user.Email });
    }

    [HttpPut("{id}/email")]
    [Authorize]
    public async Task<IActionResult> UpdateEmail(string id, [FromBody] EmailDto emailDto)
    {
        var user = await _accountRepository.GetByAccountId(id);
        if (user == null)
        {
            _logger.LogError("[AccountController] Account not found");
            return NotFound(new { message = "Account not found." });
        }

        // Updates account details
        user.Email = emailDto.Email;
        user.UserName = emailDto.Email;
        IdentityResult? result = await _accountRepository.Update(user);

        if (result == null)
        {
            return BadRequest(new { message = "Unexpected error occurred when updating email in database" });
        }

        // Updates user session to use new user information
        await _signInManager.RefreshSignInAsync(user);

        if (result.Succeeded)
        {
            _logger.LogInformation("[AccountController] Email updated successfully");
            return Ok(new { message = "Email updated successfully" });
        }

        foreach (var error in result.Errors)
        {
            if (error.Code == "DuplicateEmail")
            {
                _logger.LogError($"[AccountController] Account with e-mail: {emailDto.Email} already exists");
                return BadRequest(new { message = "Account with e-mail already exists" });
            }
        }
        _logger.LogError("[AccountController] Failed to update email");
        return BadRequest(new { message = $"Failed to update email" });

    }

    [HttpPut("{id}/password")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword(string id, [FromBody] PasswordDto passwordDto)
    {
        var user = await _accountRepository.GetByAccountId(id);
        if (user == null)
        {
            _logger.LogError("[AccountController] Account not found");
            return NotFound(new { message = "Account not found." });
        }

        var result = await _userManager.ChangePasswordAsync(user, passwordDto.OldPassword, passwordDto.Password);
        if (result.Succeeded)
        {
            _logger.LogInformation("[AccountController] Password updated successfully.");
            return Ok(new { message = "Password updated successfully" });
        }
        _logger.LogError("[AccountController] Failed to update password");
        return BadRequest(new { message = $"Failed to update password" });
    }
}