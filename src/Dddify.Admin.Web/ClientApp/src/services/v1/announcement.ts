// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 查询公告列表。 权限标识：<code>system:announcement:index</code> GET /api/v1/announcements */
export async function searchAnnouncements(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.SearchAnnouncementsParams,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfPagedResultOfAnnouncementListDto>(
    "/api/v1/announcements",
    {
      method: "GET",
      params: {
        ...params,
      },
      ...(options || {}),
    }
  );
}

/** 创建公告。 权限标识：<code>system:announcement:create</code> POST /api/v1/announcements */
export async function createAnnouncement(
  body: API.CreateAnnouncementRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/announcements", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 获取公告详情。 权限标识：<code>system:announcement:index</code> GET /api/v1/announcements/${param0} */
export async function getAnnouncement(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.GetAnnouncementParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResultOfAnnouncementDetailDto>(
    `/api/v1/announcements/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 修改公告。 权限标识：<code>system:announcement:update</code> PUT /api/v1/announcements/${param0} */
export async function updateAnnouncement(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.UpdateAnnouncementParams,
  body: API.UpdateAnnouncementRequest,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/announcements/${param0}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 删除公告。 权限标识：<code>system:announcement:delete</code> DELETE /api/v1/announcements/${param0} */
export async function deleteAnnouncement(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.DeleteAnnouncementParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/announcements/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 发布公告。 权限标识：<code>system:announcement:publish</code> PUT /api/v1/announcements/${param0}/publish */
export async function publishAnnouncement(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.PublishAnnouncementParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/announcements/${param0}/publish`, {
    method: "PUT",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 撤回公告。 权限标识：<code>system:announcement:withdraw</code> PUT /api/v1/announcements/${param0}/withdraw */
export async function withdrawAnnouncement(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.WithdrawAnnouncementParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/announcements/${param0}/withdraw`, {
    method: "PUT",
    params: { ...queryParams },
    ...(options || {}),
  });
}
