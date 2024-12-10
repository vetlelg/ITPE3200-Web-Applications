namespace ITPE3200ExamProject.Models
{
    public class Image
    {
        // Primary Key
        public int ImageId { get; set; }

        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }

        // Foreign key
        public int PointId { get; set; }

        // Navigation attibute
        public Point? Point { get; set; }
    }
}