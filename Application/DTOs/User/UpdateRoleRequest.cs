namespace Application.DTOs.User;

public record UpdateRoleRequest(
    string Role  // "Player" | "Moderator" 
);