import {
  DeleteOutlined,
  DownOutlined,
  EditOutlined,
  PlusOutlined,
  UpOutlined,
} from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useAccess, useIntl } from '@umijs/max';
import { Button, Divider, message, Popconfirm, Space, Tag } from 'antd';
import type { FC, Key } from 'react';
import { useCallback, useMemo, useRef, useState } from 'react';
import { deletePermission, searchPermissions } from '@/services/v1/permission';
import PermissionForm from './components/PermissionForm';
import type { PermissionTreeNode } from './data';
import { buildPermissionTree, permissionTypeValueEnum } from './data';

const PermissionPage: FC = () => {
  const intl = useIntl();
  const access = useAccess();
  const queryClient = useQueryClient();
  const actionRef = useRef<ActionType | null>(null);
  const [messageApi, contextHolder] = message.useMessage();
  const [expandedRowKeys, setExpandedRowKeys] = useState<Key[]>([]);
  const [allRowKeys, setAllRowKeys] = useState<Key[]>([]);

  const reloadTable = useCallback(() => {
    actionRef.current?.reload();
    queryClient.invalidateQueries({ queryKey: ['permissions'] });
  }, [queryClient]);

  const handleDelete = useCallback(
    async (permission: API.PermissionDto) => {
      const { success, errorMessage } = await deletePermission({
        id: permission.id,
      });

      if (!success) {
        messageApi.error(
          errorMessage ?? intl.formatMessage({ id: 'message.delete.failure' }),
        );
        return;
      }

      messageApi.success(intl.formatMessage({ id: 'message.delete.success' }));
      reloadTable();
    },
    [intl, messageApi, reloadTable],
  );

  const columns = useMemo<ProColumns<PermissionTreeNode>[]>(
    () => [
      {
        title: <FormattedMessage id="permission.label.name" />,
        dataIndex: 'name',
        minWidth: 160,
        ellipsis: true,
      },
      {
        title: <FormattedMessage id="permission.label.code" />,
        dataIndex: 'code',
        minWidth: 180,
        ellipsis: true,
        copyable: true,
      },
      {
        title: <FormattedMessage id="permission.label.type" />,
        dataIndex: 'type',
        valueType: 'select',
        valueEnum: permissionTypeValueEnum,
        minWidth: 130,
        render: (_, record) => {
          const option = permissionTypeValueEnum[record.type];

          if (!option) {
            return '-';
          }

          return (
            <Tag icon={option.icon} color={option.color} variant="filled">
              {option.text}
            </Tag>
          );
        },
      },
      {
        title: <FormattedMessage id="permission.label.order" />,
        dataIndex: 'order',
        minWidth: 100,
        search: false,
      },
      {
        title: <FormattedMessage id="permission.label.option" />,
        dataIndex: 'option',
        valueType: 'option',
        fixed: 'right',
        width: 170,
        render: (_, record) => (
          <Space size={0} split={<Divider type="vertical" />}>
            {access.has('system:permission:update') && (
              <PermissionForm
                trigger={
                  <Button type="link" size="small" icon={<EditOutlined />}>
                    <FormattedMessage id="common.button.update" />
                  </Button>
                }
                permission={record}
                onSuccess={reloadTable}
              />
            )}

            {access.has('system:permission:delete') && (
              <Popconfirm
                title={<FormattedMessage id="common.confirmText.delete" />}
                okButtonProps={{ danger: true }}
                onConfirm={() => handleDelete(record)}
              >
                <Button
                  type="link"
                  size="small"
                  danger
                  icon={<DeleteOutlined />}
                >
                  <FormattedMessage id="common.button.delete" />
                </Button>
              </Popconfirm>
            )}
          </Space>
        ),
      },
    ],
    [access, handleDelete, reloadTable],
  );

  return (
    <PageContainer title={false}>
      {contextHolder}
      <ProTable<PermissionTreeNode, API.SearchPermissionsParams>
        actionRef={actionRef}
        columns={columns}
        scroll={{ x: 'max-content' }}
        rowKey="id"
        pagination={false}
        search={{ labelWidth: 120 }}
        request={async (params) => {
          const { success, data } = await searchPermissions(params);
          const permissions = (data ?? []) as API.PermissionDto[];

          setAllRowKeys(permissions.map((item) => item.id));

          return {
            data: buildPermissionTree(permissions),
            success: success ?? false,
          };
        }}
        expandable={{
          expandedRowKeys,
          onExpandedRowsChange: (keys) => setExpandedRowKeys([...keys]),
        }}
        headerTitle={
          <Space>
            {access.has('system:permission:create') && (
              <PermissionForm
                trigger={
                  <Button type="primary" icon={<PlusOutlined />}>
                    <FormattedMessage id="common.button.create" />
                  </Button>
                }
                onSuccess={reloadTable}
              />
            )}
            <Button
              key="expand"
              icon={
                expandedRowKeys.length > 0 ? <UpOutlined /> : <DownOutlined />
              }
              onClick={() =>
                setExpandedRowKeys(
                  expandedRowKeys.length > 0 ? [] : [...allRowKeys],
                )
              }
            >
              {expandedRowKeys.length > 0 ? (
                <FormattedMessage id="common.button.collapseAll" />
              ) : (
                <FormattedMessage id="common.button.expandAll" />
              )}
            </Button>
          </Space>
        }
      />
    </PageContainer>
  );
};

export default PermissionPage;
