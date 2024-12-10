using ITPE3200ExamProject.Models;
using Microsoft.AspNetCore.Identity;

namespace ITPE3200ExamProject.DAL;

public interface IAccountRepository
{
    Task<IEnumerable<Account>?> GetAllAccounts();
    Task<Account?> GetByAccountId(string id);
    Task<IdentityResult?> Create(Account account, string id);
    Task<IdentityResult?> Update(Account account);
    Task<bool> Delete(string id);
}