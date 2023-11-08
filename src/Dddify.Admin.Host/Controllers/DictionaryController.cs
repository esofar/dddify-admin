using Dddify.Admin.Application.Commands.Dictionaries;
using Dddify.Admin.Application.Dtos.Dictionaries;
using Dddify.Admin.Application.Queries.Dictionaries;
using Dddify.Admin.Host.Models.Dictionaries;

namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/dictionaries")]
public class DictionaryController : BaseController
{
    /// <summary>
    /// 字典列表
    /// </summary>
    /// <param name="current">当前页</param>
    /// <param name="pageSize">页容量</param>
    /// <param name="code">字典编码</param>
    /// <param name="name">字典名称</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IPagedResult<DictionaryDto>> GetAsync(int current, int pageSize, string? code, string? name)
    {
        return await SendAsync(new GetDictionariesQuery(
            current,
            pageSize,
            code,
            name));
    }

    /// <summary>
    /// 获取字典
    /// </summary>
    /// <param name="id">字典ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<DictionaryDetailDto> GetAsync(Guid id)
    {
        return await SendAsync(new GetDictionaryByIdQuery(id));
    }

    /// <summary>
    /// 新建字典
    /// </summary>
    /// <param name="request">新增模型</param>
    /// <returns></returns>
    [HttpPost]
    public async Task CreateAsync([FromBody] CreateDictionaryRequest request)
    {
        await SendAsync(new CreateDictionaryCommand(
           request.Code,
           request.Name,
           request.Description));
    }

    /// <summary>
    /// 修改字典
    /// </summary>
    /// <param name="id">字典ID</param>
    /// <param name="request">更新模型</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, [FromBody] UpdateDictionaryRequest request)
    {
        await SendAsync(new UpdateDictionaryCommand(
            id,
            request.Name,
            request.Description,
            request.ConcurrencyStamp));
    }

    /// <summary>
    /// 删除字典
    /// </summary>
    /// <param name="id">字典ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await SendAsync(new DeleteDictionaryCommand(id));
    }

    /// <summary>
    /// 字典选项列表
    /// </summary>
    /// <param name="dictionaryId">字典ID</param>
    /// <returns></returns>
    [HttpGet("{dictionaryId}/items")]
    public async Task<IEnumerable<DictionaryItemDto>> GetItemsAsync(Guid dictionaryId)
    {
        return await SendAsync(new GetItemsByDictionaryIdQuery(dictionaryId));
    }

    /// <summary>
    /// 新建字典选项
    /// </summary>
    /// <param name="dictionaryId">字典ID</param>
    /// <param name="request">新增模型</param>
    /// <returns></returns>
    [HttpPost("{dictionaryId}/items")]
    public async Task CreateItemAsync(Guid dictionaryId, [FromBody] CreateDictionaryItemRequest request)
    {
        await SendAsync(new CreateDictionaryItemCommand(
            dictionaryId,
            request.Code,
            request.Content));
    }

    /// <summary>
    /// 排序字典选项
    /// </summary>
    /// <param name="dictionaryId">字典ID</param>
    /// <param name="request">排序模型</param>
    /// <returns></returns>
    [HttpPut("{dictionaryId}/items")]
    public async Task SortItemsAsync(Guid dictionaryId, [FromBody] SortDictionaryItemRequest request)
    {
        await SendAsync(new SortDictionaryItemCommand(
            dictionaryId,
            request.DictionaryItemIds));
    }

    /// <summary>
    /// 获取字典选项
    /// </summary>
    /// <param name="dictionaryId">字典ID</param>
    /// <param name="dictionaryItemId">字典选项ID</param>
    /// <returns></returns>
    [HttpGet("{dictionaryId}/items/{dictionaryItemId}")]
    public async Task<DictionaryItemDetailDto> GetItemAsync(Guid dictionaryId, Guid dictionaryItemId)
    {
        return await SendAsync(new GetDictionaryItemByIdQuery(
            dictionaryId,
            dictionaryItemId));
    }

    /// <summary>
    /// 修改字典选项
    /// </summary>
    /// <param name="dictionaryId">字典ID</param>
    /// <param name="dictionaryItemId">字典选项ID</param>
    /// <param name="request">更新模型</param>
    /// <returns></returns>
    [HttpPut("{dictionaryId}/items/{dictionaryItemId}")]
    public async Task UpdateItemAsync(Guid dictionaryId, Guid dictionaryItemId, [FromBody] UpdateDictionaryItemRequest request)
    {
        await SendAsync(new UpdateDictionaryItemCommand(
            dictionaryId,
            dictionaryItemId,
            request.Name,
            request.ConcurrencyStamp));
    }

    /// <summary>
    /// 删除字典选项
    /// </summary>
    /// <param name="dictionaryId">字典ID</param>
    /// <param name="dictionaryItemId">字典选项ID</param>
    /// <returns></returns>
    [HttpDelete("{dictionaryId}/items/{dictionaryItemId}")]
    public async Task DeleteItemAsync(Guid dictionaryId, Guid dictionaryItemId)
    {
        await SendAsync(new DeleteDictionaryItemCommand(
            dictionaryId,
            dictionaryItemId));
    }
}