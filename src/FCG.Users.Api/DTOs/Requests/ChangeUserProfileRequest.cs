using FCG.Users.Domain.Enums;

namespace FCG.Users.Api.DTOs.Requests;

public class ChangeUserProfileRequest
{
    public UserProfile Profile { get; set; }
}