// @ts-ignore
/* eslint-disable */

import { request } from '@umijs/max';

/**
 * 用户列表
 */
export async function getUsers(
  params: {
    current?: number;
    pageSize?: number;
  },
  options?: { [key: string]: any },
) {
  return request('/api/v1/users', {
    method: 'GET',
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/**
 * 新建用户
 */
export async function createUser(values: User, options?: { [key: string]: any }) {
  return request<ApiResult<User>>('/api/v1/users', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 修改用户
 */
export async function updateUser(values: User, options?: { [key: string]: any }) {
  return request<ApiResult<User>>(`/api/v1/users/${values.id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 删除用户
 */
export async function deleteUser(id?: string, options?: { [key: string]: any }) {
  return request<ApiResult<User>>(`/api/v1/users/${id}`, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
    },
    data: { id },
    ...(options || {}),
  });
}

/**
 * 人员类型列表
 */
export async function getUserTypes(options?: { [key: string]: any }) {
  return request<ApiResult<DictionaryItem[]>>('/api/v1/users/types', {
    method: 'GET',
    ...(options || {}),
  });
}