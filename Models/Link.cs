using System.ComponentModel.DataAnnotations;

namespace Labb3Api.Models
{
    public class Link
    {
        [Key]
        public int LinkId { get; set; }
        public string Url { get; set; }
    }
}
