using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeblogAspNet.Data;

namespace WeblogAspNet.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDBContext _context;

        public AdminController(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var newPostsCount = await _context.Posts
                .CountAsync(p => p.CreatedAt >= DateTime.Now.AddDays(-30));

            var totalUsersCount = await _context.Users.CountAsync();

            var totalCategoriesCount = await _context.Categories.CountAsync();

            var viewModel = (NewPostsCount: newPostsCount, TotalUsersCount: totalUsersCount, TotalCategoriesCount: totalCategoriesCount);
            return View(viewModel);
        }

        private List<T> Paginate<T>(List<T> items, int page, int pageSize)
        {
            return items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        private void SetPaginationViewBag(int page, int pageSize, int totalItems)
        {
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        }

        public async Task<IActionResult> ManageUsers(int page = 1, int pageSize = 10)
        {
            var users = await _context.Users
                .OrderByDescending(u => !u.IsClose)
                .ThenByDescending(u => u.IsAdmin)
                .ThenByDescending(u => u.CreatedAt)
                .ToListAsync();

            var paginatedUsers = Paginate(users, page, pageSize);
            SetPaginationViewBag(page, pageSize, users.Count);

            return View(paginatedUsers);
        }

        public async Task<IActionResult> ManagePosts(int page = 1, int pageSize = 10)
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => !p.IsBanned)
                .ThenByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.UpdatedAt)
                .ToListAsync();

            var paginatedPosts = Paginate(posts, page, pageSize);
            SetPaginationViewBag(page, pageSize, posts.Count);

            return View(paginatedPosts);
        }

        [HttpPost]
        public async Task<IActionResult> TogglePostStatus(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            post.IsBanned = !(post.IsBanned ?? false);
            var status = (post.IsBanned ?? false) ? "แบน" : "ปลดแบน";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"โพสต์ถูก {status} สำเร็จ";
            return RedirectToAction("ManagePosts");
        }

        public async Task<IActionResult> ViewPost(int id)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.IsClose = !user.IsClose;
            var status = user.IsClose ? "แบน" : "ปลดแบน";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"สถานะผู้ใช้ถูก {status} สำเร็จ";
            return RedirectToAction("ManageUsers");
        }

        public async Task<IActionResult> ManageCategory(int page = 1, int pageSize = 10)
        {
            var categories = await _context.Categories.ToListAsync();

            var paginatedCategories = Paginate(categories, page, pageSize);
            SetPaginationViewBag(page, pageSize, categories.Count);

            return View(paginatedCategories);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryForm(int? id)
        {
            if (id == null)
            {
                return View(new Category());
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CategoryForm(Category model)
        {
            if (ModelState.IsValid)
            {
                if (model.CategoryId == 0)
                {
                    _context.Categories.Add(model);
                    TempData["SuccessMessage"] = "หมวดหมู่ถูกเพิ่มสำเร็จ";
                }
                else
                {
                    _context.Categories.Update(model);
                    TempData["SuccessMessage"] = "หมวดหมู่ถูกแก้ไขสำเร็จ";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("ManageCategory");
            }

            return View(model);
        }
    }
}