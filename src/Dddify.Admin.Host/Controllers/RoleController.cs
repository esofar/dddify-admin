using Dddify.Admin.Application.Commands.Roles;
using Dddify.Admin.Application.Dtos.Roles;
using Dddify.Admin.Application.Queries.Roles;
using Dddify.Admin.Host.Models.Roles;

namespace Dddify.Admin.Host.Controllers;

[Route("api/v1/roles")]
public class RoleController : BaseController
{
    /// <summary>
    /// 角色列表
    /// </summary>
    /// <param name="current">当前页</param>
    /// <param name="pageSize">页容量</param>
    /// <param name="code">角色编码</param>
    /// <param name="name">角色名称</param>
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
    /// 获取角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<RoleDto> GetAsync(Guid id)
    {
        return await SendAsync(new GetRoleByIdQuery(id));
    }

    /// <summary>
    /// 新建角色
    /// </summary>
    /// <param name="request">新增模型</param>
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
    /// 修改角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <param name="request">更新模型</param>
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
    /// 删除角色
    /// </summary>
    /// <param name="id">角色ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await SendAsync(new DeleteRoleCommand(id));
    }
}