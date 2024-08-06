using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WeblogAspNet.Data;
using WeblogAspNet.Models;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace WeblogAspNet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDBContext _context;

        public HomeController(ApplicationDBContext context)
        {
            _context = context;
        }

        private async Task<List<Post>> GetBlogPostsAsync()
        {
            return await _context.Posts
                .Where(bp => bp.IsBanned == false)
                .OrderByDescending(bp => bp.CreatedAt)
                .ThenByDescending(bp => bp.UpdatedAt)
                .ToListAsync();
        }

        private async Task<List<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<IActionResult> Index()
        {
            var blogPosts = await GetBlogPostsAsync();
            var users = await GetUsersAsync();
            var model = (BlogPosts: blogPosts, Users: users);

            return View(model);
        }
    }
}
