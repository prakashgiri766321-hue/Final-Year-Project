using TicketManagementSystem.Common.Enum;

namespace TicketManagementSystem.Domain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public IssueType IssueType { get; set; }    
        public TicketPriority Priority { get; set; }
        public TicketStatus Status { get; set; }

        public int BranchId {  get; set; }
        public Branch BranchName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }

    }
}
