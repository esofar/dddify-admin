// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 查询用户列表。 权限标识：<code>system:user:index</code> GET /api/v1/users */
export async function searchUsers(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.SearchUsersParams,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfPagedResultOfUserListDto>("/api/v1/users", {
    method: "GET",
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/** 新增用户。 权限标识：<code>system:user:create</code> POST /api/v1/users */
export async function createUser(
  body: API.CreateUserRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/users", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 获取用户详情。 GET /api/v1/users/${param0} */
export async function getUserDetail(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.GetUserDetailParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResultOfUserDetailDto>(`/api/v1/users/${param0}`, {
    method: "GET",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 修改用户。 权限标识：<code>system:user:update</code> PUT /api/v1/users/${param0} */
export async function updateUser(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.UpdateUserParams,
  body: API.UpdateUserRequest,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/users/${param0}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 删除用户。 权限标识：<code>system:user:delete</code> DELETE /api/v1/users/${param0} */
export async function deleteUser(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.DeleteUserParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/users/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 禁用用户。 权限标识：<code>system:user:disable</code> PATCH /api/v1/users/${param0}/disable */
export async function disableUser(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.DisableUserParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/users/${param0}/disable`, {
    method: "PATCH",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 启用用户。 权限标识：<code>system:user:enable</code> PATCH /api/v1/users/${param0}/enable */
export async function enableUser(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.EnableUserParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/users/${param0}/enable`, {
    method: "PATCH",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 重置用户密码。 权限标识：<code>system:user:reset-password</code> PATCH /api/v1/users/${param0}/password */
export async function resetUserPassword(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.ResetUserPasswordParams,
  body: API.ResetUserPasswordRequest,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/users/${param0}/password`, {
    method: "PATCH",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 获取用户角色。 GET /api/v1/users/${param0}/roles */
export async function getUserRoles(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.GetUserRolesParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResultOfIEnumerableOfUserRoleDto>(
    `/api/v1/users/${param0}/roles`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 分配用户角色。 权限标识：<code>system:user:assign-roles</code> PUT /api/v1/users/${param0}/roles */
export async function assignUserRoles(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.AssignUserRolesParams,
  body: string[],
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/users/${param0}/roles`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}
