using Microsoft.AspNetCore.Http;

namespace InterestRooms.ViewModels
{
    public class ProfileViewModel
    {
        public string Username { get; set; } = null!;
        public string? CurrentProfileImagePath { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}