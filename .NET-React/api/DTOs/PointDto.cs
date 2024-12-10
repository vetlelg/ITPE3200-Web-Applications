using ITPE3200ExamProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ITPE3200ExamProject.DTOs
{
    public class PointDto
    {
        public int PointId { get; set; }
        [Required(ErrorMessage = "Point Name is required")]
        [RegularExpression("^[a-zA-Z0-9Ê¯Â∆ÿ≈ ]{3,40}$", ErrorMessage = "Mer enn 3 tegn, og mindre enn 20")]
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(150)]
        public string Description { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;

        public Comment? Comment { get; set; } = null;
        public List<Image>? Images { get; set; } = [];
        public List<string>? UploadedImages { get; set; } = new List<string>();
        public List<string>? UploadedImagesNames { get; set; } = new List<string>();

        public List<Comment> Comments { get; set; } = [];
    }
}