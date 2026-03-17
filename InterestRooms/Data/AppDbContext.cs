using InterestRooms.Models;
using Microsoft.EntityFrameworkCore;

namespace InterestRooms.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomMember> RoomMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RoomNickname> RoomNicknames { get; set; }
        public DbSet<MessageLike> MessageLikes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.CreatedByUser)
                .WithMany(u => u.CreatedRooms)
                .HasForeignKey(r => r.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RoomMember>()
                .HasOne(rm => rm.Room)
                .WithMany(r => r.Members)
                .HasForeignKey(rm => rm.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomMember>()
                .HasOne(rm => rm.User)
                .WithMany(u => u.RoomMemberships)
                .HasForeignKey(rm => rm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Room)
                .WithMany(r => r.Messages)
                .HasForeignKey(m => m.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RoomNickname>()
        .HasOne(rn => rn.Room)
        .WithMany(r => r.RoomNicknames)
        .HasForeignKey(rn => rn.RoomId)
        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomNickname>()
                .HasOne(rn => rn.User)
                .WithMany(u => u.RoomNicknames)
                .HasForeignKey(rn => rn.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoomNickname>()
                .HasIndex(rn => new { rn.RoomId, rn.UserId })
                .IsUnique();
            modelBuilder.Entity<MessageLike>()
         .HasOne(ml => ml.Message)
         .WithMany(m => m.Likes)
         .HasForeignKey(ml => ml.MessageId)
         .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageLike>()
                .HasOne(ml => ml.User)
                .WithMany(u => u.MessageLikes)
                .HasForeignKey(ml => ml.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageLike>()
                .HasIndex(ml => new { ml.MessageId, ml.UserId })
                .IsUnique();

        }


    }
}