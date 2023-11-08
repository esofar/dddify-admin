// @ts-ignore
/* eslint-disable */

import { request } from '@umijs/max';

/**
 * 角色列表
 */
export async function getRoles(
  params: {
    current?: number;
    pageSize?: number;
  },
  options?: { [key: string]: any },
) {
  return request('/api/v1/roles', {
    method: 'GET',
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/**
 * 新建角色
 */
export async function createRole(values: Role, options?: { [key: string]: any }) {
  return request<ApiResult<Role>>('/api/v1/roles', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 修改角色
 */
export async function updateRole(values: Role, options?: { [key: string]: any }) {
  return request<ApiResult<Role>>(`/api/v1/roles/${values.id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 删除角色
 */
export async function deleteRole(id?: string, options?: { [key: string]: any }) {
  return request<ApiResult<Role>>(`/api/v1/roles/${id}`, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
    },
    data: { id },
    ...(options || {}),
  });
}