using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Domain.Entities;

namespace TicketManagementSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        { 
        }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

    }
}
