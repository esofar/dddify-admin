using Dddify.Admin.Application.Commands.Announcements;
using Dddify.Admin.Application.Dtos.Announcements;
using Dddify.Admin.Application.Queries.Announcements;
using Dddify.Admin.Web.Requests.Announcements;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 公告管理。
/// </summary>
/// <param name="sender">请求发送器。</param>
[Route("api/v1/announcements")]
public class AnnouncementController(ISender sender) : BaseController
{
    /// <summary>
    /// 查询公告列表。
    /// </summary>
    /// <param name="request">查询公告列表请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet(Name = "SearchAnnouncements")]
    [Permission("system:announcement:index")]
    [ProducesResponseType<ApiResult<PagedResult<AnnouncementListDto>>>(StatusCodes.Status200OK)]
    public async Task<IPagedResult<AnnouncementListDto>> SearchAsync(
        [FromQuery] SearchAnnouncementsRequest request,
        CancellationToken cancellationToken)
    {
        return await sender.Send(
            new SearchAnnouncementsQuery(
                request.Current,
                request.PageSize,
                request.Keyword,
                request.Status),
            cancellationToken);
    }

    /// <summary>
    /// 获取公告详情。
    /// </summary>
    /// <param name="id">公告ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("{id}", Name = "GetAnnouncement")]
    [Permission("system:announcement:index")]
    [ProducesResponseType<ApiResult<AnnouncementDetailDto>>(StatusCodes.Status200OK)]
    public async Task<AnnouncementDetailDto> GetAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new GetAnnouncementByIdQuery(id), cancellationToken);
    }

    /// <summary>
    /// 创建公告。
    /// </summary>
    /// <param name="request">创建公告请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost(Name = "CreateAnnouncement")]
    [Permission("system:announcement:create")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    public async Task CreateAsync(
        [FromBody] CreateAnnouncementRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new CreateAnnouncementCommand(
                request.Title,
                request.Summary,
                request.ContentHtml,
                request.AudienceType,
                request.AudienceTargetIds),
            cancellationToken);
    }

    /// <summary>
    /// 修改公告。
    /// </summary>
    /// <param name="id">公告ID。</param>
    /// <param name="request">修改公告请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}", Name = "UpdateAnnouncement")]
    [Permission("system:announcement:update")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    public async Task UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateAnnouncementRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(
            new UpdateAnnouncementCommand(
                id,
                request.Title,
                request.Summary,
                request.ContentHtml,
                request.AudienceType,
                request.AudienceTargetIds),
            cancellationToken);
    }

    /// <summary>
    /// 发布公告。
    /// </summary>
    /// <param name="id">公告ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}/publish", Name = "PublishAnnouncement")]
    [Permission("system:announcement:publish")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    public async Task PublishAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await sender.Send(new PublishAnnouncementCommand(id), cancellationToken);
    }

    /// <summary>
    /// 撤回公告。
    /// </summary>
    /// <param name="id">公告ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}/withdraw", Name = "WithdrawAnnouncement")]
    [Permission("system:announcement:withdraw")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    public async Task WithdrawAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await sender.Send(new WithdrawAnnouncementCommand(id), cancellationToken);
    }

    /// <summary>
    /// 删除公告。
    /// </summary>
    /// <param name="id">公告ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpDelete("{id}", Name = "DeleteAnnouncement")]
    [Permission("system:announcement:delete")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    public async Task DeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteAnnouncementCommand(id), cancellationToken);
    }
}
