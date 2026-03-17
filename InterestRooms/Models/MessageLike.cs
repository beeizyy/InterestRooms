namespace InterestRooms.Models
{
    public class MessageLike
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }

        public Message Message { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}