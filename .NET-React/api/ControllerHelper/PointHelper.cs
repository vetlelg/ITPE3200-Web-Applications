using ITPE3200ExamProject.DAL;

namespace ITPE3200ExamProject.ControllerHelper
{
    public static  class PointHelper
    {
        public static async Task<Models.Point> GetPoint(IPointRepository _pointRepository,int id)
        {
            Models.Point point = await _pointRepository.GetByPointId(id) ?? throw new NullReferenceException($"Failed to find point") ;

            return point;
        }
    }
}
