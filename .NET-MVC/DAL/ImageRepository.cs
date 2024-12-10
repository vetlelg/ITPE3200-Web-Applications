using ITPE3200ExamProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ITPE3200ExamProject.DAL;

public class ImageRepository : IImageRepository
{
    private readonly PointDbContext _dbContext;
    private readonly ILogger<ImageRepository> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ImageRepository(PointDbContext dbContext, ILogger<ImageRepository> logger, IWebHostEnvironment webHost)
    {
        _dbContext = dbContext;
        _logger = logger;
        _webHostEnvironment = webHost;
    }

    public async Task<bool> Create(Image image)
    {
        try
        {
            await _dbContext.Images.AddAsync(image);
            // SaveChangesAsync() is called externally, so it is omitted here
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[ImageRepository] Failed to create image, Error: {e}", e.Message);
            return false;
        }
    }


    public async Task<List<Image>?> GetAllByPointId(int pointId)
    {
        try
        {
            return await _dbContext.Images.Where(p => p.PointId == pointId).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[ImageRepository] Failed to get all images by point ID, Error: {e}", e.Message);
            return null;
        }
    }


    public async Task<Image?> Delete(int id)
    {
        try
        {
            var image = await _dbContext.Images.FindAsync(id);
            if (image == null)
            {
                _logger.LogError("[ImageRepository] Image with id {id} not found", id);
                return null;
            }
            _dbContext.Images.Remove(image);
            await _dbContext.SaveChangesAsync();
            return image;
        }
        catch (Exception e)
        {
            _logger.LogError("[ImageRepository] Failed to delete image, Error: {e}", e.Message);
            return null;
        }
    }

    // Saves list of images to the server and database
    public async Task SaveImagesAsync(int pointId, IFormFile[] images)
    {
        // Goes thorugh list of images
        foreach (var image in images)
        {
            // Determines path and name of file
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string uniqueFilename = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFilename);
            
            // Copies image to the server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            var pointImage = new Image
            {
                FilePath = "/uploads/" + uniqueFilename,
                UploadedAt = DateTime.Now,
                PointId = pointId
            };

            // Use the image repository to save the image to the database
            await Create(pointImage);
        }
    }

}