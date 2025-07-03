using Microsoft.EntityFrameworkCore;

namespace Online_Help_Desk.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prevent cascade delete on UpdatedByUser → User (to avoid multiple cascade paths)
            modelBuilder.Entity<StatusHistory>()
                .HasOne(sh => sh.UpdatedByUser)
                .WithMany()
                .HasForeignKey(sh => sh.UpdatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent cascade delete on AssignedToUser → User (same reason)
            modelBuilder.Entity<Request>()
                .HasOne(r => r.AssignedToUser)
                .WithMany()
                .HasForeignKey(r => r.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
