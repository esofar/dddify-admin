using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Dddify.Admin.Web.Authorization;

/// <summary>
/// 为 OpenAPI 接口描述补充权限标识。
/// </summary>
public class PermissionOperationTransformer : IOpenApiOperationTransformer
{
    /// <summary>
    /// 将所需权限标识追加到接口描述。
    /// </summary>
    /// <param name="operation">OpenAPI 操作。</param>
    /// <param name="context">操作转换上下文。</param>
    /// <param name="cancellationToken">取消令牌。</param>
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var permissions = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<PermissionAttribute>()
            .Select(p => p.Code)
            .ToList();

        if (permissions.Count != 0)
        {
            var permissionText = string.Join(", ", permissions);

            if (!string.IsNullOrEmpty(operation.Description))
            {
                operation.Description += "<br/>";
            }

            operation.Description += $"权限标识：<code>{permissionText}</code>";
        }

        return Task.CompletedTask;
    }
}
