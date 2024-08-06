using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeblogAspNet.Data
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required(ErrorMessage = "กรุณากรอกหัวข้อ")]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "กรุณากรอกเนื้อหา")]
        public string? Content { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsBanned { get; set; } = false;

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public AppUser? User { get; set; }

        [ForeignKey("Category")]
        [Required(ErrorMessage = "กรุณาเลือกหมวดหมู่")]
        public int? CategoryId { get; set; }
    }
}
