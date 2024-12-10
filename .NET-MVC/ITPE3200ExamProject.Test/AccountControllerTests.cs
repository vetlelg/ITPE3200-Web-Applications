using ITPE3200ExamProject.Controllers;
using ITPE3200ExamProject.DAL;
using ITPE3200ExamProject.Models;
using ITPE3200ExamProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Principal;
using Xunit;

namespace ITPE3200ExamProject.Test;

public class AccountControllerTests
{
    private Mock<IAccountRepository> mockAccountRepository;
    private AccountController accountController;
    private Mock<SignInManager<Account>> mockSignInManager;
    // Test constructor creates shared objects
    public AccountControllerTests()
    {
        mockAccountRepository = new Mock<IAccountRepository>();

        var mockUserStore = new Mock<IUserStore<Account>>();
        // Mocks sign in manager by using mock UserStore and assigning neccesery arguments to be null
        var mockUserManager = new Mock<UserManager<Account>>(mockUserStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        // Mocks sign in manager by using mockUserManager and assigning neccesery arguments
        mockSignInManager = new Mock<SignInManager<Account>>(mockUserManager.Object, new HttpContextAccessor(), new Mock<IUserClaimsPrincipalFactory<Account>>().Object, null!, null!, null!, null!);

        var mockLogger = new Mock<ILogger<AccountController>>();
        accountController = new AccountController(mockSignInManager.Object, mockLogger.Object, mockAccountRepository.Object, mockUserManager.Object);

        // Creates fake logged inn user 
        var fakeIdentity = new GenericIdentity("User");
        var user1 = new GenericPrincipal(fakeIdentity, null);
        ControllerContext controllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = user1
            }
        };
        accountController.ControllerContext = controllerContext;
    }


    // Positive test

    [Fact]
    public async void TestRegisterUser()
    {
        // Arrange

        // Create mock IdentityResult
        var mockIdentityResult = Mock.Of<Microsoft.AspNetCore.Identity.IdentityResult>(a => a.Succeeded == true);

        // Create RegisterViewModel
        var registerViewModel = new RegisterViewModel();
        registerViewModel.Email = "Test@e-mail.com";
        registerViewModel.Password = "TestPassword123";
        registerViewModel.ConfirmPassword = "TestPassword123";

        //Set up create so that a valid IdentityResult is returned
        mockAccountRepository.Setup(a => a.Create(It.IsAny<Account>(), registerViewModel.Password)).ReturnsAsync(mockIdentityResult);


        // Act
        RedirectToActionResult result = (RedirectToActionResult) await accountController.Register(registerViewModel);

        // Assert
        // If Create was called once, then the controller works as it should

        mockAccountRepository.Verify(a => a.Create(It.IsAny<Account>(), registerViewModel.Password), Times.Once);

        //Ensures that Accont controller redirects to login page
        Assert.True(result.ActionName == "Login" && result.ControllerName == "Account");
    }

    // Positive test
    [Fact]
    public async void TestLogIn()
    {
        // Arrange

        // Create LoginViewModel
        LoginViewModel loginViewModel = new LoginViewModel();
        loginViewModel.Email = "Test@e-mail.com";
        loginViewModel.Password = "TestPassword123";
        loginViewModel.RememberMe = false;


        var mockSignInResult = Mock.Of<Microsoft.AspNetCore.Identity.SignInResult>(a => a.Succeeded == true);

        // setup sign in result to return true
        mockSignInManager.Setup(a => a.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).ReturnsAsync(mockSignInResult);

        // Act
        // Result is the value of the page which the account Controller redirected to
        RedirectToActionResult result = (RedirectToActionResult)await accountController.Login(loginViewModel);
      
        // Assert
        // If account controller redirected to the index page then that means log in was succesful
        Assert.True(result.ActionName == "Index");
    }
}