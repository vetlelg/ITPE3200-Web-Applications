using ITPE3200ExamProject.Models;

namespace ITPE3200ExamProject.DAL;

public interface ICommentRepository
{
    Task<bool> Create(Comment comment);
    Task<List<Comment>?> GetAllByPointId(int pointId);
    Task<Comment?> Delete(int id);
}