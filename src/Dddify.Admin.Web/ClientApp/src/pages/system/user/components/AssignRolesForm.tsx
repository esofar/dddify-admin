import { ModalForm } from '@ant-design/pro-components';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import type { TransferProps } from 'antd';
import { message, Space, Tag, Transfer } from 'antd';
import type { FC, Key, ReactElement } from 'react';
import { useCallback, useState } from 'react';
import { getAllRoles } from '@/services/v1/role';
import { assignUserRoles, getUserRoles } from '@/services/v1/user';

type AssignRolesFormProps = {
  trigger?: ReactElement;
  user: API.UserListDto;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
  onSuccess?: () => void;
};

const AssignRolesForm: FC<AssignRolesFormProps> = ({
  trigger,
  user,
  open,
  onOpenChange,
  onSuccess,
}) => {
  const intl = useIntl();
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const [innerOpen, setInnerOpen] = useState(false);
  const [roles, setRoles] = useState<API.RoleListDto[]>([]);
  const [targetKeys, setTargetKeys] = useState<Key[]>([]);
  const [selectedKeys, setSelectedKeys] = useState<Key[]>([]);
  const mergedOpen = open ?? innerOpen;

  const saveMutation = useMutation({
    mutationFn: (roleIds: string[]) =>
      assignUserRoles({ id: user.id }, roleIds),
  });

  const loadRoles = useCallback(async () => {
    const [allRolesResult, userRolesResult] = await Promise.all([
      getAllRoles(),
      getUserRoles({ id: user.id }),
    ]);

    if (!allRolesResult.success || !allRolesResult.data) {
      messageApi.error(allRolesResult.errorMessage);
      return;
    }

    if (!userRolesResult.success || !userRolesResult.data) {
      messageApi.error(userRolesResult.errorMessage);
      return;
    }

    const userRoles = userRolesResult.data as API.UserRoleDto[];

    setRoles(allRolesResult.data);
    setTargetKeys(userRoles.map((role) => role.roleId));
  }, [messageApi, user.id]);

  const handleOpenChange = useCallback(
    async (nextOpen: boolean) => {
      if (open === undefined) {
        setInnerOpen(nextOpen);
      }

      onOpenChange?.(nextOpen);

      if (!nextOpen) {
        setRoles([]);
        setTargetKeys([]);
        setSelectedKeys([]);
        return;
      }

      await loadRoles();
    },
    [loadRoles, onOpenChange, open],
  );

  const handleChange: TransferProps<API.RoleListDto>['onChange'] = (
    nextTargetKeys,
  ) => {
    setTargetKeys(nextTargetKeys);
  };

  const handleSelectChange: TransferProps<API.RoleListDto>['onSelectChange'] = (
    sourceSelectedKeys,
    targetSelectedKeys,
  ) => {
    setSelectedKeys([...sourceSelectedKeys, ...targetSelectedKeys]);
  };

  return (
    <>
      {contextHolder}
      <ModalForm
        title={
          <Space>
            <FormattedMessage id="user.formTitle.assignRoles" />
            <Tag
              color="geekblue"
              variant="filled"
              style={{ fontWeight: 'normal' }}
            >
              {user.name}
            </Tag>
          </Space>
        }
        trigger={trigger}
        width={520}
        autoComplete="off"
        open={mergedOpen}
        onOpenChange={handleOpenChange}
        onFinish={async () => {
          const roleIds = roles
            .filter((role) => targetKeys.includes(role.id))
            .map((role) => role.id);

          const { success, errorMessage } =
            await saveMutation.mutateAsync(roleIds);

          if (!success) {
            messageApi.error(
              errorMessage ??
                intl.formatMessage({ id: 'message.assign.failure' }),
            );
            return false;
          }

          messageApi.success(
            intl.formatMessage({ id: 'message.assign.success' }),
          );
          queryClient.invalidateQueries({ queryKey: ['users'] });
          onSuccess?.();
          return true;
        }}
        modalProps={{
          destroyOnHidden: true,
          maskClosable: false,
          okButtonProps: {
            loading: saveMutation.isPending,
          },
        }}
      >
        <Transfer<API.RoleListDto>
          dataSource={roles}
          titles={[
            <FormattedMessage key="source" id="user.assign.roles.source" />,
            <FormattedMessage key="target" id="user.assign.roles.target" />,
          ]}
          targetKeys={targetKeys}
          selectedKeys={selectedKeys}
          onChange={handleChange}
          onSelectChange={handleSelectChange}
          rowKey={(item) => item.id}
          render={(item) => item.name}
          oneWay
          style={{ marginTop: 15 }}
          listStyle={{
            width: 220,
            height: 350,
          }}
        />
      </ModalForm>
    </>
  );
};

export default AssignRolesForm;
