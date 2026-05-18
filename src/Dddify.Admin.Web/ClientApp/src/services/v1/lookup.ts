// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 查询字典列表。 权限标识：<code>system:lookup:index</code> GET /api/v1/lookups */
export async function searchLookups(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.SearchLookupsParams,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfPagedResultOfLookupDto>("/api/v1/lookups", {
    method: "GET",
    params: {
      ...params,
    },
    ...(options || {}),
  });
}

/** 新增字典。 权限标识：<code>system:lookup:create</code> POST /api/v1/lookups */
export async function createLookup(
  body: API.CreateLookupRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/lookups", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 修改字典。 权限标识：<code>system:lookup:update</code> PUT /api/v1/lookups/${param0} */
export async function updateLookup(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.UpdateLookupParams,
  body: API.UpdateLookupRequest,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/lookups/${param0}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 删除字典。 权限标识：<code>system:lookup:delete</code> DELETE /api/v1/lookups/${param0} */
export async function deleteLookup(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.DeleteLookupParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/lookups/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 获取字典项列表。 GET /api/v1/lookups/${param0}/items */
export async function getLookupItems(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.GetLookupItemsParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResultOfIEnumerableOfLookupItemDto>(
    `/api/v1/lookups/${param0}/items`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 新增字典项。 权限标识：<code>system:lookup:item:create</code> POST /api/v1/lookups/${param0}/items */
export async function createLookupItem(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.CreateLookupItemParams,
  body: API.CreateLookupItemRequest,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/lookups/${param0}/items`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 修改字典项。 权限标识：<code>system:lookup:item:update</code> PUT /api/v1/lookups/${param0}/items/${param1} */
export async function updateLookupItem(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.UpdateLookupItemParams,
  body: API.UpdateLookupItemRequest,
  options?: { [key: string]: any }
) {
  const { id: param0, itemId: param1, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/lookups/${param0}/items/${param1}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 禁用字典项。 权限标识：<code>system:lookup:item:disable</code> PUT /api/v1/lookups/${param0}/items/${param1}/disable */
export async function disableLookupItem(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.DisableLookupItemParams,
  options?: { [key: string]: any }
) {
  const { id: param0, itemId: param1, ...queryParams } = params;
  return request<API.ApiResult>(
    `/api/v1/lookups/${param0}/items/${param1}/disable`,
    {
      method: "PUT",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 启用字典项。 权限标识：<code>system:lookup:item:enable</code> PUT /api/v1/lookups/${param0}/items/${param1}/enable */
export async function enableLookupItem(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.EnableLookupItemParams,
  options?: { [key: string]: any }
) {
  const { id: param0, itemId: param1, ...queryParams } = params;
  return request<API.ApiResult>(
    `/api/v1/lookups/${param0}/items/${param1}/enable`,
    {
      method: "PUT",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 排序字典项。 PUT /api/v1/lookups/${param0}/items/sort */
export async function sortLookupItems(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.SortLookupItemsParams,
  body: string[],
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/lookups/${param0}/items/sort`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 获取可用字典项列表。 GET /api/v1/lookups/items */
export async function getLookupActiveItems(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.GetLookupActiveItemsParams,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfIEnumerableOfLookupActiveItemDto>(
    "/api/v1/lookups/items",
    {
      method: "GET",
      params: {
        ...params,
      },
      ...(options || {}),
    }
  );
}
