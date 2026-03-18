namespace InterestRooms.ViewModels
{
    public class JoinRequestDisplayViewModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Nickname { get; set; } = null!;
        public DateTime RequestedAt { get; set; }
    }
}