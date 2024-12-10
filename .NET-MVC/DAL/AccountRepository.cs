using Microsoft.EntityFrameworkCore;
using ITPE3200ExamProject.Models;
using Microsoft.AspNetCore.Identity;
using ITPE3200ExamProject.ViewModels;
using System.Linq;

namespace ITPE3200ExamProject.DAL;

public class AccountRepository : IAccountRepository
{
    // UserManager handles database operations for accounts instead of dbcontext
    // It automatically handles hashing and salting of passwords for example
    private readonly UserManager<Account> _userManager;
    private readonly ILogger<AccountRepository> _logger;
    public AccountRepository(ILogger<AccountRepository> logger, UserManager<Account> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<IEnumerable<Account>?> GetAllAccounts()
    {
        try
        {
            return await _userManager.Users.ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[PointRepository] Failed to get all accounts, Error: {e}", e.Message);
            return null;
        }
    }
    public async Task<Account?> GetByAccountId(string id)
    {
        try
        {
            return await _userManager.FindByIdAsync(id);
        }
        catch (Exception e)
        {
            _logger.LogError("[AccountRepository] Failed to get account by id {id}, Error: {e}", id, e.Message);
            return null;
        }
    }

    public async Task<IdentityResult> Create(Account account, string password)
    {
        try
        {
            var result = await _userManager.CreateAsync(account, password);

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError("[AccountRepository] Failed to create account, Error: {e}", e.Message);
            IdentityResult emptyResult = new IdentityResult();
            return emptyResult;
        }
    }

    public async Task<bool> Update(Account account)
    {
        try
        {
            var result = await _userManager.UpdateAsync(account);
            return result.Succeeded;
        }
        catch (Exception e)
        {
            _logger.LogError("[AccountRepository] Failed to update account, Error: {e}", e.Message);
            return false;
        }
    }

    public async Task<bool> Delete(string id)
    {
        try
        {
            var account = await _userManager.FindByIdAsync(id);
            if (account == null)
            {
                _logger.LogError("[AccountRepository] Account with id {id} not found", id);
                return false;
            }
            var result = await _userManager.DeleteAsync(account);
            return result.Succeeded;
        }
        catch (Exception e)
        {
            _logger.LogError("[AccountRepository] Failed to delete account with id {id}, Error: {e}",id , e.Message);
            return false;
        }
    }
}