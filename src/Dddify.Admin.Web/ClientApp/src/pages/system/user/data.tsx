import { ManOutlined, WomanOutlined } from '@ant-design/icons';
import { FormattedMessage } from '@umijs/max';
import type { ReactNode } from 'react';

export const USER_GENDER = {
  Male: 'Male',
  Female: 'Female',
} as const;

export const USER_STATUS = {
  Enabled: 'Enabled',
  Disabled: 'Disabled',
} as const;

export const USER_PERMISSIONS = {
  create: 'system:user:create',
  update: 'system:user:update',
  delete: 'system:user:delete',
  disable: 'system:user:disable',
  enable: 'system:user:enable',
  assignRoles: 'system:user:assign-roles',
  resetPassword: 'system:user:reset-password',
} as const;

export type UserGenderValue = (typeof USER_GENDER)[keyof typeof USER_GENDER];

export type UserStatusValue = (typeof USER_STATUS)[keyof typeof USER_STATUS];

type UserGenderOption = {
  text: ReactNode;
  icon: ReactNode;
  color: string;
};

type UserStatusOption = {
  text: ReactNode;
  status: 'Error' | 'Success';
};

export type DepartmentTreeNode = {
  id: string;
  pId?: string | null;
  name: string;
  disabled?: boolean;
};

export const userGenderValueEnum: Record<string, UserGenderOption> = {
  [USER_GENDER.Male]: {
    text: <FormattedMessage id="user.gender.male" />,
    icon: <ManOutlined />,
    color: 'blue',
  },
  [USER_GENDER.Female]: {
    text: <FormattedMessage id="user.gender.female" />,
    icon: <WomanOutlined />,
    color: 'magenta',
  },
};

export const userStatusValueEnum: Record<string, UserStatusOption> = {
  [USER_STATUS.Enabled]: {
    text: (
      <FormattedMessage id="user.status.enabled" defaultMessage="Enabled" />
    ),
    status: 'Success',
  },
  [USER_STATUS.Disabled]: {
    text: (
      <FormattedMessage id="user.status.disabled" defaultMessage="Disabled" />
    ),
    status: 'Error',
  },
};

export function toDepartmentTreeNodes(
  departments: API.DepartmentListDto[],
): DepartmentTreeNode[] {
  return departments.map((department) => ({
    id: department.id,
    pId: department.parentId,
    name: department.name,
    disabled: !department.isEnabled,
  }));
}

export function toRoleValueEnum(roles: API.RoleListDto[]) {
  return roles.reduce<Record<string, { text: string }>>((valueEnum, role) => {
    valueEnum[role.id] = { text: role.name };
    return valueEnum;
  }, {});
}
