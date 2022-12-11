using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoREST.Domain
{
    public class Tag
    {
        [Key]
        public string TagName { get; set; }

        public virtual IList<PostTag> Posts{ get; set; }
    }
}
