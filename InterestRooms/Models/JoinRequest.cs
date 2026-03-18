using System;

namespace InterestRooms.Models
{
    public class JoinRequest
    {
        public int Id { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Nickname { get; set; } = null!;

        public DateTime RequestedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";
    }
}