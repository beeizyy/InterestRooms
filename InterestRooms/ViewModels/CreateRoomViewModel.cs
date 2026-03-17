using System.ComponentModel.DataAnnotations;

namespace InterestRooms.ViewModels
{
    public class CreateRoomViewModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string Category { get; set; } = null!;
    }
}