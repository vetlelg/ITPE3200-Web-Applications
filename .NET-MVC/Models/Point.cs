using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITPE3200ExamProject.Models
{
    public class Point
    {
        public int PointId { get; set; }
        [Required(ErrorMessage = "Point Name is required")]
        [RegularExpression(@"^[\wæøåÆØÅ\s\p{P}]{3,50}$", ErrorMessage = "Mer enn 3 tegn, og mindre enn 20")]
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [StringLength(150)]
        public string Description { get; set; } = string.Empty;


        // This is a guid, so it should be a string
        public string AccountId { get; set; } = string.Empty;
        public Account? Account { get; set; }
        public List<Comment> Comments { get; set; } = [];
        public List<Image>? Images { get; set; }

        [NotMapped]
        public IFormFile[]? UploadedImages { get; set; }

    }
}
