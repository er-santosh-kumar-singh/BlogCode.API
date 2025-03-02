using BlogCode.API.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace BlogCode.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }       

    }
}
