using System.ComponentModel.DataAnnotations;

namespace WeblogAspNet.Models
{
    public class EditProfileModel
    {
        public string? Username { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อจริง")]
        [MaxLength(50, ErrorMessage = "ชื่อจริงต้องมีความยาวไม่เกิน 50 ตัวอักษร")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "กรุณากรอกนามสกุล")]
        [MaxLength(50, ErrorMessage = "นามสกุลต้องมีความยาวไม่เกิน 50 ตัวอักษร")]
        public string? Lastname { get; set; }
    }
}
