using Microsoft.EntityFrameworkCore;

namespace Online_Help_Desk.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
        public DbSet<Users> Users { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
    }
}
