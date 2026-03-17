using System;

namespace InterestRooms.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;

        public int? ReplyToMessageId { get; set; }

        public Room Room { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<MessageLike> Likes { get; set; } = new List<MessageLike>();
    }
}