using MediatR;

namespace ECommerce.Application.Identity.Queries;

public record UserDto(Guid Id, string FirstName, string LastName, string Email, string Role, DateTime CreatedAt);

public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;
