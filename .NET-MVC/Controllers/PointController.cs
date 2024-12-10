using Microsoft.AspNetCore.Mvc;
using ITPE3200ExamProject.Models;
using ITPE3200ExamProject.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ITPE3200ExamProject.ViewModels;
using ITPE3200ExamProject.ControllerHelper;
using Point = ITPE3200ExamProject.Models.Point;
using Microsoft.EntityFrameworkCore;

namespace ITPE3200ExamProject.Controllers;

public class PointController : Controller
{
    private readonly IPointRepository _pointRepository;
    private readonly IImageRepository _imageRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ILogger<PointController> _logger;
    private readonly UserManager<Account> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly PointDbContext _dbContext;

    public PointController(IPointRepository pointRepository, IImageRepository imageRepository, ICommentRepository commentRepository, UserManager<Account> userManager, ILogger<PointController> logger, IWebHostEnvironment webHost, PointDbContext pointDbContext)
    {
        _pointRepository = pointRepository;
        _imageRepository = imageRepository;
        _commentRepository = commentRepository;
        _logger = logger;
        _userManager = userManager;
        _webHostEnvironment = webHost;
        _dbContext = pointDbContext;
    }

    // Shows the create point page
    [HttpGet]
    [Authorize]
    public IActionResult Create() => View();

    // Creates a new point
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Point point, IFormFile[] images)
    {
        try
        {
            // Gets the account id from the logged in user
            point.AccountId = AccountHelper.GetAccountId(User);


            if (ModelState.IsValid)
            {
                using (var trans = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        // Use the point repository to create the point
                        bool returnOk = await _pointRepository.Create(point);

                        if (point.UploadedImages != null && point.UploadedImages.Length > 0)
                        {
                            // Delegate image handling logic to a separate method in the repository
                            await _imageRepository.SaveImagesAsync(point.PointId, point.UploadedImages);
                        }

                        // Commit the transaction if everything is successful
                        await trans.CommitAsync();
                        await _dbContext.SaveChangesAsync();

                        if (returnOk)
                        {
                            return RedirectToAction("Index", "Home");

                        }
                    }
                    catch (Exception e)
                    {
                        await trans.RollbackAsync();
                        _logger.LogError("[PointController] Failed to create point or save images: {0}", e.Message);
                        return BadRequest($"{e.Message}");
                    }
                }
            }
            _logger.LogError("[PointController] Failed to create point {@point}", point);
            return View();
        }
        catch (NullReferenceException)
        {
            _logger.LogError($"[PointController] Logged in user doesn't have a name");
            return BadRequest($"Logged in user doesn't have a name");
        }
        catch
        {
            _logger.LogError($"[PointController] (Create) Unknown error occurred");
            return BadRequest($"Unknown error occurred");
        }
    }

    // Shows the edit point page
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var point = await PointHelper.GetPoint(_pointRepository, id);
            
            if (point.Account!.Email != AccountHelper.ReturnUserName(User))
            {
                _logger.LogError($"[PointController] Tried to load {point.Name} without the correct user being logged inn. ");
                return RedirectToAction("Index", "Home");
            }
            if (point == null)
            {
                _logger.LogError("[PointController] Point with id {id} not found when updating", id);
                return RedirectToAction("Index", "Home");
            }

            ShowPointViewModel showPointViewModel = new ShowPointViewModel();
            showPointViewModel.Point = point;
            showPointViewModel.Comments = await _commentRepository.GetAllByPointId(id);
            showPointViewModel.Images = await _imageRepository.GetAllByPointId(id);
            return View(showPointViewModel);
        }
        catch (Exception ex)
        {
            if (ex.Message == "User has no account name")
            {
                _logger.LogError($"[PointController] Logged in user doesn't have a name");
                return BadRequest($"Logged in user doesn't have a name");
            }
            else if (ex.Message == $"Failed to find point")
            {
                _logger.LogError("[PointController] Point with id {id} not found", id);
                return NotFound($"Point with id {id} not found");
            }
            _logger.LogError($"[PointController] (Edit) Unknown error occurred");
            return BadRequest($"Unknown error occurred");
        }

    }

    // Edits an existing point
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(Point point)
    {
        try
        {
            // Ensures that the point's account value matches the logged in user
            var account = await _userManager.FindByIdAsync(point.AccountId);
            AccountHelper.checkRightAccount(account, point);


            if (ModelState.IsValid)
            {
                bool returnOk = await _pointRepository.Edit(point);
                if (point.UploadedImages != null && point.UploadedImages.Length > 0)
                {
                    await _imageRepository.SaveImagesAsync(point.PointId, point.UploadedImages);
                    await _dbContext.SaveChangesAsync();
                }
                if (returnOk)
                    return RedirectToAction("Index", "Home");
            }
            _logger.LogError("[PointController] Failed to update point {@point}", point);
            return View();
        }
        catch (Exception ex)
        {
            if (ex.Message == "Logged in user and point's owner do not match")
            {
                _logger.LogError($"[PointController] Logged in user and point's owner do not match");
                return BadRequest($"Logged in user and point's owner do not match");
            }
            if (ex.Message == "Could not find account")
            {
                _logger.LogError($"[PointController] Could not find account conected to point.");
                return BadRequest($"Could not find account conected to point.");
            }
            if (ex.Message.Contains("Could not find a part of the path"))
            {
                _logger.LogError($"[PointController] {ex.Message}");
                return BadRequest($"{ex.Message}");
            }
            _logger.LogError($"[PointController] (Edit) Unknown error occurred");
            return BadRequest($"Unknown error occurred");
        }
    }

    // Shows information about point
    [HttpGet]
    public async Task<IActionResult> Show(int id)
    {
        try
        {
            ShowPointViewModel showPointViewModel = new ShowPointViewModel();
            //Point point = await _pointRepository.GetByPointId(id);
            Point point = await PointHelper.GetPoint(_pointRepository, id);

            showPointViewModel.Point = point;

            if (point == null)
            {
                _logger.LogError("[PointController] Point with id {id} not found", id);
                return NotFound($"Point with id {id} not found");
            }

            showPointViewModel.Point = point;
            showPointViewModel.Images = await _imageRepository.GetAllByPointId(id);

            return View(showPointViewModel);
        }
        catch (Exception ex)
        {
            if (ex.Message == "Failed to find point")
            {
                _logger.LogError("[PointController] Point with id {id} not found", id);
                return NotFound($"Point with id {id} not found");
            }

            _logger.LogError($"[PointController] (Edit) Unknown error occurred");
            return BadRequest($"Unknown error occurred");
        }
    }

    // Saves comment to point
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateComment(ShowPointViewModel model)
    {
        try
        {
            Point point = await PointHelper.GetPoint(_pointRepository, model.Comment!.PointId);

            if (ModelState.IsValid)
            {
                model.Comment.AccountId = AccountHelper.GetAccountId(User);
                if (point.Comments is null) throw new NullReferenceException($"point with id {point.PointId} does not have any comment");
                point.Comments.Add(model.Comment);
                bool returnOk = await _pointRepository.Edit(point);

                if (returnOk)
                    return RedirectToAction("Show", "Point", new { id = model.Comment.PointId });
            }
            _logger.LogError("[PointController] Failed to update point {@point}", point);
            return View();
        }
        catch (Exception ex)
        {
            if (ex.Message == $"Failed to find point")
            {
                _logger.LogError("[PointController] Point with id {id} not found. Error: {ex}", model.Point!.PointId, ex);
                return NotFound($"Point with id {model.Point.PointId} not found");
            }

            _logger.LogError($"[PointController] (Edit) Unknown error occurred");
            return BadRequest($"Unknown error occurred");
        }
    }

    // Deletes the selected marker
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var point = await PointHelper.GetPoint(_pointRepository, id);

            if (point == null)
            {
                _logger.LogError("[PointController] Point with id {id} not found when deleting", id);
                return BadRequest($"Point with id {id} not found");
            }
            if (point.Account!.Email != User.Identities.First().Name)
            {
                _logger.LogError($"[PointController] Tried to Delete {point.Name} without the correct user being logged inn.");
                return RedirectToAction("Index", "Home");
            }

            bool returnOk = await _pointRepository.Delete(point.PointId);
            if (returnOk)
                return RedirectToAction("Index", "Home");
            else
            {
                _logger.LogError("[PointController] Point with id {id} failed to delete", id);
                return RedirectToAction("Index", "Home");
            }
        }
        catch (Exception ex)
        {
            if (ex.Message == $"Failed to find point")
            {
                _logger.LogError("[PointController] Point with id {id} not found", id);
                return NotFound($"Point with id {id} not found");
            }

            _logger.LogError($"[PointController] (Edit) Unknown error occurred");
            return BadRequest($"Unknown error occurred");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var image = await _imageRepository.Delete(imageId);

        if (image != null)
        {
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, image.FilePath.TrimStart('/'));
            Console.WriteLine($"Deleting file at path: {filePath}");

            // Removes the image from the wwwroot/uploads folder
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Returns the edit View of the same point
            return RedirectToAction("Edit", new { id = image.PointId });
        }

        return NotFound("Image not found.");
    }

    // Deletes the selected comment
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteComment(string id)
    {
        try
        {
            string[] ids = id.Split(',');
            int pointId = int.Parse(ids[0]);
            int commentId = int.Parse(ids[1]);

            bool returnOk = await _pointRepository.DeleteComment(pointId, commentId);
            if (returnOk)
                return RedirectToAction("Show", new { id = pointId});
            else
            {
                _logger.LogError("[PointController] Comment with id {id} failed to delete", id);
                return RedirectToAction("Index", "Home");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[PointController] (Edit) Unknown error occurred: {ex}", ex);
            return BadRequest($"Unknown error occurred");
        }
    }
}
