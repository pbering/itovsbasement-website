using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Data
{
    public interface IPostRepository
    {
        Task<IEnumerable<PostModel>> GetAsync();
    }
}