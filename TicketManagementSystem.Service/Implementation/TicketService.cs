using Microsoft.EntityFrameworkCore;
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

    public List<Ticket> GetAllTickets()
    {
        return _context.Tickets
            .Include(x => x.BranchName)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }
    public Ticket GetTicketById(int id)
    {
        return _context.Tickets
            .Include(x => x.BranchName)
            .FirstOrDefault(x => x.Id == id);
    }
}