using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    public string Username { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; } = UserRole.Player;
    public bool IsBlocked { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public DateTime? BlockedUntil { get; private set; }
    
    public Character? Character { get; private set; }
    public IReadOnlyCollection<FriendRequest> SentFriendRequests => _sentFriendRequests.AsReadOnly();
    public IReadOnlyCollection<FriendRequest> ReceivedFriendRequests => _receivedFriendRequests.AsReadOnly();
    public IReadOnlyCollection<SupportTicket> SupportTickets => _supportTickets.AsReadOnly();

    private readonly List<FriendRequest> _sentFriendRequests = [];
    private readonly List<FriendRequest> _receivedFriendRequests = [];
    private readonly List<SupportTicket> _supportTickets = [];

    private User() { }

    public static User Create(string username, Email email, string passwordHash, DateTime? dateOfBirth = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        return new User
        {
            Username     = username,
            Email        = email,
            PasswordHash = passwordHash,
            DateOfBirth  = dateOfBirth
        };
    }

    public void Block()   => IsBlocked = true;
    public void Unblock() => IsBlocked = false;

    public void ChangeRole(UserRole role)
    {
        Role = role;
        SetUpdatedAt();
    }

    public void UpdatePassword(string newHash)
    {
        PasswordHash = newHash;
        SetUpdatedAt();
    }
}