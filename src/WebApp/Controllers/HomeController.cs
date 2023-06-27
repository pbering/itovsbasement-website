using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApp.Data;
using WebApp.Models;

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
        public async Task<IActionResult> Index()
        {
            var posts = await _repository.GetAsync();

            return View(new HomeModel(posts)
            {
                Title = "Home",
                Url = "/"
            });
        }
    }
}