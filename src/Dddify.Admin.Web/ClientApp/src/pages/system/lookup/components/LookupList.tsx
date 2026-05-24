import { DeleteOutlined, EditOutlined, PlusOutlined } from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { FormattedMessage, useAccess, useIntl } from '@umijs/max';
import { Button, Divider, message, Popconfirm, Space } from 'antd';
import type { FC, MutableRefObject } from 'react';
import { useCallback, useMemo } from 'react';
import { deleteLookup, searchLookups } from '@/services/v1/lookup';
import { LOOKUP_PERMISSIONS } from '../data';
import LookupForm from './LookupForm';

type LookupListProps = {
  actionRef?: MutableRefObject<ActionType | null>;
  selectedRowKey?: React.Key;
  onLoaded?: (lookups: API.LookupDto[]) => void;
  onSelect: (lookup: API.LookupDto) => void;
  onSuccess?: () => void;
};

const LookupList: FC<LookupListProps> = ({
  actionRef,
  selectedRowKey,
  onLoaded,
  onSelect,
  onSuccess,
}) => {
  const intl = useIntl();
  const access = useAccess();
  const [messageApi, contextHolder] = message.useMessage();

  const handleDelete = useCallback(
    async (lookup: API.LookupDto) => {
      const { success, errorMessage } = await deleteLookup({ id: lookup.id });

      if (!success) {
        messageApi.error(
          errorMessage ?? intl.formatMessage({ id: 'message.delete.failure' }),
        );
        return;
      }

      messageApi.success(intl.formatMessage({ id: 'message.delete.success' }));
      onSuccess?.();
    },
    [intl, messageApi, onSuccess],
  );

  const columns = useMemo<ProColumns<API.LookupDto>[]>(
    () => [
      {
        title: <FormattedMessage id="lookup.label.index" />,
        dataIndex: 'index',
        valueType: 'index',
        width: 64,
        align: 'center',
        fixed: 'left',
        search: false,
      },
      {
        title: <FormattedMessage id="lookup.label.name" />,
        dataIndex: 'name',
        minWidth: 120,
        ellipsis: true,
      },
      {
        title: <FormattedMessage id="lookup.label.code" />,
        dataIndex: 'code',
        minWidth: 140,
        ellipsis: true,
        copyable: true,
      },
      {
        title: <FormattedMessage id="lookup.label.description" />,
        dataIndex: 'description',
        minWidth: 160,
        ellipsis: true,
        search: false,
      },
      {
        title: <FormattedMessage id="lookup.label.option" />,
        dataIndex: 'option',
        valueType: 'option',
        fixed: 'right',
        width: 170,
        render: (_, record) => (
          <Space size={0} split={<Divider type="vertical" />}>
            {access.has(LOOKUP_PERMISSIONS.update) && (
              <LookupForm
                trigger={
                  <Button type="link" size="small" icon={<EditOutlined />}>
                    <FormattedMessage id="common.button.update" />
                  </Button>
                }
                lookup={record}
                onSuccess={onSuccess}
              />
            )}

            {access.has(LOOKUP_PERMISSIONS.delete) && (
              <Popconfirm
                title={<FormattedMessage id="common.confirmText.delete" />}
                okButtonProps={{ danger: true }}
                onConfirm={() => handleDelete(record)}
              >
                <Button
                  type="link"
                  size="small"
                  danger
                  icon={<DeleteOutlined />}
                >
                  <FormattedMessage id="common.button.delete" />
                </Button>
              </Popconfirm>
            )}
          </Space>
        ),
      },
    ],
    [access, handleDelete, onSuccess],
  );

  return (
    <>
      {contextHolder}
      <ProTable<API.LookupDto, API.SearchLookupsParams>
        rowKey="id"
        actionRef={actionRef}
        columns={columns}
        scroll={{ x: 'max-content' }}
        search={{ filterType: 'light' }}
        pagination={{ showSizeChanger: true }}
        onRow={(record) => ({
          onClick: () => onSelect(record),
        })}
        rowClassName={(record) =>
          record.id === selectedRowKey ? 'ant-table-row-selected' : ''
        }
        request={async (params) => {
          const { success, data } = await searchLookups(params);
          const items = data?.items ?? [];

          onLoaded?.(items);

          return {
            data: items,
            total: data?.total ?? 0,
            success: success ?? false,
          };
        }}
        headerTitle={
          <Space>
            {access.has(LOOKUP_PERMISSIONS.create) && (
              <LookupForm
                trigger={
                  <Button type="primary" icon={<PlusOutlined />}>
                    <FormattedMessage id="lookup.button.createLookup" />
                  </Button>
                }
                onSuccess={onSuccess}
              />
            )}
          </Space>
        }
        options={{ density: false }}
      />
    </>
  );
};

export default LookupList;
