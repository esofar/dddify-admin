import { ModalForm, ProFormText } from '@ant-design/pro-components';
import { useMutation } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import { message, Space, Tag } from 'antd';
import type { FC, ReactElement } from 'react';
import { resetUserPassword } from '@/services/v1/user';
import { passwordRule, requiredRule } from '@/utils/rules';

type ResetPasswordFormValues = API.ResetUserPasswordRequest & {
  confirmPassword?: string;
};

type ResetPasswordFormProps = {
  trigger?: ReactElement;
  user: API.UserListDto;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
};

const ResetPasswordForm: FC<ResetPasswordFormProps> = ({
  trigger,
  user,
  open,
  onOpenChange,
}) => {
  const intl = useIntl();
  const [messageApi, contextHolder] = message.useMessage();

  const resetMutation = useMutation({
    mutationFn: (values: API.ResetUserPasswordRequest) =>
      resetUserPassword({ id: user.id }, values),
  });

  return (
    <>
      {contextHolder}
      <ModalForm<ResetPasswordFormValues>
        title={
          <Space>
            <FormattedMessage id="user.formTitle.restPassword" />
            <Tag color="blue" variant="filled" style={{ fontWeight: 'normal' }}>
              {user.name}
            </Tag>
          </Space>
        }
        trigger={trigger}
        open={open}
        onOpenChange={onOpenChange}
        grid
        layout="horizontal"
        labelCol={{ span: 5 }}
        colProps={{ xs: 24, sm: 24 }}
        width={500}
        autoComplete="off"
        onFinish={async (values) => {
          const { success, errorMessage } = await resetMutation.mutateAsync({
            newPassword: values.newPassword,
          });

          if (!success) {
            messageApi.error(
              errorMessage ??
                intl.formatMessage({ id: 'message.reset.failure' }),
            );
            return false;
          }

          messageApi.success(
            intl.formatMessage({ id: 'message.reset.success' }),
          );
          return true;
        }}
        modalProps={{
          destroyOnHidden: true,
          maskClosable: false,
          okButtonProps: {
            loading: resetMutation.isPending,
          },
        }}
      >
        <ProFormText.Password
          name="newPassword"
          label={<FormattedMessage id="user.label.password" />}
          tooltip={<FormattedMessage id="form.rules.password.tooltip" />}
          fieldProps={{
            showCount: false,
            maxLength: 16,
            autoComplete: 'new-password',
          }}
          rules={[requiredRule, passwordRule]}
        />
        <ProFormText.Password
          name="confirmPassword"
          label={<FormattedMessage id="user.label.confirmPassword" />}
          dependencies={['newPassword']}
          fieldProps={{
            showCount: false,
            maxLength: 16,
            autoComplete: 'new-password',
          }}
          rules={[
            requiredRule,
            ({ getFieldValue }) => ({
              validator(_, value) {
                if (!value || getFieldValue('newPassword') === value) {
                  return Promise.resolve();
                }
                return Promise.reject(
                  new Error(
                    intl.formatMessage({ id: 'form.rules.confirmPassword' }),
                  ),
                );
              },
            }),
          ]}
        />
      </ModalForm>
    </>
  );
};

export default ResetPasswordForm;
