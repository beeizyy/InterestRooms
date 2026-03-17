using System.ComponentModel.DataAnnotations;

namespace InterestRooms.ViewModels
{
    public class JoinRoomViewModel
    {
        public int RoomId { get; set; }
        public string? RoomName { get; set; }

        [Required]
        public string Nickname { get; set; } = null!;
    }
}