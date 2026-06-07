using Dddify.Admin.Application.Dtos.Announcements;
using Dddify.Admin.Application.Exceptions.Announcements;
using Dddify.Admin.Application.Services.Announcements;

namespace Dddify.Admin.Application.Queries.Announcements;

public record GetAnnouncementByIdQuery(Guid Id) : IQuery<AnnouncementDetailDto>;

public class GetAnnouncementByIdQueryHandler(
    IAnnouncementRepository announcementRepository,
    IAudienceResolver audienceResolver,
    IMapper mapper)
    : IQueryHandler<GetAnnouncementByIdQuery, AnnouncementDetailDto>
{
    public async Task<AnnouncementDetailDto> Handle(GetAnnouncementByIdQuery query, CancellationToken cancellationToken)
    {
        var announcement = await announcementRepository.GetAsync(query.Id, cancellationToken)
            ?? throw new AnnouncementNotFoundException(query.Id);

        var announcementDetail = mapper.Map<AnnouncementDetailDto>(announcement);

        announcementDetail.Audience.Targets = await audienceResolver.ResolveDisplayTargetsAsync(announcement.Audience, cancellationToken);

        return announcementDetail;
    }
}
