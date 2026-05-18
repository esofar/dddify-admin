import {
  AppstoreOutlined,
  BarsOutlined,
  ControlOutlined,
} from '@ant-design/icons';
import { FormattedMessage } from '@umijs/max';
import type { ReactNode } from 'react';

export const PERMISSION_TYPE = {
  Catalog: 'Catalog',
  Menu: 'Menu',
  Button: 'Button',
} as const;

type PermissionTypeOption = {
  text: ReactNode;
  icon: ReactNode;
  color: string;
};

export type PermissionTreeNode = API.PermissionDto & {
  children?: PermissionTreeNode[];
};

export type PermissionSelectNode = {
  id: string;
  parentId?: string | null;
  name: string;
};

export const permissionTypeValueEnum: Record<string, PermissionTypeOption> = {
  [PERMISSION_TYPE.Catalog]: {
    text: <FormattedMessage id="permission.type.catalog" />,
    icon: <AppstoreOutlined />,
    color: 'purple',
  },
  [PERMISSION_TYPE.Menu]: {
    text: <FormattedMessage id="permission.type.menu" />,
    icon: <BarsOutlined />,
    color: 'blue',
  },
  [PERMISSION_TYPE.Button]: {
    text: <FormattedMessage id="permission.type.button" />,
    icon: <ControlOutlined />,
    color: 'green',
  },
};

export function buildPermissionTree(
  permissions: API.PermissionDto[],
): PermissionTreeNode[] {
  const map = new Map<string, PermissionTreeNode>();
  const roots: PermissionTreeNode[] = [];

  permissions.forEach((permission) => {
    map.set(permission.id, { ...permission });
  });

  permissions.forEach((permission) => {
    const node = map.get(permission.id);

    if (!node) {
      return;
    }

    if (permission.parentId && map.has(permission.parentId)) {
      const parent = map.get(permission.parentId);

      if (parent) {
        parent.children ??= [];
        parent.children.push(node);
      }

      return;
    }

    roots.push(node);
  });

  return roots;
}

export function toPermissionSelectNodes(
  permissions: API.PermissionDto[],
): PermissionSelectNode[] {
  return permissions.map((permission) => ({
    id: permission.id,
    parentId: permission.parentId,
    name: permission.name,
  }));
}
