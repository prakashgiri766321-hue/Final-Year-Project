using TicketManagementSystem.Application.DTOs;
using TicketManagementSystem.Common.Enum;
using TicketManagementSystem.Domain.Entities;
using TicketManagementSystem.Domain.Interface;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;

    public TicketService(ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
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

        _ticketRepository.Add(ticket);
        _ticketRepository.SaveChanges();
    }
    public void AssignTicket(int ticketId, string userId)
    {
        var ticket = _ticketRepository.GetById(ticketId);

        if (ticket == null) return;

        ticket.AssignedToId = userId;

        _ticketRepository.SaveChanges();
    }

    public void ResolveTicket(int id)
    {
        var ticket = _ticketRepository.GetById(id);

        if (ticket == null) return;

        ticket.Status = TicketStatus.Closed;
        ticket.ResolvedAt = DateTime.UtcNow;

        _ticketRepository.SaveChanges();
    }

    public List<Ticket> GetAllTickets()
    {
        return _ticketRepository.GetAllWithBranchOrderedDesc();
    }

    public PaginatedResult<Ticket> GetTicketsPaged(TicketFilterDto filter, string? currentUserId)
    {
        IQueryable<Ticket> query = _ticketRepository.QueryWithBranch();

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
    public Ticket? GetTicketById(int id)
    {
        return _ticketRepository.GetByIdWithBranch(id);
    }

    public List<TicketCategoryCountDto> GetTicketCategoryCounts()
    {
        var grouped = _ticketRepository.Query()
            .GroupBy(t => t.IssueType)
            .Select(g => new { IssueType = g.Key, Count = g.Count() })
            .ToDictionary(x => x.IssueType, x => x.Count);

        return new List<TicketCategoryCountDto>
        {
            new() { Category = "Hardware", Count = grouped.GetValueOrDefault(IssueType.Hardware, 0) },
            new() { Category = "Software", Count = grouped.GetValueOrDefault(IssueType.Software, 0) },
            new() { Category = "Network", Count = grouped.GetValueOrDefault(IssueType.Network, 0) }
        };
    }

    public List<MonthlyTicketCountDto> GetMonthlyTicketCreationStats()
    {
        return _ticketRepository.Query()
            .GroupBy(t => new { t.CreatedAt.Year, t.CreatedAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .AsEnumerable()
            .Select(x => new MonthlyTicketCountDto
            {
                Month = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
                Count = x.Count
            })
            .ToList();
    }

    public DashboardStatisticsDto GetDashboardStatistics()
    {
        var query = _ticketRepository.Query();

        return new DashboardStatisticsDto
        {
            TotalTickets = query.Count(),
            OpenTickets = query.Count(t => t.Status == TicketStatus.Open),
            InProgressTickets = query.Count(t => t.Status == TicketStatus.InProgress),
            ClosedTickets = query.Count(t => t.Status == TicketStatus.Closed || t.Status == TicketStatus.Resolved),
            CategoryCounts = GetTicketCategoryCounts(),
            MonthlyTicketCounts = GetMonthlyTicketCreationStats()
        };
    }
}