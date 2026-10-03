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
import { Alert, message, Space, theme } from 'antd';
import type { FC, ReactElement } from 'react';
import { useCallback, useRef, useState } from 'react';
import {
  createPermission,
  getAllPermissions,
  updatePermission,
} from '@/services/v1/permission';
import { permissionCodeRule, requiredRule } from '@/utils/rules';
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
  const { token } = theme.useToken();
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
                ? 'permission.action.update'
                : 'permission.action.create'
            }
          />
        }
        trigger={trigger}
        grid
        layout="horizontal"
        labelCol={{ span: 5 }}
        colProps={{ xs: 24, sm: 24 }}
        width={560}
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
        <div
          style={{
            width: '100%',
            padding: `${token.paddingMD}px ${token.padding}px 0`,
            border: `1px solid ${token.colorBorderSecondary}`,
            borderRadius: token.borderRadius,
          }}
        >
          <ProFormText
            name="name"
            label={<FormattedMessage id="permission.label.name" />}
            fieldProps={{
              showCount: true,
              maxLength: 50,
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
            rules={[requiredRule, permissionCodeRule]}
          />

          <ProFormSegmented
            name="type"
            label={<FormattedMessage id="permission.label.type" />}
            valueEnum={permissionTypeValueEnum}
            fieldProps={{
              block: true,
            }}
            rules={[requiredRule]}
          />

          <ProFormTreeSelect
            name="parentId"
            label={<FormattedMessage id="permission.label.parentId" />}
            tooltip={<FormattedMessage id="permission.label.parentId.tooltip" />}
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
        </div>

        <Alert
          style={{
            width: '100%',
            marginTop: 16,
          }}
          title={<FormattedMessage id="permission.form.notice.title" />}
          description={
            <Space orientation="vertical" size={4}>
              <span>
                <FormattedMessage id="permission.form.notice.item0" />
              </span>
              <span>
                <FormattedMessage id="permission.form.notice.item1" />
              </span>
              <span>
                <FormattedMessage id="permission.form.notice.item2" />
              </span>
              <span>
                <FormattedMessage id="permission.form.notice.item3" />
              </span>
              <span>
                <FormattedMessage id="permission.form.notice.item4" />
              </span>
            </Space>
          }
          type="info"
          showIcon
        />
      </DrawerForm>
    </>
  );
};

export default PermissionForm;
