using System.ComponentModel.DataAnnotations.Schema;

namespace DemoREST.Domain
{
    public class PostTag
    {
        public Guid PostId { get; set; }
        
        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

        public string TagName { get; set; }

        [ForeignKey(nameof(TagName))]
        public virtual Tag Tag { get; set; }

    }
}
