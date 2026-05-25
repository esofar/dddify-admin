import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  DeleteOutlined,
  EditOutlined,
  PlusOutlined,
} from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { DragSortTable } from '@ant-design/pro-components';
import { FormattedMessage, useAccess, useIntl } from '@umijs/max';
import { Button, Divider, message, Modal, Space, Tag, Tooltip } from 'antd';
import type { FC, MutableRefObject } from 'react';
import { useCallback, useMemo } from 'react';
import {
  deleteLookupItem,
  disableLookupItem,
  enableLookupItem,
  getLookupItems,
  sortLookupItems,
} from '@/services/v1/lookup';
import {
  LOOKUP_PERMISSIONS,
  lookupItemStatusValueEnum,
  yesOrNoValueEnum,
} from '../data';
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
  const [modalApi, modalContextHolder] = Modal.useModal();

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

  const handleDisable = useCallback(
    (item: API.LookupItemDto) => {
      modalApi.confirm({
        title: <FormattedMessage id="lookup.item.confirm.disable.title" />,
        content: <FormattedMessage id="lookup.item.confirm.disable.description" />,
        okButtonProps: {
          danger: true,
        },
        onOk: () =>
          runItemAction(
            item,
            disableLookupItem,
            'message.disable.success',
            'message.disable.failure',
          ),
      });
    },
    [modalApi, runItemAction],
  );

  const handleEnable = useCallback(
    (item: API.LookupItemDto) => {
      modalApi.confirm({
        title: <FormattedMessage id="lookup.item.confirm.enable.title" />,
        content: <FormattedMessage id="lookup.item.confirm.enable.description" />,
        onOk: () =>
          runItemAction(
            item,
            enableLookupItem,
            'message.enable.success',
            'message.enable.failure',
          ),
      });
    },
    [modalApi, runItemAction],
  );

  const handleDelete = useCallback(
    (item: API.LookupItemDto) => {
      modalApi.confirm({
        title: <FormattedMessage id="lookup.item.confirm.delete.title" />,
        content: <FormattedMessage id="lookup.item.confirm.delete.description" />,
        okButtonProps: {
          danger: true,
        },
        onOk: () =>
          runItemAction(
            item,
            deleteLookupItem,
            'message.delete.success',
            'message.delete.failure',
          ),
      });
    },
    [modalApi, runItemAction],
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
        copyable: false,
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
        width: 240,
        fixed: 'right',
        render: (_, record) => (
          <Space size={0} separator={<Divider orientation="vertical" />}>
            {access.has(LOOKUP_PERMISSIONS.updateItem) && (
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
              ? access.has(LOOKUP_PERMISSIONS.disableItem) && (
                <Button
                  type="link"
                  size="small"
                  danger
                  icon={<CloseCircleOutlined />}
                  onClick={() => handleDisable(record)}
                >
                  <FormattedMessage id="common.button.disable" />
                </Button>
                )
              : access.has(LOOKUP_PERMISSIONS.enableItem) && (
                <Button
                  type="link"
                  size="small"
                  icon={<CheckCircleOutlined />}
                  onClick={() => handleEnable(record)}
                >
                  <FormattedMessage id="common.button.enable" />
                </Button>
                )}

            {access.has(LOOKUP_PERMISSIONS.deleteItem) && (
              <Tooltip
                title={
                  record.isPreset ? (
                    <FormattedMessage id="lookup.item.label.isPreset.tooltip" />
                  ) : undefined
                }
              >
                <Button
                  type="link"
                  size="small"
                  danger
                  disabled={record.isPreset}
                  icon={<DeleteOutlined />}
                  onClick={() => handleDelete(record)}
                >
                  <FormattedMessage id="common.button.delete" />
                </Button>
              </Tooltip>
            )}
          </Space>
        ),
      },
    ],
    [access, handleDelete, handleDisable, handleEnable, lookupId, onSuccess],
  );

  return (
    <>
      {contextHolder}
      {modalContextHolder}
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
            {access.has(LOOKUP_PERMISSIONS.createItem) && (
              <LookupItemForm
                trigger={
                  <Button
                    type="primary"
                    icon={<PlusOutlined />}
                    disabled={!lookupId}
                  >
                    <FormattedMessage id="lookup.item.action.create" />
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
