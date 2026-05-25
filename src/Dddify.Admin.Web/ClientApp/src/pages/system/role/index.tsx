import {
  DeleteOutlined,
  EditOutlined,
  PlusOutlined,
  SettingOutlined,
} from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useAccess, useIntl } from '@umijs/max';
import {
  Button,
  Divider,
  message,
  Modal,
  Space,
  Tag,
  Tooltip,
  Typography,
} from 'antd';
import type { FC } from 'react';
import { useCallback, useMemo, useRef, useState } from 'react';
import { deleteRole, searchRoles } from '@/services/v1/role';
import AssignPermissionsForm from './components/AssignPermissionsForm';
import RoleForm from './components/RoleForm';
import { ROLE_PERMISSIONS } from './data';

const { Text } = Typography;

const RolePage: FC = () => {
  const intl = useIntl();
  const access = useAccess();
  const queryClient = useQueryClient();
  const actionRef = useRef<ActionType | null>(null);
  const [messageApi, contextHolder] = message.useMessage();
  const [modalApi, modalContextHolder] = Modal.useModal();
  const [assigningRole, setAssigningRole] = useState<API.RoleListDto>();

  const reloadTable = useCallback(() => {
    actionRef.current?.reload();
    queryClient.invalidateQueries({ queryKey: ['roles'] });
  }, [queryClient]);

  const handleDelete = useCallback(
    (role: API.RoleListDto) => {
      modalApi.confirm({
        title: <FormattedMessage id="role.confirm.delete.title" />,
        content: <FormattedMessage id="role.confirm.delete.description" />,
        okButtonProps: {
          danger: true,
        },
        onOk: async () => {
          const { success, errorMessage } = await deleteRole({ id: role.id });

          if (!success) {
            messageApi.error(
              errorMessage ?? intl.formatMessage({ id: 'message.delete.failure' }),
            );
            return;
          }

          messageApi.success(intl.formatMessage({ id: 'message.delete.success' }));
          reloadTable();
        },
      });
    },
    [intl, messageApi, modalApi, reloadTable],
  );

  const columns = useMemo<ProColumns<API.RoleListDto>[]>(
    () => [
      {
        title: <FormattedMessage id="role.label.index" />,
        dataIndex: 'index',
        valueType: 'index',
        width: 64,
        align: 'center',
        fixed: 'left',
        search: false,
      },
      {
        title: <FormattedMessage id="role.label.name" />,
        dataIndex: 'name',
        minWidth: 200,
        ellipsis: true,
        render: (_, record) => (
          <Space size={4}>
            <Text>{record.name}</Text>
            {record.isPreset && (
              <Tag color="purple" variant="filled">
                <FormattedMessage id="role.tag.preset" />
              </Tag>
            )}
            {record.isDefault && (
              <Tag color="cyan" variant="filled">
                <FormattedMessage id="role.tag.default" />
              </Tag>
            )}
          </Space>
        ),
      },
      {
        title: <FormattedMessage id="role.label.description" />,
        dataIndex: 'description',
        minWidth: 260,
        ellipsis: true,
        search: false,
      },
      {
        title: <FormattedMessage id="role.label.assignedUserCount" />,
        dataIndex: 'assignedUserCount',
        minWidth: 140,
        search: false,
      },
      {
        title: <FormattedMessage id="role.label.order" />,
        dataIndex: 'order',
        minWidth: 100,
        search: false,
      },
      {
        title: <FormattedMessage id="role.label.option" />,
        dataIndex: 'option',
        valueType: 'option',
        fixed: 'right',
        width: 230,
        render: (_, record) => (
          <Space size={0} separator={<Divider orientation="vertical" />}>
            {access.has(ROLE_PERMISSIONS.update) && (
              <RoleForm
                trigger={
                  <Button type="link" size="small" icon={<EditOutlined />}>
                    <FormattedMessage id="common.button.update" />
                  </Button>
                }
                role={record}
                onSuccess={reloadTable}
              />
            )}

            {access.has(ROLE_PERMISSIONS.delete) && (
              <Tooltip
                title={
                  record.isPreset ? (
                    <FormattedMessage id="role.tooltip.builtIn.delete" />
                  ) : undefined
                }
              >
                <Button
                  type="link"
                  size="small"
                  danger
                  disabled={record.isPreset}
                  icon={<DeleteOutlined />}
                  onClick={() => handleDelete(record)}
                >
                  <FormattedMessage id="common.button.delete" />
                </Button>
              </Tooltip>
            )}

            {access.has(ROLE_PERMISSIONS.assignPermissions) && (
              <Button
                type="link"
                size="small"
                icon={<SettingOutlined />}
                onClick={() => setAssigningRole(record)}
              >
                <FormattedMessage id="role.action.assignPermissions" />
              </Button>
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
      {modalContextHolder}
      <ProTable<API.RoleListDto, API.SearchRolesParams>
        actionRef={actionRef}
        columns={columns}
        rowKey="id"
        scroll={{ x: 'max-content' }}
        search={{ labelWidth: 120 }}
        pagination={{ showSizeChanger: true }}
        request={async (params) => {
          const { success, data } = await searchRoles(params);

          return {
            data: data?.items ?? [],
            total: data?.total ?? 0,
            success: success ?? false,
          };
        }}
        headerTitle={
          <Space>
            {access.has(ROLE_PERMISSIONS.create) && (
              <RoleForm
                trigger={
                  <Button type="primary" icon={<PlusOutlined />}>
                    <FormattedMessage id="common.button.create" />
                  </Button>
                }
                onSuccess={reloadTable}
              />
            )}
          </Space>
        }
      />
      {access.has(ROLE_PERMISSIONS.assignPermissions) && assigningRole && (
        <AssignPermissionsForm
          open
          role={assigningRole}
          onOpenChange={(open) => {
            if (!open) {
              setAssigningRole(undefined);
            }
          }}
          onSuccess={reloadTable}
        />
      )}
    </PageContainer>
  );
};

export default RolePage;
