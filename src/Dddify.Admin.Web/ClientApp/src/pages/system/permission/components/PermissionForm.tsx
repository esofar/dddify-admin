import type { ProFormInstance } from '@ant-design/pro-components';
import {
  DrawerForm,
  ProFormDigit,
  ProFormSegmented,
  ProFormText,
  ProFormTreeSelect,
} from '@ant-design/pro-components';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import { Alert, message } from 'antd';
import type { FC, ReactElement } from 'react';
import { useCallback, useRef, useState } from 'react';
import {
  createPermission,
  getAllPermissions,
  updatePermission,
} from '@/services/v1/permission';
import { requiredRule } from '@/utils/rules';
import type { PermissionSelectNode } from '../data';
import { permissionTypeValueEnum, toPermissionSelectNodes } from '../data';

type PermissionFormValues = {
  parentId?: string | null;
  code: string;
  name: string;
  type: string;
  order: number;
};

type PermissionFormProps = {
  trigger: ReactElement;
  permission?: API.PermissionDto;
  onSuccess?: () => void;
};

const PermissionForm: FC<PermissionFormProps> = ({
  trigger,
  permission,
  onSuccess,
}) => {
  const intl = useIntl();
  const queryClient = useQueryClient();
  const formRef = useRef<ProFormInstance<PermissionFormValues> | undefined>(
    undefined,
  );
  const [open, setOpen] = useState(false);
  const [messageApi, contextHolder] = message.useMessage();

  const isEditing = Boolean(permission?.id);

  const { data: permissions = [] } = useQuery<PermissionSelectNode[]>({
    queryKey: ['permissions', 'all', 'select'],
    enabled: open,
    queryFn: async () => {
      const { success, data, errorMessage } = await getAllPermissions();

      if (!success || !data) {
        throw new Error(errorMessage ?? 'Load permissions failed.');
      }

      return toPermissionSelectNodes(data);
    },
  });

  const saveMutation = useMutation({
    mutationFn: async (values: PermissionFormValues) => {
      const body: API.CreateOrUpdatePermissionRequest = {
        parentId: values.parentId,
        code: values.code,
        name: values.name,
        type: values.type,
        order: values.order,
      };

      if (isEditing && permission?.id) {
        return updatePermission({ id: permission.id }, body);
      }

      return createPermission(body);
    },
  });

  const handleOpenChange = useCallback(
    (nextOpen: boolean) => {
      setOpen(nextOpen);

      if (!nextOpen) {
        formRef.current?.resetFields();
        return;
      }

      formRef.current?.setFieldsValue(
        permission
          ? {
            parentId: permission.parentId,
            code: permission.code,
            name: permission.name,
            type: permission.type,
            order: permission.order,
          }
          : {
            type: 'Menu',
            order: 0,
          },
      );
    },
    [permission],
  );

  return (
    <>
      {contextHolder}
      <DrawerForm<PermissionFormValues>
        title={
          <FormattedMessage
            id={
              isEditing
                ? 'permission.formTitle.update'
                : 'permission.formTitle.create'
            }
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
          queryClient.invalidateQueries({ queryKey: ['permissions'] });
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
        <ProFormSegmented
          name="type"
          label={<FormattedMessage id="permission.label.type" />}
          valueEnum={permissionTypeValueEnum}
          fieldProps={{
            block: true,
          }}
          rules={[requiredRule]}
        />

        <ProFormText
          name="code"
          label={<FormattedMessage id="permission.label.code" />}
          fieldProps={{
            showCount: true,
            maxLength: 50,
          }}
          rules={[
            requiredRule,
            {
              pattern: /^[a-z0-9-:]+$/,
              message: intl.formatMessage({ id: 'permission.rules.code' }),
            },
          ]}
        />

        <ProFormText
          name="name"
          label={<FormattedMessage id="permission.label.name" />}
          fieldProps={{
            showCount: true,
            maxLength: 50,
          }}
          rules={[requiredRule]}
        />

        <ProFormTreeSelect
          name="parentId"
          label={<FormattedMessage id="permission.label.parentId" />}
          fieldProps={{
            treeData: permissions,
            allowClear: true,
            showSearch: true,
            treeNodeFilterProp: 'name',
            treeDataSimpleMode: {
              id: 'id',
              pId: 'parentId',
            },
            treeLine: true,
            fieldNames: {
              label: 'name',
              value: 'id',
            },
          }}
        />

        <ProFormDigit
          name="order"
          label={<FormattedMessage id="permission.label.order" />}
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

        <Alert
          style={{
            width: '100%'
          }}
          title={<FormattedMessage id="permission.form.alert.title" />}
          description={
            <div style={{ lineHeight: 1.6 }}>
              <div>
                <FormattedMessage id="permission.form.alert.point1" />
              </div>
              <div>
                <FormattedMessage id="permission.form.alert.point2" />
              </div>
              <div>
                <FormattedMessage id="permission.form.alert.point3" />
              </div>
              <div>
                <FormattedMessage id="permission.form.alert.point4" />
              </div>
            </div>
          }
          type="warning"
          showIcon
        />
      </DrawerForm>
    </>
  );
};

export default PermissionForm;
