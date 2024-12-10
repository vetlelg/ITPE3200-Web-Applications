using ITPE3200ExamProject.Models;

namespace ITPE3200ExamProject.DAL;

public interface IImageRepository
{
    Task<bool> Create(Image image);
    Task SaveImagesAsync(int pointId, IFormFile[] images);
    Task<List<Image>?> GetAllByPointId(int pointId);
    Task<Image?> Delete(int id);
}