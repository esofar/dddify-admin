import {
  DeleteOutlined,
  EditOutlined,
  PlusOutlined,
  RollbackOutlined,
  SendOutlined,
} from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, history, useAccess, useIntl, useLocation } from '@umijs/max';
import { Button, Divider, message, Modal, Space, Tooltip, Typography } from 'antd';
import dayjs from 'dayjs';
import type { FC } from 'react';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
  deleteAnnouncement,
  publishAnnouncement,
  searchAnnouncements,
  withdrawAnnouncement,
} from '@/services/v1/announcement';
import AnnouncementDetail from './components/AnnouncementDetail';
import AnnouncementForm from './components/AnnouncementForm';
import {
  ANNOUNCEMENT_PERMISSIONS,
  ANNOUNCEMENT_STATUS,
  announcementAudienceValueEnum,
  announcementStatusValueEnum,
} from './data';

const { Text } = Typography;

type AnnouncementAction = (params: { id: string }) => Promise<API.ApiResult>;

const formatDate = (value?: string | null) =>
  value ? dayjs(value).format('YYYY-MM-DD HH:mm') : '-';

const AnnouncementPage: FC = () => {
  const intl = useIntl();
  const access = useAccess();
  const location = useLocation();
  const queryClient = useQueryClient();
  const actionRef = useRef<ActionType | null>(null);
  const [messageApi, contextHolder] = message.useMessage();
  const [modalApi, modalContextHolder] = Modal.useModal();
  const [detailId, setDetailId] = useState<string>();

  useEffect(() => {
    const announcementId = new URLSearchParams(location.search).get('announcementId');

    if (announcementId) {
      setDetailId(announcementId);
    }
  }, [location.search]);

  const closeDetail = useCallback(() => {
    setDetailId(undefined);

    const searchParams = new URLSearchParams(location.search);
    if (!searchParams.has('announcementId')) {
      return;
    }

    searchParams.delete('announcementId');
    history.replace({
      pathname: location.pathname,
      search: searchParams.toString(),
    });
  }, [location.pathname, location.search]);

  const reloadTable = useCallback(() => {
    actionRef.current?.reload();
    queryClient.invalidateQueries({ queryKey: ['announcements'] });
  }, [queryClient]);

  const runAction = useCallback(
    async (
      record: API.AnnouncementListDto,
      action: AnnouncementAction,
      successMessageId: string,
      failureMessageId: string,
    ) => {
      const { success, errorMessage } = await action({ id: record.id });

      if (!success) {
        messageApi.error(
          errorMessage ?? intl.formatMessage({ id: failureMessageId }),
        );
        return;
      }

      messageApi.success(intl.formatMessage({ id: successMessageId }));
      reloadTable();
    },
    [intl, messageApi, reloadTable],
  );

  const confirmAction = useCallback(
    (
      record: API.AnnouncementListDto,
      action: AnnouncementAction,
      titleId: string,
      descriptionId: string,
      successMessageId: string,
      failureMessageId: string,
      danger?: boolean,
    ) => {
      modalApi.confirm({
        title: <FormattedMessage id={titleId} />,
        content: <FormattedMessage id={descriptionId} />,
        okButtonProps: {
          danger,
        },
        onOk: () =>
          runAction(record, action, successMessageId, failureMessageId),
      });
    },
    [modalApi, runAction],
  );

  const columns = useMemo<ProColumns<API.AnnouncementListDto>[]>(
    () => [
      {
        title: <FormattedMessage id="announcement.label.index" />,
        dataIndex: 'index',
        valueType: 'index',
        width: 64,
        align: 'center',
        fixed: 'left',
        search: false,
      },
      {
        title: <FormattedMessage id="announcement.label.title" />,
        dataIndex: 'title',
        minWidth: 220,
        ellipsis: true,
        render: (_, record) => (
          <Space orientation="vertical" size={2}>
            <Button
              type="link"
              size="small"
              style={{ height: 'auto', padding: 0, fontWeight: 600 }}
              onClick={() => setDetailId(record.id)}
            >
              {record.title}
            </Button>
            <Tooltip title={record.summary}>
              <Text
                type="secondary"
                ellipsis
                style={{ display: 'block', maxWidth: 360 }}
              >
                {record.summary}
              </Text>
            </Tooltip>
          </Space>
        ),
      },
      {
        title: <FormattedMessage id="announcement.label.status" />,
        dataIndex: 'status',
        valueType: 'select',
        width: 120,
        valueEnum: announcementStatusValueEnum,
      },
      {
        title: <FormattedMessage id="announcement.label.audienceType" />,
        dataIndex: 'audienceType',
        width: 140,
        search: false,
        render: (_, record) => {
          const option =
            announcementAudienceValueEnum[
              record.audienceType as keyof typeof announcementAudienceValueEnum
            ];

          return option?.text ?? record.audienceType;
        },
      },
      {
        title: <FormattedMessage id="announcement.label.createdAt" />,
        dataIndex: 'createdAt',
        width: 170,
        search: false,
        render: (_, record) => formatDate(record.createdAt),
      },
      {
        title: <FormattedMessage id="announcement.label.publishedAt" />,
        dataIndex: 'publishedAt',
        width: 170,
        search: false,
        render: (_, record) => formatDate(record.publishedAt),
      },
      {
        title: <FormattedMessage id="announcement.label.withdrawnAt" />,
        dataIndex: 'withdrawnAt',
        width: 170,
        search: false,
        render: (_, record) => formatDate(record.withdrawnAt),
      },
      {
        title: <FormattedMessage id="announcement.label.option" />,
        dataIndex: 'option',
        valueType: 'option',
        fixed: 'right',
        width: 260,
        render: (_, record) => {
          const isDraft = record.status === ANNOUNCEMENT_STATUS.Draft;
          const isPublished = record.status === ANNOUNCEMENT_STATUS.Published;

          return (
            <Space size={0} separator={<Divider orientation="vertical" />}>
              {isDraft && access.has(ANNOUNCEMENT_PERMISSIONS.update) && (
                <AnnouncementForm
                  trigger={
                    <Button type="link" size="small" icon={<EditOutlined />}>
                      <FormattedMessage id="common.button.update" />
                    </Button>
                  }
                  announcement={record}
                  onSuccess={reloadTable}
                />
              )}

              {isDraft && access.has(ANNOUNCEMENT_PERMISSIONS.publish) && (
                <Button
                  type="link"
                  size="small"
                  icon={<SendOutlined />}
                  onClick={() =>
                    confirmAction(
                      record,
                      publishAnnouncement,
                      'announcement.confirm.publish.title',
                      'announcement.confirm.publish.description',
                      'announcement.message.publish.success',
                      'announcement.message.publish.failure',
                    )
                  }
                >
                  <FormattedMessage id="announcement.action.publish" />
                </Button>
              )}

              {isPublished && access.has(ANNOUNCEMENT_PERMISSIONS.withdraw) && (
                <Button
                  type="link"
                  size="small"
                  icon={<RollbackOutlined />}
                  onClick={() =>
                    confirmAction(
                      record,
                      withdrawAnnouncement,
                      'announcement.confirm.withdraw.title',
                      'announcement.confirm.withdraw.description',
                      'announcement.message.withdraw.success',
                      'announcement.message.withdraw.failure',
                    )
                  }
                >
                  <FormattedMessage id="announcement.action.withdraw" />
                </Button>
              )}

              {!isPublished && access.has(ANNOUNCEMENT_PERMISSIONS.delete) && (
                <Button
                  type="link"
                  size="small"
                  danger
                  icon={<DeleteOutlined />}
                  onClick={() =>
                    confirmAction(
                      record,
                      deleteAnnouncement,
                      'announcement.confirm.delete.title',
                      'announcement.confirm.delete.description',
                      'message.delete.success',
                      'message.delete.failure',
                      true,
                    )
                  }
                >
                  <FormattedMessage id="common.button.delete" />
                </Button>
              )}
            </Space>
          );
        },
      },
    ],
    [access, confirmAction, reloadTable],
  );

  return (
    <PageContainer title={false}>
      {contextHolder}
      {modalContextHolder}
      <ProTable<API.AnnouncementListDto, API.SearchAnnouncementsParams>
        actionRef={actionRef}
        columns={columns}
        rowKey="id"
        scroll={{ x: 'max-content' }}
        search={{ labelWidth: 120 }}
        pagination={{ showSizeChanger: true }}
        request={async (params) => {
          const { current, pageSize, ...rest } = params as API.SearchAnnouncementsParams & {
            current?: number;
            pageSize?: number;
          };
          const { success, data } = await searchAnnouncements({
            ...rest,
            Current: current,
            PageSize: pageSize,
          });

          return {
            data: data?.items ?? [],
            total: data?.total ?? 0,
            success: success ?? false,
          };
        }}
        headerTitle={
          <Space>
            {access.has(ANNOUNCEMENT_PERMISSIONS.create) && (
              <AnnouncementForm
                trigger={
                  <Button type="primary" icon={<PlusOutlined />}>
                    <FormattedMessage id="common.button.create" />
                  </Button>
                }
                onSuccess={reloadTable}
              />
            )}
          </Space>
        }
      />

      <AnnouncementDetail
        id={detailId}
        open={Boolean(detailId)}
        onOpenChange={(open) => {
          if (!open) {
            closeDetail();
          }
        }}
      />
    </PageContainer>
  );
};

export default AnnouncementPage;
