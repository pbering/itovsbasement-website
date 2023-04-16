﻿using ColorCode.Styling;
using Markdig;
using Markdown.ColorCode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using WebApp.Data;
using WebApp.Middleware;

namespace WebApp
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;

        public Startup(IWebHostEnvironment env)
        {
            _environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddResponseCaching();
            services.AddResponseCompression();
            services.AddSingleton(x => new MarkdownPipelineBuilder().UseAdvancedExtensions().UseColorCode(StyleDictionary.DefaultDark).Build());
                        
            if (_environment.IsDevelopment())
            {
                services.AddSingleton<IPostRepository, DevelopmentPostRepository>();
            }
            else
            {
                services.AddSingleton<IPostRepository, PostRepository>();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                HttpContextExtensions.AlwaysUseHttp = true;
            }

            using (var rules = File.OpenText(Path.Combine(env.ContentRootPath, "iis-rewrite-rules.xml")))
            {
                app.UseRewriter(new RewriteOptions().AddIISUrlRewrite(rules));
            }

            var contentTypeMappings = new FileExtensionContentTypeProvider();

            contentTypeMappings.Mappings[".css"] = "text/css; charset=utf-8";

            app.UseStaticFiles(new StaticFileOptions
            {
                ContentTypeProvider = contentTypeMappings,
                OnPrepareResponse = ctx =>
                {
                    if (!env.IsDevelopment())
                    {
                        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + TimeSpan.FromDays(365).TotalSeconds;
                    }
                }
            });

            app.UseHsts(hsts => hsts.AllResponses().MaxAge(365).IncludeSubdomains());
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.Deny());
            app.UseCsp(opts => opts
                               .BlockAllMixedContent()
                               .StyleSources(s => s.Self())
                               .StyleSources(s => s.UnsafeInline())
                               .FontSources(s => s.Self())
                               .FormActions(s => s.Self())
                               .FrameAncestors(s => s.Self())
                               .ImageSources(s => s.Self())
                               .ScriptSources(s => s.Self())
                      );

            if (!env.IsDevelopment())
            {
                app.UseResponseCaching();
                //app.UseResponseCompression();
            }

            app.UseMiddleware<RobotsTxtMiddleware>();
            app.UseMiddleware<SitemapMiddleware>();
            app.UseMiddleware<RssMiddleware>();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}