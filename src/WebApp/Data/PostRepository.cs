using Markdig;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data.Markdown;
using WebApp.Models;

namespace WebApp.Data
{
    public class PostRepository : IPostRepository
    {
        private readonly IWebHostEnvironment _environment;
        private readonly MarkdownPipeline _markdownPipeline;
        private IEnumerable<PostModel> _posts;

        public PostRepository(IWebHostEnvironment environment, MarkdownPipeline markdownPipeline)
        {
            _environment = environment;
            _markdownPipeline = markdownPipeline;
        }

        public async Task<IEnumerable<PostModel>> GetAsync()
        {
            await LoadAsync(_environment, _markdownPipeline);

            return _posts;
        }

        private async Task LoadAsync(IWebHostEnvironment environment, MarkdownPipeline markdownPipeline)
        {
            var posts = new List<PostModel>();
            var postsPath = Path.Combine(environment.ContentRootPath, "Posts");
            
            foreach (var filePath in Directory.GetFiles(postsPath, "*.md"))
            {
                var file = new MarkdownFile(new FileInfo(filePath));

                await file.ParseAsync(markdownPipeline);

                var post = new PostModel
                {
                    Name = file.Name,
                    Published = DateTime.Parse(file.Fields["date"]),
                    Title = file.Fields["title"],
                    Text = file.Body,
                    Tags = file.Fields["tags"].Split(',').Select(t => t.Trim().ToLowerInvariant()).ToArray(),
                    Summary = file.Fields["summary"],
                    Url = $"/posts/{file.Name}"
                };

                posts.Add(post);
            }

            _posts = posts.OrderByDescending(p => p.Published);
        }
    }
}