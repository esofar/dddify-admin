using Dddify.Admin.Application.Commands.Lookups;
using Dddify.Admin.Application.Dtos.Lookups;
using Dddify.Admin.Application.Queries.Lookups;
using Dddify.Admin.Web.Requests.Lookups;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 字典管理。
/// </summary>
/// <param name="sender">请求发送器。</param>
[Route("api/v1/lookups")]
public class LookupController(ISender sender) : BaseController
{
    /// <summary>
    /// 查询字典列表。
    /// </summary>
    /// <param name="request">查询字典列表请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet(Name = "SearchLookups")]
    [Permission("system:lookup:index")]
    [ProducesResponseType<ApiResult<PagedResult<LookupDto>>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<IPagedResult<LookupDto>> SearchAsync([FromQuery] SearchLookupRequest request, CancellationToken cancellationToken)
    {
        return await sender.Send(
            new SearchLookupsQuery(
                request.Current,
                request.PageSize,
                request.Code,
                request.Name),
            cancellationToken);
    }

    /// <summary>
    /// 新增字典。
    /// </summary>
    /// <param name="request">新增字典请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost(Name = "CreateLookup")]
    [Permission("system:lookup:create")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task CreateAsync([FromBody] CreateLookupRequest request, CancellationToken cancellationToken)
    {
        await sender.Send(new CreateLookupCommand(request.Code, request.Name, request.Description), cancellationToken);
    }

    /// <summary>
    /// 修改字典。
    /// </summary>
    /// <param name="id">字典ID。</param>
    /// <param name="request">修改字典请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}", Name = "UpdateLookup")]
    [Permission("system:lookup:update")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateLookupRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateLookupCommand(id, request.Name, request.Description), cancellationToken);
    }

    /// <summary>
    /// 删除字典。
    /// </summary>
    /// <param name="id">字典ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpDelete("{id}", Name = "DeleteLookup")]
    [Permission("system:lookup:delete")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteLookupCommand(id), cancellationToken);
    }

    /// <summary>
    /// 获取字典项列表。
    /// </summary>
    /// <param name="id">字典ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("{id}/items", Name = "GetLookupItems")]
    [ProducesResponseType<ApiResult<IEnumerable<LookupItemDto>>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<LookupItemDto>> GetItemsAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new GetLookupItemsQuery(id), cancellationToken);
    }

    /// <summary>
    /// 新增字典项。
    /// </summary>
    /// <param name="id">字典ID。</param>
    /// <param name="request">新增字典项请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost("{id}/items", Name = "CreateLookupItem")]
    [Permission("system:lookup:item:create")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task CreateItemAsync(
        [FromRoute] Guid id,
        [FromBody] CreateLookupItemRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new CreateLookupItemCommand(id, request.Value, request.Label, request.Color), cancellationToken);
    }

    /// <summary>
    /// 修改字典项。
    /// </summary>
    /// <param name="id">字典ID。</param>
    /// <param name="itemId">字典项ID。</param>
    /// <param name="request">修改字典项请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}/items/{itemId}", Name = "UpdateLookupItem")]
    [Permission("system:lookup:item:update")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task UpdateItemAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId,
        [FromBody] UpdateLookupItemRequest request,
        CancellationToken cancellationToken)
    {
        await sender.Send(new UpdateLookupItemCommand(id, itemId, request.Label, request.Color), cancellationToken);
    }

    /// <summary>
    /// 删除字典项。
    /// </summary>
    /// <param name="id">字典ID。</param>
    /// <param name="itemId">字典项ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpDelete("{id}/items/{itemId}", Name = "DeleteLookupItem")]
    [Permission("system:lookup:item:delete")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task DeleteItemAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteLookupItemCommand(id, itemId), cancellationToken);
    }

    /// <summary>
    /// 启用字典项。
    /// </summary>
    /// <param name="id">字典ID。</param>
    /// <param name="itemId">字典项ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}/items/{itemId}/enable", Name = "EnableLookupItem")]
    [Permission("system:lookup:item:enable")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task EnableItemAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        await sender.Send(new EnableLookupItemCommand(id, itemId), cancellationToken);
    }

    /// <summary>
    /// 禁用字典项。
    /// </summary>
    /// <param name="id">字典ID。</param>
    /// <param name="itemId">字典项ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}/items/{itemId}/disable", Name = "DisableLookupItem")]
    [Permission("system:lookup:item:disable")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task DisableItemAsync(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DisableLookupItemCommand(id, itemId), cancellationToken);
    }

    /// <summary>
    /// 排序字典项。
    /// </summary>
    /// <param name="id">字典ID。</param>
    /// <param name="orderedItemIds">已排序的字典项ID列表。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}/items/sort", Name = "SortLookupItems")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task SortItemsAsync(
        [FromRoute] Guid id,
        [FromBody] Guid[] orderedItemIds,
        CancellationToken cancellationToken)
    {
        await sender.Send(new SortLookupItemsCommand(id, orderedItemIds), cancellationToken);
    }

    /// <summary>
    /// 获取可用字典项列表。
    /// </summary>
    /// <param name="code">字典编码。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("items", Name = "GetLookupActiveItems")]
    [ProducesResponseType<ApiResult<IEnumerable<LookupActiveItemDto>>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<LookupActiveItemDto>> GetActiveItemsAsync(
        [FromQuery] string code,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new GetLookupActiveItemsQuery(code), cancellationToken);
    }
}
