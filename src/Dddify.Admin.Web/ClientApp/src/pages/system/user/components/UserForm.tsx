import type { ProFormInstance } from '@ant-design/pro-components';
import {
  DrawerForm,
  ProFormDatePicker,
  ProFormSelect,
  ProFormText,
  ProFormTreeSelect,
} from '@ant-design/pro-components';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import { message } from 'antd';
import type { FC, ReactElement } from 'react';
import { useCallback, useRef, useState } from 'react';
import { createUser, getUserDetail, updateUser } from '@/services/v1/user';
import {
  emailRule,
  noSpecialRule,
  passwordRule,
  phoneNumberRule,
  requiredRule,
} from '@/utils/rules';
import type { DepartmentTreeNode } from '../data';
import { userGenderValueEnum } from '../data';

type UserFormValues = {
  concurrencyStamp?: string | null;
  password?: string;
  confirmPassword?: string;
  name: string;
  nickName?: string | null;
  gender: string;
  birthDate?: string | null;
  email: string;
  phoneNumber: string;
  departmentId: string;
};

type UserFormProps = {
  trigger: ReactElement;
  user?: API.UserListDto;
  departments: DepartmentTreeNode[];
  onSuccess?: () => void;
};

const UserForm: FC<UserFormProps> = ({
  trigger,
  user,
  departments,
  onSuccess,
}) => {
  const intl = useIntl();
  const queryClient = useQueryClient();
  const formRef = useRef<ProFormInstance<UserFormValues> | undefined>(
    undefined,
  );
  const [open, setOpen] = useState(false);
  const [messageApi, contextHolder] = message.useMessage();

  const isEditing = Boolean(user?.id);

  const saveMutation = useMutation({
    mutationFn: async (values: UserFormValues) => {
      if (isEditing && user?.id) {
        const body: API.UpdateUserRequest = {
          name: values.name,
          nickName: values.nickName,
          gender: values.gender,
          birthDate: values.birthDate,
          email: values.email,
          phoneNumber: values.phoneNumber,
          departmentId: values.departmentId,
          concurrencyStamp: values.concurrencyStamp,
        };

        return updateUser({ id: user.id }, body);
      }

      const body: API.CreateUserRequest = {
        password: values.password ?? '',
        name: values.name,
        nickName: values.nickName,
        gender: values.gender,
        birthDate: values.birthDate,
        email: values.email,
        phoneNumber: values.phoneNumber,
        departmentId: values.departmentId,
      };

      return createUser(body);
    },
  });

  const loadUserDetail = useCallback(async () => {
    if (!user?.id) {
      formRef.current?.resetFields();
      return;
    }

    const { success, data, errorMessage } = await getUserDetail({
      id: user.id,
    });

    if (!success || !data) {
      messageApi.error(errorMessage);
      return;
    }

    formRef.current?.setFieldsValue({
      concurrencyStamp: data.concurrencyStamp,
      name: data.name,
      nickName: data.nickName,
      gender: data.gender,
      birthDate: data.birthDate,
      email: data.email,
      phoneNumber: data.phoneNumber,
      departmentId: data.department?.id,
    });
  }, [messageApi, user?.id]);

  const handleOpenChange = useCallback(
    async (nextOpen: boolean) => {
      setOpen(nextOpen);

      if (!nextOpen) {
        formRef.current?.resetFields();
        return;
      }

      if (isEditing) {
        await loadUserDetail();
      }
    },
    [isEditing, loadUserDetail],
  );

  return (
    <>
      {contextHolder}
      <DrawerForm<UserFormValues>
        title={
          <FormattedMessage
            id={isEditing ? 'user.formTitle.update' : 'user.formTitle.create'}
          />
        }
        trigger={trigger}
        grid
        layout="horizontal"
        labelCol={{ span: 5 }}
        colProps={{ xs: 24, sm: 24 }}
        width={520}
        autoComplete="off"
        formRef={formRef}
        open={open}
        onOpenChange={handleOpenChange}
        onFinish={async (values) => {
          const { success, errorMessage } =
            await saveMutation.mutateAsync(values);

          if (!success) {
            messageApi.error(
              errorMessage ??
                intl.formatMessage({
                  id: isEditing
                    ? 'message.update.failure'
                    : 'message.create.failure',
                }),
            );
            return false;
          }

          messageApi.success(
            intl.formatMessage({
              id: isEditing
                ? 'message.update.success'
                : 'message.create.success',
            }),
          );
          queryClient.invalidateQueries({ queryKey: ['users'] });
          onSuccess?.();
          return true;
        }}
        submitter={{
          submitButtonProps: {
            loading: saveMutation.isPending,
          },
        }}
        drawerProps={{
          destroyOnHidden: true,
          maskClosable: false,
        }}
      >
        <ProFormText name="concurrencyStamp" hidden colProps={{ span: 0 }} />

        <ProFormText
          name="name"
          label={<FormattedMessage id="user.label.name" />}
          fieldProps={{
            showCount: true,
            maxLength: 20,
          }}
          rules={[requiredRule, noSpecialRule]}
        />

        <ProFormText
          name="nickName"
          label={<FormattedMessage id="user.label.nickName" />}
          fieldProps={{
            showCount: true,
            maxLength: 20,
          }}
          rules={[noSpecialRule]}
        />

        <ProFormSelect
          name="gender"
          label={<FormattedMessage id="user.label.gender" />}
          valueEnum={userGenderValueEnum}
          rules={[requiredRule]}
        />

        <ProFormDatePicker
          name="birthDate"
          label={<FormattedMessage id="user.label.birthDate" />}
          width="100%"
        />

        <ProFormText
          name="email"
          label={<FormattedMessage id="user.label.email" />}
          fieldProps={{
            showCount: true,
            maxLength: 50,
          }}
          rules={[requiredRule, emailRule]}
        />

        <ProFormText
          name="phoneNumber"
          label={<FormattedMessage id="user.label.phoneNumber" />}
          fieldProps={{
            showCount: true,
            maxLength: 11,
          }}
          rules={[requiredRule, phoneNumberRule]}
        />

        <ProFormTreeSelect
          name="departmentId"
          label={<FormattedMessage id="user.label.departmentId" />}
          fieldProps={{
            treeData: departments,
            allowClear: true,
            treeDataSimpleMode: true,
            treeDefaultExpandAll: true,
            treeLine: true,
            fieldNames: {
              label: 'name',
              value: 'id',
            },
          }}
          rules={[requiredRule]}
        />

        {!isEditing && (
          <>
            <ProFormText.Password
              name="password"
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
              dependencies={['password']}
              fieldProps={{
                showCount: false,
                maxLength: 16,
                autoComplete: 'new-password',
              }}
              rules={[
                requiredRule,
                ({ getFieldValue }) => ({
                  validator(_, value) {
                    if (!value || getFieldValue('password') === value) {
                      return Promise.resolve();
                    }
                    return Promise.reject(
                      new Error(
                        intl.formatMessage({
                          id: 'form.rules.confirmPassword',
                        }),
                      ),
                    );
                  },
                }),
              ]}
            />
          </>
        )}
      </DrawerForm>
    </>
  );
};

export default UserForm;
