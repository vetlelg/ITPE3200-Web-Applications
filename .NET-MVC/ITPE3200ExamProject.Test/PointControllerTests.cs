using Castle.Components.DictionaryAdapter;
using ITPE3200ExamProject.ControllerHelper;
using ITPE3200ExamProject.Controllers;
using ITPE3200ExamProject.DAL;
using ITPE3200ExamProject.Models;
using ITPE3200ExamProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System.Security.Claims;
using System.Security.Principal;
using Xunit;

namespace ITPE3200ExamProject.Test;

public class PointControllerTests
{
    private Mock<IPointRepository> mockPointRepository;
    private PointController pointController;
    private Account account;
    private Point point;
    private Point wrongPoint;
    private Mock<PointDbContext> mockDbContext;
    private Mock<UserManager<Account>> mockUserManager;
    // Test constructor creates shared objects
    public PointControllerTests()
    {
        mockPointRepository = new Mock<IPointRepository>();
        account = new Account
        {
            Email = "TestE-mail@test.com"
        };

        point = new Point
        {
            Name = "Test",
            Account = account,
            AccountId = account.Id,
            Latitude = 10,
            Longitude = 40.01,
            Description = "Descryption"

        };
         wrongPoint = new Point
        {
            Name = "Wrong",
            Account = account,
            AccountId = account.Id,
            Latitude = 0,
            Longitude = 0,
            Description = "Wrong point",
            PointId = 1
        };

        var store = new Mock<IUserStore<Account>>();
        mockUserManager = new Mock<UserManager<Account>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var mockLogger = new Mock<ILogger<PointController>>();
        var mockImageRepository = new Mock<IImageRepository>();
        var mockCommentRepository = new Mock<ICommentRepository>();
        var mockWebHost = new Mock<IWebHostEnvironment>();
        DbContextOptions<PointDbContext> options = new DbContextOptions<PointDbContext>();
        mockDbContext = new Mock<PointDbContext>(options);

        // Creates pointController
        pointController = new PointController(mockPointRepository.Object, mockImageRepository.Object, mockCommentRepository.Object, mockUserManager.Object, mockLogger.Object, mockWebHost.Object, mockDbContext.Object);


        // Creates claims which will be used to create a fake user
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, "TestUser"),
            new Claim(ClaimTypes.Name, "TestE-mail@test.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);

        // Mocks httpContext and setup to return the fake user
        var httpContextMock = new Mock<HttpContext>();
        httpContextMock.Setup(a => a.User).Returns(principal);

        // Creates controller context with the fake user
        ControllerContext controllerContext = new ControllerContext
        {
            HttpContext = httpContextMock.Object
        };
        pointController.ControllerContext = controllerContext;
    }

    // Positive test
    [Fact]
    public async void TestInsertPoint()
    {
        // Arrange

        //Mocks databse facade and transcation
        var mockDatabaseFacade = new Mock<DatabaseFacade>(mockDbContext.Object);
        var mockDbTransaction = new Mock<IDbContextTransaction>();
        
        // Setup both dbcontext.database and dbcontext.Database.BeginTransaction inorder to get passed controllers BeginTransaction
        mockDbContext.Setup(a => a.Database).Returns(mockDatabaseFacade.Object);
        mockDatabaseFacade.Setup(a => a.BeginTransaction()).Returns(mockDbTransaction.Object);
        // Creates empty list of images
        FormFile[] formFile = { };

        //Mocks mockPointRepository to return true, to pass if test in controller
        mockPointRepository.Setup(a => a.Create(point)).ReturnsAsync(true);
        // Act
        var result = await pointController.Create(point, formFile);

        // Assert
        // If Create was called once, then the controller works as it should

        mockPointRepository.Verify(a => a.Create(point), Times.Once);

        //Ensures that the controller method redirected to the right action (RedirectToAction means it tried to redirect to another controller method, which only happens if the controller method completed successfully )
        Assert.IsType<RedirectToActionResult>(result);
        
    }

    //Negative test
    [Fact]
    public async void TestInsertPointWithInvalidModelState()
    {
        // Arrange


        //Mocks databse facade and transcation
        var mockDatabaseFacade = new Mock<DatabaseFacade>(mockDbContext.Object);
        var mockDbTransaction = new Mock<IDbContextTransaction>();

        // Setup both dbcontext.database and dbcontext.Database.BeginTransaction so the test gets through the Begintranscation in the controller
        mockDbContext.Setup(a => a.Database).Returns(mockDatabaseFacade.Object);
        mockDatabaseFacade.Setup(a => a.BeginTransaction()).Returns(mockDbTransaction.Object);

        pointController.ModelState.AddModelError("error", "error");

        // Creates empty list of images
        FormFile[] formFile = { };

        // Act
        var result = await pointController.Create(point, formFile);

        // Assert
        // If Create wasn't called, then the point wasn't attempted to be inserted
        mockPointRepository.Verify(a => a.Create(point), Times.Never);

        //Ensures that the controller method redirected to the right action (RedirectToAction means it tried to redirect to another controller method, which only happens if the controller method completed successfully )
        Assert.IsNotType<RedirectToActionResult>(result);
    }

    // Positive test
    [Fact]
    public async void TestEditPoint()
    {
        // Arrange
        // Setup to return point from GetByPointId method
        mockPointRepository.Setup(a => a.GetByPointId(point.PointId)).ReturnsAsync(point);

        // Setup to return account from FindByEmailAsync method
        mockUserManager.Setup(a => a.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(account);

        //Mocks mockPointRepository to return true, to pass if test in controller
        mockPointRepository.Setup(a => a.Edit(point)).ReturnsAsync(true);
        // Act
        var result = await pointController.Edit(point);

        // Assert
        // If Edit was called once, then the controller works as it should

        mockPointRepository.Verify(a => a.Edit(point), Times.Once);

        //Ensures that the controller method redirected to the right action (RedirectToAction means it tried to redirect to another controller method, which only happens if the controller method completed successfully )
        Assert.IsType<RedirectToActionResult>(result);
    }

    // Negative test
    [Fact]
    public async void TestEditWithWrongPoint()
    {
        // Arrange
      
        // Setup to return point from GetByPointIp method
        mockPointRepository.Setup(a => a.GetByPointId(point.PointId)).ReturnsAsync(wrongPoint);

        // Act
        var result = await pointController.Edit(point);

        // Assert
        // If Edit was never called, then the controller works as it should

        mockPointRepository.Verify(a => a.Edit(point), Times.Never);

        //Ensures that the controller method redirected to the right action (RedirectToAction means it tried to redirect to another controller method, which only happens if the controller method completed successfully )
        Assert.IsNotType<RedirectToActionResult>(result);
    }

    // Positive test
    [Fact]
    public async void TestDeletePoint()
    {
        // Arrange

        // Setup to return point from GetByPointIp method
        mockPointRepository.Setup(a => a.GetByPointId(point.PointId)).ReturnsAsync(point);

        // Act
        var result = await pointController.Delete(point.PointId);

        // Assert
        // If Delete was called once, then the controller works as it should
        mockPointRepository.Verify(a => a.Delete(point.PointId), Times.Once);

        //Ensures that the controller method redirected to the right action (RedirectToAction means it tried to redirect to another controller method, which only happens if the controller method completed successfully )
        Assert.IsType<RedirectToActionResult>(result);
    }

    // Negative test
    [Fact]
    public async void TestDeleteWithNoPoint()
    {
        // Arrange

        // Setup to return point from GetByPointIp method
        Point nullPoint = null!;
        mockPointRepository.Setup(a => a.GetByPointId(point.PointId)).ReturnsAsync(nullPoint);

        // Act
        var result = await pointController.Delete(point.PointId);

        // Assert
        // If Delete was never called, then the controller work as it should
        mockPointRepository.Verify(a => a.Delete(point.PointId), Times.Never);

        //Ensures that the controller method redirected to the right action (RedirectToAction means it tried to redirect to another controller method, which only happens if the controller method completed successfully )
        Assert.IsNotType<RedirectToActionResult>(result);
    }

    // Positive test
    [Fact]
    public async void TestCreateComment()
    {
        // Arrange

        // Setup to return point from GetByPointIp method
        mockPointRepository.Setup(a => a.GetByPointId(point.PointId)).ReturnsAsync(point);

        ShowPointViewModel viewModel = new ShowPointViewModel();
        viewModel.Point = point;
        Comment comment = new Comment { AccountId = "User", CommentId = 0, PointId = 0, Rating = 3, Text = "Test comment" };
        viewModel.Comment = comment;

        //Mocks mockPointRepository to return true, to pass if test in controller
        mockPointRepository.Setup(a => a.Edit(point)).ReturnsAsync(true);


        // Act
        var result = await pointController.CreateComment(viewModel);

        // Assert
        // If Delete was called once, then the controller works as it should
        mockPointRepository.Verify(a => a.Edit(point), Times.Once);

        //Ensures that the controller method redirected to the right action (RedirectToAction means it tried to redirect to another controller method, which only happens if the controller method completed successfully )
        Assert.IsType<RedirectToActionResult>(result);
    }

    // Positive test
    [Fact]
    public async void TestCreateCommentWithWrongPointId()
    {
        // Arrange

        // Setup to return point from GetByPointIp method
        mockPointRepository.Setup(a => a.GetByPointId(point.PointId)).ReturnsAsync(point);

        ShowPointViewModel viewModel = new ShowPointViewModel();
        viewModel.Point = point;
        Comment comment = new Comment { AccountId = "User", CommentId = 0, PointId = 3, Rating = 3, Text = "Test comment" };
        viewModel.Comment = comment;

        // Act
        var result = await pointController.CreateComment(viewModel);

        // Assert
        // If Delete was never called, then the controller works as it should
        mockPointRepository.Verify(a => a.Edit(point), Times.Never);

        //Ensures that the controller method redirected to the right action (RedirectToAction means it tried to redirect to another controller method, which only happens if the controller method completed successfully )
        Assert.IsNotType<RedirectToActionResult>(result);
    }
}