import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  DeleteOutlined,
  EditOutlined,
  ExclamationCircleFilled,
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
  Modal,
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
  resetUserPassword,
  searchUsers,
} from '@/services/v1/user';
import AssignRolesForm from './components/AssignRolesForm';
import UserForm from './components/UserForm';
import {
  toDepartmentTreeNodes,
  toRoleValueEnum,
  USER_PERMISSIONS,
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
  const [modalApi, modalContextHolder] = Modal.useModal();
  const [assigningUser, setAssigningUser] = useState<API.UserListDto>();

  const { data: departments = [] } = useQuery({
    queryKey: ['users', 'departments', 'all'],
    queryFn: async () => {
      const { success, data, errorMessage } = await getAllDepartments();

      if (!success) {
        throw new Error(errorMessage);
      }

      return toDepartmentTreeNodes(data);
    },
  });

  const { data: roleValueEnum = {} } = useQuery({
    queryKey: ['users', 'roles', 'all'],
    queryFn: async () => {
      const { success, data, errorMessage } = await getAllRoles();

      if (!success) {
        throw new Error(errorMessage);
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

  const handleResetPassword = useCallback(
    (record: API.UserListDto) => {
      modalApi.confirm({
        title: <FormattedMessage id="user.resetPassword.title" />,
        content: (
          <Space orientation="vertical" size="small" style={{ width: '100%' }}>
            <Text>
              <FormattedMessage id="user.resetPassword.summary" />
            </Text>
            <Text type="secondary">{record.email}</Text>
          </Space>
        ),
        okButtonProps: {
          danger: true,
        },
        maskClosable: false,
        onOk: async () => {
          const { success, errorMessage } = await resetUserPassword({
            id: record.id,
          });

          if (!success) {
            messageApi.error(
              errorMessage ?? intl.formatMessage({ id: 'message.reset.failure' }),
            );
            throw new Error(errorMessage ?? 'Reset password failed.');
          }

          messageApi.success(
            intl.formatMessage({ id: 'message.reset.success' }),
          );
        },
      });
    },
    [intl, messageApi, modalApi],
  );

  const handleConfirmUserAction = useCallback(
    (
      record: API.UserListDto,
      action: UserAction,
      titleId: string,
      descriptionId: string,
      successMessageId: string,
      failureMessageId: string,
      danger?: boolean,
    ) => {
      modalApi.confirm({
        title: <FormattedMessage id={titleId} />,
        content: <FormattedMessage id={descriptionId} />,
        okButtonProps: {
          danger,
        },
        onOk: () =>
          runUserAction(
            record,
            action,
            successMessageId,
            failureMessageId,
          ),
      });
    },
    [modalApi, runUserAction],
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
        render: (_, record) => {
          const canAssignRoles = access.has(USER_PERMISSIONS.assignRoles);
          const canResetPassword = access.has(USER_PERMISSIONS.resetPassword);

          return (
            <Space size={0} separator={<Divider orientation="vertical" />}>
              {access.has(USER_PERMISSIONS.update) && (
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

              {record.status === USER_STATUS.Enabled
                ? access.has(USER_PERMISSIONS.disable) && (
                  <Button
                    type="link"
                    size="small"
                    danger
                    icon={<CloseCircleOutlined />}
                    onClick={() =>
                      handleConfirmUserAction(
                        record,
                        disableUser,
                        'user.confirm.disable.title',
                        'user.confirm.disable.description',
                        'message.disable.success',
                        'message.disable.failure',
                        true,
                      )
                    }
                  >
                    <FormattedMessage id="common.button.disable" />
                  </Button>
                )
                : access.has(USER_PERMISSIONS.enable) && (
                  <Button
                    type="link"
                    size="small"
                    icon={<CheckCircleOutlined />}
                    onClick={() =>
                      handleConfirmUserAction(
                        record,
                        enableUser,
                        'user.confirm.enable.title',
                        'user.confirm.enable.description',
                        'message.enable.success',
                        'message.enable.failure',
                      )
                    }
                  >
                    <FormattedMessage id="common.button.enable" />
                  </Button>
                )}

              {access.has(USER_PERMISSIONS.delete) && (
                <Button
                  type="link"
                  size="small"
                  danger
                  icon={<DeleteOutlined />}
                  onClick={() =>
                    handleConfirmUserAction(
                      record,
                      deleteUser,
                      'user.confirm.delete.title',
                      'user.confirm.delete.description',
                      'message.delete.success',
                      'message.delete.failure',
                      true,
                    )
                  }
                >
                  <FormattedMessage id="common.button.delete" />
                </Button>
              )}

              {(canAssignRoles || canResetPassword) && (
                <TableDropdown
                  menus={[
                    ...(canAssignRoles
                      ? [
                        {
                          key: 'assignRoles',
                          name: (
                            <Space size={4}>
                              <SafetyCertificateOutlined />
                              <FormattedMessage id="user.action.assignRoles" />
                            </Space>
                          ),
                        },
                      ]
                      : []),
                    ...(canResetPassword
                      ? [
                        {
                          key: 'resetPassword',
                          name: (
                            <Space size={4}>
                              <UndoOutlined />
                              <FormattedMessage id="user.action.resetPassword" />
                            </Space>
                          ),
                        },
                      ]
                      : []),
                  ]}
                  onSelect={(key) => {
                    if (key === 'assignRoles' && canAssignRoles) {
                      setAssigningUser(record);
                      return;
                    }

                    if (key === 'resetPassword' && canResetPassword) {
                      handleResetPassword(record);
                    }
                  }}
                />
              )}
            </Space>
          );
        },
      },
    ],
    [
      access,
      departments,
      handleConfirmUserAction,
      handleResetPassword,
      reloadTable,
      roleValueEnum,
      runUserAction,
    ],
  );

  return (
    <PageContainer title={false}>
      {contextHolder}
      {modalContextHolder}
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
            {access.has(USER_PERMISSIONS.create) && (
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
      {access.has(USER_PERMISSIONS.assignRoles) && assigningUser && (
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
    </PageContainer>
  );
};

export default UserPage;
