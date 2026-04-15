
using TicketManagementSystem.Application.DTOs;
using TicketManagementSystem.Domain.Entities;

public interface ITicketService
{
    void CreateTicket(CreateTicketDto dto);
    List<Ticket> GetAllTickets();
    Ticket GetTicketById(int id);
}