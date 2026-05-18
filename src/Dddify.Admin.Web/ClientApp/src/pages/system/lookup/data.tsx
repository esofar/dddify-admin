import { FormattedMessage } from '@umijs/max';

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
    status: 'Default',
  },
} as const;

export const yesOrNoValueEnum = {
  true: {
    text: <FormattedMessage id="common.label.yes" defaultMessage="Yes" />,
    status: 'Success',
  },
  false: {
    text: <FormattedMessage id="common.label.no" defaultMessage="No" />,
    status: 'Default',
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
