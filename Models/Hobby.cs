using System.ComponentModel.DataAnnotations;

namespace Labb3Api.Models
{
    public class Hobby
    {
        [Key]
        public int HobbyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Link> Links { get; set; }
    }
}
