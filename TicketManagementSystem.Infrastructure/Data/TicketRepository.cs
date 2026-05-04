using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Domain.Entities;
using TicketManagementSystem.Domain.Interface;
using TicketManagementSystem.Infrastructure.Data;

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;

    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public Ticket? GetById(int id)
    {
        return _context.Tickets.Find(id);
    }

    public Ticket? GetByIdWithBranch(int id)
    {
        return _context.Tickets
            .Include(x => x.BranchName)
            .FirstOrDefault(x => x.Id == id);
    }

    public IQueryable<Ticket> Query()
    {
        return _context.Tickets;
    }

    public IQueryable<Ticket> QueryWithBranch()
    {
        return _context.Tickets
            .AsNoTracking()
            .Include(x => x.BranchName);
    }

    public List<Ticket> GetAllWithBranchOrderedDesc()
    {
        return _context.Tickets
            .Include(x => x.BranchName)
            .OrderByDescending(x => x.CreatedAt)
            .ToList();
    }
}
