
using TicketManagementSystem.Application.DTOs;
using TicketManagementSystem.Domain.Entities;

public interface ITicketService
{
    void CreateTicket(CreateTicketDto dto);
    List<Ticket> GetAllTickets();
    PaginatedResult<Ticket> GetTicketsPaged(TicketFilterDto filter, string? currentUserId);
    Ticket GetTicketById(int id);
    void AssignTicket(int ticketId, string userId);
    void ResolveTicket(int id);
}