// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 查询角色列表。 权限标识：<code>system:role:index</code> GET /api/v1/roles */
export async function searchRoles(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.SearchRolesParams,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfPagedResultOfRoleListDto>("/api/v1/roles", {
    method: "GET",
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/** 新增角色。 权限标识：<code>system:role:create</code> POST /api/v1/roles */
export async function createRole(
  body: API.CreateRoleRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/roles", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 获取角色详情。 GET /api/v1/roles/${param0} */
export async function getRoleDetail(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.GetRoleDetailParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResultOfRoleDetailDto>(`/api/v1/roles/${param0}`, {
    method: "GET",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 修改角色。 权限标识：<code>system:role:update</code> PUT /api/v1/roles/${param0} */
export async function updateRole(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.UpdateRoleParams,
  body: API.UpdateRoleRequest,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/roles/${param0}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 删除角色。 权限标识：<code>system:role:delete</code> DELETE /api/v1/roles/${param0} */
export async function deleteRole(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.DeleteRoleParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/roles/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 获取角色权限。 GET /api/v1/roles/${param0}/permissions */
export async function getRolePermissions(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.GetRolePermissionsParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResultOfIEnumerableOfRolePermissionDto>(
    `/api/v1/roles/${param0}/permissions`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 分配角色权限。 权限标识：<code>system:role:assign-permissions</code> PUT /api/v1/roles/${param0}/permissions */
export async function assignRolePermissions(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.AssignRolePermissionsParams,
  body: string[],
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/roles/${param0}/permissions`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 获取所有角色。 GET /api/v1/roles/all */
export async function getAllRoles(options?: { [key: string]: any }) {
  return request<API.ApiResultOfIEnumerableOfRoleListDto>("/api/v1/roles/all", {
    method: "GET",
    ...(options || {}),
  });
}
