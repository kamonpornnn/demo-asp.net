using System.ComponentModel.DataAnnotations;

namespace WeblogAspNet.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อผู้ใช้")]
        [MaxLength(50, ErrorMessage = "ชื่อผู้ใช้ต้องไม่เกิน 50 ตัวอักษร")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "กรุณายืนยันรหัสผ่าน")]
        [MinLength(6, ErrorMessage = "รหัสผ่านต้องมีความยาวไม่น้อยกว่า 6 ตัวอักษร")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{6,}$", ErrorMessage = "รหัสผ่านต้องประกอบด้วยตัวอักษรใหญ่ ตัวอักษรเล็ก และตัวเลข")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "รหัสผ่านและการยืนยันรหัสผ่านไม่ตรงกัน")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อจริง")]
        [MaxLength(50, ErrorMessage = "ชื่อจริงต้องไม่เกิน 50 ตัวอักษร")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "กรุณากรอกนามสกุล")]
        [MaxLength(50, ErrorMessage = "นามสกุลต้องไม่เกิน 50 ตัวอักษร")]
        public string? Lastname { get; set; }
    }

}
