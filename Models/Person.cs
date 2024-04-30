using System.ComponentModel.DataAnnotations;

namespace Labb3Api.Models
{
    public class Person
    {
        [Key]
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string Email { get; set; }
        public string Phonenumber { get; set; }
        public List<Hobby> Hobbies { get; set; }
    }
}
