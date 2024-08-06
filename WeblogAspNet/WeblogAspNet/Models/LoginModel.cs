using System.ComponentModel.DataAnnotations;

namespace WeblogAspNet.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
