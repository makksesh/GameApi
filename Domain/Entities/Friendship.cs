using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// Результат принятого запроса — двусторонняя дружба.
/// Создаётся в Application после Accept().
/// </summary>
public class Friendship : BaseEntity
{
    public Guid UserId1 { get; private set; }
    public Guid UserId2 { get; private set; }

    private Friendship() { }

    public static Friendship Create(Guid userId1, Guid userId2)
        => new() { UserId1 = userId1, UserId2 = userId2 };
}