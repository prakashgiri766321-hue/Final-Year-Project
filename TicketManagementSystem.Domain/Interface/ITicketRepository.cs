using TicketManagementSystem.Domain.Entities;

namespace TicketManagementSystem.Domain.Interface
{
    public interface ITicketRepository
    {
        void Add(Ticket ticket);
        void SaveChanges();
        Ticket? GetById(int id);
        Ticket? GetByIdWithBranch(int id);
        IQueryable<Ticket> Query();
        IQueryable<Ticket> QueryWithBranch();
        List<Ticket> GetAllWithBranchOrderedDesc();
    }
}
