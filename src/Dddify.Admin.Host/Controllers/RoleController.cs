using Dddify.Admin.Application.Commands.Roles;
using Dddify.Admin.Application.Dtos.Roles;
using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Host.Models.Roles;

namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/roles")]
public class RoleController : BaseController
{
    /// <summary>
    /// ��ɫ�б�
    /// </summary>
    /// <param name="current">��ǰҳ</param>
    /// <param name="pageSize">ҳ����</param>
    /// <param name="code">��ɫ����</param>
    /// <param name="name">��ɫ����</param>
    /// <returns></returns>
    [ProducesDefaultResponseType(typeof(IApiResult<IPagedResult<RoleDto>>))]
    [HttpGet]
    public async Task<IPagedResult<RoleDto>> GetAsync(int current, int pageSize, string? code, string? name)
    {
        return await SendAsync(new GetRolesQuery(
            current,
            pageSize,
            code,
            name));
    }

    /// <summary>
    /// ��ȡ��ɫ
    /// </summary>
    /// <param name="id">��ɫID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<RoleDto> GetAsync(Guid id)
    {
        return await SendAsync(new GetRoleByIdQuery(id));
    }

    /// <summary>
    /// �½���ɫ
    /// </summary>
    /// <param name="request">����ģ��</param>
    /// <returns></returns>
    [HttpPost]
    public async Task CreateAsync(CreateRoleRequest request)
    {
        await SendAsync(new CreateRoleCommand(
            request.Code,
            request.Name,
            request.Description));
    }

    /// <summary>
    /// �޸Ľ�ɫ
    /// </summary>
    /// <param name="id">��ɫID</param>
    /// <param name="request">����ģ��</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task UpdateAsync(Guid id, UpdateRoleRequest request)
    {
        await SendAsync(new UpdateRoleCommand(
            id,
            request.Name,
            request.Description,
            request.ConcurrencyStamp));
    }

    /// <summary>
    /// ɾ����ɫ
    /// </summary>
    /// <param name="id">��ɫID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await SendAsync(new DeleteRoleCommand(id));
    }
}