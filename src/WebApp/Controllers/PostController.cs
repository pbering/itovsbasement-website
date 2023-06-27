using System;
using System.Linq;
using WebApp.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class PostController : Controller
    {
        private readonly IPostRepository _repository;

        public PostController(IPostRepository repository)
        {
            _repository = repository;
        }

        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [Route("posts/{*name}")]
        public async Task<IActionResult> Index(string name)
        {
            var posts = await _repository.GetAsync();
            var post = posts.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (post != null)
            {
                return View(post);
            }

            return NotFound();
        }
    }
}