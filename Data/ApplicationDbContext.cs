using Labb3Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Labb3Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Hobby> Hobbys { get; set; }
        public DbSet<Link> Links { get; set; }
    }
}
