using WebApp.Data;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostRepository _repository;

        public HomeController(IPostRepository repository)
        {
            _repository = repository;
        }

        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public IActionResult Index()
        {
            return View(new HomeModel(_repository.Get())
            {
                Title = "Home",
                Url = "/"
            });
        }
    }
}