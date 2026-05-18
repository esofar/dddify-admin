import type { ProFormInstance } from '@ant-design/pro-components';
import {
  DrawerForm,
  ProFormDigit,
  ProFormSwitch,
  ProFormText,
  ProFormTextArea,
} from '@ant-design/pro-components';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import { message } from 'antd';
import type { FC, ReactElement } from 'react';
import { useCallback, useRef, useState } from 'react';
import { createRole, getRoleDetail, updateRole } from '@/services/v1/role';
import { noSpecialRule, requiredRule } from '@/utils/rules';

type RoleFormValues = {
  concurrencyStamp?: string | null;
  name: string;
  isDefault: boolean;
  order: number;
  description: string;
};

type RoleFormProps = {
  trigger: ReactElement;
  role?: API.RoleListDto;
  onSuccess?: () => void;
};

const RoleForm: FC<RoleFormProps> = ({ trigger, role, onSuccess }) => {
  const intl = useIntl();
  const queryClient = useQueryClient();
  const formRef = useRef<ProFormInstance<RoleFormValues> | undefined>(
    undefined,
  );
  const [open, setOpen] = useState(false);
  const [messageApi, contextHolder] = message.useMessage();

  const isEditing = Boolean(role?.id);

  const saveMutation = useMutation({
    mutationFn: async (values: RoleFormValues) => {
      if (isEditing && role?.id) {
        const body: API.UpdateRoleRequest = {
          name: values.name,
          isDefault: values.isDefault,
          order: values.order,
          description: values.description,
          concurrencyStamp: values.concurrencyStamp,
        };

        return updateRole({ id: role.id }, body);
      }

      const body: API.CreateRoleRequest = {
        name: values.name,
        isDefault: values.isDefault,
        order: values.order,
        description: values.description,
      };

      return createRole(body);
    },
  });

  const loadRoleDetail = useCallback(async () => {
    if (!role?.id) {
      formRef.current?.resetFields();
      formRef.current?.setFieldsValue({
        isDefault: false,
        order: 0,
      });
      return;
    }

    const { success, data, errorMessage } = await getRoleDetail({
      id: role.id,
    });

    if (!success || !data) {
      messageApi.error(errorMessage);
      return;
    }

    formRef.current?.setFieldsValue({
      concurrencyStamp: data.concurrencyStamp,
      name: data.name,
      isDefault: data.isDefault,
      order: data.order,
      description: data.description,
    });
  }, [messageApi, role?.id]);

  const handleOpenChange = useCallback(
    async (nextOpen: boolean) => {
      setOpen(nextOpen);

      if (!nextOpen) {
        formRef.current?.resetFields();
        return;
      }

      await loadRoleDetail();
    },
    [loadRoleDetail],
  );

  return (
    <>
      {contextHolder}
      <DrawerForm<RoleFormValues>
        title={
          <FormattedMessage
            id={isEditing ? 'role.formTitle.update' : 'role.formTitle.create'}
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
          queryClient.invalidateQueries({ queryKey: ['roles'] });
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
          label={<FormattedMessage id="role.label.name" />}
          fieldProps={{
            showCount: true,
            maxLength: 50,
          }}
          rules={[requiredRule, noSpecialRule]}
        />

        <ProFormSwitch
          name="isDefault"
          label={<FormattedMessage id="role.label.isdefault" />}
          checkedChildren={<FormattedMessage id="common.label.yes" />}
          unCheckedChildren={<FormattedMessage id="common.label.no" />}
          rules={[requiredRule]}
        />

        <ProFormDigit
          name="order"
          label={<FormattedMessage id="role.label.order" />}
          fieldProps={{
            min: 0,
            max: 999,
            changeOnWheel: true,
          }}
          rules={[
            requiredRule,
            {
              type: 'number',
            },
          ]}
        />

        <ProFormTextArea
          name="description"
          label={<FormattedMessage id="role.label.description" />}
          fieldProps={{
            showCount: true,
            maxLength: 100,
          }}
          rules={[requiredRule]}
        />
      </DrawerForm>
    </>
  );
};

export default RoleForm;
