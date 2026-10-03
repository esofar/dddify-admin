import {
  BellOutlined,
  MailOutlined,
} from '@ant-design/icons';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import {
  Badge,
  Button,
  Empty,
  message,
  Modal,
  Popover,
  Segmented,
  Spin,
  Tag,
  Tooltip,
  Typography,
} from 'antd';
import { createStyles } from 'antd-style';
import dayjs from 'dayjs';
import { useMemo, useState } from 'react';
import {
  getInboxItemSource,
  getUnreadInboxItemCount,
  markInboxItemsAsRead,
  searchMeInboxItems,
} from '@/services/v1/me';
import FroFormTiptapPreview from '../business/FroFormTiptapPreview';

const { Text } = Typography;

const INBOX_PAGE_SIZE = 8;

const sourceTypeMap = {
  Announcement: {
    labelId: 'component.inbox.source.announcement',
    emptyId: 'component.inbox.empty.announcement',
    color: 'purple',
  },
  System: {
    labelId: 'component.inbox.source.system',
    emptyId: 'component.inbox.empty.system',
    color: 'blue',
  },
  Todo: {
    labelId: 'component.inbox.source.todo',
    emptyId: 'component.inbox.empty.todo',
    color: 'orange',
  },
  Approval: {
    labelId: 'component.inbox.source.approval',
    emptyId: 'component.inbox.empty.approval',
    color: 'green',
  },
  Security: {
    labelId: 'component.inbox.source.security',
    emptyId: 'component.inbox.empty.security',
    color: 'red',
  },
} as const;

const sourceTypeAlias: Record<string, keyof typeof sourceTypeMap> = {
  '1': 'Announcement',
  Announcement: 'Announcement',
  '2': 'System',
  System: 'System',
  '3': 'Todo',
  Todo: 'Todo',
  '4': 'Approval',
  Approval: 'Approval',
  '5': 'Security',
  Security: 'Security',
};

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
  bellIcon: {
    fontSize: 16,
    color: token.colorText,
  },
  panel: {
    width: 420,
  },
  header: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '14px 16px',
    borderBottom: `1px solid ${token.colorBorderSecondary}`,
  },
  title: {
    margin: 0,
    fontSize: 16,
    fontWeight: 600,
    color: token.colorText,
  },
  filters: {
    padding: '8px 12px',
    borderBottom: `1px solid ${token.colorBorderSecondary}`,
  },
  segmented: {
    width: '100%',
    background: token.colorFillQuaternary,
    '& .ant-segmented-item': {
      borderRadius: token.borderRadiusSM,
    },
    '& .ant-segmented-item-selected': {
      boxShadow: 'none',
    },
  },
  list: {
    height: 420,
    overflowY: 'auto',
  },
  item: {
    padding: '12px 14px',
    borderBottom: `1px solid ${token.colorBorderSecondary}`,
    cursor: 'pointer',
    transition: `background ${token.motionDurationMid}`,
    '&:hover': {
      background: token.colorFillTertiary,
    },
  },
  unreadItem: {
    background: token.colorFillQuaternary,
  },
  titleRow: {
    display: 'flex',
    alignItems: 'center',
    gap: 6,
    minWidth: 0,
  },
  titleBadge: {
    flex: '0 0 auto',
    width: 6,
    height: 6,
    borderRadius: '50%',
    background: token.colorError,
  },
  itemTitle: {
    display: 'block',
    minWidth: 0,
    fontSize: 14,
    fontWeight: 600,
    color: token.colorText,
  },
  readTitle: {
    fontWeight: 400,
    color: token.colorTextSecondary,
  },
  summary: {
    display: 'block',
    marginTop: 2,
  },
  meta: {
    display: 'flex',
    alignItems: 'center',
    gap: 8,
    marginTop: 6,
  },
  source: {
    flex: '0 0 auto',
    marginInlineEnd: 0,
    paddingInline: 6,
    lineHeight: '18px',
  },
  footer: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '12px 16px',
    borderTop: `1px solid ${token.colorBorderSecondary}`,
  },
  empty: {
    padding: '150px 0 0',
  },
  modalSummary: {
    marginBottom: 16,
  },
  modalMeta: {
    display: 'flex',
    alignItems: 'center',
    gap: 8,
    marginBottom: 16,
  },
}));

const getSourceType = (item: API.InboxItemListDto) =>
  sourceTypeAlias[String(item.source?.type ?? '')] ?? 'System';

const getRelativeTime = (value?: string | null) =>
  value ? dayjs(value).fromNow() : '-';

const InboxItemsDropdown = () => {
  const { styles, cx } = useStyles();
  const intl = useIntl();
  const queryClient = useQueryClient();
  const [messageApi, contextHolder] = message.useMessage();
  const [open, setOpen] = useState(false);
  const [sourceType, setSourceType] = useState<'All' | keyof typeof sourceTypeMap>(
    'All',
  );
  const [markingId, setMarkingId] = useState<string>();
  const [openingId, setOpeningId] = useState<string>();
  const [selectedItem, setSelectedItem] = useState<API.InboxItemListDto>();
  const [selectedAnnouncement, setSelectedAnnouncement] =
    useState<API.AnnouncementDetailDto>();

  const unreadQuery = useQuery({
    queryKey: ['me', 'inbox-items', 'unread-count'],
    queryFn: async () => {
      const { success, data } = await getUnreadInboxItemCount();
      return success ? (data?.count ?? 0) : 0;
    },
    refetchInterval: 60_000,
  });

  const inboxQuery = useQuery({
    queryKey: ['me', 'inbox-items', 'popover'],
    enabled: open,
    queryFn: async () => {
      const { success, data } = await searchMeInboxItems({
        Current: 1,
        PageSize: INBOX_PAGE_SIZE,
      });

      return success ? (data?.items ?? []) : [];
    },
  });

  const markReadMutation = useMutation({
    mutationFn: async (ids: string[]) => markInboxItemsAsRead({ ids }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['me', 'inbox-items'] });
    },
    onSettled: () => {
      setMarkingId(undefined);
    },
  });

  const items = inboxQuery.data ?? [];
  const filteredItems = useMemo(
    () =>
      sourceType === 'All'
        ? items
        : items.filter((item) => getSourceType(item) === sourceType),
    [items, sourceType],
  );

  const unreadCount = unreadQuery.data ?? 0;
  const emptyMessageId =
    sourceType === 'All'
      ? 'component.inbox.empty.all'
      : sourceTypeMap[sourceType].emptyId;
  const selectedSourceType = selectedItem ? getSourceType(selectedItem) : undefined;
  const selectedSourceLabel = selectedSourceType
    ? intl.formatMessage({ id: sourceTypeMap[selectedSourceType].labelId })
    : undefined;

  const filterOptions = useMemo(
    () => [
      {
        label: intl.formatMessage({ id: 'component.inbox.source.all' }),
        value: 'All',
      },
      ...Object.entries(sourceTypeMap).map(([key, config]) => ({
        label: intl.formatMessage({ id: config.labelId }),
        value: key,
      })),
    ],
    [intl],
  );

  const markAsRead = (item: API.InboxItemListDto) => {
    if (item.isRead || markReadMutation.isPending) {
      return;
    }

    setMarkingId(item.id);
    markReadMutation.mutate([item.id]);
  };

  const handleOpenChange = (nextOpen: boolean) => {
    setOpen(nextOpen);

    if (nextOpen) {
      unreadQuery.refetch();
      queryClient.invalidateQueries({ queryKey: ['me', 'inbox-items', 'popover'] });
    }
  };

  const openGenericMessage = (item: API.InboxItemListDto) => {
    setOpen(false);
    setSelectedAnnouncement(undefined);
    setSelectedItem(item);
  };

  const openAnnouncementMessage = async (item: API.InboxItemListDto) => {
    const sourceResult = await getInboxItemSource({ id: item.id });
    const sourceDetail = sourceResult.data;

    if (!sourceResult.success || !sourceDetail) {
      openGenericMessage(item);
      return;
    }

    if (sourceDetail.isAvailable && sourceDetail.announcement) {
      setOpen(false);
      setSelectedAnnouncement(sourceDetail.announcement);
      setSelectedItem(item);
      return;
    }

    messageApi.warning(
      intl.formatMessage({ id: 'component.inbox.sourceUnavailable' }),
    );
  };

  const handleItemClick = async (item: API.InboxItemListDto) => {
    if (openingId) {
      return;
    }

    markAsRead(item);

    setOpeningId(item.id);
    try {
      if (getSourceType(item) === 'Announcement') {
        await openAnnouncementMessage(item);
      } else {
        openGenericMessage(item);
      }
    } finally {
      setOpeningId(undefined);
    }
  };

  const markCurrentPageAsRead = () => {
    const unreadIds = items.filter((item) => !item.isRead).map((item) => item.id);

    if (unreadIds.length === 0) {
      return;
    }

    setMarkingId('current-page');
    markReadMutation.mutate(unreadIds);
  };

  const renderItem = (item: API.InboxItemListDto) => {
    const currentSourceType = getSourceType(item);
    const sourceConfig = sourceTypeMap[currentSourceType];
    const sourceLabel = intl.formatMessage({ id: sourceConfig.labelId });

    return (
      <div
        key={item.id}
        className={cx(styles.item, !item.isRead && styles.unreadItem)}
        onClick={() => handleItemClick(item)}
      >
        <div>
          <div className={styles.titleRow}>
            {!item.isRead && <span className={styles.titleBadge} />}
            <Text
              ellipsis
              className={cx(styles.itemTitle, item.isRead && styles.readTitle)}
            >
              {item.title}
            </Text>
          </div>
          <Text type="secondary" ellipsis className={styles.summary}>
            {item.summary}
          </Text>
          <div className={styles.meta}>
              
            <Tag variant="filled" className={styles.source} color={sourceConfig.color}>
              {sourceLabel}
            </Tag>
            <Text type="secondary">{getRelativeTime(item.createdAt)}</Text>
          </div>
        </div>
      </div>
    );
  };

  const content = (
    <div className={styles.panel}>
      <div className={styles.header}>
        <h3 className={styles.title}>
          <FormattedMessage id="component.globalHeader.notification" />
        </h3>
        <MailOutlined />
      </div>

      <div className={styles.filters}>
        <Segmented
          block
          size="small"
          className={styles.segmented}
          value={sourceType}
          options={filterOptions}
          onChange={(value) =>
            setSourceType(value as 'All' | keyof typeof sourceTypeMap)
          }
        />
      </div>

      <Spin spinning={inboxQuery.isFetching}>
        <div className={styles.list}>
          {filteredItems.length > 0 ? (
            filteredItems.map(renderItem)
          ) : (
            <Empty
              className={styles.empty}
              image={Empty.PRESENTED_IMAGE_SIMPLE}
              description={
                <FormattedMessage id={emptyMessageId} />
              }
            />
          )}
        </div>
      </Spin>

      <div className={styles.footer}>
        <Button
          type="link"
          size="small"
          disabled={!items.some((item) => !item.isRead)}
          loading={markingId === 'current-page'}
          onClick={markCurrentPageAsRead}
        >
          <FormattedMessage id="component.inbox.markCurrentPageRead" />
        </Button>
        <Button
          type="primary"
          size="small"
          loading={inboxQuery.isFetching}
          onClick={() => inboxQuery.refetch()}
        >
          <FormattedMessage id="component.inbox.refresh" />
        </Button>
      </div>
    </div>
  );

  return (
    <>
      {contextHolder}
      <Popover
        arrow={false}
        trigger="click"
        placement="bottomRight"
        open={open}
        content={content}
        styles={{
          container: {
            padding: 0
          }
        }}
        onOpenChange={handleOpenChange}
      >
        <Tooltip title={<FormattedMessage id="component.globalHeader.notification" />}>
          <Button
            type="text"
            className={styles.action}
            aria-label={intl.formatMessage({
              id: 'component.globalHeader.notification',
            })}
          >
            <Badge count={unreadCount} overflowCount={99} size="small" offset={[4, -4]}>
              <BellOutlined className={styles.bellIcon} />
            </Badge>
          </Button>
        </Tooltip>
      </Popover>

      <Modal
        title={selectedAnnouncement?.title ?? selectedItem?.title}
        open={Boolean(selectedItem)}
        width={760}
        footer={null}
        destroyOnHidden
        onCancel={() => {
          setSelectedItem(undefined);
          setSelectedAnnouncement(undefined);
        }}
      >
        {selectedItem && (
          <>
            <div className={styles.modalMeta}>
              {selectedSourceLabel && (
                <Tag
                  bordered={false}
                  color={
                    selectedSourceType
                      ? sourceTypeMap[selectedSourceType].color
                      : undefined
                  }
                  className={styles.source}
                >
                  {selectedSourceLabel}
                </Tag>
              )}
              <Text type="secondary">{getRelativeTime(selectedItem.createdAt)}</Text>
            </div>
            {selectedAnnouncement ? (
              <FroFormTiptapPreview
                value={selectedAnnouncement.content?.html}
                minHeight={240}
                maxHeight={520}
              />
            ) : (
              <Typography.Paragraph
                type="secondary"
                className={styles.modalSummary}
              >
                {selectedItem.summary}
              </Typography.Paragraph>
            )}
          </>
        )}
      </Modal>
    </>
  );
};

export default InboxItemsDropdown;
