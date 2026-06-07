import type { RequestOptions } from '@@/plugin-request/request';
import { getIntl, history, request } from '@umijs/max';
import { message } from 'antd';
import { getFingerprint } from '@/hooks/useFingerprint';
import { refreshToken as requestRefreshToken } from '@/services/v1/auth';

const AUTHORIZATION_HEADER = 'Authorization';
const DEVICE_ID_HEADER = 'X-Device-Id';

const TOKEN_WHITELIST = [
  '/api/v1/auth/login/account',
  '/api/v1/auth/login/sms',
  '/api/v1/auth/token/refresh',
];

const HTTP_ERROR_MESSAGE_MAP: Record<number, string> = {
  400: 'error.http.badRequest',
  403: 'error.http.forbidden',
  404: 'error.http.notFound',
  409: 'error.http.conflict',
  500: 'error.http.internalServerError',
};

let refreshAccessTokenPromise: Promise<API.ApiResultOfstring> | null = null;
let accessToken = '';

export function saveAccessToken(token: string) {
  accessToken = token;
}

export function clearAccessToken() {
  accessToken = '';
}

export function getAccessToken() {
  return accessToken;
}

export function showHttpErrorMessage(error: any) {
  const intl = getIntl();
  let messageId = 'error.http.network';

  const status = error?.response?.status;

  if (status) {
    messageId = HTTP_ERROR_MESSAGE_MAP[status] || 'error.http.unknown';
  } else if (
    error?.code === 'ECONNABORTED' ||
    error?.message?.includes('timeout')
  ) {
    messageId = 'error.http.timeout';
  }

  message.error(intl.formatMessage({ id: messageId }));
}

function getRequestPath(url: string) {
  try {
    const baseUrl = typeof window === 'undefined' ? 'http://localhost' : window.location.origin;

    return new URL(url, baseUrl).pathname;
  } catch {
    return url.split('?')[0];
  }
}

function isTokenWhiteUrl(url: string) {
  const path = getRequestPath(url);

  return TOKEN_WHITELIST.some((whiteUrl) => path === whiteUrl);
}

async function refreshAccessToken(): Promise<API.ApiResultOfstring> {
  if (!refreshAccessTokenPromise) {
    refreshAccessTokenPromise = (async () => {
      try {
        const { deviceId } = await getFingerprint();

        return await requestRefreshToken({
          headers: {
            [DEVICE_ID_HEADER]: deviceId,
          },
        });
      } catch (error: any) {
        return {
          success: false,
          errorMessage:
            error?.info?.errorMessage ||
            error?.response?.data?.errorMessage ||
            error?.data?.errorMessage,
        };
      } finally {
        refreshAccessTokenPromise = null;
      }
    })();
  }

  return refreshAccessTokenPromise;
}

export const authRequestInterceptor = (config: RequestOptions) => {
  const url = config.url ?? '';
  const accessToken = getAccessToken();

  if (!accessToken || isTokenWhiteUrl(url)) {
    return config;
  }

  return {
    ...config,
    headers: {
      ...config.headers,
      [AUTHORIZATION_HEADER]: `Bearer ${accessToken}`,
    },
  };
};

export const authResponseInterceptor: [
  (response: any) => Promise<any>,
  (error: any) => Promise<never>,
] = [
  async (response: any) => {
    const originalConfig = response?.config || {};
    const status = response?.status;
    const requestUrl = response?.request?.responseURL || originalConfig.url || '';

    if (status !== 401 || isTokenWhiteUrl(requestUrl) || originalConfig._retry) {
      return response;
    }

    const { success, data: newAccessToken, errorMessage } = await refreshAccessToken();

    if (!success || !newAccessToken) {
      clearAccessToken();
      if (errorMessage) {
        message.error(errorMessage);
      }
      setTimeout(() => history.push('/auth/login'), 100);
      return response;
    }

    saveAccessToken(newAccessToken);

    originalConfig._retry = true;
    originalConfig.headers = {
      ...(originalConfig.headers || {}),
      [AUTHORIZATION_HEADER]: `Bearer ${newAccessToken}`,
    };

    try {
      const retriedData = await request(originalConfig.url, {
        ...originalConfig,
      });

      return {
        ...response,
        status: 200,
        data: retriedData,
      };
    } catch {
      return response;
    }
  },
  (error: any) => {
    showHttpErrorMessage(error);
    return Promise.reject(error);
  },
];
