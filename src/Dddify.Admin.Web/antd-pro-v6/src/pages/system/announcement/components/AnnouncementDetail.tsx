import { useQuery } from '@tanstack/react-query';
import { FormattedMessage } from '@umijs/max';
import {
  Alert,
  Descriptions,
  Divider,
  Drawer,
  Space,
  Spin,
  Tag,
  Typography,
} from 'antd';
import dayjs from 'dayjs';
import type { FC } from 'react';
import FroFormTiptapPreview from '@/components/business/FroFormTiptapPreview';
import { getAnnouncement } from '@/services/v1/announcement';
import {
  announcementAudienceValueEnum,
  announcementStatusValueEnum,
} from '../data';

type AnnouncementDetailProps = {
  id?: string;
  open: boolean;
  onOpenChange: (open: boolean) => void;
};

const formatDate = (value?: string | null) =>
  value ? dayjs(value).format('YYYY-MM-DD HH:mm') : '-';

const getValueText = (
  valueEnum: Record<string, { text: React.ReactNode }>,
  value?: string,
) => valueEnum[value ?? '']?.text ?? value ?? '-';

const statusColorMap: Record<string, string> = {
  Draft: 'default',
  Published: 'success',
  Withdrawn: 'warning',
};

const AnnouncementDetail: FC<AnnouncementDetailProps> = ({
  id,
  open,
  onOpenChange,
}) => {
  const { data, error, isError, isLoading } = useQuery({
    queryKey: ['announcements', 'detail', id],
    enabled: open && Boolean(id),
    queryFn: async () => {
      const { success, data: detail, errorMessage } = await getAnnouncement({
        id: id ?? '',
      });

      if (!success || !detail) {
        throw new Error(errorMessage ?? 'Load announcement failed.');
      }

      return detail;
    },
  });
  const audienceTargets = data?.audience?.targets ?? [];

  return (
    <Drawer
      title={<FormattedMessage id="announcement.action.detail" />}
      open={open}
      width={760}
      onClose={() => onOpenChange(false)}
      destroyOnHidden
    >
      <Spin spinning={isLoading}>
        {isError && (
          <Alert
            showIcon
            type="error"
            message={
              error instanceof Error
                ? error.message
                : 'Load announcement failed.'
            }
          />
        )}

        {data && (
          <>
            <Space align="start" size={8} style={{ marginBottom: 8 }}>
              <Typography.Title level={4} style={{ margin: 0 }}>
                {data.title}
              </Typography.Title>
              <Tag color={statusColorMap[data.status] ?? 'default'}>
                {getValueText(announcementStatusValueEnum, data.status)}
              </Tag>
            </Space>

            <Typography.Paragraph type="secondary">
              {data.summary}
            </Typography.Paragraph>

            <Descriptions column={2} size="small">
              <Descriptions.Item
                label={<FormattedMessage id="announcement.label.createdAt" />}
              >
                {formatDate(data.createdAt)}
              </Descriptions.Item>
              <Descriptions.Item
                label={<FormattedMessage id="announcement.label.publishedAt" />}
              >
                {formatDate(data.publishedAt)}
              </Descriptions.Item>
              <Descriptions.Item
                label={<FormattedMessage id="announcement.label.withdrawnAt" />}
              >
                {formatDate(data.withdrawnAt)}
              </Descriptions.Item>

              <Descriptions.Item
                label={<FormattedMessage id="announcement.label.audienceType" />}
              >
                {getValueText(announcementAudienceValueEnum, data.audience?.type)}
              </Descriptions.Item>
            </Descriptions>

            {audienceTargets.length > 0 && (
              <div style={{ marginTop: 12 }}>
                <Typography.Text type="secondary">
                  <FormattedMessage id="announcement.label.audienceTargetIds" />
                  ：
                </Typography.Text>
                <Space wrap size={[4, 4]} style={{ marginLeft: 4 }}>
                  {audienceTargets.map((target) => (
                    <Tag key={target.id}>{target.name}</Tag>
                  ))}
                </Space>
              </div>
            )}

            <Divider />

            <Typography.Title level={5}>
              <FormattedMessage id="announcement.label.content" />
            </Typography.Title>
            <FroFormTiptapPreview
              value={data.content?.html}
              minHeight={240}
              maxHeight="calc(100vh - 360px)"
            />
          </>
        )}
      </Spin>
    </Drawer>
  );
};

export default AnnouncementDetail;
