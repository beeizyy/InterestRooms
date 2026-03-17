using System;

namespace InterestRooms.Models
{
    public class RoomNickname
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public string Nickname { get; set; } = null!;

        public Room Room { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}