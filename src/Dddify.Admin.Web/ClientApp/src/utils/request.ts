import type { RequestOptions } from '@@/plugin-request/request';
import { getIntl, history, request } from '@umijs/max';
import { message } from 'antd';
import { getFingerprint } from '@/hooks/useFingerprint';
import { refreshToken } from '@/services/v1/auth';

const AUTHORIZATION_HEADER = 'Authorization';
const DEVICE_ID_HEADER = 'X-Device-Id';
const ACCESS_TOKEN_KEY = 'x-access-token';

const TOKEN_WHITELIST = [
  '/api/v1/auth/login/account',
  '/api/v1/auth/login/sms',
  '/api/v1/auth/token/refresh',
];

let refreshPromise: Promise<API.ApiResultOfstring> | null = null;
let accessTokenCache = '';

export function saveAccessToken(accessToken: string) {
  accessTokenCache = accessToken;

  try {
    sessionStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  } catch {
    // Fall back to in-memory token cache when sessionStorage is unavailable.
  }
}

export function clearAccessToken() {
  accessTokenCache = '';

  try {
    sessionStorage.removeItem(ACCESS_TOKEN_KEY);
  } catch {
    // Ignore storage errors.
  }
}

export function getAccessToken() {
  if (accessTokenCache) {
    return accessTokenCache;
  }

  try {
    accessTokenCache = sessionStorage.getItem(ACCESS_TOKEN_KEY) ?? '';
  } catch {
    accessTokenCache = '';
  }

  return accessTokenCache;
}

const HTTP_ERROR_MESSAGE_MAP: Record<number, string> = {
  400: 'error.http.badRequest',
  403: 'error.http.forbidden',
  404: 'error.http.notFound',
  409: 'error.http.conflict',
  500: 'error.http.internalServerError',
};

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

function createRefreshTokenFailureResult(error: any): API.ApiResultOfstring {
  return {
    success: false,
    errorMessage:
      error?.info?.errorMessage ||
      error?.response?.data?.errorMessage ||
      error?.data?.errorMessage,
  };
}

async function obtainNewAccessToken(): Promise<API.ApiResultOfstring> {
  if (!refreshPromise) {
    refreshPromise = (async () => {
      try {
        const { deviceId } = await getFingerprint();

        return await refreshToken({
          headers: {
            [DEVICE_ID_HEADER]: deviceId,
          },
        });
      } catch (error) {
        return createRefreshTokenFailureResult(error);
      } finally {
        refreshPromise = null;
      }
    })();
  }

  return refreshPromise;
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

    const currentAccessToken = getAccessToken();
    if (!currentAccessToken) {
      clearAccessToken();
      setTimeout(() => history.push('/auth/login'), 100);
      return response;
    }

    const { success, data: accessToken, errorMessage } = await obtainNewAccessToken();

    if (!success) {
      clearAccessToken();
      if (errorMessage) {
        message.error(errorMessage);
      }
      setTimeout(() => history.push('/auth/login'), 100);
      return response;
    }

    saveAccessToken(accessToken);

    originalConfig._retry = true;
    originalConfig.headers = {
      ...(originalConfig.headers || {}),
      [AUTHORIZATION_HEADER]: `Bearer ${accessToken}`,
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
