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
    public class ProfileController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ProfileController(ApplicationDBContext context)
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

        private async Task<AppUser> GetUserAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var user = await GetUserAsync(userId.Value);

                if (user == null)
                {
                    return NotFound();
                }

                var model = new EditProfileModel
                {
                    Username = user.Username,
                    Firstname = user.Firstname,
                    Lastname = user.Lastname
                };

                return View(model);
            }
            catch (Exception)
            {
                TempData["systemErrorMessage"] = "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(EditProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = GetUserId();

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var user = await GetUserAsync(userId.Value);

                if (user == null)
                {
                    return NotFound();
                }

                user.Firstname = model.Firstname;
                user.Lastname = model.Lastname;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "โปรไฟล์ถูกแก้ไขสำเร็จ";
                return View(model);
            }
            catch (Exception)
            {
                TempData["systemErrorMessage"] = "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ";
                return View(model);
            }
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "ข้อมูลไม่ถูกต้อง";
                return View(model);
            }

            var userId = GetUserId();

            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            try
            {
                var user = await GetUserAsync(userId.Value);

                if (user == null)
                {
                    return NotFound();
                }

                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
                {
                    TempData["ErrorMessage"] = "รหัสผ่านปัจจุบันไม่ถูกต้อง";
                    return View(model);
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "รหัสผ่านถูกเปลี่ยนแปลงสำเร็จ";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["systemErrorMessage"] = "เกิดข้อผิดพลาด กรุณาลองใหม่อีกครั้งหรือติดต่อผู้ดูแลระบบ";
                return View(model);
            }
        }
    }
}