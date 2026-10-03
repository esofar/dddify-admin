import { FormattedMessage } from '@umijs/max';

export const LOOKUP_PERMISSIONS = {
  create: 'system:lookup:create',
  update: 'system:lookup:update',
  delete: 'system:lookup:delete',
  createItem: 'system:lookup:item:create',
  updateItem: 'system:lookup:item:update',
  enableItem: 'system:lookup:item:enable',
  disableItem: 'system:lookup:item:disable',
  deleteItem: 'system:lookup:item:delete',
} as const;

export const lookupItemStatusValueEnum = {
  true: {
    text: (
      <FormattedMessage id="common.button.enable" defaultMessage="Enable" />
    ),
    status: 'Success',
  },
  false: {
    text: (
      <FormattedMessage id="common.button.disable" defaultMessage="Disable" />
    ),
    status: 'Error',
  },
} as const;

export const yesOrNoValueEnum = {
  true: {
    text: <FormattedMessage id="common.label.yes" defaultMessage="Yes" />,
    status: 'Success',
  },
  false: {
    text: <FormattedMessage id="common.label.no" defaultMessage="No" />,
    status: 'Error',
  },
} as const;

export const tagColors = [
  'processing',
  'success',
  'error',
  'warning',
  'magenta',
  'red',
  'volcano',
  'orange',
  'gold',
  'lime',
  'green',
  'cyan',
  'blue',
  'geekblue',
  'purple',
];
