using ITPE3200ExamProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ITPE3200ExamProject.DTOs
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? AccountId { get; set; }
        public int PointId { get; set; }
        public Account? Account { get; set; }
        public int Rating { get; set; }
    }
}