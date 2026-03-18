using System;
using System.Collections.Generic;

namespace InterestRooms.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsPrivate { get; set; } = false;

        public User CreatedByUser { get; set; } = null!;
        public ICollection<RoomMember> Members { get; set; } = new List<RoomMember>();
        public ICollection<RoomNickname> RoomNicknames { get; set; } = new List<RoomNickname>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<JoinRequest> JoinRequests { get; set; } = new List<JoinRequest>();

    }
}