import { FormattedMessage } from '@umijs/max';

export const ANNOUNCEMENT_PERMISSIONS = {
  create: 'system:announcement:create',
  update: 'system:announcement:update',
  delete: 'system:announcement:delete',
  publish: 'system:announcement:publish',
  withdraw: 'system:announcement:withdraw',
} as const;

export const ANNOUNCEMENT_STATUS = {
  Draft: 'Draft',
  Published: 'Published',
  Withdrawn: 'Withdrawn',
} as const;

export const ANNOUNCEMENT_AUDIENCE_TYPE = {
  AllUsers: 'AllUsers',
  Departments: 'Departments',
  Roles: 'Roles',
  SpecificUsers: 'SpecificUsers',
} as const;

export const announcementStatusValueEnum = {
  Draft: {
    text: <FormattedMessage id="announcement.status.draft" />,
    status: 'Default',
  },
  Published: {
    text: <FormattedMessage id="announcement.status.published" />,
    status: 'Success',
  },
  Withdrawn: {
    text: <FormattedMessage id="announcement.status.withdrawn" />,
    status: 'Warning',
  },
} as const;

export const announcementAudienceValueEnum = {
  AllUsers: {
    text: <FormattedMessage id="announcement.audience.allUsers" />,
  },
  Departments: {
    text: <FormattedMessage id="announcement.audience.departments" />,
  },
  Roles: {
    text: <FormattedMessage id="announcement.audience.roles" />,
  },
  SpecificUsers: {
    text: <FormattedMessage id="announcement.audience.specificUsers" />,
  },
} as const;

export function toRoleValueEnum(
  roles?: API.RoleListDto[] | null,
): Record<string, { text: string }> {
  return (roles ?? []).reduce<Record<string, { text: string }>>(
    (valueEnum, role) => {
      valueEnum[role.id] = {
        text: role.name,
      };

      return valueEnum;
    },
    {},
  );
}
