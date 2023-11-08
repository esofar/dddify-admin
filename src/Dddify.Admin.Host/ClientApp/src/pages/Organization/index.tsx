import React, { useEffect, useRef, useState } from 'react';
import { message, Button, Popconfirm, Divider, Space } from 'antd';
import { FormattedMessage, useIntl } from '@umijs/max';
import { PlusOutlined, QuestionCircleOutlined } from '@ant-design/icons';
import { getOrganizations, deleteOrganization, getOrganizationTypes } from '@/services/organization';
import type { ProColumns, ActionType } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import ProSkeleton from '@ant-design/pro-skeleton';
import EditDrawerForm from "./components/EditDrawerForm";
import { arrayToTree } from '@/utils';

const OrganizationList: React.FC = () => {
  const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
  const [currentRow, setCurrentRow] = useState<Organization>();
  const [currentOption, setCurrentOption] = useState<OptionType>('create');
  const [defaultExpandedRowKeys, setDefaultExpandedRowKeys] = useState<string[]>([]);
  const [typeOptions, setTypeOptions] = useState<{ value: string, label: string }[]>([]);

  const actionRef = useRef<ActionType>();
  const intl = useIntl();

  const statusEnum = {
    true: {
      text: <FormattedMessage id='organization.label.status.enabled' />,
      status: 'Success',
    },
    false: {
      text: <FormattedMessage id='organization.label.status.disabled' />,
      status: 'Error',
    },
  };

  const handleDelete = async (id?: string) => {
    const { success } = await deleteOrganization(id);
    if (success) {
      actionRef.current?.reload();
      message.success(intl.formatMessage({ id: `common.message.success.delete` }));
    }
  };

  const organizations = async (params?: any) => {
    const { success, data } = await getOrganizations(params);
    const treeOrgs = arrayToTree<Organization>(data as Organization[], { id: 'id', parentId: 'parentId' });
    return {
      data: treeOrgs,
      dataSource: data,
      success,
    };
  };

  const organizationTypes = async () => {
    const { success, data } = await getOrganizationTypes();
    return success && data ? data.map(item => {
      return { value: item.code, label: item.name, disabled: !item.isEnabled };
    }) : [];
  };

  useEffect(() => {
    organizations().then(({ dataSource }) => {
      const ids = dataSource?.map(org => org.id as string) || [];
      setDefaultExpandedRowKeys(ids);
    });
    organizationTypes().then((options) => {
      setTypeOptions(options);
    });
  }, []);

  const columns: ProColumns<Organization>[] = [
    {
      title: <FormattedMessage id='organization.label.name' />,
      dataIndex: 'name',
      width: '30%',
      ellipsis: true,
    },
    {
      title: <FormattedMessage id='organization.label.type' />,
      dataIndex: 'type',
      valueType: 'select',
      fieldProps: {
        options: typeOptions
      },
      width: '30%',
      ellipsis: true,
      renderText(text) {
        return text;
      },
    },
    {
      title: <FormattedMessage id='organization.label.status' />,
      dataIndex: 'isEnabled',
      valueType: 'select',
      valueEnum: statusEnum,
      width: '20%',
    },
    {
      title: <FormattedMessage id='organization.label.order' />,
      dataIndex: 'order',
      width: '20%',
      search: false,
    },
    {
      title: <FormattedMessage id='organization.label.option' />,
      dataIndex: 'option',
      valueType: 'option',
      align: 'center',
      fixed: 'right',
      width: 130,
      render: (_, row) => [
        <Space key={'option'} size={0} split={<Divider type="vertical" />}>
          <a
            key='update'
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
        </Space>
      ],
    },
  ];

  return (
    <PageContainer>
      {defaultExpandedRowKeys ? <ProTable<Organization>
        actionRef={actionRef}
        columns={columns}
        pagination={false}
        search={{
          labelWidth: 'auto'
        }}
        expandable={{
          defaultExpandedRowKeys,
        }}
        request={organizations}
        headerTitle={
          <Space>
            <Button
              type='primary'
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
      /> : <ProSkeleton />}
      <EditDrawerForm
        option={currentOption}
        values={currentRow}
        visible={editModalVisible}
        typeOptions={typeOptions}
        onVisibleChange={setEditModalVisible}
        onSuccess={() => {
          actionRef.current?.reload();
        }}
      />
    </PageContainer>
  );
};

export default OrganizationList; 