using Microsoft.AspNetCore.Mvc;
using ITPE3200ExamProject.Models;
using ITPE3200ExamProject.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ITPE3200ExamProject.DTOs;
using Point = ITPE3200ExamProject.Models.Point;
using ITPE3200ExamProject.ControllerHelper;
namespace ITPE3200ExamProject.Controllers;

[ApiController]
[Route("api/points")]
public class PointController : Controller
{
    private readonly IPointRepository _pointRepository;
    private readonly IImageRepository _imageRepository;
    private readonly ILogger<PointController> _logger;
    private readonly UserManager<Account> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly PointDbContext _dbContext;


    public PointController(IPointRepository pointRepository, IImageRepository imageRepository, UserManager<Account> userManager, ILogger<PointController> logger, IWebHostEnvironment webHost, PointDbContext pointDbContext)
    {
        _pointRepository = pointRepository;
        _imageRepository = imageRepository;
        _logger = logger;
        _userManager = userManager;
        _webHostEnvironment = webHost;
        _dbContext = pointDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var points = await _pointRepository.GetAll();
            return Ok(points);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[PointController] Point list not found: {ex}", ex);
            return NotFound($"Point list not found");
        }
    }
    
    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] PointDto pointDto)
    {
        try
        {
            Point point = new Point
            {
                PointId = pointDto.PointId,
                Name = pointDto.Name,
                Description = pointDto.Description,
                Comments = pointDto.Comments,
                Latitude = pointDto.Latitude,
                Longitude = pointDto.Longitude,
            };

            // Gets the account id from the logged in user
            point.AccountId = pointDto.AccountId;

            // Creates an empty list for the comments on the point
            point.Comments = new List<Comment>();

            using (var trans = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // Use the point repository to create the point
                    var retPoint = await _pointRepository.Create(point);

                    if (pointDto.UploadedImages != null && pointDto.UploadedImages.Count > 0 && pointDto.UploadedImagesNames != null)
                    {
                        // Delegate image handling logic to a separate method in the repository
                        await _imageRepository.SaveImagesAsync(point.PointId, pointDto.UploadedImages, pointDto.UploadedImagesNames);
                    }


                    // Commit the transaction if everything is successful
                    await trans.CommitAsync();
                    await _dbContext.SaveChangesAsync();

                    if (retPoint != null)
                    {

                        return Ok(retPoint);

                    }
                }
                catch (Exception e)
                {
                    await trans.RollbackAsync();
                    _logger.LogError("[PointController] Failed to create point or save images: {0}", e.Message);
                    return BadRequest($"{e.Message}");
                }
            }

            _logger.LogError("[PointController] Failed to create point {@point}", point);
            return BadRequest($"Unknown error occurred");
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

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Edit([FromBody] PointDto pointDto)
    {
        try
        {
            Point point = await PointHelper.GetPoint(_pointRepository, pointDto.PointId);
            point.Name = pointDto.Name;
            point.Description = pointDto.Description;

            // Ensures that the point's account value matches the logged in user
            AccountHelper.checkRightAccount(await _userManager.FindByEmailAsync(point.AccountId) ?? await _userManager.FindByIdAsync(point.AccountId), point);

            bool retValue = await _pointRepository.Edit(point);
            if (pointDto.UploadedImages != null && pointDto.UploadedImages.Count > 0 && pointDto.UploadedImagesNames != null)
            {
                await _imageRepository.SaveImagesAsync(point.PointId, pointDto.UploadedImages, pointDto.UploadedImagesNames);
                await _dbContext.SaveChangesAsync();
            }
            if (retValue == true)
            {
                Point editedPoint = await PointHelper.GetPoint(_pointRepository, pointDto.PointId);
                return Ok(editedPoint);
            }

            _logger.LogError($"[PointController] Failed to update point {point}");
            return BadRequest($"Failed to update point {point}");
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

    [HttpDelete("{id}/delete")]
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
                return BadRequest($"Unknown error occurred");
            }

            bool returnOk = await _pointRepository.Delete(point.PointId);
            if (returnOk)
            {
                string resultString = "{\"Result\":\"Delete Succeded\"}";
                return Ok(resultString);
            }
            else
            {
                _logger.LogError("[PointController] Point with id {id} failed to delete", id);
                return BadRequest($"Unknown error occurred");

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

    [HttpPost("createComment")]
    [Authorize]
    public async Task<IActionResult> CreateComment([FromBody] CommentDto commentDto)
    {
        try
        {
            Comment comment = new Comment
            {
                CommentId = commentDto.CommentId,
                Text = commentDto.Text,
                AccountId = commentDto.AccountId,
                PointId = commentDto.PointId,
                Rating = commentDto.Rating,
            };

            Point point = await PointHelper.GetPoint(_pointRepository, commentDto.PointId);

            if (comment.AccountId != null)
            {
                comment.Account = await _userManager.FindByIdAsync(comment.AccountId);

                if (point.Comments is null) throw new NullReferenceException($"point with id {point.PointId} does not have any comment");
                point.Comments.Add(comment);
                bool retValue = await _pointRepository.Edit(point);

                if (retValue)
                    return Ok(comment);
            }
            _logger.LogError("[PointController] Failed to update point {@point}", point);
            return BadRequest($"Unknown error occurred");
        }
        catch (Exception ex)
        {
            if (ex.Message == $"Failed to find point")
            {
                _logger.LogError("[PointController] Point with id {id} not found. Error: {ex}", commentDto.PointId, ex);
                return NotFound($"Point with id {commentDto.PointId} not found");
            }

            _logger.LogError($"[PointController] (Edit) Unknown error occurred");
            return BadRequest($"Unknown error occurred");
        }
    }

    [HttpPost("deleteComment")]
    [Authorize]
    public async Task<IActionResult> DeleteComment([FromBody] DeleteCommentDto deleteCommentDto)
    {
        try
        {
            bool returnOk = await _pointRepository.DeleteComment(deleteCommentDto.PointId, deleteCommentDto.CommentId);
            if (returnOk)
            {
                string resultString = "{\"Result\":\"Delete Succeded\"}";
                return Ok(resultString);
            }
            else
            {
                _logger.LogError("[PointController] Comment with id {id} failed to delete", deleteCommentDto.CommentId);
                return BadRequest($"Unknown error occurred");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"[PointController] (Edit) Unknown error occurred: {ex}", ex);
            return BadRequest($"Unknown error occurred");
        }
    }

    // Method to ensure that the DeleteImage recieves the correct input
    public class ImageIdModel
    {
        public int ImageId { get; set; }
    }

    [HttpPost("deleteImage")]
    public async Task<IActionResult> DeleteImage([FromBody] ImageIdModel model)
    {
        try
        {
            int imageId = model.ImageId;
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

                string resultString = "{\"Result\":\"Delete Succeded\"}";
                return Ok(resultString);
            }

            return NotFound("Image not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[PointController] (DeleteImage) Unknown error occurred: {ex}", ex);
            return BadRequest($"Unknown error occurred");
        }

    }

}
