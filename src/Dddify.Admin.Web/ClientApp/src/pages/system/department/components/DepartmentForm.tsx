import type { ProFormInstance } from '@ant-design/pro-components';
import {
  DrawerForm,
  ProFormDigit,
  ProFormSelect,
  ProFormSwitch,
  ProFormText,
  ProFormTreeSelect,
} from '@ant-design/pro-components';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useIntl } from '@umijs/max';
import { message } from 'antd';
import type { FC, ReactElement } from 'react';
import { useCallback, useRef, useState } from 'react';
import ProFormUserSelect from '@/components/business/ProFormUserSelect';
import {
  createDepartment,
  getAllDepartments,
  getDepartmentDetail,
  updateDepartment,
} from '@/services/v1/department';
import { noSpecialRule, requiredRule } from '@/utils/rules';
import type { DepartmentSelectNode } from '../data';
import { toDepartmentSelectNodes } from '../data';

type DepartmentFormValues = {
  concurrencyStamp?: string | null;
  parentId?: string | null;
  name: string;
  type: string;
  leaderId: string;
  isEnabled: boolean;
  order: number;
};

type DepartmentFormProps = {
  trigger: ReactElement;
  department?: API.DepartmentListDto;
  typeValueEnum: Record<string, { text: string; color?: string }>;
  onSuccess?: () => void;
};

const DepartmentForm: FC<DepartmentFormProps> = ({
  trigger,
  department,
  typeValueEnum,
  onSuccess,
}) => {
  const intl = useIntl();
  const queryClient = useQueryClient();
  const formRef = useRef<ProFormInstance<DepartmentFormValues> | undefined>(
    undefined,
  );
  const [open, setOpen] = useState(false);
  const [messageApi, contextHolder] = message.useMessage();

  const isEditing = Boolean(department?.id);

  const { data: departments = [] } = useQuery<DepartmentSelectNode[]>({
    queryKey: ['departments', 'all', 'select'],
    enabled: open,
    queryFn: async () => {
      const { success, data, errorMessage } = await getAllDepartments();

      if (!success || !data) {
        throw new Error(errorMessage ?? 'Load departments failed.');
      }

      return toDepartmentSelectNodes(data);
    },
  });

  const saveMutation = useMutation({
    mutationFn: async (values: DepartmentFormValues) => {
      if (isEditing && department?.id) {
        const body: API.UpdateDepartmentRequest = {
          parentId: values.parentId,
          name: values.name,
          type: values.type,
          leaderId: values.leaderId,
          isEnabled: values.isEnabled,
          order: values.order,
          concurrencyStamp: values.concurrencyStamp,
        };

        return updateDepartment({ id: department.id }, body);
      }

      const body: API.CreateDepartmentRequest = {
        parentId: values.parentId,
        name: values.name,
        type: values.type,
        leaderId: values.leaderId,
        isEnabled: values.isEnabled,
        order: values.order,
      };

      return createDepartment(body);
    },
  });

  const loadDepartmentDetail = useCallback(async () => {
    if (!department?.id) {
      formRef.current?.resetFields();
      formRef.current?.setFieldsValue({
        isEnabled: true,
        order: 0,
      });
      return;
    }

    const { success, data, errorMessage } = await getDepartmentDetail({
      id: department.id,
    });

    if (!success || !data) {
      messageApi.error(errorMessage);
      return;
    }

    formRef.current?.setFieldsValue({
      concurrencyStamp: data.concurrencyStamp,
      parentId: data.parentId,
      name: data.name,
      type: data.type,
      leaderId: data.leader?.id
        ? ({
            value: data.leader.id,
            label: data.leader.name,
          } as unknown as string)
        : undefined,
      isEnabled: data.isEnabled,
      order: data.order,
    });
  }, [department?.id, messageApi]);

  const handleOpenChange = useCallback(
    async (nextOpen: boolean) => {
      setOpen(nextOpen);

      if (!nextOpen) {
        formRef.current?.resetFields();
        return;
      }

      await loadDepartmentDetail();
    },
    [loadDepartmentDetail],
  );

  return (
    <>
      {contextHolder}
      <DrawerForm<DepartmentFormValues>
        title={
          <FormattedMessage
            id={
              isEditing
                ? 'department.formTitle.update'
                : 'department.formTitle.create'
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
          queryClient.invalidateQueries({ queryKey: ['departments'] });
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
        <ProFormText name="concurrencyStamp" hidden colProps={{ span: 0 }} />

        <ProFormText
          name="name"
          label={<FormattedMessage id="department.label.name" />}
          fieldProps={{
            showCount: true,
            maxLength: 50,
          }}
          rules={[requiredRule, noSpecialRule]}
        />

        <ProFormTreeSelect
          name="parentId"
          label={<FormattedMessage id="department.label.parentId" />}
          fieldProps={{
            treeData: departments,
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
        />

        <ProFormSelect
          name="type"
          label={<FormattedMessage id="department.label.type" />}
          valueEnum={typeValueEnum}
          rules={[requiredRule]}
        />

        <ProFormUserSelect
          name="leaderId"
          multiple={false}
          showAvatar
          label={<FormattedMessage id="department.label.leaderId" />}
          rules={[requiredRule]}
        />

        <ProFormSwitch
          name="isEnabled"
          label={<FormattedMessage id="department.label.isEnabled" />}
        />

        <ProFormDigit
          name="order"
          label={<FormattedMessage id="department.label.order" />}
          fieldProps={{
            min: 0,
            max: 999,
            changeOnWheel: true,
          }}
          rules={[
            requiredRule,
            {
              type: 'number',
            },
          ]}
        />
      </DrawerForm>
    </>
  );
};

export default DepartmentForm;
