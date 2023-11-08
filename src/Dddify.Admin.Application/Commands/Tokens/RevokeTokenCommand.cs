using Dddify.Admin.Domain.Exceptions.Users;

namespace Dddify.Admin.Application.Commands.Tokens;

public record RevokeTokenCommand(string RefreshToken) : ICommand;

public class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenCommandValidator()
    {
        RuleFor(v => v.RefreshToken).NotEmpty();
    }
}

public class RevokeTokenCommandHandler : ICommandHandler<RevokeTokenCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public RevokeTokenCommandHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(RevokeTokenCommand command, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(c => c.RefreshTokens.Where(c => c.UserId == _currentUser.Id))
            .SingleOrDefaultAsync(c => c.Id == _currentUser.Id, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundException(_currentUser.Id);
        }

        user.RevokeRefreshToken(command.RefreshToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}