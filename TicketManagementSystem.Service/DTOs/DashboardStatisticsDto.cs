namespace TicketManagementSystem.Application.DTOs
{
    public class TicketCategoryCountDto
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class MonthlyTicketCountDto
    {
        public string Month { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class DashboardStatisticsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ClosedTickets { get; set; }
        public List<TicketCategoryCountDto> CategoryCounts { get; set; } = new();
        public List<MonthlyTicketCountDto> MonthlyTicketCounts { get; set; } = new();
    }
}
