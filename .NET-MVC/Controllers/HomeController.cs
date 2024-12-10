using Microsoft.AspNetCore.Mvc;
using ITPE3200ExamProject.ViewModels;
using ITPE3200ExamProject.DAL;

namespace ITPE3200ExamProject.Controllers;

public class HomeController : Controller
{
    private readonly IPointRepository _pointRepository;
    private readonly IAccountRepository _accountRepository;

    public HomeController(IPointRepository pointRepository, IAccountRepository accountRepository)
    {
        _pointRepository = pointRepository;
        _accountRepository = accountRepository;
    }

    // Shows the home page with the map
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = new IndexViewModel
        {
            Points = await _pointRepository.GetAll() ?? null!,
            Accounts = await _accountRepository.GetAllAccounts() ?? null!
        };
        return View(model);
    }
}
