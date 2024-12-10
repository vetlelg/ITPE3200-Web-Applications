using ITPE3200ExamProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ITPE3200ExamProject.DTOs
{
    public class DeleteCommentDto
    {
        public int PointId { get; set; }
        public int CommentId { get; set; }
    }
}