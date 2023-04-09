using Microsoft.EntityFrameworkCore;

namespace Events.API.Data
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    
    public  DbSet<Participant> Participants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // User - OwnedEvents relationship (One-to-Many)
      modelBuilder.Entity<User>()
        .HasMany(u => u.OwnedEvents)
        .WithOne()
        .HasForeignKey(e => e.OwnerId)
        .OnDelete(DeleteBehavior.Cascade);

      // User - RegisteredEvents relationship (Many-to-Many)
      modelBuilder.Entity<Participant>()
        .HasIndex(p => new { p.UserId, p.EventId }).IsUnique();

      modelBuilder.Entity<Participant>()
        .HasOne<User>()
        .WithMany(u => u.RegisteredEvents)
        .HasForeignKey(p => p.UserId);

      modelBuilder.Entity<Participant>()
        .HasOne<Event>()
        .WithMany(e => e.Participants)
        .HasForeignKey(p => p.EventId);

      // User - Invites relationship (One-to-Many)
      modelBuilder.Entity<User>()
        .HasMany(u => u.Invites)
        .WithOne()
        .HasForeignKey(i => i.InvitedId)
        .OnDelete(DeleteBehavior.Cascade);

      // Event - Invites relationship (One-to-Many)
      modelBuilder.Entity<Event>()
        .HasMany(e => e.Invites)
        .WithOne()
        .HasForeignKey(i => i.EventId)
        .OnDelete(DeleteBehavior.Cascade);
    }

  }
}