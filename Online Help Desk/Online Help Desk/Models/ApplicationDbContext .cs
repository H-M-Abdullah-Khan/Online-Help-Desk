using Microsoft.EntityFrameworkCore;

namespace Online_Help_Desk.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Users> Users { get; set; }
    }
}
