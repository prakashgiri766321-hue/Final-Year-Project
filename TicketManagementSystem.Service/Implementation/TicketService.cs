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
            CreatedAt = DateTime.UtcNow,
            CreatedById=dto.CreatedById
        };

        _context.Tickets.Add(ticket);
        _context.SaveChanges();
    }
    public void AssignTicket(int ticketId, string userId)
    {
        var ticket = _context.Tickets.Find(ticketId);

        if (ticket == null) return;

        ticket.AssignedToId = userId;

        _context.SaveChanges();
    }

    public void ResolveTicket(int id)
    {
        var ticket = _context.Tickets.Find(id);

        if (ticket == null) return;

        ticket.Status = TicketStatus.Closed;
        ticket.ResolvedAt = DateTime.UtcNow;

        _context.SaveChanges();
    }

    public List<Ticket> GetAllTickets()
    {
        return _context.Tickets
            .Include(x => x.BranchName)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }

    public PaginatedResult<Ticket> GetTicketsPaged(TicketFilterDto filter, string? currentUserId)
    {
        IQueryable<Ticket> query = _context.Tickets.AsNoTracking().Include(x => x.BranchName);

        if (filter.Status.HasValue)
            query = query.Where(t => t.Status == filter.Status.Value);

        if (filter.BranchId.HasValue)
            query = query.Where(t => t.BranchId == filter.BranchId.Value);

        if (filter.MyTickets && !string.IsNullOrEmpty(currentUserId))
            query = query.Where(t => t.AssignedToId == currentUserId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            query = query.Where(t =>
                (t.Title != null && t.Title.Contains(term)) ||
                (t.Description != null && t.Description.Contains(term)));
        }

        var totalCount = query.Count();

        var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;
        var pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;

        var items = query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PaginatedResult<Ticket>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
    public Ticket GetTicketById(int id)
    {
        return _context.Tickets
            .Include(x => x.BranchName)
            .FirstOrDefault(x => x.Id == id);
    }
}