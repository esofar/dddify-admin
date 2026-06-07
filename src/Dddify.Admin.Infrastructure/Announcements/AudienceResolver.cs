using Dddify.Admin.Application.Dtos.Announcements;
using Dddify.Admin.Application.Services.Announcements;
using Dddify.Admin.Domain.Aggregates.Announcements;
using Dddify.Admin.Domain.Aggregates.Users;
using System.Runtime.CompilerServices;

namespace Dddify.Admin.Infrastructure.Announcements;

[ScopedDependency(RegistrationMode.AsImplementedInterfaces)]
public class AudienceResolver(ApplicationDbContext context) : IAudienceResolver
{
    public async IAsyncEnumerable<IReadOnlyCollection<Guid>> ResolveUserIdsAsync(
        AnnouncementAudience audience,
        int batchSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var query = BuildQuery(audience)
            .OrderBy(id => id);

        Guid? lastId = null;

        while (true)
        {
            var batchQuery = lastId.HasValue
                ? query.Where(id => id.CompareTo(lastId.Value) > 0)
                : query;

            var batch = await batchQuery
                .Take(batchSize)
                .ToArrayAsync(cancellationToken);

            if (batch.Length == 0)
            {
                yield break;
            }

            yield return batch;
            lastId = batch[^1];
        }
    }

    public async Task<IReadOnlyCollection<AnnouncementAudienceTargetDto>> ResolveDisplayTargetsAsync(
        AnnouncementAudience audience,
        CancellationToken cancellationToken = default)
    {
        var targetIds = audience.TargetIds.Distinct().ToArray();

        if (targetIds.Length == 0)
        {
            return [];
        }

        return audience.Type switch
        {
            AnnouncementAudienceType.Departments => await context.Departments
                .AsNoTracking()
                .Where(department => targetIds.Contains(department.Id))
                .OrderBy(department => department.Order)
                .Select(department => new AnnouncementAudienceTargetDto(department.Id, department.Name))
                .ToArrayAsync(cancellationToken),

            AnnouncementAudienceType.Roles => await context.Roles
                .AsNoTracking()
                .Where(role => targetIds.Contains(role.Id))
                .OrderBy(role => role.Order)
                .Select(role => new AnnouncementAudienceTargetDto(role.Id, role.Name))
                .ToArrayAsync(cancellationToken),

            AnnouncementAudienceType.SpecificUsers => await context.Users
                .AsNoTracking()
                .Where(user => targetIds.Contains(user.Id))
                .OrderBy(user => user.Name)
                .Select(user => new AnnouncementAudienceTargetDto(user.Id, user.Name))
                .ToArrayAsync(cancellationToken),

            AnnouncementAudienceType.AllUsers => [],

            _ => throw new NotSupportedException($"Unsupported audience type: {audience.Type}")
        };
    }

    private IQueryable<Guid> BuildQuery(AnnouncementAudience audience)
    {
        return audience.Type switch
        {
            AnnouncementAudienceType.AllUsers => context.Users
                .AsNoTracking()
                .Where(u => u.Status == UserStatus.Enabled)
                .Select(u => u.Id),

            AnnouncementAudienceType.Departments => context.Users
                .AsNoTracking()
                .Where(u => u.Status == UserStatus.Enabled && audience.TargetIds.Contains(u.Department.Id))
                .Select(u => u.Id),

            AnnouncementAudienceType.Roles => context.Users
                .AsNoTracking()
                .Where(u => u.Status == UserStatus.Enabled && u.Roles.Any(r => audience.TargetIds.Contains(r.RoleId)))
                .Select(u => u.Id),

            AnnouncementAudienceType.SpecificUsers => context.Users
                .AsNoTracking()
                .Where(u => u.Status == UserStatus.Enabled && audience.TargetIds.Contains(u.Id))
                .Select(u => u.Id),

            _ => throw new NotSupportedException($"Unsupported audience type: {audience.Type}")
        };
    }
}
