using Dddify.Admin.Application.Commands.Departments;
using Dddify.Admin.Application.Dtos.Departments;
using Dddify.Admin.Application.Queries.Departments;
using Dddify.Admin.Application.Queries.Users;
using Dddify.Admin.Web.Requests.Departments;

namespace Dddify.Admin.Web.Controllers;

/// <summary>
/// 部门管理。
/// </summary>
/// <param name="sender">请求发送器。</param>
[Route("api/v1/departments")]
public class DepartmentController(ISender sender) : BaseController
{
    /// <summary>
    /// 获取所有部门。
    /// </summary>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("all", Name = "GetAllDepartments")]
    [ProducesResponseType<ApiResult<IEnumerable<DepartmentListDto>>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<DepartmentListDto>> GetAllDepartmentsAsync(CancellationToken cancellationToken)
    {
        return await sender.Send(new GetAllDepartmentsQuery(), cancellationToken);
    }

    /// <summary>
    /// 查询部门列表。
    /// </summary>
    /// <param name="name">部门名称。</param>
    /// <param name="type">部门类型。</param>
    /// <param name="isEnabled">启用状态。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet(Name = "SearchDepartments")]
    [Permission("system:department:index")]
    [ProducesResponseType<ApiResult<IEnumerable<DepartmentListDto>>>(StatusCodes.Status200OK)]
    public async Task<IEnumerable<DepartmentListDto>> SearchDepartmentsAsync(
        [FromQuery] string? name,
        [FromQuery] string? type,
        [FromQuery] bool? isEnabled,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new SearchDepartmentsQuery(name, type, isEnabled), cancellationToken);
    }

    /// <summary>
    /// 获取部门详情。
    /// </summary>
    /// <param name="id">部门ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpGet("{id}", Name = "GetDepartmentDetail")]
    [Permission("system:department:index")]
    [ProducesResponseType<ApiResult<DepartmentDetailDto>>(StatusCodes.Status200OK)]
    public async Task<DepartmentDetailDto> GetDepartmentDetailAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return await sender.Send(new GetDepartmentByIdQuery(id), cancellationToken);
    }

    /// <summary>
    /// 新增部门。
    /// </summary>
    /// <param name="request">新增部门请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPost(Name = "CreateDepartment")]
    [Permission("system:department:create")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task CreateAsync([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var leader = await sender.Send(new GetUserByIdQuery(request.LeaderId), cancellationToken);

        await sender.Send(
            new CreateDepartmentCommand(
                request.ParentId,
                request.Name,
                request.Type,
                leader.Id,
                leader.Name,
                request.IsEnabled,
                request.Order),
            cancellationToken);
    }

    /// <summary>
    /// 修改部门。
    /// </summary>
    /// <param name="id">部门ID。</param>
    /// <param name="request">修改部门请求。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpPut("{id}", Name = "UpdateDepartment")]
    [Permission("system:department:update")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task UpdateAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var leader = await sender.Send(new GetUserByIdQuery(request.LeaderId), cancellationToken);

        await sender.Send(
            new UpdateDepartmentCommand(
                id,
                request.ParentId,
                request.Name,
                request.Type,
                leader.Id,
                leader.Name,
                request.IsEnabled,
                request.Order,
                request.ConcurrencyStamp),
            cancellationToken);
    }

    /// <summary>
    /// 删除部门。
    /// </summary>
    /// <param name="id">部门ID。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    /// <returns></returns>
    [HttpDelete("{id}", Name = "DeleteDepartment")]
    [Permission("system:department:delete")]
    [ProducesResponseType<ApiResult>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteDepartmentCommand(id), cancellationToken);
    }
}
