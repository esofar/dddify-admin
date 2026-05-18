import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  EditOutlined,
  PlusOutlined,
} from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { DragSortTable } from '@ant-design/pro-components';
import { FormattedMessage, useAccess, useIntl } from '@umijs/max';
import { Button, Divider, message, Popconfirm, Space, Tag } from 'antd';
import type { FC, MutableRefObject } from 'react';
import { useCallback, useMemo } from 'react';
import {
  disableLookupItem,
  enableLookupItem,
  getLookupItems,
  sortLookupItems,
} from '@/services/v1/lookup';
import { lookupItemStatusValueEnum, yesOrNoValueEnum } from '../data';
import LookupItemForm from './LookupItemForm';

type LookupItemListProps = {
  lookup?: API.LookupDto;
  actionRef?: MutableRefObject<ActionType | null>;
  onSuccess?: () => void;
};

const LookupItemList: FC<LookupItemListProps> = ({
  lookup,
  actionRef,
  onSuccess,
}) => {
  const intl = useIntl();
  const access = useAccess();
  const [messageApi, contextHolder] = message.useMessage();

  const lookupId = lookup?.id;

  const runItemAction = useCallback(
    async (
      item: API.LookupItemDto,
      action: (params: {
        id: string;
        itemId: string;
      }) => Promise<API.ApiResult>,
      successMessageId: string,
      failureMessageId: string,
    ) => {
      if (!lookupId) {
        return;
      }

      const { success, errorMessage } = await action({
        id: lookupId,
        itemId: item.id,
      });

      if (!success) {
        messageApi.error(
          errorMessage ?? intl.formatMessage({ id: failureMessageId }),
        );
        return;
      }

      messageApi.success(intl.formatMessage({ id: successMessageId }));
      onSuccess?.();
    },
    [intl, lookupId, messageApi, onSuccess],
  );

  const columns = useMemo<ProColumns<API.LookupItemDto>[]>(
    () => [
      {
        title: <FormattedMessage id="lookup.item.label.sort" />,
        dataIndex: 'sort',
        width: 60,
        className: 'drag-visible',
      },
      {
        title: <FormattedMessage id="lookup.item.label.label" />,
        dataIndex: 'label',
        minWidth: 120,
        render: (_, record) => (
          <Tag color={record.color || 'default'} variant="filled">
            {record.label}
          </Tag>
        ),
      },
      {
        title: <FormattedMessage id="lookup.item.label.value" />,
        dataIndex: 'value',
        minWidth: 120,
        copyable: true,
        ellipsis: true,
      },
      {
        title: <FormattedMessage id="lookup.item.label.isPreset" />,
        dataIndex: 'isPreset',
        minWidth: 100,
        valueEnum: yesOrNoValueEnum,
      },
      {
        title: <FormattedMessage id="lookup.item.label.status" />,
        dataIndex: 'isEnabled',
        minWidth: 100,
        valueEnum: lookupItemStatusValueEnum,
      },
      {
        title: <FormattedMessage id="lookup.item.label.option" />,
        dataIndex: 'option',
        valueType: 'option',
        width: 180,
        fixed: 'right',
        render: (_, record) => (
          <Space size={0} split={<Divider type="vertical" />}>
            {access.has('system:lookup:item:update') && (
              <LookupItemForm
                trigger={
                  <Button type="link" size="small" icon={<EditOutlined />}>
                    <FormattedMessage id="common.button.update" />
                  </Button>
                }
                lookupId={lookupId}
                item={record}
                onSuccess={onSuccess}
              />
            )}

            {record.isEnabled
              ? access.has('system:lookup:item:disable') && (
                  <Popconfirm
                    title={<FormattedMessage id="common.confirmText.disable" />}
                    disabled={record.isPreset}
                    onConfirm={() =>
                      runItemAction(
                        record,
                        disableLookupItem,
                        'message.disable.success',
                        'message.disable.failure',
                      )
                    }
                  >
                    <Button
                      type="link"
                      size="small"
                      disabled={record.isPreset}
                      icon={<CloseCircleOutlined />}
                    >
                      <FormattedMessage id="common.button.disable" />
                    </Button>
                  </Popconfirm>
                )
              : access.has('system:lookup:item:enable') && (
                  <Popconfirm
                    title={<FormattedMessage id="common.confirmText.enable" />}
                    disabled={record.isPreset}
                    onConfirm={() =>
                      runItemAction(
                        record,
                        enableLookupItem,
                        'message.enable.success',
                        'message.enable.failure',
                      )
                    }
                  >
                    <Button
                      type="link"
                      size="small"
                      disabled={record.isPreset}
                      icon={<CheckCircleOutlined />}
                    >
                      <FormattedMessage id="common.button.enable" />
                    </Button>
                  </Popconfirm>
                )}
          </Space>
        ),
      },
    ],
    [access, lookupId, onSuccess, runItemAction],
  );

  return (
    <>
      {contextHolder}
      <DragSortTable<API.LookupItemDto>
        rowKey="id"
        dragSortKey="sort"
        actionRef={actionRef}
        columns={columns}
        scroll={{ x: 'max-content' }}
        search={false}
        pagination={false}
        onDragSortEnd={async (_, __, list) => {
          if (!lookupId) {
            return;
          }

          const { success, errorMessage } = await sortLookupItems(
            { id: lookupId },
            list.map((item) => item.id),
          );

          if (!success) {
            messageApi.error(
              errorMessage ??
                intl.formatMessage({ id: 'message.order.failure' }),
            );
            return;
          }

          messageApi.success(
            intl.formatMessage({ id: 'message.order.success' }),
          );
          onSuccess?.();
        }}
        request={async () => {
          if (!lookupId) {
            return { data: [], success: true, total: 0 };
          }

          const { success, data } = await getLookupItems({ id: lookupId });

          return {
            data: data ?? [],
            success: success ?? false,
          };
        }}
        headerTitle={
          <Space>
            {access.has('system:lookup:item:create') && (
              <LookupItemForm
                trigger={
                  <Button
                    type="primary"
                    icon={<PlusOutlined />}
                    disabled={!lookupId}
                  >
                    <FormattedMessage id="lookup.button.createLookupItem" />
                  </Button>
                }
                lookupId={lookupId}
                onSuccess={onSuccess}
              />
            )}
          </Space>
        }
      />
    </>
  );
};

export default LookupItemList;
