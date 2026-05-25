import { DeleteOutlined, EditOutlined, PlusOutlined } from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { ProList } from '@ant-design/pro-components';
import { FormattedMessage, useAccess, useIntl } from '@umijs/max';
import { Button, message, Modal, Space, Tag, Typography } from 'antd';
import type { FC, Key, RefObject } from 'react';
import { useCallback, useMemo } from 'react';
import { deleteLookup, searchLookups } from '@/services/v1/lookup';
import { LOOKUP_PERMISSIONS } from '../data';
import LookupForm from './LookupForm';

const { Text } = Typography;

type LookupListProps = {
  actionRef?: RefObject<ActionType | null>;
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
  const [modalApi, modalContextHolder] = Modal.useModal();

  const handleDelete = useCallback(
    (lookup: API.LookupDto) => {
      modalApi.confirm({
        title: <FormattedMessage id="lookup.confirm.delete.title" />,
        content: <FormattedMessage id="lookup.confirm.delete.description" />,
        okButtonProps: {
          danger: true,
        },
        onOk: async () => {
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
      });
    },
    [intl, messageApi, modalApi, onSuccess],
  );

  const columns = useMemo<ProColumns<API.LookupDto>[]>(
    () => [
      {
        dataIndex: 'name',
        title: <FormattedMessage id="lookup.label.name" />,
        listSlot: 'title',
        render: (_: unknown, record: API.LookupDto) => (
          <Text strong ellipsis>
            {record.name}
          </Text>
        ),
      },
      {
        dataIndex: 'code',
        title: <FormattedMessage id="lookup.label.code" />,
        listSlot: 'subTitle',
        render: (_: unknown, record: API.LookupDto) => (
          <Tag color="secondary" variant="filled">{record.code}</Tag>
        ),
      },
      {
        dataIndex: 'description',
        title: <FormattedMessage id="lookup.label.description" />,
        search: false,
        listSlot: 'description',
        render: (_: unknown, record: API.LookupDto) =>
          record.description ? (
            <Text type="secondary" ellipsis>
              {record.description}
            </Text>
          ) : (
            <Text type="secondary">-</Text>
          ),
      },
      {
        dataIndex: 'actions',
        valueType: 'option',
        listSlot: 'actions',
        render: (_: unknown, record: API.LookupDto) => [
          access.has(LOOKUP_PERMISSIONS.update) && (
            <LookupForm
              key="update"
              trigger={
                <Button type="link" size="small" icon={<EditOutlined />}>
                  <FormattedMessage id="common.button.update" />
                </Button>
              }
              lookup={record}
              onSuccess={onSuccess}
            />
          ),
          access.has(LOOKUP_PERMISSIONS.delete) && (
            <Button
              key="delete"
              type="link"
              size="small"
              danger
              icon={<DeleteOutlined />}
              onClick={() => handleDelete(record)}
            >
              <FormattedMessage id="common.button.delete" />
            </Button>
          ),
        ].filter(Boolean),
      },
    ],
    [access, handleDelete, onSuccess],
  );

  return (
    <>
      {contextHolder}
      {modalContextHolder}
      <ProList<API.LookupDto, API.SearchLookupsParams>
        rowKey="id"
        actionRef={actionRef}
        split={true}
        variant="borderless"
        columns={columns}
        search={{ filterType: 'light' }}
        pagination={{ showSizeChanger: true }}
        onItem={(record) => ({
          onClick: () => onSelect(record),
          style: { cursor: 'pointer' },
        })}
        // rowClassName={(record: { id: Key | undefined; }) =>
        //   record.id === selectedRowKey ? 'ant-table-row-selected' : ''
        // }
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
                    <FormattedMessage id="lookup.action.create" />
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
