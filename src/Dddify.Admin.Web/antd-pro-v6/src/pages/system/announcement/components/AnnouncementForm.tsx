import type { ProFormInstance } from '@ant-design/pro-components';
import {
  DrawerForm,
  ProFormDependency,
  ProFormRadio,
  ProFormSelect,
  ProFormText,
  ProFormTextArea,
  ProFormTreeSelect,
} from '@ant-design/pro-components';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import { Form, message, theme } from 'antd';
import type { FC, ReactElement, ReactNode } from 'react';
import { useCallback, useRef, useState } from 'react';
import ProFormUserSelect from '@/components/business/ProFormUserSelect';
import FroFormTiptap from '@/components/business/FroFormTiptap';
import { getAllDepartments } from '@/services/v1/department';
import { getAllRoles } from '@/services/v1/role';
import {
  createAnnouncement,
  getAnnouncement,
  updateAnnouncement,
} from '@/services/v1/announcement';
import { requiredRule } from '@/utils/rules';
import type { DepartmentSelectNode } from '../../department/data';
import { toDepartmentSelectNodes } from '../../department/data';
import {
  ANNOUNCEMENT_AUDIENCE_TYPE,
  announcementAudienceValueEnum,
  toRoleValueEnum,
} from '../data';

type AnnouncementFormValues = {
  title: string;
  summary: string;
  contentHtml: string;
  audienceType: string;
  audienceTargetIds?: AnnouncementTargetValue[];
};

type AnnouncementTargetSelectValue = {
  value: string;
  label: ReactNode;
};

type AnnouncementTargetValue = string | AnnouncementTargetSelectValue;

type AnnouncementFormProps = {
  trigger: ReactElement;
  announcement?: API.AnnouncementListDto;
  onSuccess?: () => void;
};

const toAudienceTargetIds = (values?: AnnouncementTargetValue[]) =>
  (values ?? []).map((item) => (typeof item === 'string' ? item : item.value));

const toAudienceTargetFormValue = (
  audience?: API.AnnouncementAudienceDto,
): AnnouncementTargetValue[] => {
  if (!audience) {
    return [];
  }

  if (audience.type !== ANNOUNCEMENT_AUDIENCE_TYPE.SpecificUsers) {
    return audience.targetIds ?? [];
  }

  return (audience.targets ?? []).map((target) => ({
    value: target.id,
    label: target.name,
  }));
};

const AnnouncementForm: FC<AnnouncementFormProps> = ({
  trigger,
  announcement,
  onSuccess,
}) => {
  const intl = useIntl();
  const queryClient = useQueryClient();
  const { token } = theme.useToken();
  const formRef = useRef<ProFormInstance<AnnouncementFormValues> | undefined>(
    undefined,
  );
  const [open, setOpen] = useState(false);
  const [messageApi, contextHolder] = message.useMessage();

  const isEditing = Boolean(announcement?.id);

  const { data: departments = [] } = useQuery<DepartmentSelectNode[]>({
    queryKey: ['announcements', 'departments', 'select'],
    enabled: open,
    queryFn: async () => {
      const { success, data, errorMessage } = await getAllDepartments();

      if (!success || !data) {
        throw new Error(errorMessage ?? 'Load departments failed.');
      }

      return toDepartmentSelectNodes(data);
    },
  });

  const { data: roleValueEnum = {} } = useQuery({
    queryKey: ['announcements', 'roles', 'select'],
    enabled: open,
    queryFn: async () => {
      const { success, data, errorMessage } = await getAllRoles();

      if (!success || !data) {
        throw new Error(errorMessage ?? 'Load roles failed.');
      }

      return toRoleValueEnum(data);
    },
  });

  const saveMutation = useMutation({
    mutationFn: async (values: AnnouncementFormValues) => {
      const body: API.CreateAnnouncementRequest = {
        title: values.title,
        summary: values.summary,
        contentHtml: values.contentHtml,
        audienceType: values.audienceType,
        audienceTargetIds:
          values.audienceType === ANNOUNCEMENT_AUDIENCE_TYPE.AllUsers
            ? []
            : toAudienceTargetIds(values.audienceTargetIds),
      };

      if (isEditing && announcement?.id) {
        return updateAnnouncement({ id: announcement.id }, body);
      }

      return createAnnouncement(body);
    },
  });

  const loadAnnouncementDetail = useCallback(async () => {
    if (!announcement?.id) {
      formRef.current?.resetFields();
      formRef.current?.setFieldsValue({
        audienceType: ANNOUNCEMENT_AUDIENCE_TYPE.AllUsers,
        audienceTargetIds: [],
      });
      return;
    }

    const { success, data, errorMessage } = await getAnnouncement({
      id: announcement.id,
    });

    if (!success || !data) {
      messageApi.error(errorMessage);
      return;
    }

    formRef.current?.setFieldsValue({
      title: data.title,
      summary: data.summary,
      contentHtml: data.content?.html,
      audienceType: data.audience?.type,
      audienceTargetIds: toAudienceTargetFormValue(data.audience),
    });
  }, [announcement?.id, messageApi]);

  const handleOpenChange = useCallback(
    async (nextOpen: boolean) => {
      setOpen(nextOpen);

      if (!nextOpen) {
        formRef.current?.resetFields();
        return;
      }

      await loadAnnouncementDetail();
    },
    [loadAnnouncementDetail],
  );

  return (
    <>
      {contextHolder}
      <DrawerForm<AnnouncementFormValues>
        title={
          <FormattedMessage
            id={
              isEditing
                ? 'announcement.action.update'
                : 'announcement.action.create'
            }
          />
        }
        trigger={trigger}
        grid
        layout="horizontal"
        labelCol={{ span: 4 }}
        colProps={{ xs: 24, sm: 24 }}
        width={760}
        autoComplete="off"
        formRef={formRef}
        open={open}
        onOpenChange={handleOpenChange}
        onValuesChange={(changedValues) => {
          if ('audienceType' in changedValues) {
            formRef.current?.setFieldValue('audienceTargetIds', []);
          }
        }}
        onFinish={async (values) => {
          const { success, errorMessage } =
            await saveMutation.mutateAsync(values);

          if (!success) {
            messageApi.error(
              errorMessage ??
                intl.formatMessage({
                  id: isEditing
                    ? 'message.update.failure'
                    : 'message.create.failure',
                }),
            );
            return false;
          }

          messageApi.success(
            intl.formatMessage({
              id: isEditing
                ? 'message.update.success'
                : 'message.create.success',
            }),
          );
          queryClient.invalidateQueries({ queryKey: ['announcements'] });
          onSuccess?.();
          return true;
        }}
        drawerProps={{
          destroyOnHidden: true,
          maskClosable: false,
        }}
        submitter={{
          submitButtonProps: {
            loading: saveMutation.isPending,
          },
        }}
      >
        <div
          style={{
            width: '100%',
            padding: `${token.paddingMD}px ${token.padding}px 0`,
            border: `1px solid ${token.colorBorderSecondary}`,
            borderRadius: token.borderRadius,
          }}
        >
          <ProFormText
            name="title"
            label={<FormattedMessage id="announcement.label.title" />}
            fieldProps={{
              showCount: true,
              maxLength: 100,
            }}
            rules={[requiredRule]}
          />

          <ProFormTextArea
            name="summary"
            label={<FormattedMessage id="announcement.label.summary" />}
            fieldProps={{
              showCount: true,
              maxLength: 500,
              autoSize: {
                minRows: 3,
                maxRows: 5,
              },
            }}
            rules={[requiredRule]}
          />

          <ProFormRadio.Group
            name="audienceType"
            label={<FormattedMessage id="announcement.label.audienceType" />}
            valueEnum={announcementAudienceValueEnum}
            rules={[requiredRule]}
          />

          <ProFormDependency name={['audienceType']}>
            {({ audienceType }) => {
              if (audienceType === ANNOUNCEMENT_AUDIENCE_TYPE.Departments) {
                return (
                  <ProFormTreeSelect
                    name="audienceTargetIds"
                    label={
                      <FormattedMessage id="announcement.label.audienceTargetIds" />
                    }
                    fieldProps={{
                      treeData: departments,
                      multiple: true,
                      allowClear: true,
                      showSearch: true,
                      treeNodeFilterProp: 'name',
                      treeDataSimpleMode: {
                        id: 'id',
                        pId: 'parentId',
                      },
                      treeLine: true,
                      fieldNames: {
                        label: 'name',
                        value: 'id',
                      },
                    }}
                    rules={[requiredRule]}
                  />
                );
              }

              if (audienceType === ANNOUNCEMENT_AUDIENCE_TYPE.Roles) {
                return (
                  <ProFormSelect
                    name="audienceTargetIds"
                    label={
                      <FormattedMessage id="announcement.label.audienceTargetIds" />
                    }
                    valueEnum={roleValueEnum}
                    fieldProps={{
                      mode: 'multiple',
                      allowClear: true,
                      showSearch: true,
                    }}
                    rules={[requiredRule]}
                  />
                );
              }

              if (audienceType === ANNOUNCEMENT_AUDIENCE_TYPE.SpecificUsers) {
                return (
                  <ProFormUserSelect
                    name="audienceTargetIds"
                    label={
                      <FormattedMessage id="announcement.label.audienceTargetIds" />
                    }
                    multiple
                    showAvatar
                    rules={[requiredRule]}
                  />
                );
              }

              return null;
            }}
          </ProFormDependency>

          <Form.Item
            name="contentHtml"
            label={<FormattedMessage id="announcement.label.content" />}
            rules={[requiredRule]}
          >
            <FroFormTiptap minHeight={260} maxHeight={520} />
          </Form.Item>
        </div>
      </DrawerForm>
    </>
  );
};

export default AnnouncementForm;
