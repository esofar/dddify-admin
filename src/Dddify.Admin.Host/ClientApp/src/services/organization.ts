// @ts-ignore
/* eslint-disable */

import { request } from '@umijs/max';

/**
 * 机构列表
 */
export async function getOrganizations(
  params: {
    current?: number;
    pageSize?: number;
    name?: string;
    isEnabled?: boolean;
  },
  options?: { [key: string]: any },
) {
  return request<ApiResult<Organization[]>>('/api/v1/organizations', {
    method: 'GET',
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/**
 * 新建机构
 */
export async function createOrganization(values: Organization, options?: { [key: string]: any }) {
  return request<ApiResult<Organization>>('/api/v1/organizations', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 修改机构
 */
export async function updateOrganization(values: Organization, options?: { [key: string]: any }) {
  return request<ApiResult<Organization>>(`/api/v1/organizations/${values.id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 删除机构
 */
export async function deleteOrganization(id?: string, options?: { [key: string]: any }) {
  return request<ApiResult<Organization>>(`/api/v1/organizations/${id}`, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
    },
    data: { id },
    ...(options || {}),
  });
}

/**
 * 机构类型列表
 */
export async function getOrganizationTypes(options?: { [key: string]: any }) {
  return request<ApiResult<DictionaryItem[]>>('/api/v1/organizations/types', {
    method: 'GET',
    ...(options || {}),
  });
}