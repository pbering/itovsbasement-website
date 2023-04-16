using Markdig;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebApp.Data.Markdown;
using WebApp.Models;

namespace WebApp.Data
{
    public class PostRepository : IPostRepository
    {
        private IEnumerable<PostModel> _posts;

        public PostRepository(IWebHostEnvironment environment, MarkdownPipeline markdownPipeline)
        {
            Initialize(environment, markdownPipeline);
        }

        public virtual IEnumerable<PostModel> Get()
        {
            return _posts;
        }

        protected void Initialize(IWebHostEnvironment environment, MarkdownPipeline markdownPipeline)
        {
            var posts = new List<PostModel>();
            var postsPath = Path.Combine(environment.ContentRootPath, "Posts");

            foreach (var filePath in Directory.GetFiles(postsPath, "*.md"))
            {
                var file = new MarkdownFile(new FileInfo(filePath));

                file.Parse(markdownPipeline);

                var post = new PostModel
                {
                    Name = file.Name,
                    Published = DateTime.Parse(file.Fields["date"]),
                    Title = file.Fields["title"],
                    Text = file.Body,
                    Tags = file.Fields["tags"].Split(',').Select(t => t.Trim().ToLowerInvariant()).ToArray(),
                    Summary = file.Fields["summary"],
                    Url = "/posts/" + file.Name
                };

                posts.Add(post);
            }

            _posts = posts.OrderByDescending(p => p.Published);
        }
    }

    public class DevelopmentPostRepository : PostRepository
    {
        private readonly IWebHostEnvironment _environment;
        private readonly MarkdownPipeline _markdownPipeline;

        public DevelopmentPostRepository(IWebHostEnvironment environment, MarkdownPipeline markdownPipeline) : base(environment, markdownPipeline)
        {
            _environment = environment;
            _markdownPipeline = markdownPipeline;
        }

        public override IEnumerable<PostModel> Get()
        {
            Initialize(_environment, _markdownPipeline);

            return base.Get();
        }
    }
}