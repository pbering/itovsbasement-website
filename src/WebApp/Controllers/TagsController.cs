using WebApp.Data;
using WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public class TagsController : Controller
    {
        private readonly IPostRepository _repository;

        public TagsController(IPostRepository repository)
        {
            _repository = repository;
        }

        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        [Route("tags/{*name}")]
        public async Task<IActionResult> Index(string name)
        {
            var posts = await _repository.GetAsync();
            var model = new HomeModel(posts.Where(p => p.Tags.Contains(name)));

            if (model.Posts.Any())
            {
                model.Title = $"Posts tagged with '{name}'";
                model.Url = $"/tags/{name}";

                return View(model);
            }

            return NotFound();
        }
    }
}