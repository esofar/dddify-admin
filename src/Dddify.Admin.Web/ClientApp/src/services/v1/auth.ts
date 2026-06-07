// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 账号密码登录。 POST /api/v1/auth/login/account */
export async function accountLogin(
  body: API.AccountLoginRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfstring>("/api/v1/auth/login/account", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 手机验证码登录。 POST /api/v1/auth/login/sms */
export async function smsLogin(
  body: API.SmsLoginRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfstring>("/api/v1/auth/login/sms", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 发送登录短信验证码。 POST /api/v1/auth/login/sms-code */
export async function sendLoginSmsCode(
  body: API.SendLoginSmsCodeRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/auth/login/sms-code", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 退出登录。 POST /api/v1/auth/logout */
export async function logout(options?: { [key: string]: any }) {
  return request<API.ApiResult>("/api/v1/auth/logout", {
    method: "POST",
    ...(options || {}),
  });
}

/** 所有设备退出登录。 POST /api/v1/auth/logout/all */
export async function logoutAll(options?: { [key: string]: any }) {
  return request<API.ApiResult>("/api/v1/auth/logout/all", {
    method: "POST",
    ...(options || {}),
  });
}

/** 刷新访问令牌。 POST /api/v1/auth/token/refresh */
export async function refreshToken(options?: { [key: string]: any }) {
  return request<API.ApiResultOfstring>("/api/v1/auth/token/refresh", {
    method: "POST",
    ...(options || {}),
  });
}
