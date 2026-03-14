namespace Application.DTOs.Support;

public record CreateTicketRequest(
    string Subject,
    string Message
);