// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 邮箱密码登录。 POST /api/v1/auth/login/password */
export async function loginWithPassword(
  body: API.PasswordLoginRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfstring>("/api/v1/auth/login/password", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 邮箱验证码登录。 POST /api/v1/auth/login/verification-code */
export async function loginWithVerificationCode(
  body: API.VerificationCodeLoginRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfstring>("/api/v1/auth/login/verification-code", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 发送邮箱登录验证码。 POST /api/v1/auth/login/verification-code/send */
export async function sendEmailVerificationCode(
  body: API.SendEmailVerificationCodeRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/auth/login/verification-code/send", {
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
