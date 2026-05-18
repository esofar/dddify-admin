// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 获取当前用户信息。 GET /api/v1/me */
export async function getMe(options?: { [key: string]: any }) {
  return request<API.ApiResultOfCurrentUserDto>("/api/v1/me", {
    method: "GET",
    ...(options || {}),
  });
}

/** 切换当前用户角色。 PUT /api/v1/me/roles */
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
