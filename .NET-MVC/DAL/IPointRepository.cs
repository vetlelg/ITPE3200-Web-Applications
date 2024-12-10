using ITPE3200ExamProject.Models;
using ITPE3200ExamProject.ViewModels;

namespace ITPE3200ExamProject.DAL;

public interface IPointRepository
{
    Task<IEnumerable<Point>?> GetAll();
    Task<IEnumerable<Point>?> GetAllByAccountId(string id);
    Task<Point?> GetByPointId(int id);
    Task<bool> Create(Point point);
    Task<bool> Edit(Point point);
    Task<bool> Delete(int id);
    Task<bool> DeleteComment(int pointId, int commentId);
    //Task<bool> EditComment(int id);

}