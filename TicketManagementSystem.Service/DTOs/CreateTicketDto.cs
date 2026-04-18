using System.ComponentModel.DataAnnotations;
using TicketManagementSystem.Common.Enum;

namespace TicketManagementSystem.Application.DTOs
{
    public class CreateTicketDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public IssueType IssueType { get; set; }

        [Required]
        public TicketPriority Priority { get; set; }

        [Required]
        public int BranchId { get; set; }

        public string CreatedById { get; set; }
    }
}