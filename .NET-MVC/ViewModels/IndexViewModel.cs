using ITPE3200ExamProject.Models;

namespace ITPE3200ExamProject.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<Point> Points { get; set; } = [];
        public IEnumerable<Account> Accounts { get; set; } = [];
    }
}
