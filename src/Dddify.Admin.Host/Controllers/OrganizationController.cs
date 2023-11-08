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
    /// 机构列表
    /// </summary>
    /// <param name="name">机构名称</param>
    /// <param name="type">机构类型</param>
    /// <param name="isEnabled">是否启用</param>
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
    /// 获取机构
    /// </summary>
    /// <param name="id">机构ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<OrganizationDto> GetAsync(Guid id)
    {
        return await SendAsync(new GetOrganizationByIdQuery(id));
    }

    /// <summary>
    /// 新建机构
    /// </summary>
    /// <param name="request">新增模型</param>
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
    /// 修改机构
    /// </summary>
    /// <param name="id">机构ID</param>
    /// <param name="request">更新模型</param>
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
    /// 删除机构
    /// </summary>
    /// <param name="id">机构ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await SendAsync(new DeleteOrganizationCommand(id));
    }

    /// <summary>
    /// 机构类型列表
    /// </summary>
    /// <returns></returns>
    [HttpGet("types")]
    public async Task<IEnumerable<DictionaryItemDto>> GetTypesAsync()
    {
        return await SendAsync(new GetItemsByDictionaryCodeQuery(DictionaryCodes.OrganizationType));
    }
}