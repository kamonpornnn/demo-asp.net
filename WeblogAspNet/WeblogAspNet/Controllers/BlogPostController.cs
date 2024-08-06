using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WeblogAspNet.Data;
using WeblogAspNet.Models;
using System.Threading.Tasks;

namespace WeblogAspNet.Controllers
{
    [Authorize]
    public class BlogPostController : Controller
    {
        private readonly ApplicationDBContext _context;

        public BlogPostController(ApplicationDBContext context)
        {
            _context = context;
        }

        private int? GetUserId()
        {
            if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return userId;
            }
            return null;
        }
        private async Task<List<Post>> GetBlogPostsAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                return await _context.Posts
                .Where(bp => bp.UserId == userId && bp.IsBanned == false)
                .OrderByDescending(bp => bp.CreatedAt)
                .ThenByDescending(bp => bp.UpdatedAt)
                .ToListAsync();
            }
            else
            {
                return new List<Post>();
            }
        }

        private async Task<Post?> FindPostByIdAsync(int id)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PostId == id);

            var userId = GetUserId();
            return post != null && (userId == null || post.UserId != userId.Value) ? null : post;
        }

        private async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var blogPosts = await GetBlogPostsAsync();
                var categories = await GetCategoriesAsync();

                var model = (BlogPosts: blogPosts, Categories: categories);
                return View(model);
            }
            catch (Exception)
            {
                TempData["systemErrorMessage"] = "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ";
                return View();
            }
        }

        private async Task<IActionResult> RenderPostViewWithCategoriesAsync(Post post)
        {
            var categories = await GetCategoriesAsync();
            var model = new PostCreateModel
            {
                Post = post,
                Categories = categories
            };
            return View(model);
        }

        public async Task<IActionResult> Post(int? id)
        {
            try
            {
                var post = id.HasValue ? await FindPostByIdAsync(id.Value) : new Post();

                if (id.HasValue && post == null)
                {
                    return NotFound();
                }

                return await RenderPostViewWithCategoriesAsync(post);
            }
            catch (Exception)
            {
                TempData["SystemErrorMessage"] = "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(int id, [Bind("PostId,Title,Content,Status,CategoryId")] Post post)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "ข้อมูลที่กรอกไม่ถูกต้อง";
                return await RenderPostViewWithCategoriesAsync(post);
            }

            try
            {
                post.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                post.UpdatedAt = DateTime.Now;

                if (post.PostId == 0)
                {
                    post.CreatedAt = DateTime.Now;
                    _context.Add(post);
                    TempData["SuccessMessage"] = "บันทึกสำเร็จ";
                }
                else
                {
                    var postForDb = await FindPostByIdAsync(id);
                    if (postForDb == null)
                    {
                        return NotFound();
                    }

                    post.CreatedAt = postForDb.CreatedAt;
                    _context.Update(post);
                    TempData["SuccessMessage"] = "อัปเดตสำเร็จ";
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["SystemErrorMessage"] = "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ";
                return await RenderPostViewWithCategoriesAsync(post);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await FindPostByIdAsync(id);
            if (post == null)
            {
                TempData["systemErrorMessage"] = "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ";
                return RedirectToAction("Index");
            }

            try
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "ลบโพสต์สำเร็จ";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["systemErrorMessage"] = "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ";
                return RedirectToAction("Index");
            }
        }
    }
}