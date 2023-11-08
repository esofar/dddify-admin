import Footer from '@/components/Footer';
import {
  AlipayCircleOutlined,
  LockOutlined,
  MobileOutlined,
  TaobaoCircleOutlined,
  UserOutlined,
  WeiboCircleOutlined,
} from '@ant-design/icons';
import type { ProFormInstance } from '@ant-design/pro-components';
import {
  LoginForm,
  ProFormCaptcha,
  ProFormCheckbox,
  ProFormText,
} from '@ant-design/pro-components';
import { FormattedMessage, history, SelectLang, useIntl, useModel } from '@umijs/max';
import { message, Tabs } from 'antd';
import React, { useState, useRef } from 'react';
import styles from './index.less';

import { saveToken } from '@/utils';
import { signin, getSigninCaptcha } from '@/services/account';

type LoginType = 'phone' | 'email';

const Login: React.FC = () => {
  const [loginType, setLoginType] = useState<LoginType>('email');
  const { initialState, setInitialState } = useModel('@@initialState');

  const intl = useIntl();
  const formRef = useRef<ProFormInstance>();

  const fetchUserInfo = async () => {
    const userInfo = await initialState?.fetchUserInfo?.();
    if (userInfo) {
      await setInitialState((s) => ({
        ...s,
        currentUser: userInfo,
      }));
    }
  };

  const handleSubmit = async (values: API.SigninRequest) => {
    const { success, data, errorMessage, error } = await signin({ ...values, type: loginType });
    if (success) {
      saveToken(data?.accessToken, data?.refreshToken);
      await fetchUserInfo();
      message.success(intl.formatMessage({ id: 'pages.login.success' }));
      const urlParams = new URL(window.location.href).searchParams;
      history.push(urlParams.get('redirect') || '/');
    } else {
      if (errorMessage) {
        message.error(errorMessage);
      }
      if (error) {
        const fields = [];
        for (const key in error) {
          fields.push({ name: key, errors: error[key] });
        }
        formRef.current?.setFields(fields);
      }
    }
  };

  return (
    <div className={styles.container}>
      <div className={styles.lang} data-lang>
        {SelectLang && <SelectLang />}
      </div>
      <div className={styles.content}>
        <LoginForm
          formRef={formRef}
          logo={<img alt="logo" src="/logo.svg" />}
          title={intl.formatMessage({ id: 'pages.layouts.title' })}
          subTitle={intl.formatMessage({ id: 'pages.layouts.subtitle' })}
          initialValues={{
            autoLogin: true,
          }}
          actions={[
            <FormattedMessage key="loginWith" id="pages.login.loginWith" />,
            <AlipayCircleOutlined key="AlipayCircleOutlined" className={styles.icon} />,
            <TaobaoCircleOutlined key="TaobaoCircleOutlined" className={styles.icon} />,
            <WeiboCircleOutlined key="WeiboCircleOutlined" className={styles.icon} />,
          ]}
          onFinish={async (values: API.SigninRequest) => {
            await handleSubmit(values);
          }}
        >
          <Tabs activeKey={loginType} onChange={(activeKey) => setLoginType(activeKey as LoginType)}>
            <Tabs.TabPane
              key="email"
              tab={intl.formatMessage({ id: 'pages.login.emailTab.label' })}
            />
            <Tabs.TabPane
              key="phone"
              tab={intl.formatMessage({ id: 'pages.login.phoneTab.label' })}
            />
          </Tabs>

          {loginType === 'email' && (
            <>
              <ProFormText
                name="email"
                fieldProps={{
                  size: 'large',
                  prefix: <UserOutlined className={styles.prefixIcon} />,
                }}
                placeholder={intl.formatMessage({ id: 'pages.login.email.placeholder' })}
                rules={[
                  {
                    required: true,
                    message: <FormattedMessage id="pages.login.email.required" />,
                  },
                ]}
              />
              <ProFormText.Password
                name="password"
                fieldProps={{
                  size: 'large',
                  prefix: <LockOutlined className={styles.prefixIcon} />,
                }}
                placeholder={intl.formatMessage({ id: 'pages.login.password.placeholder' })}
                rules={[
                  {
                    required: true,
                    message: <FormattedMessage id="pages.login.password.required" />,
                  },
                ]}
              />
            </>
          )}

          {loginType === 'phone' && (
            <>
              <ProFormText
                fieldProps={{
                  size: 'large',
                  prefix: <MobileOutlined className={styles.prefixIcon} />,
                }}
                name="phoneNumber"
                placeholder={intl.formatMessage({ id: 'pages.login.phoneNumber.placeholder' })}
                rules={[
                  {
                    required: true,
                    message: <FormattedMessage id="pages.login.phoneNumber.required" />
                  },
                  {
                    pattern: /^1\d{10}$/,
                    message: <FormattedMessage id="pages.login.phoneNumber.invalid" />
                  },
                ]}
              />
              <ProFormCaptcha
                fieldProps={{
                  size: 'large',
                  prefix: <LockOutlined className={styles.prefixIcon} />,
                }}
                captchaProps={{
                  size: 'large',
                }}
                placeholder={intl.formatMessage({ id: 'pages.login.captcha.placeholder' })}
                captchaTextRender={(timing, count) => {
                  if (timing) {
                    return `${count} ${intl.formatMessage({ id: 'pages.login.getCaptchaSecondText' })}`;
                  }
                  return intl.formatMessage({ id: 'pages.login.getVerificationCode' });
                }}
                name="captcha"
                phoneName="phoneNumber"
                rules={[
                  {
                    required: true,
                    message: <FormattedMessage id="pages.login.captcha.required" />,
                  },
                ]}
                onGetCaptcha={async (phone) => {
                  const res = await getSigninCaptcha({ phone });
                  if (res.success) {
                    message.success(intl.formatMessage({ id: 'pages.login.captcha.success' }));
                  } else {
                    message.error(intl.formatMessage({ id: 'pages.login.captcha.failure' }));
                  }
                }}
              />
            </>
          )}

          <div style={{ marginBottom: 24 }}>
            <ProFormCheckbox noStyle name="rememberMe">
              <FormattedMessage id="pages.login.rememberMe" />
            </ProFormCheckbox>
            <a style={{ float: 'right' }}>
              <FormattedMessage id="pages.login.forgotPassword" />
            </a>
          </div>
        </LoginForm>
      </div>
      <Footer />
    </div>
  );
};

export default Login;