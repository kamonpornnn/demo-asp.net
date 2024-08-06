using System.ComponentModel.DataAnnotations;

namespace WeblogAspNet.Data
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage ="กรุณากรอกหมวดหมู่")]
        [MaxLength(50)]
        public string CategoryName { get; set; }

    }
}
