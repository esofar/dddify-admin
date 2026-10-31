import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import { App } from 'antd';
import { beforeEach, describe, expect, it, vi } from 'vitest';

const api = vi.hoisted(() => ({
  loginWithPassword: vi.fn(),
  loginWithVerificationCode: vi.fn(),
  sendEmailVerificationCode: vi.fn(),
}));

vi.mock('@/services/v1/auth', () => api);
vi.mock('@/components', () => ({
  Footer: () => null,
  LangDropdown: () => null,
}));
vi.mock('@/utils/request', () => ({ saveAccessToken: vi.fn() }));
vi.mock('@/hooks/useFingerprint', () => ({
  useFingerprint: () => ({
    ready: true,
    deviceId: 'device',
    deviceName: 'browser',
  }),
  getFingerprint: vi.fn(),
}));
vi.mock('@umijs/max', async () => {
  const { default: messages } = await import('@/locales/zh-CN/projectPages');
  const formatMessage = ({ id }: { id: string }) =>
    messages[id as keyof typeof messages] ?? id;
  return {
    useIntl: () => ({ formatMessage }),
    FormattedMessage: ({ id }: { id: string }) => formatMessage({ id }),
    Helmet: () => null,
  };
});

import Login from './index';

function showLogin() {
  return render(
    <App>
      <Login />
    </App>,
  );
}

describe('邮箱登录', () => {
  beforeEach(() => {
    vi.clearAllMocks();
    api.sendEmailVerificationCode.mockResolvedValue({ success: true });
    api.loginWithPassword.mockResolvedValue({
      success: false,
      errorMessage: '测试登录失败',
    });
    api.loginWithVerificationCode.mockResolvedValue({
      success: false,
      errorMessage: '测试登录失败',
    });
  });

  it('提供密码和验证码登录，切换时保留邮箱', () => {
    showLogin();
    expect(screen.getByRole('checkbox')).not.toBeChecked();
    expect(screen.getByRole('tab', { name: '密码登录' })).toHaveAttribute(
      'aria-selected',
      'true',
    );
    fireEvent.change(screen.getByPlaceholderText('请输入邮箱'), {
      target: { value: 'member@example.com' },
    });
    fireEvent.click(screen.getByRole('tab', { name: '验证码登录' }));
    expect(screen.getByPlaceholderText('请输入邮箱')).toHaveValue(
      'member@example.com',
    );
    expect(screen.getByPlaceholderText('请输入验证码')).toBeInTheDocument();
    expect(
      screen.queryByPlaceholderText('请输入手机号'),
    ).not.toBeInTheDocument();
    expect(screen.queryByPlaceholderText('请输入密码')).not.toBeInTheDocument();
  });

  it('邮箱格式错误时阻止发送验证码', async () => {
    showLogin();
    fireEvent.click(screen.getByRole('tab', { name: '验证码登录' }));
    fireEvent.change(screen.getByPlaceholderText('请输入邮箱'), {
      target: { value: '13800138000' },
    });
    fireEvent.click(screen.getByRole('button', { name: '获取验证码' }));
    expect(await screen.findByText('邮箱格式无效')).toBeInTheDocument();
    expect(api.sendEmailVerificationCode).not.toHaveBeenCalled();
  });

  it('将验证码发送至邮箱并启动倒计时', async () => {
    showLogin();
    fireEvent.click(screen.getByRole('tab', { name: '验证码登录' }));
    fireEvent.change(screen.getByPlaceholderText('请输入邮箱'), {
      target: { value: 'member@example.com' },
    });
    fireEvent.click(screen.getByRole('button', { name: '获取验证码' }));
    await waitFor(() =>
      expect(api.sendEmailVerificationCode).toHaveBeenCalledWith({
        email: 'member@example.com',
      }),
    );
    expect(
      await screen.findByRole('button', { name: /秒后重新获取/ }),
    ).toBeDisabled();
  });

  it('密码登录使用邮箱账号', async () => {
    showLogin();
    fireEvent.click(screen.getByRole('checkbox'));
    fireEvent.change(screen.getByPlaceholderText('请输入邮箱'), {
      target: { value: 'member@example.com' },
    });
    fireEvent.change(screen.getByPlaceholderText('请输入密码'), {
      target: { value: 'Password123' },
    });
    fireEvent.click(
      screen.getByRole('button', { name: /登\s*录|Login|Log in/i }),
    );
    await waitFor(() =>
      expect(api.loginWithPassword).toHaveBeenCalledWith({
        email: 'member@example.com',
        password: 'Password123',
        deviceId: 'device',
        deviceName: 'browser',
        isPersistent: true,
      }),
    );
    expect(api.loginWithVerificationCode).not.toHaveBeenCalled();
  });

  it('验证码登录提交邮箱与验证码', async () => {
    showLogin();
    fireEvent.click(screen.getByRole('tab', { name: '验证码登录' }));
    fireEvent.change(screen.getByPlaceholderText('请输入邮箱'), {
      target: { value: 'member@example.com' },
    });
    fireEvent.change(screen.getByPlaceholderText('请输入验证码'), {
      target: { value: '012345' },
    });
    fireEvent.click(
      screen.getByRole('button', { name: /登\s*录|Login|Log in/i }),
    );
    await waitFor(() =>
      expect(api.loginWithVerificationCode).toHaveBeenCalledWith({
        email: 'member@example.com',
        code: '012345',
        deviceId: 'device',
        deviceName: 'browser',
        isPersistent: false,
      }),
    );
    expect(api.loginWithPassword).not.toHaveBeenCalled();
  });
});
