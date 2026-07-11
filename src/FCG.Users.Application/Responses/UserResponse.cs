using FCG.BuildingBlocks.Enums;
using FCG.Users.Domain.Enums;

namespace FCG.Users.Application.Responses;

public class UserResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserProfile Profile { get; set; }

    public StatusType Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}