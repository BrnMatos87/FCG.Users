using FCG.Users.Application.Abstractions.Queries;
using FCG.Users.Application.Contracts;
using FCG.Users.Application.Responses;

namespace FCG.Users.Application.Queries.Users.Handlers;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, UserResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponse?> HandleAsync(GetUserByIdQuery query, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(query.Id, ct);

        if (user is null)
            return null;

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Profile = user.Profile,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}