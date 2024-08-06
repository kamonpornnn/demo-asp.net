using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeblogAspNet.Models;
using WeblogAspNet.Models.Service;

namespace WeblogAspNet.Controllers
{
    [Controller]
    public class AuthController : Controller
    {
        private readonly JwtTokenService _tokenService;
        private readonly AuthService _authService;

        public AuthController(JwtTokenService tokenService, AuthService authService)
        {
            _tokenService = tokenService;
            _authService = authService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "ข้อมูลที่กรอกไม่ถูกต้อง";
                return View(model);
            }

            try
            {
                bool result = await _authService.RegisterUser(model);
                if (result)
                {
                    TempData["SuccessMessage"] = "สมัครสมาชิกสำเร็จ!";
                    return RedirectToAction("Login", "Auth");
                }
                else
                {
                    TempData["ErrorMessage"] = "เกิดข้อผิดพลาดขณะสมัครสมาชิก";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "เกิดข้อผิดพลาดขณะสมัครสมาชิก";
                return View(model);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _authService.AuthenticateUser(login.Username, login.Password);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
                    return RedirectToAction("Login", "Auth");
                }
                else if (user.IsClose)
                {
                    TempData["ErrorMessage"] = "ผู้ใช้ถูกแบนกรุณาติดต่อแอดมิน";
                    return RedirectToAction("Login", "Auth");
                }

                var tokenString = _tokenService.GenerateToken(user);

                // เก็บโทเค็นในคุกกี้
                Response.Cookies.Append("jwt", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                TempData["SuccessMessage"] = "เข้าสู่ระบบสำเร็จ";
                return user.IsAdmin ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "BlogPost");
            }
            catch (Exception ex)
            {
                // บันทึกข้อผิดพลาดสำหรับการดีบัก
                return StatusCode(500, "เกิดข้อผิดพลาดภายในเซิร์ฟเวอร์");
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Delete("jwt");
                TempData["SuccessMessage"] = "ออกจากระบบสำเร็จ";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "เกิดข้อผิดพลาดในการออกจากระบบ");
            }
        }
    }
}