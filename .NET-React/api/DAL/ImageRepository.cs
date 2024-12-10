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
        
        // If WebRootPath variable is empty, direct to it manually through CurrentDirectory
        if (string.IsNullOrEmpty(_webHostEnvironment.WebRootPath))
        {
            _webHostEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "../../../wwwroot");
        }
    }

    public async Task<bool> Create(Image image)
    {
        try
        {
            await _dbContext.Images.AddAsync(image);
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

    
    public async Task SaveImagesAsync(int pointId, List<string> images, List<string> imagesNames)
    {
        // recieves list of images and and the names of the images
        // Goal is to save the images to the server and save the image paths to the database
        var formFiles = new List<IFormFile>();

        //Goes through all images
        for (int i = 0; i < images.Count; i++)
        {
            // Converts the base64 string to a byte array
            string base64Data = images[i].Split(',')[1];
            var bytes = Convert.FromBase64String(base64Data);
            var stream = new MemoryStream(bytes);

            // Converts byte stream to a form file
            FormFile formFile = new FormFile(stream, 0, bytes.Length, "file", imagesNames[i])
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
            // Adds formfile to list of formfiles
            formFiles.Add(formFile);
        }

        // Goes through all formfiles
        foreach (var image in formFiles)
        {
            // Detrmines the path to save the image
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            string uniqueFilename = image.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFilename);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            // Copies image to the server with correct name
            var pointImage = new Image
            {
                FilePath = "/images/" + uniqueFilename,
                UploadedAt = DateTime.Now,
                PointId = pointId
            };

            // Use the image repository to save the image
            await Create(pointImage);
        }
    }
}