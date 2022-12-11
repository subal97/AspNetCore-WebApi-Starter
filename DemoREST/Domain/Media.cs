using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoREST.Domain
{
    public class Media
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }

        public string PostId { get; set; }

    }
}
