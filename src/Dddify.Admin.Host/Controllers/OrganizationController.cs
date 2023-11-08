using Dddify.Admin.Application.Commands.Organizations;
using Dddify.Admin.Application.Dtos.Dictionaries;
using Dddify.Admin.Application.Dtos.Organizations;
using Dddify.Admin.Application.Queries.Dictionaries;
using Dddify.Admin.Application.Queries.Organizations;

namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/organizations")]
public class OrganizationController : BaseController
{
    /// <summary>
    /// �����б�
    /// </summary>
    /// <param name="name">��������</param>
    /// <param name="type">��������</param>
    /// <param name="isEnabled">�Ƿ�����</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IList<OrganizationDto>> GetAsync(string? name, string? type, bool? isEnabled)
    {
        return await SendAsync(new GetOrganizationsQuery(
            name,
            type,
            isEnabled));
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <param name="id">����ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<OrganizationDto> GetAsync(Guid id)
    {
        return await SendAsync(new GetOrganizationByIdQuery(id));
    }

    /// <summary>
    /// �½�����
    /// </summary>
    /// <param name="request">����ģ��</param>
    /// <returns></returns>
    [HttpPost]
    public async Task CreateAsync([FromBody] CreateOrganizationRequest request)
    {
        await SendAsync(new CreateOrganizationCommand(
            request.ParentId,
            request.Name,
            request.Type,
            request.Order,
            request.IsEnabled));
    }

    /// <summary>
    /// �޸Ļ���
    /// </summary>
    /// <param name="id">����ID</param>
    /// <param name="request">����ģ��</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, [FromBody] UpdateOrganizationRequest request)
    {
        await SendAsync(new UpdateOrganizationCommand(
            id,
            request.ParentId,
            request.Name,
            request.Type,
            request.Order,
            request.IsEnabled,
            request.ConcurrencyStamp));
    }

    /// <summary>
    /// ɾ������
    /// </summary>
    /// <param name="id">����ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await SendAsync(new DeleteOrganizationCommand(id));
    }

    /// <summary>
    /// ���������б�
    /// </summary>
    /// <returns></returns>
    [HttpGet("types")]
    public async Task<IEnumerable<DictionaryItemDto>> GetTypesAsync()
    {
        return await SendAsync(new GetItemsByDictionaryCodeQuery(DictionaryCodes.OrganizationType));
    }
}