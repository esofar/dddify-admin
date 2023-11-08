/* eslint-disable */

import { request } from '@umijs/max';

/**
 * 账号登录接口
 */
export async function signin(body: API.SigninRequest, options?: { [key: string]: any }) {
  return request<ApiResult<Token>>('/api/v1/account/signin', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    data: body,
    ...(options || {}),
  });
}

/**
 * 账号刷新接口
 */
export async function refresh(body: { accessToken: string, refreshToken: string }, options?: { [key: string]: any }) {
  return request<ApiResult<Token>>('/api/v1/account/refresh', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    data: body,
    ...(options || {}),
  });
}

/**
 * 获取账号属性接口
 */
export async function profile() {
  return request<{
    data: API.CurrentUser;
  }>('/api/v1/account/profile', {
    method: 'GET',
  });
}

/**
 * 登录验证码接口
 */
export async function getSigninCaptcha(
  params: {
    phone: string;
  },
  options?: { [key: string]: any },
) {
  return request<ApiResult<string>>(`/api/v1/account/${params.phone}/captcha`, {
    method: 'GET',
    ...(options || {}),
  });
}