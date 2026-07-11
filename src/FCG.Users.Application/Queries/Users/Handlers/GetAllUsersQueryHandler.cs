using FCG.Users.Application.Abstractions.Queries;
using FCG.Users.Application.Contracts;
using FCG.Users.Application.Responses;

namespace FCG.Users.Application.Queries.Users.Handlers;

public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, IList<UserResponse>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IList<UserResponse>> HandleAsync(GetAllUsersQuery query, CancellationToken ct = default)
    {
        var users = await _userRepository.GetAllAsync(ct);

        return users
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Profile = user.Profile,
                Status = user.Status,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            })
            .ToList();
    }
}