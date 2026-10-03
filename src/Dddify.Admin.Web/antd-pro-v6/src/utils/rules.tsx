import type { FormRule } from 'antd';
import { FormattedMessage } from '@umijs/max';

export const requiredRule: FormRule = {
  required: true,
  message: <FormattedMessage id="form.rules.required" />,
};

export const lengthRule: (min: number, max: number) => FormRule = (min, max) => ({
  min,
  max,
  message: <FormattedMessage id="form.rules.length" values={{ min, max }} />,
});

export const minLengthRule: (min: number) => FormRule = (min) => ({
  min,
  message: <FormattedMessage id="form.rules.minLength" values={{ min }} />,
});

export const maxLengthRule: (max: number) => FormRule = (max) => ({
  max,
  message: <FormattedMessage id="form.rules.maxLength" values={{ max }} />,
});

export const noSpecialRule: FormRule = {
  pattern: /^[\p{L}\p{N} ]+$/u,
  message: <FormattedMessage id="form.rules.noSpecial" />,
};

export const noSpecialNoSpaceRule = {
  pattern: /^[\p{L}\p{N}]+$/u,
  message: <FormattedMessage id="form.rules.noSpecialNoSpace" />,
};

export const emailRule: FormRule = {
  type: 'email',
  message: <FormattedMessage id="form.rules.email" />,
};

export const phoneNumberRule: FormRule = {
  pattern: /^1[3-9]\d{9}$/,
  message: <FormattedMessage id="form.rules.phoneNumber" />,
};

export const passwordRule: FormRule = {
  pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/,
  message: <FormattedMessage id="form.rules.password" />,
};

export const upperCaseCodeRule: FormRule = {
  pattern: /^[A-Z0-9_]+$/,
  message: <FormattedMessage id="form.rules.upperCaseCode" />,
};

export const lowerCaseCodeRule: FormRule = {
  pattern: /^[a-z0-9_]+$/,
  message: <FormattedMessage id="form.rules.lowerCaseCode" />,
};

export const permissionCodeRule: FormRule = {
  pattern: /^[a-z0-9-:]+$/,
  message: <FormattedMessage id="form.rules.permissionCode" />,
};
