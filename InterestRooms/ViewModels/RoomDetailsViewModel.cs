using InterestRooms.Models;
using System.ComponentModel.DataAnnotations;

namespace InterestRooms.ViewModels
{
    public class RoomDetailsViewModel
    {
        public Room Room { get; set; } = null!;
        public List<MessageDisplayViewModel> Messages { get; set; } = new List<MessageDisplayViewModel>();

        [Required]
        public string NewMessageContent { get; set; } = null!;

        public int? ReplyToMessageId { get; set; }
        
    }

    public class MessageDisplayViewModel
    {
        public int MessageId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime SentAt { get; set; }
        public string Nickname { get; set; } = null!;
        public string? ProfileImagePath { get; set; }

        public int? ReplyToMessageId { get; set; }
        public string? ReplyToNickname { get; set; }
        public string? ReplyToContent { get; set; }

        public int LikeCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public bool IsOwnMessage { get; set; }
    }
}