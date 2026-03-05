using ECommerce.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Identity.Queries;

public class GetUsersHandler : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IApplicationDbContext _db;
    public GetUsersHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery q, CancellationToken ct)
    {
        return await _db.Users.AsNoTracking()
            .Select(u => new UserDto(u.Id, u.FirstName, u.LastName, u.Email, u.Role.ToString(), u.CreatedAt))
            .ToListAsync(ct);
    }
}
