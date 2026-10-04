import {
  LogoutOutlined,
  SkinOutlined,
} from '@ant-design/icons';
import { history, useModel } from '@umijs/max';
import type { MenuProps } from 'antd';
import { message, Spin } from 'antd';
import React, { startTransition } from 'react';
import { getFingerprint } from '@/hooks/useFingerprint';
import { logout } from '@/services/v1/auth';
import { clearAccessToken } from '@/utils/request';
import HeaderDropdown from '../HeaderDropdown';

type GlobalHeaderRightProps = {
  children?: React.ReactNode;
};

const menuItems: MenuProps['items'] = [
  {
    key: 'theme',
    icon: <SkinOutlined />,
    label: '主题设置',
  },
  {
    type: 'divider' as const,
  },
  {
    key: 'logout',
    icon: <LogoutOutlined />,
    label: '退出登录',
  },
];

export const AvatarDropdown: React.FC<GlobalHeaderRightProps> = ({
  children,
}) => {
  const { initialState, setInitialState } = useModel('@@initialState');

  const loginOut = async () => {
    try {
      const { deviceId } = await getFingerprint();
      const { success, errorMessage } = await logout({
        headers: { 'X-Device-Id': deviceId },
      });
      if (!success) {
        message.error(errorMessage);
        return;
      }

      clearAccessToken();
      startTransition(() => {
        setInitialState((s) => ({ ...s, currentUser: undefined }));
      });

      const { search, pathname } = window.location;
      const urlParams = new URL(window.location.href).searchParams;
      const searchParams = new URLSearchParams({ redirect: pathname + search });
      if (pathname !== '/auth/login' && !urlParams.get('redirect')) {
        history.replace({
          pathname: '/auth/login',
          search: searchParams.toString(),
        });
      }
    } catch {
      // 请求拦截器负责显示网络错误，保留当前登录状态。
    }
  };

  const onMenuClick: MenuProps['onClick'] = (event) => {
    const { key } = event;
    if (key === 'logout') {
      void loginOut();
      return;
    }
    if (key === 'theme') {
      setInitialState((s) => ({ ...s, settingDrawerOpen: true }));
      return;
    }
  };

  if (!initialState) {
    return <Spin size="small" />;
  }

  const { currentUser } = initialState;

  if (!currentUser) {
    return <Spin size="small" />;
  }

  return (
    <HeaderDropdown
      placement="bottomRight"
      menu={{
        selectedKeys: [],
        onClick: onMenuClick,
        items: menuItems,
      }}
      arrow
    >
      {children}
    </HeaderDropdown>
  );
};
