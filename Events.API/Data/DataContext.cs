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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

      modelBuilder.Entity<Invitation>()
          .HasOne<Event>(i => i.Event)
          .WithMany(e => e.Invitations)
          .HasForeignKey(i => i.EventId)
          .OnDelete(DeleteBehavior.Cascade);

      modelBuilder.Entity<Invitation>()
          .HasOne<User>(i => i.EventOwner)
          .WithMany(i => i.Invites)
          .HasForeignKey(i => i.EventOwnerId)
          .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<Event>()
      .HasOne(e => e.Owner)
      .WithMany(u => u.Events)
      .HasForeignKey(e => e.OwnerId)
      .OnDelete(DeleteBehavior.Restrict);

      modelBuilder.Entity<User>()
        .HasMany(u => u.Events)
        .WithOne(e => e.Owner)
        .HasForeignKey(e => e.OwnerId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);





    }

  }
}