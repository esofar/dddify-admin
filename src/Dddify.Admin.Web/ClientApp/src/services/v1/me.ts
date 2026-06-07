// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 获取用户属性信息。 GET /api/v1/me */
export async function getProfile(options?: { [key: string]: any }) {
  return request<API.ApiResultOfCurrentUserDto>("/api/v1/me", {
    method: "GET",
    ...(options || {}),
  });
}

/** 查询收件箱列表。 GET /api/v1/me/inbox-items */
export async function searchMeInboxItems(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.SearchMeInboxItemsParams,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfPagedResultOfInboxItemListDto>(
    "/api/v1/me/inbox-items",
    {
      method: "GET",
      params: {
        ...params,
      },
      ...(options || {}),
    }
  );
}

/** 删除收件箱消息。 DELETE /api/v1/me/inbox-items/${param0} */
export async function deleteMeInboxItem(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.DeleteMeInboxItemParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/me/inbox-items/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 读取收件箱消息来源详情。 GET /api/v1/me/inbox-items/${param0}/source */
export async function getInboxItemSource(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.GetInboxItemSourceParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResultOfInboxItemSourceDetailDto>(
    `/api/v1/me/inbox-items/${param0}/source`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 标记收件箱消息为已读。 PUT /api/v1/me/inbox-items/read */
export async function markInboxItemsAsRead(
  body: API.MarkInboxItemsAsReadRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/me/inbox-items/read", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 查询未读收件箱数量。 GET /api/v1/me/inbox-items/unread-count */
export async function getUnreadInboxItemCount(options?: {
  [key: string]: any;
}) {
  return request<API.ApiResultOfUnreadInboxItemCountDto>(
    "/api/v1/me/inbox-items/unread-count",
    {
      method: "GET",
      ...(options || {}),
    }
  );
}

/** 切换当前角色。 PUT /api/v1/me/roles */
export async function switchRole(
  body: API.SwitchUserCurrentRoleRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/me/roles", {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}
