namespace ITPE3200ExamProject.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? AccountId { get; set; }
        public Account? Account { get; set; }
        public int PointId { get; set; }
        public int Rating { get; set; }
    }
}
