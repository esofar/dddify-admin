import React, { useEffect, useRef, useState } from 'react';
import { message, Button, Popconfirm, Avatar, Divider, Space, Tag, Typography } from 'antd';
import { FormattedMessage, useIntl } from "@umijs/max";
import { ManOutlined, PlusOutlined, QuestionCircleOutlined, UserOutlined, WomanOutlined } from '@ant-design/icons';
import { getUsers, deleteUser, getUserTypes } from '@/services/user';
import { getOrganizations } from '@/services/organization';
import type { ProColumns, ActionType } from '@ant-design/pro-components';
import { PageContainer, ProTable, TableDropdown } from '@ant-design/pro-components';
import EditDrawerForm from "./components/EditDrawerForm";
import { ClockCircleOutlined, CheckCircleOutlined, CloseCircleOutlined, ExclamationCircleOutlined } from '@ant-design/icons';

const { Text } = Typography;

const UserList: React.FC = () => {
  const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
  const [currentRow, setCurrentRow] = useState<User>();
  const [currentOption, setCurrentOption] = useState<OptionType>('create');
  const [typeOptions, setTypeOptions] = useState<{ value: string, label: string }[]>([]);

  const actionRef = useRef<ActionType>();
  const intl = useIntl();

  const genderEnum = {
    1: {
      text: <FormattedMessage id='user.gender.enum.male' />,
      icon: <ManOutlined style={{ color: '#5AA7F9' }} />,
    },
    0: {
      text: <FormattedMessage id='user.gender.enum.female' />,
      icon: <WomanOutlined style={{ color: '#FF9DC6' }} />
    },
  };

  const statusEnum = {
    0: {
      text: (
        <Tag icon={<ClockCircleOutlined />} color="#E6F4FF" style={{ color: '#1677FF' }}>
          <FormattedMessage id='user.status.enum.notActivated' />
        </Tag>),
    },
    1: {
      text: (
        <Tag icon={<CheckCircleOutlined />} color="#F6FFED" style={{ color: '#52C41A' }}>
          <FormattedMessage id='user.status.enum.normal' />
        </Tag>)
    },
    2: {
      text: (
        <Tag icon={<ExclamationCircleOutlined />} color="#FFFBE6" style={{ color: '#FAB324' }}>
          <FormattedMessage id='user.status.enum.suspended' />
        </Tag>)
    },
    3: {
      text: (
        <Tag icon={<CloseCircleOutlined />} color="#FFF2F0" style={{ color: '#FF4D4F' }}>
          <FormattedMessage id='user.status.enum.resigned' />
        </Tag>)
    },
  };

  const organizations = async (params: { keyWords?: string }) => {
    const { success, data } = await getOrganizations({ name: params.keyWords });
    return success ? (data as Organization[]).map(org => {
      return { id: org.id, pId: org.parentId, name: org.name, disabled: false };
    }) : [];
  };

  const userTypes = async () => {
    const { success, data } = await getUserTypes();
    return success && data ? data.map(item => {
      return { value: item.code, label: item.name, disabled: !item.isEnabled };
    }) : [];
  };

  const handleDelete = async (id?: string) => {
    const { success } = await deleteUser(id);
    if (success) {
      actionRef.current?.reload();
      message.success(intl.formatMessage({ id: `common.message.success.delete` }));
    }
  };

  useEffect(() => {
    userTypes().then((options) => {
      setTypeOptions(options);
    });
  }, []);

  const columns: ProColumns<User>[] = [
    {
      title: <FormattedMessage id='user.label.avatar' />,
      dataIndex: 'avatar',
      width: 65,
      search: false,
      render: (_, row) => [
        (row.avatar
          ? <Avatar src={row.avatar} size={25} />
          : <Avatar icon={<UserOutlined />} size={25} />)
      ],
    },
    {
      title: <FormattedMessage id='user.label.fullName' />,
      dataIndex: 'fullName',
      width: '15%',
      ellipsis: true,
      render: (_, row) => [
        (<Space key={row.id} size={5}>
          <Text>{row.fullName}</Text>
          <Text>{genderEnum[row.gender].icon}</Text>
        </Space>)
      ],
    },
    {
      title: <FormattedMessage id='user.label.status' />,
      dataIndex: 'status',
      valueType: 'select',
      valueEnum: statusEnum,
      width: '15%',
    },
    {
      title: <FormattedMessage id='user.label.type' />,
      dataIndex: 'type',
      valueType: 'select',
      fieldProps: {
        options: typeOptions
      },
      width: '15%',
      ellipsis: true,
      renderText(text) {
        return text;
      },
    },
    {
      title: <FormattedMessage id='user.label.organization' />,
      dataIndex: 'organizationName',
      width: '20%',
      valueType: 'treeSelect',
      request: organizations,
      search: {
        transform: (value) => {
          return {
            organizationId: value,
          };
        },
      },
      fieldProps: {
        placeholder: intl.formatMessage({ id: `common.form.treeselect.placeholder` }),
        allowClear: true,
        treeDataSimpleMode: true,
        treeDefaultExpandAll: true,
        showSearch: true,
        treeLine: {
          showLeafIcon: false
        },
        fieldNames: {
          label: 'name',
          value: 'id',
        },
      },
    },
    // {
    //   title: <FormattedMessage id='user.label.birthDate' />,
    //   dataIndex: 'birthDate',
    //   width: '15%',
    //   valueType: 'date',
    //   search: false,
    // },
    {
      title: <FormattedMessage id='user.label.email' />,
      dataIndex: 'email',
      width: '20%',
      ellipsis: true,
      search: false,
      copyable: true,
    },
    {
      title: <FormattedMessage id='user.label.phoneNumber' />,
      dataIndex: 'phoneNumber',
      width: '15%',
      ellipsis: true,
      search: false,
      copyable: true,
    },
    {
      title: <FormattedMessage id="user.label.option" />,
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
              { key: 'assigningRoles', name: '分配角色' },
              { key: 'disable', name: '暂停账号' },
              { key: 'resign', name: '操作离职' },
              { key: 'resetPassword', name: '重置密码' },
            ]}
          />
        </Space>
      ],
    },
  ];

  return (
    <PageContainer>
      <ProTable<User>
        actionRef={actionRef}
        columns={columns}
        search={{
          labelWidth: 'auto'
        }}
        rowSelection={{}}
        request={async (params) => {
          const { success, data } = await getUsers(params);
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
        genderEnum={genderEnum}
        statusEnum={statusEnum}
        onVisibleChange={setEditModalVisible}
        onSuccess={() => {
          actionRef.current?.reload();
        }}
      />
    </PageContainer>
  );
};

export default UserList;