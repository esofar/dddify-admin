import type { ProFormInstance } from '@ant-design/pro-components';
import {
  DrawerForm,
  ProFormSelect,
  ProFormText,
} from '@ant-design/pro-components';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import { message, Tag } from 'antd';
import type { FC, ReactElement } from 'react';
import { useCallback, useRef, useState } from 'react';
import { createLookupItem, updateLookupItem } from '@/services/v1/lookup';
import {
  lowerCaseCodeRule,
  noSpecialNoSpaceRule,
  requiredRule,
} from '@/utils/rules';
import { tagColors } from '../data';

type LookupItemFormValues = {
  value: string;
  label: string;
  color?: string | null;
};

type LookupItemFormProps = {
  trigger: ReactElement;
  lookupId?: string;
  item?: API.LookupItemDto;
  onSuccess?: () => void;
};

const LookupItemForm: FC<LookupItemFormProps> = ({
  trigger,
  lookupId,
  item,
  onSuccess,
}) => {
  const intl = useIntl();
  const queryClient = useQueryClient();
  const formRef = useRef<ProFormInstance<LookupItemFormValues> | undefined>(
    undefined,
  );
  const [open, setOpen] = useState(false);
  const [messageApi, contextHolder] = message.useMessage();

  const isEditing = Boolean(item?.id);

  const saveMutation = useMutation({
    mutationFn: async (values: LookupItemFormValues) => {
      if (!lookupId) {
        return { success: false } as API.ApiResult;
      }

      if (isEditing && item?.id) {
        const body: API.UpdateLookupItemRequest = {
          label: values.label,
          color: values.color,
        };

        return updateLookupItem({ id: lookupId, itemId: item.id }, body);
      }

      const body: API.CreateLookupItemRequest = {
        value: values.value,
        label: values.label,
        color: values.color,
      };

      return createLookupItem({ id: lookupId }, body);
    },
  });

  const handleOpenChange = useCallback(
    (nextOpen: boolean) => {
      setOpen(nextOpen);

      if (!nextOpen) {
        formRef.current?.resetFields();
        return;
      }

      if (!item) {
        formRef.current?.resetFields();
        return;
      }

      formRef.current?.setFieldsValue({
        value: item.value,
        label: item.label,
        color: item.color,
      });
    },
    [item],
  );

  return (
    <>
      {contextHolder}
      <DrawerForm<LookupItemFormValues>
        title={
          <FormattedMessage
            id={
              isEditing
                ? 'lookup.item.formTitle.update'
                : 'lookup.item.formTitle.create'
            }
          />
        }
        trigger={trigger}
        grid
        layout="horizontal"
        labelCol={{ span: 5 }}
        colProps={{ xs: 24, sm: 24 }}
        width={520}
        autoComplete="off"
        formRef={formRef}
        open={open}
        onOpenChange={handleOpenChange}
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
          queryClient.invalidateQueries({ queryKey: ['lookups'] });
          onSuccess?.();
          return true;
        }}
        submitter={{
          submitButtonProps: {
            loading: saveMutation.isPending,
          },
        }}
        drawerProps={{
          destroyOnHidden: true,
          maskClosable: false,
        }}
      >
        <ProFormText
          name="value"
          label={<FormattedMessage id="lookup.item.label.value" />}
          disabled={isEditing}
          fieldProps={{
            showCount: true,
            maxLength: 20,
          }}
          rules={[requiredRule, lowerCaseCodeRule]}
        />

        <ProFormText
          name="label"
          label={<FormattedMessage id="lookup.item.label.label" />}
          fieldProps={{
            showCount: true,
            maxLength: 20,
          }}
          rules={[requiredRule, noSpecialNoSpaceRule]}
        />

        <ProFormSelect
          name="color"
          className=''
          label={<FormattedMessage id="lookup.item.label.color" />}
          allowClear
          options={tagColors.map((color) => ({
            label: (
              <Tag color={color} variant="filled">
                {color}
              </Tag>
            ),
            value: color,
          }))}
        />
      </DrawerForm>
    </>
  );
};

export default LookupItemForm;
