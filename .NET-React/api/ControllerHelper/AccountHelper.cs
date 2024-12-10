using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Security.Claims;

namespace ITPE3200ExamProject.ControllerHelper
{
    public static class AccountHelper
    {
        // Returns name of logged in user or throws exception if logged in user does not have a name
        public static string ReturnUserName(ClaimsPrincipal User)
        {
            string name = User.Identities.First().Name ?? throw new NullReferenceException("User has no account name");

            return name;
        }

        public static string GetAccountId(ClaimsPrincipal User)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new NullReferenceException("No logged in user found");
            return user.Value;
        }

        // Returns name of logged in user or throws exception if logged in user does not have a name
        public static bool checkRightAccount(Models.Account? account, Models.Point point)
        {
            if(account == null)
            {
                throw new Exception(message: "Could not find account");
            }
            if (point.AccountId != account.Email && point.AccountId != account.Id)
            {
                throw new Exception(message: "Logged in user and point's owner do not match");
            }

            return true;
        }
    }
}
