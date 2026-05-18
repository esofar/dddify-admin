import type { ProFormInstance } from '@ant-design/pro-components';
import {
  DrawerForm,
  ProFormText,
  ProFormTextArea,
} from '@ant-design/pro-components';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import { message } from 'antd';
import type { FC, ReactElement } from 'react';
import { useCallback, useRef, useState } from 'react';
import { createLookup, updateLookup } from '@/services/v1/lookup';
import {
  noSpecialNoSpaceRule,
  requiredRule,
  lowerCaseCodeRule,
} from '@/utils/rules';

type LookupFormValues = {
  code: string;
  name: string;
  description?: string | null;
};

type LookupFormProps = {
  trigger: ReactElement;
  lookup?: API.LookupDto;
  onSuccess?: () => void;
};

const LookupForm: FC<LookupFormProps> = ({ trigger, lookup, onSuccess }) => {
  const intl = useIntl();
  const queryClient = useQueryClient();
  const formRef = useRef<ProFormInstance<LookupFormValues> | undefined>(
    undefined,
  );
  const [open, setOpen] = useState(false);
  const [messageApi, contextHolder] = message.useMessage();

  const isEditing = Boolean(lookup?.id);

  const saveMutation = useMutation({
    mutationFn: async (values: LookupFormValues) => {
      if (isEditing && lookup?.id) {
        const body: API.UpdateLookupRequest = {
          name: values.name,
          description: values.description,
        };

        return updateLookup({ id: lookup.id }, body);
      }

      const body: API.CreateLookupRequest = {
        code: values.code,
        name: values.name,
        description: values.description,
      };

      return createLookup(body);
    },
  });

  const handleOpenChange = useCallback(
    (nextOpen: boolean) => {
      setOpen(nextOpen);

      if (!nextOpen) {
        formRef.current?.resetFields();
        return;
      }

      if (!lookup) {
        formRef.current?.resetFields();
        return;
      }

      formRef.current?.setFieldsValue({
        code: lookup.code,
        name: lookup.name,
        description: lookup.description,
      });
    },
    [lookup],
  );

  return (
    <>
      {contextHolder}
      <DrawerForm<LookupFormValues>
        title={
          <FormattedMessage
            id={
              isEditing ? 'lookup.formTitle.update' : 'lookup.formTitle.create'
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
          name="code"
          label={<FormattedMessage id="lookup.label.code" />}
          disabled={isEditing}
          fieldProps={{
            showCount: true,
            maxLength: 20,
          }}
          rules={[requiredRule, lowerCaseCodeRule]}
        />

        <ProFormText
          name="name"
          label={<FormattedMessage id="lookup.label.name" />}
          fieldProps={{
            showCount: true,
            maxLength: 20,
          }}
          rules={[requiredRule, noSpecialNoSpaceRule]}
        />

        <ProFormTextArea
          name="description"
          label={<FormattedMessage id="lookup.label.description" />}
          fieldProps={{
            showCount: true,
            maxLength: 100,
          }}
        />
      </DrawerForm>
    </>
  );
};

export default LookupForm;
