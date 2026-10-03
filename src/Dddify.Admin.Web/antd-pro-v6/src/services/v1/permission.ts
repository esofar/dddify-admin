// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 查询权限列表。 权限标识：<code>system:permission:index</code> GET /api/v1/permissions */
export async function searchPermissions(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.SearchPermissionsParams,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfIEnumerableOfPermissionDto>(
    "/api/v1/permissions",
    {
      method: "GET",
      params: {
        ...params,
      },
      ...(options || {}),
    }
  );
}

/** 新增权限。 权限标识：<code>system:permission:create</code> POST /api/v1/permissions */
export async function createPermission(
  body: API.CreateOrUpdatePermissionRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/permissions", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 修改权限。 权限标识：<code>system:permission:update</code> PUT /api/v1/permissions/${param0} */
export async function updatePermission(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.UpdatePermissionParams,
  body: API.CreateOrUpdatePermissionRequest,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/permissions/${param0}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 删除权限。 权限标识：<code>system:permission:delete</code> DELETE /api/v1/permissions/${param0} */
export async function deletePermission(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.DeletePermissionParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/permissions/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 获取所有权限。 GET /api/v1/permissions/all */
export async function getAllPermissions(options?: { [key: string]: any }) {
  return request<API.ApiResultOfIEnumerableOfPermissionDto>(
    "/api/v1/permissions/all",
    {
      method: "GET",
      ...(options || {}),
    }
  );
}
