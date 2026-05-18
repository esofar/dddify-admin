import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  DeleteOutlined,
  EditOutlined,
  PlusOutlined,
  SafetyCertificateOutlined,
  UndoOutlined,
  UserOutlined,
} from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import {
  PageContainer,
  ProTable,
  TableDropdown,
} from '@ant-design/pro-components';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useAccess, useIntl } from '@umijs/max';
import {
  Avatar,
  Button,
  Divider,
  message,
  Popconfirm,
  Space,
  Tag,
  Typography,
} from 'antd';
import type { FC } from 'react';
import { useCallback, useMemo, useRef, useState } from 'react';
import TagPopover from '@/components/TagPopover';
import { getAllDepartments } from '@/services/v1/department';
import { getAllRoles } from '@/services/v1/role';
import {
  deleteUser,
  disableUser,
  enableUser,
  searchUsers,
} from '@/services/v1/user';
import AssignRolesForm from './components/AssignRolesForm';
import ResetPasswordForm from './components/ResetPasswordForm';
import UserForm from './components/UserForm';
import {
  toDepartmentTreeNodes,
  toRoleValueEnum,
  USER_STATUS,
  userGenderValueEnum,
  userStatusValueEnum,
} from './data';

const { Text } = Typography;

type UserAction = (params: { id: string }) => Promise<API.ApiResult>;

const UserPage: FC = () => {
  const intl = useIntl();
  const access = useAccess();
  const queryClient = useQueryClient();
  const actionRef = useRef<ActionType | null>(null);
  const [messageApi, contextHolder] = message.useMessage();
  const [assigningUser, setAssigningUser] = useState<API.UserListDto>();
  const [resettingUser, setResettingUser] = useState<API.UserListDto>();

  const { data: departments = [] } = useQuery({
    queryKey: ['users', 'departments', 'all'],
    queryFn: async () => {
      const { success, data, errorMessage } = await getAllDepartments();

      if (!success || !data) {
        throw new Error(errorMessage ?? 'Load departments failed.');
      }

      return toDepartmentTreeNodes(data);
    },
  });

  const { data: roleValueEnum = {} } = useQuery({
    queryKey: ['users', 'roles', 'all'],
    queryFn: async () => {
      const { success, data, errorMessage } = await getAllRoles();

      if (!success || !data) {
        throw new Error(errorMessage ?? 'Load roles failed.');
      }

      return toRoleValueEnum(data);
    },
  });

  const reloadTable = useCallback(() => {
    actionRef.current?.reload();
    queryClient.invalidateQueries({ queryKey: ['users'] });
  }, [queryClient]);

  const runUserAction = useCallback(
    async (
      record: API.UserListDto,
      action: UserAction,
      successMessageId: string,
      failureMessageId: string,
    ) => {
      const { success, errorMessage } = await action({ id: record.id });

      if (!success) {
        messageApi.error(
          errorMessage ?? intl.formatMessage({ id: failureMessageId }),
        );
        return;
      }

      messageApi.success(intl.formatMessage({ id: successMessageId }));
      reloadTable();
    },
    [intl, messageApi, reloadTable],
  );

  const columns = useMemo<ProColumns<API.UserListDto>[]>(
    () => [
      {
        title: <FormattedMessage id="user.label.index" />,
        dataIndex: 'index',
        valueType: 'index',
        width: 64,
        align: 'center',
        fixed: 'left',
        search: false,
      },
      {
        title: <FormattedMessage id="user.label.name" />,
        dataIndex: 'name',
        minWidth: 140,
        ellipsis: true,
        render: (_, record) => (
          <Space>
            {record.avatar ? (
              <Avatar size="small" src={record.avatar} />
            ) : (
              <Avatar icon={<UserOutlined />} size="small" />
            )}
            <Text>{record.name}</Text>
          </Space>
        ),
      },
      {
        title: <FormattedMessage id="user.label.nickName" />,
        dataIndex: 'nickName',
        minWidth: 120,
        search: false,
        ellipsis: true,
      },
      {
        title: <FormattedMessage id="user.label.gender" />,
        dataIndex: 'gender',
        valueType: 'select',
        valueEnum: userGenderValueEnum,
        width: 120,
        render: (_, record) => {
          const option = userGenderValueEnum[record.gender];

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
        title: <FormattedMessage id="user.label.email" />,
        dataIndex: 'email',
        minWidth: 180,
        ellipsis: true,
        copyable: true,
      },
      {
        title: <FormattedMessage id="user.label.phoneNumber" />,
        dataIndex: 'phoneNumber',
        minWidth: 150,
        ellipsis: true,
        copyable: true,
      },
      {
        title: <FormattedMessage id="user.label.departmentId" />,
        dataIndex: 'departmentId',
        valueType: 'treeSelect',
        minWidth: 160,
        fieldProps: {
          treeData: departments,
          allowClear: true,
          treeDataSimpleMode: true,
          treeDefaultExpandAll: true,
          treeLine: true,
          fieldNames: {
            label: 'name',
            value: 'id',
          },
        },
        render: (_, record) => record.department?.name ?? '-',
      },
      {
        title: <FormattedMessage id="user.label.roles" />,
        dataIndex: 'roleId',
        valueType: 'select',
        valueEnum: roleValueEnum,
        minWidth: 200,
        render: (_, record) => {
          const roles = record.roles ?? [];

          if (!roles.length) {
            return '-';
          }

          return (
            <TagPopover
              items={roles.map((role) => ({
                key: role.roleId,
                label: role.roleName,
                color: 'geekblue',
              }))}
              showAllInPopover={false}
            />
          );
        },
      },
      {
        title: <FormattedMessage id="user.label.status" />,
        dataIndex: 'status',
        valueType: 'select',
        width: 120,
        valueEnum: userStatusValueEnum,
      },
      {
        title: <FormattedMessage id="user.label.option" />,
        dataIndex: 'option',
        valueType: 'option',
        fixed: 'right',
        width: 300,
        render: (_, record) => (
          <Space size={0} split={<Divider type="vertical" />}>
            {access.has('system:user:update') && (
              <UserForm
                trigger={
                  <Button type="link" size="small" icon={<EditOutlined />}>
                    <FormattedMessage id="common.button.update" />
                  </Button>
                }
                user={record}
                departments={departments}
                onSuccess={reloadTable}
              />
            )}

            {access.has('system:user:delete') && (
              <Popconfirm
                title={<FormattedMessage id="common.confirmText.delete" />}
                okButtonProps={{ danger: true }}
                onConfirm={() =>
                  runUserAction(
                    record,
                    deleteUser,
                    'message.delete.success',
                    'message.delete.failure',
                  )
                }
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

            {record.status === USER_STATUS.Enabled
              ? access.has('system:user:disable') && (
                  <Popconfirm
                    title={<FormattedMessage id="common.confirmText.disable" />}
                    onConfirm={() =>
                      runUserAction(
                        record,
                        disableUser,
                        'message.disable.success',
                        'message.disable.failure',
                      )
                    }
                  >
                    <Button
                      type="link"
                      size="small"
                      icon={<CloseCircleOutlined />}
                    >
                      <FormattedMessage id="common.button.disable" />
                    </Button>
                  </Popconfirm>
                )
              : access.has('system:user:enable') && (
                  <Popconfirm
                    title={<FormattedMessage id="common.confirmText.enable" />}
                    onConfirm={() =>
                      runUserAction(
                        record,
                        enableUser,
                        'message.enable.success',
                        'message.enable.failure',
                      )
                    }
                  >
                    <Button
                      type="link"
                      size="small"
                      icon={<CheckCircleOutlined />}
                    >
                      <FormattedMessage id="common.button.enable" />
                    </Button>
                  </Popconfirm>
                )}

            {(access.has('system:user:assign-roles') ||
              access.has('system:user:reset-password')) && (
              <TableDropdown
                menus={[
                  ...(access.has('system:user:assign-roles')
                    ? [
                        {
                          key: 'assignRoles',
                          name: (
                            <Space size={4}>
                              <SafetyCertificateOutlined />
                              <FormattedMessage id="user.button.assignRoles" />
                            </Space>
                          ),
                        },
                      ]
                    : []),
                  ...(access.has('system:user:reset-password')
                    ? [
                        {
                          key: 'resetPassword',
                          name: (
                            <Space size={4}>
                              <UndoOutlined />
                              <FormattedMessage id="user.button.resetPassword" />
                            </Space>
                          ),
                        },
                      ]
                    : []),
                ]}
                onSelect={(key) => {
                  if (key === 'assignRoles') {
                    setAssigningUser(record);
                    return;
                  }

                  if (key === 'resetPassword') {
                    setResettingUser(record);
                  }
                }}
              />
            )}
          </Space>
        ),
      },
    ],
    [access, departments, reloadTable, roleValueEnum, runUserAction],
  );

  return (
    <PageContainer title={false}>
      {contextHolder}
      <ProTable<API.UserListDto, API.SearchUsersParams>
        actionRef={actionRef}
        rowKey="id"
        columns={columns}
        scroll={{ x: 'max-content' }}
        search={{ labelWidth: 120 }}
        pagination={{ showSizeChanger: true }}
        request={async (params) => {
          const { success, data } = await searchUsers(params);

          return {
            data: data?.items ?? [],
            total: data?.total ?? 0,
            success: success ?? false,
          };
        }}
        headerTitle={
          <Space>
            {access.has('system:user:create') && (
              <UserForm
                trigger={
                  <Button type="primary" icon={<PlusOutlined />}>
                    <FormattedMessage id="common.button.create" />
                  </Button>
                }
                departments={departments}
                onSuccess={reloadTable}
              />
            )}
          </Space>
        }
      />
      {assigningUser && (
        <AssignRolesForm
          open
          user={assigningUser}
          onOpenChange={(open) => {
            if (!open) {
              setAssigningUser(undefined);
            }
          }}
          onSuccess={reloadTable}
        />
      )}
      {resettingUser && (
        <ResetPasswordForm
          open
          user={resettingUser}
          onOpenChange={(open) => {
            if (!open) {
              setResettingUser(undefined);
            }
          }}
        />
      )}
    </PageContainer>
  );
};

export default UserPage;
