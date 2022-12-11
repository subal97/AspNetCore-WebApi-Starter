using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoREST.Domain
{
    public class Post
    {
        [Key]
        public Guid PostId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        public virtual IList<PostTag> Tags { get; set; }

    }
}
