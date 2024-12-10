using ITPE3200ExamProject.Models;

namespace ITPE3200ExamProject.ViewModels
{
    public class ShowPointViewModel
    {
        public Point? Point { get; set; } = null;
        public Comment? Comment { get; set; } = null;
        public List<Comment>? Comments { get; set; } = [];
        public List<Image>? Images { get; set; } = [];
        public double AvgRating()
        {
            if(Point == null)
            {
                return 0;
            }
            if (Point.Comments == null)
            {
                return 0;
            }
            if (Point.Comments.Count > 0)
            {
                double sum = 0;
                foreach (var comment in Point.Comments)
                {
                    sum += comment.Rating;
                }
                return Math.Round(sum / Point.Comments.Count, 1);
            }
            return 0;
        }
    }
}