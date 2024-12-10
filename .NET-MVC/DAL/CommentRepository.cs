using Microsoft.EntityFrameworkCore;
using ITPE3200ExamProject.DAL;

namespace ITPE3200ExamProject.Models;

public class CommentRepository : ICommentRepository
{
    private readonly PointDbContext _dbContext;
    private readonly ILogger<CommentRepository> _logger;

    public CommentRepository(PointDbContext dbContext, ILogger<CommentRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> Create(Comment comment)
    {
        try
        {
            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[CommentRepository] Failed to create comment, Error: {e}", e.Message);
            return false;
        }
    }

    public async Task<List<Comment>?> GetAllByPointId(int pointId)
    {
        try
        {
            return await _dbContext.Comments
            .Include(a => a.Account)
            .Where(p => p.PointId == pointId).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[CommentRepository] Failed to get all comments by point id {pointId}, Error: {e}", pointId, e.Message);
            return null;
        }
    }

    public async Task<Comment?> Delete(int id)
    {
        try
        {
            var comment = await _dbContext.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }
            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();
            return comment;
        }
        catch (Exception e)
        {
            _logger.LogError("[CommentRepository] Failed to delete comment by id {id}, Error: {e}", id, e.Message);
            return null;
        }
    }



}