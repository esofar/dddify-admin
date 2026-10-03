import { CheckOutlined, UserSwitchOutlined } from '@ant-design/icons';
import { FormattedMessage, useIntl, useModel } from '@umijs/max';
import type { MenuProps } from 'antd';
import { Button, message, Space, Tag, Tooltip } from 'antd';
import { createStyles } from 'antd-style';
import React, { useMemo, useState } from 'react';
import { switchRole } from '@/services/v1/me';
import HeaderDropdown from '../HeaderDropdown';

const useStyles = createStyles(({ token, css }) => ({
  action: css`
    display: inline-flex !important;
    align-items: center !important;
    justify-content: center !important;
    height: 36px !important;
    min-width: 36px;
    padding-inline: 8px !important;
    padding-block: 0 !important;
    border-radius: ${token.borderRadius}px !important;
  `,
  roleLabel: css`
    min-width: 140px;
  `,
}));

const SwitchRoleDropdown: React.FC = () => {
  const intl = useIntl();
  const { styles } = useStyles();
  const { initialState } = useModel('@@initialState');
  const [messageApi, contextHolder] = message.useMessage();
  const [switching, setSwitching] = useState(false);

  const roles = initialState?.currentUser?.roles ?? [];
  const currentRoleId =
    initialState?.currentUser?.currentRoleId ??
    roles.find((role) => role.isCurrent)?.roleId;

  const menuItems = useMemo<MenuProps['items']>(
    () =>
      roles.map((role) => {
        const selected = role.roleId === currentRoleId || role.isCurrent;

        return {
          key: role.roleId,
          disabled: selected || switching,
          icon: selected ? <CheckOutlined /> : <span style={{ width: 14 }} />,
          label: (
            <Space className={styles.roleLabel}>
              <span>{role.roleName}</span>
              {selected && (
                <Tag color="blue" bordered={false}>
                  <FormattedMessage id="component.switchRole.current" />
                </Tag>
              )}
            </Space>
          ),
        };
      }),
    [currentRoleId, roles, styles.roleLabel, switching],
  );

  const handleClick: MenuProps['onClick'] = async ({ key }) => {
    const roleId = String(key);

    if (!roleId || roleId === currentRoleId) {
      return;
    }

    setSwitching(true);

    try {
      const { success, errorMessage } = await switchRole({ roleId });

      if (!success) {
        messageApi.error(
          errorMessage ??
            intl.formatMessage({ id: 'component.switchRole.failure' }),
        );
        return;
      }
      messageApi.success(
        intl.formatMessage({ id: 'component.switchRole.success' }),
      );
      window.setTimeout(() => {
        window.location.reload();
      }, 300);
    } finally {
      setSwitching(false);
    }
  };

  if (roles.length <= 1) {
    return null;
  }

  return (
    <>
      {contextHolder}
      <HeaderDropdown
        placement="bottomRight"
        arrow
        menu={{
          items: menuItems,
          onClick: handleClick,
          selectedKeys: currentRoleId ? [currentRoleId] : [],
          style: { minWidth: 200 },
        }}
      >
        <Tooltip>
          <Button
            type="text"
            className={styles.action}
            loading={switching}
            icon={<UserSwitchOutlined />}
            aria-label={intl.formatMessage({
              id: 'component.switchRole.tooltip',
            })}
          />
        </Tooltip>
      </HeaderDropdown>
    </>
  );
};

export default SwitchRoleDropdown;
