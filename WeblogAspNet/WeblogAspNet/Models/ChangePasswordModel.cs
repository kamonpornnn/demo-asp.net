using System.ComponentModel.DataAnnotations;

namespace WeblogAspNet.Models
{
    public class ChangePasswordModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "กรุณากรอกรหัสผ่านเดิม")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "กรุณากรอกรหัสผ่านใหม่")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "รหัสผ่านไม่ตรงกัน")]
        [Required(ErrorMessage = "กรุณากรอกรหัสผ่านใหม่อีกรอบ")]
        public string? ConfirmPassword { get; set; }
    }
}
