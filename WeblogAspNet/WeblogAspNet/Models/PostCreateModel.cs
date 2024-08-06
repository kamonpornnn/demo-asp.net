using WeblogAspNet.Data;

namespace WeblogAspNet.Models
{
    public class PostCreateModel
    {
        public Post? Post { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
