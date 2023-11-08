// @ts-ignore
/* eslint-disable */

import { request } from '@umijs/max';

/**
 * 字典列表
 */
export async function getDictionaries(
  params: {
    current?: number;
    pageSize?: number;
    code?: string;
    name?: string;
  },
  options?: { [key: string]: any },
) {
  return request('/api/v1/dictionaries', {
    method: 'GET',
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/**
 * 新建字典
 */
export async function createDictionary(values: Role, options?: { [key: string]: any }) {
  return request<ApiResult<Dictionary>>('/api/v1/dictionaries', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 修改字典
 */
export async function updateDictionary(values: Role, options?: { [key: string]: any }) {
  return request<ApiResult<Dictionary>>(`/api/v1/dictionaries/${values.id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 删除字典
 */
export async function deleteDictionary(id?: string, options?: { [key: string]: any }) {
  return request<ApiResult<Dictionary>>(`/api/v1/dictionaries/${id}`, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
    },
    data: { id },
    ...(options || {}),
  });
}

/**
 * 字典选项列表
 */
export async function getDictionaryItems(
  dictionaryId?: string,
  options?: { [key: string]: any },
) {
  return request<ApiResult<DictionaryItem[]>>(`/api/v1/dictionaries/${dictionaryId}/items`, {
    method: 'GET',
    ...(options || {}),
  });
}

/**
 * 新建字典选项
 */
export async function createDictionaryItem(
  dictionaryId?: string,
  values?: DictionaryItem,
  options?: { [key: string]: any }) {
  return request<ApiResult<DictionaryItem>>(`/api/v1/dictionaries/${dictionaryId}/items`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 修改字典选项
 */
export async function updateDictionaryItem(
  dictionaryId: string,
  values: DictionaryItem,
  options?: { [key: string]: any }) {
  return request<ApiResult<DictionaryItem>>(`/api/v1/dictionaries/${dictionaryId}/items/${values.id}`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    data: values,
    ...(options || {}),
  });
}

/**
 * 删除字典选项
 */
export async function deleteDictionaryItem(
  id: string,
  dictionaryId?: string,
  options?: { [key: string]: any }) {
  return request<ApiResult<DictionaryItem>>(`/api/v1/dictionaries/${dictionaryId}/items/${id}`, {
    method: 'DELETE',
    headers: {
      'Content-Type': 'application/json',
    },
    data: { id },
    ...(options || {}),
  });
}

/**
 * 排序字典选项
 */
export async function sortDictionaryItems(
  dictionaryId?: string,
  orderedIds?: string[],
  options?: { [key: string]: any }) {
  return request<ApiResult<DictionaryItem>>(`/api/v1/dictionaries/${dictionaryId}/items`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    data: { orderedIds },
    ...(options || {}),
  });
}

