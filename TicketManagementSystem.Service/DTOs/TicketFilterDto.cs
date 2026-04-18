using TicketManagementSystem.Common.Enum;

namespace TicketManagementSystem.Application.DTOs
{
    public class TicketFilterDto
    {
        public TicketStatus? Status { get; set; }

        public int? BranchId { get; set; }

        public bool MyTickets { get; set; }

        public string? Search { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
