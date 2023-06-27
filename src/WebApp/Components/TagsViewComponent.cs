using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Data;
using System.Linq;

namespace WebApp.Components
{
    public class TagsViewComponent : ViewComponent
    {
        private readonly IPostRepository _repository;

        public TagsViewComponent(IPostRepository repository)
        {
            _repository = repository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _repository.GetAsync();
            var tags = new List<string>();

            foreach (var post in posts)
            {
                tags.AddRange(post.Tags);
            }


            return View(new TagsModel { Tags = tags.Distinct().OrderBy(x => x).ToArray() });
        }
    }

    public class TagsModel
    {
        public string[] Tags { get; set; }
    }
}
