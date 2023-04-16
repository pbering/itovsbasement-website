using System.Collections.Generic;
using WebApp.Models;

namespace WebApp.Data
{
    public interface IPostRepository
    {
        IEnumerable<PostModel> Get();
    }
}