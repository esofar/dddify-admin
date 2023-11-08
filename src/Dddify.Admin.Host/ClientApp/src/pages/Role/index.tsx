import React, { useRef, useState } from 'react';
import { message, Button, Popconfirm, Divider, Space } from 'antd';
import { FormattedMessage, useIntl } from "@umijs/max";
import { PlusOutlined, QuestionCircleOutlined } from '@ant-design/icons';
import { getRoles, deleteRole } from '@/services/role';
import type { ProColumns, ActionType } from '@ant-design/pro-components';
import { PageContainer, ProTable, TableDropdown } from '@ant-design/pro-components';
import EditDrawerForm from "./components/EditDrawerForm";

const RoleList: React.FC = () => {
  const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
  const [currentRow, setCurrentRow] = useState<Role>();
  const [currentOption, setCurrentOption] = useState<OptionType>('create');

  const actionRef = useRef<ActionType>();
  const intl = useIntl();

  const handleDelete = async (id?: string) => {
    const { success } = await deleteRole(id);
    if (success) {
      actionRef.current?.reload();
      message.success(intl.formatMessage({ id: `common.message.success.delete` }));
    }
  };

  const columns: ProColumns<Role>[] = [
    {
      title: <FormattedMessage id='role.label.index' />,
      dataIndex: 'index',
      width: 54,
      valueType: 'index',
    },
    {
      title: <FormattedMessage id='role.label.code' />,
      dataIndex: 'code',
      width: '30%',
      ellipsis: true,
      copyable: true,
    },
    {
      title: <FormattedMessage id='role.label.name' />,
      dataIndex: 'name',
      width: '30%',
      ellipsis: true,
    },
    {
      title: <FormattedMessage id='role.label.description' />,
      dataIndex: 'description',
      width: '40%',
      ellipsis: true,
      search: false,
    },
    {
      title: <FormattedMessage id="role.label.option" />,
      dataIndex: 'option',
      valueType: 'option',
      align: 'center',
      fixed: 'right',
      width: 170,
      render: (_, row) => [
        <Space key={'option'} size={0} split={<Divider type="vertical" />}>
          <a
            key="update"
            onClick={() => {
              setCurrentRow(row);
              setCurrentOption('update');
              setEditModalVisible(true);
            }}
          >
            <FormattedMessage id='common.button.update' />
          </a>
          <Popconfirm
            key={'delete'}
            title={<FormattedMessage id='common.confirm.title.delete' />}
            onConfirm={async () => {
              await handleDelete(row.id);
            }}
            icon={<QuestionCircleOutlined />}>
            <a>
              <FormattedMessage id='common.button.delete' />
            </a>
          </Popconfirm>
          <TableDropdown
            key="more"
            onSelect={() => actionRef.current?.reload()}
            menus={[
              { key: 'setpers', name: '设置权限' },
            ]}
          />
        </Space>
      ],
    },
  ];

  return (
    <PageContainer>
      <ProTable<Role>
        actionRef={actionRef}
        columns={columns}
        search={{
          labelWidth: 'auto'
        }}
        request={async (params) => {
          const { success, data } = await getRoles(params);
          return {
            data: data.items,
            total: data.total,
            success,
          };
        }}
        headerTitle={
          <Space>
            <Button
              type='primary'
              key='primary'
              onClick={() => {
                setCurrentOption('create');
                setEditModalVisible(true);
              }}
            >
              <Space>
                <PlusOutlined />
                <FormattedMessage id='common.button.create' />
              </Space>
            </Button>
          </Space>
        }
      />
      <EditDrawerForm
        option={currentOption}
        values={currentRow}
        visible={editModalVisible}
        onVisibleChange={setEditModalVisible}
        onSuccess={() => {
          actionRef.current?.reload();
        }}
      />
    </PageContainer>
  );
};

export default RoleList;