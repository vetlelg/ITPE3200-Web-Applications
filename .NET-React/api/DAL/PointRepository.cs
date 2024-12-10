using Microsoft.EntityFrameworkCore;
using ITPE3200ExamProject.Models;

namespace ITPE3200ExamProject.DAL;

public class PointRepository : IPointRepository
{
    private readonly PointDbContext _dbContext;
    private readonly ILogger<PointRepository> _logger;

    public PointRepository(PointDbContext dbContext, ILogger<PointRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Point>?> GetAll()
    {
        try
        {
            return await _dbContext.Points.Include(a => a.Account).Include(b => b.Comments).Include(c => c.Images).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[PointRepository] Failed to get all points, Error: {e}", e.Message);
            return null;
        }
    }

    public async Task<IEnumerable<Point>?> GetAllByAccountId(string accountId)
    {
        try
        {
            return await _dbContext.Points.Where(p => p.AccountId == accountId).ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[PointRepository] Failed to get all points by account id {accountId}, Error: {e}", accountId, e.Message);
            return null;
        }
    }

    public async Task<Point?> GetByPointId(int id)
    {
        try
        {
            return await _dbContext.Points.Include(a=>a.Account).Include(b=>b.Comments).Include(c=>c.Images).Where(p => p.PointId == id).SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("[PointRepository] Failed to get point by id {id}, Error: {e}", id, e.Message);
            return null;
        }
    }

    public async Task<Point?> Create(Point point)
    {
        try
        {
            await _dbContext.Points.AddAsync(point);
            await _dbContext.SaveChangesAsync();

            // New point has to be returned to react application, in order to avoid having to reload page to access the new point
            Point newPoint = await GetByPointId(point.PointId) ?? throw new NullReferenceException($"{point.Name}'s PointId is null"); ;
            return newPoint;
        }
        catch (Exception e)
        {
            _logger.LogError("[PointRepository] Failed to create point, Error: {e}", e.Message);
            return null;
        }
    }

    public async Task<bool> Edit(Point point)
    {
        try
        {
            _dbContext.Points.Update(point);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[PointRepository] Failed to update point, Error: {e}", e.Message);
            return false;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
        {
            var point = await _dbContext.Points.FindAsync(id);
            if (point == null)
            {
                _logger.LogError("[PointRepository] Point with id {id} not found", id);
                return false;
            }
            _dbContext.Points.Remove(point);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("[PointRepository] Failed to delete point, Error: {e}", e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteComment(int pointId, int commentId)
    {
        try
        {
            var point = await GetByPointId(pointId);
            if (point == null)
            {
                _logger.LogError("[PointRepository] Point with id {id} not found", pointId);
                return false;
            }

            if(point.Comments == null)
            {
                throw new NullReferenceException("Points comment is null");
            }
            foreach (var comment in point.Comments)
            {
                if (comment.CommentId == commentId)
                {
                    _dbContext.Comments.Remove(comment);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
            }
            return true;

        }
        catch (Exception e)
        {
            _logger.LogError("[PointRepository] Failed to delete comment, Error: {e}", e.Message);
            return false;
        }
    }
}