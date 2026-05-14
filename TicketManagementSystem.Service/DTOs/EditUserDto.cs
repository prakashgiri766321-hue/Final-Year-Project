using System.ComponentModel.DataAnnotations;

namespace TicketManagementSystem.Application.DTOs
{
    public class EditUserDto
    {
        public string Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}