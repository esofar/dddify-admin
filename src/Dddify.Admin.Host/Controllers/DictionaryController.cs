using Dddify.Admin.Application.Commands.Dictionaries;
using Dddify.Admin.Application.Dtos.Dictionaries;
using Dddify.Admin.Application.Queries.Dictionaries;
using Dddify.Admin.Host.Models.Dictionaries;

namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/dictionaries")]
public class DictionaryController : BaseController
{
    /// <summary>
    /// �ֵ��б�
    /// </summary>
    /// <param name="current">��ǰҳ</param>
    /// <param name="pageSize">ҳ����</param>
    /// <param name="code">�ֵ����</param>
    /// <param name="name">�ֵ�����</param>
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
    /// ��ȡ�ֵ�
    /// </summary>
    /// <param name="id">�ֵ�ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<DictionaryDetailDto> GetAsync(Guid id)
    {
        return await SendAsync(new GetDictionaryByIdQuery(id));
    }

    /// <summary>
    /// �½��ֵ�
    /// </summary>
    /// <param name="request">����ģ��</param>
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
    /// �޸��ֵ�
    /// </summary>
    /// <param name="id">�ֵ�ID</param>
    /// <param name="request">����ģ��</param>
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
    /// ɾ���ֵ�
    /// </summary>
    /// <param name="id">�ֵ�ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await SendAsync(new DeleteDictionaryCommand(id));
    }

    /// <summary>
    /// �ֵ�ѡ���б�
    /// </summary>
    /// <param name="dictionaryId">�ֵ�ID</param>
    /// <returns></returns>
    [HttpGet("{dictionaryId}/items")]
    public async Task<IEnumerable<DictionaryItemDto>> GetItemsAsync(Guid dictionaryId)
    {
        return await SendAsync(new GetItemsByDictionaryIdQuery(dictionaryId));
    }

    /// <summary>
    /// �½��ֵ�ѡ��
    /// </summary>
    /// <param name="dictionaryId">�ֵ�ID</param>
    /// <param name="request">����ģ��</param>
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
    /// �����ֵ�ѡ��
    /// </summary>
    /// <param name="dictionaryId">�ֵ�ID</param>
    /// <param name="request">����ģ��</param>
    /// <returns></returns>
    [HttpPut("{dictionaryId}/items")]
    public async Task SortItemsAsync(Guid dictionaryId, [FromBody] SortDictionaryItemRequest request)
    {
        await SendAsync(new SortDictionaryItemCommand(
            dictionaryId,
            request.DictionaryItemIds));
    }

    /// <summary>
    /// ��ȡ�ֵ�ѡ��
    /// </summary>
    /// <param name="dictionaryId">�ֵ�ID</param>
    /// <param name="dictionaryItemId">�ֵ�ѡ��ID</param>
    /// <returns></returns>
    [HttpGet("{dictionaryId}/items/{dictionaryItemId}")]
    public async Task<DictionaryItemDetailDto> GetItemAsync(Guid dictionaryId, Guid dictionaryItemId)
    {
        return await SendAsync(new GetDictionaryItemByIdQuery(
            dictionaryId,
            dictionaryItemId));
    }

    /// <summary>
    /// �޸��ֵ�ѡ��
    /// </summary>
    /// <param name="dictionaryId">�ֵ�ID</param>
    /// <param name="dictionaryItemId">�ֵ�ѡ��ID</param>
    /// <param name="request">����ģ��</param>
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
    /// ɾ���ֵ�ѡ��
    /// </summary>
    /// <param name="dictionaryId">�ֵ�ID</param>
    /// <param name="dictionaryItemId">�ֵ�ѡ��ID</param>
    /// <returns></returns>
    [HttpDelete("{dictionaryId}/items/{dictionaryItemId}")]
    public async Task DeleteItemAsync(Guid dictionaryId, Guid dictionaryItemId)
    {
        await SendAsync(new DeleteDictionaryItemCommand(
            dictionaryId,
            dictionaryItemId));
    }
}