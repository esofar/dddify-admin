import { DrawerForm } from '@ant-design/pro-components';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import type { TreeDataNode, TreeProps } from 'antd';
import { Button, message, Space, Tag, Tree, theme } from 'antd';
import type { FC } from 'react';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { getAllPermissions } from '@/services/v1/permission';
import { assignRolePermissions, getRolePermissions } from '@/services/v1/role';
import { permissionTypeValueEnum } from '../../permission/data';

type AssignPermissionsFormProps = {
  open: boolean;
  role?: API.RoleListDto;
  onOpenChange: (open: boolean) => void;
  onSuccess?: () => void;
};

const EMPTY_PERMISSIONS: API.PermissionDto[] = [];
const EMPTY_ROLE_PERMISSIONS: API.RolePermissionDto[] = [];

const AssignPermissionsForm: FC<AssignPermissionsFormProps> = ({
  open,
  role,
  onOpenChange,
  onSuccess,
}) => {
  const intl = useIntl();
  const queryClient = useQueryClient();
  const { token } = theme.useToken();
  const [messageApi, contextHolder] = message.useMessage();
  const [checkedKeys, setCheckedKeys] = useState<React.Key[]>([]);
  const [expandedKeys, setExpandedKeys] = useState<React.Key[]>([]);

  const { data: permissions } = useQuery<API.PermissionDto[]>({
    queryKey: ['roles', 'permissions', 'all'],
    enabled: open,
    queryFn: async () => {
      const { success, data, errorMessage } = await getAllPermissions();

      if (!success || !data) {
        throw new Error(errorMessage ?? 'Load permissions failed.');
      }

      return data as API.PermissionDto[];
    },
  });

  const { data: rolePermissions } = useQuery<API.RolePermissionDto[]>({
    queryKey: ['roles', role?.id, 'permissions'],
    enabled: open && Boolean(role?.id),
    queryFn: async () => {
      if (!role?.id) {
        return [] as API.RolePermissionDto[];
      }

      const { success, data, errorMessage } = await getRolePermissions({
        id: role.id,
      });

      if (!success || !data) {
        throw new Error(errorMessage ?? 'Load role permissions failed.');
      }

      return data as API.RolePermissionDto[];
    },
  });

  const permissionItems = permissions ?? EMPTY_PERMISSIONS;
  const rolePermissionItems = rolePermissions ?? EMPTY_ROLE_PERMISSIONS;

  const treeData = useMemo<TreeDataNode[]>(() => {
    const map = new Map<string, TreeDataNode & { children: TreeDataNode[] }>();
    const roots: TreeDataNode[] = [];

    permissionItems.forEach((permission) => {
      const option = permissionTypeValueEnum[permission.type];

      map.set(permission.id, {
        title: permission.name,
        key: permission.id,
        icon: option?.icon,
        children: [],
      });
    });

    permissionItems.forEach((permission) => {
      const node = map.get(permission.id);

      if (!node) {
        return;
      }

      if (permission.parentId && map.has(permission.parentId)) {
        map.get(permission.parentId)?.children.push(node);
        return;
      }

      roots.push(node);
    });

    return roots;
  }, [permissionItems]);

  useEffect(() => {
    if (!open) {
      setCheckedKeys([]);
      setExpandedKeys([]);
      return;
    }

    setExpandedKeys(permissionItems.map((permission) => permission.id));
    setCheckedKeys(
      rolePermissionItems.map((permission) => permission.permissionId),
    );
  }, [open, permissionItems, rolePermissionItems]);

  const getParentIds = useCallback(
    (id: string) => {
      const parents: string[] = [];
      let current = permissionItems.find((permission) => permission.id === id);

      while (current?.parentId) {
        parents.push(current.parentId);
        current = permissionItems.find(
          (permission) => permission.id === current?.parentId,
        );
      }

      return parents;
    },
    [permissionItems],
  );

  const getChildIds = useCallback(
    (id: string) => {
      const children: string[] = [];
      const stack = [id];

      while (stack.length) {
        const current = stack.pop();

        if (!current) {
          continue;
        }

        children.push(current);
        permissionItems
          .filter((permission) => permission.parentId === current)
          .forEach((permission) => {
            stack.push(permission.id);
          });
      }

      return children;
    },
    [permissionItems],
  );

  const handleCheck: TreeProps['onCheck'] = useCallback(
    (checked, info) => {
      let next = Array.isArray(checked) ? [...checked] : [...checked.checked];
      const nodeId = String(info.node.key);

      if (info.checked) {
        next = Array.from(new Set([...next, nodeId, ...getParentIds(nodeId)]));
      } else {
        const childIds = getChildIds(nodeId);
        next = next.filter((key) => !childIds.includes(String(key)));
      }

      setCheckedKeys(next);
    },
    [getChildIds, getParentIds],
  );

  const saveMutation = useMutation({
    mutationFn: async () => {
      if (!role?.id) {
        return { success: false } as API.ApiResult;
      }

      return assignRolePermissions(
        { id: role.id },
        checkedKeys.map((key) => String(key)),
      );
    },
  });

  return (
    <>
      {contextHolder}
      <DrawerForm
        title={
          <Space>
            <FormattedMessage id="role.formTitle.assignPermissions" />
            {role && (
              <Tag color="geekblue" variant="filled">
                {role.name}
              </Tag>
            )}
          </Space>
        }
        width={560}
        open={open}
        onOpenChange={onOpenChange}
        onFinish={async () => {
          const { success, errorMessage } = await saveMutation.mutateAsync();

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
        <Space style={{ marginBottom: 16 }}>
          <Button
            onClick={() =>
              setExpandedKeys(
                expandedKeys.length
                  ? []
                  : permissionItems.map((item) => item.id),
              )
            }
          >
            {expandedKeys.length ? (
              <FormattedMessage id="role.toolbar.collapseAll" />
            ) : (
              <FormattedMessage id="role.toolbar.expandAll" />
            )}
          </Button>
          <Button
            onClick={() =>
              setCheckedKeys(
                checkedKeys.length === permissionItems.length
                  ? []
                  : permissionItems.map((item) => item.id),
              )
            }
          >
            {checkedKeys.length === permissionItems.length ? (
              <FormattedMessage id="role.toolbar.unselectAll" />
            ) : (
              <FormattedMessage id="role.toolbar.selectAll" />
            )}
          </Button>
          <span style={{ color: token.colorTextSecondary }}>
            <FormattedMessage
              id="role.toolbar.selectedCount"
              values={{
                selected: checkedKeys.length,
                total: permissionItems.length,
              }}
            />
          </span>
        </Space>

        <Tree
          rootStyle={{
            border: `1px solid ${token.colorBorder}`,
            borderRadius: token.borderRadius,
            padding: 12,
            overflow: 'auto',
          }}
          checkable
          checkStrictly
          showIcon
          showLine
          treeData={treeData}
          checkedKeys={checkedKeys}
          expandedKeys={expandedKeys}
          onCheck={handleCheck}
          onExpand={(keys) => setExpandedKeys([...keys])}
        />
      </DrawerForm>
    </>
  );
};

export default AssignPermissionsForm;
