using System;
using System.Collections.Generic;

namespace InterestRooms.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? ProfileImagePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public ICollection<Room> CreatedRooms { get; set; } = new List<Room>();
        public ICollection<RoomMember> RoomMemberships { get; set; } = new List<RoomMember>();
        public ICollection<RoomNickname> RoomNicknames { get; set; } = new List<RoomNickname>();
        public ICollection<MessageLike> MessageLikes { get; set; } = new List<MessageLike>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}