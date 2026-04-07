using TicketManagementSystem.Application.DTOs;
using TicketManagementSystem.Common.Enum;
using TicketManagementSystem.Domain.Entities;
using TicketManagementSystem.Infrastructure.Data;
public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;

    public TicketService(ApplicationDbContext context)
    {
        _context = context;
    }

    public void CreateTicket(CreateTicketDto dto)
    {
        var ticket = new Ticket
        {
            Title = dto.Title,
            Description = dto.Description,
            IssueType = dto.IssueType,
            Priority = dto.Priority,
            BranchId = dto.BranchId,

            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);
        _context.SaveChanges();
    }
}