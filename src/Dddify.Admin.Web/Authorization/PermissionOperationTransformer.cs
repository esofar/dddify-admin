using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Dddify.Admin.Web.Authorization;

/// <summary>
/// 将接口所需权限编码追加到 OpenAPI 描述中，方便前端和接口文档查看。
/// </summary>
public class PermissionOperationTransformer : IOpenApiOperationTransformer
{
    /// <summary>
    /// 转换 OpenAPI 操作描述，补充权限编码信息。
    /// </summary>
    /// <param name="operation">OpenAPI 操作对象。</param>
    /// <param name="context">OpenAPI 操作转换上下文。</param>
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
