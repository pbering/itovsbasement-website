using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApp.Components
{
    public class TagsViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
           return View();
        }
    }
}
