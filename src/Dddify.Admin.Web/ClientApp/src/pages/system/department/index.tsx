import {
  DeleteOutlined,
  DownOutlined,
  EditOutlined,
  PlusOutlined,
  UpOutlined,
} from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { PageContainer, ProTable } from '@ant-design/pro-components';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { FormattedMessage, useAccess, useIntl } from '@umijs/max';
import { Button, Divider, message, Modal, Space, Tag } from 'antd';
import type { FC, Key } from 'react';
import { useCallback, useMemo, useRef, useState } from 'react';
import { deleteDepartment, searchDepartments } from '@/services/v1/department';
import { getLookupActiveItems } from '@/services/v1/lookup';
import DepartmentForm from './components/DepartmentForm';
import type { DepartmentTreeNode } from './data';
import {
  buildDepartmentTree,
  DEPARTMENT_PERMISSIONS,
  departmentStatusValueEnum,
  toDepartmentTypeValueEnum,
} from './data';

const DepartmentPage: FC = () => {
  const intl = useIntl();
  const access = useAccess();
  const queryClient = useQueryClient();
  const actionRef = useRef<ActionType | null>(null);
  const [messageApi, contextHolder] = message.useMessage();
  const [modalApi, modalContextHolder] = Modal.useModal();
  const [expandedRowKeys, setExpandedRowKeys] = useState<Key[]>([]);
  const [allRowKeys, setAllRowKeys] = useState<Key[]>([]);

  const { data: departmentTypeValueEnum = {} } = useQuery({
    queryKey: ['departments', 'types'],
    queryFn: async () => {
      const { success, data, errorMessage } = await getLookupActiveItems({
        code: 'department_type',
      });

      if (!success) {
        throw new Error(errorMessage ?? 'Load department types failed.');
      }

      return toDepartmentTypeValueEnum(data);
    },
  });

  const reloadTable = useCallback(() => {
    actionRef.current?.reload();
    queryClient.invalidateQueries({ queryKey: ['departments'] });
  }, [queryClient]);

  const handleDelete = useCallback(
    (department: API.DepartmentListDto) => {
      modalApi.confirm({
        title: <FormattedMessage id="department.confirm.delete.title" />,
        content: <FormattedMessage id="department.confirm.delete.description" />,
        okButtonProps: {
          danger: true,
        },
        onOk: async () => {
          const { success, errorMessage } = await deleteDepartment({
            id: department.id,
          });

          if (!success) {
            messageApi.error(
              errorMessage ?? intl.formatMessage({ id: 'message.delete.failure' }),
            );
            return;
          }

          messageApi.success(intl.formatMessage({ id: 'message.delete.success' }));
          reloadTable();
        },
      });
    },
    [intl, messageApi, modalApi, reloadTable],
  );

  const columns = useMemo<ProColumns<DepartmentTreeNode>[]>(
    () => [
      {
        title: <FormattedMessage id="department.label.name" />,
        dataIndex: 'name',
        minWidth: 160,
        ellipsis: true,
      },
      {
        title: <FormattedMessage id="department.label.type" />,
        dataIndex: 'type',
        minWidth: 140,
        valueType: 'select',
        valueEnum: departmentTypeValueEnum,
        render: (_, record) => {
          const option = departmentTypeValueEnum[record.type];

          if (!option) {
            return '-';
          }

          return (
            <Tag color={option.color} variant="filled">
              {option.text}
            </Tag>
          );
        },
      },
      {
        title: <FormattedMessage id="department.label.leaderId" />,
        dataIndex: 'leaderName',
        minWidth: 140,
        search: false,
        render: (_, record) => record.leader?.name ?? '-',
      },
      {
        title: <FormattedMessage id="department.label.isEnabled" />,
        dataIndex: 'isEnabled',
        minWidth: 120,
        valueType: 'select',
        valueEnum: departmentStatusValueEnum,
      },
      {
        title: <FormattedMessage id="department.label.order" />,
        dataIndex: 'order',
        minWidth: 100,
        search: false,
      },
      {
        title: <FormattedMessage id="department.label.option" />,
        dataIndex: 'option',
        valueType: 'option',
        fixed: 'right',
        width: 170,
        render: (_, record) => (
          <Space size={0} separator={<Divider orientation="vertical" />}>
            {access.has(DEPARTMENT_PERMISSIONS.update) && (
              <DepartmentForm
                trigger={
                  <Button type="link" size="small" icon={<EditOutlined />}>
                    <FormattedMessage id="common.button.update" />
                  </Button>
                }
                department={record}
                typeValueEnum={departmentTypeValueEnum}
                onSuccess={reloadTable}
              />
            )}

            {access.has(DEPARTMENT_PERMISSIONS.delete) && (
              <Button
                type="link"
                size="small"
                danger
                icon={<DeleteOutlined />}
                onClick={() => handleDelete(record)}
              >
                <FormattedMessage id="common.button.delete" />
              </Button>
            )}
          </Space>
        ),
      },
    ],
    [access, departmentTypeValueEnum, handleDelete, reloadTable],
  );

  return (
    <PageContainer title={false}>
      {contextHolder}
      {modalContextHolder}
      <ProTable<DepartmentTreeNode, API.SearchDepartmentsParams>
        actionRef={actionRef}
        columns={columns}
        scroll={{ x: 'max-content' }}
        rowKey="id"
        pagination={false}
        search={{ labelWidth: 120 }}
        request={async (params) => {
          const { success, data } = await searchDepartments(params);
          const departments = (data ?? []) as API.DepartmentListDto[];

          setAllRowKeys(departments.map((item) => item.id));

          return {
            data: buildDepartmentTree(departments),
            success: success ?? false,
          };
        }}
        expandable={{
          expandedRowKeys,
          onExpandedRowsChange: (keys) => setExpandedRowKeys([...keys]),
        }}
        headerTitle={
          <Space>
            {access.has(DEPARTMENT_PERMISSIONS.create) && (
              <DepartmentForm
                trigger={
                  <Button type="primary" icon={<PlusOutlined />}>
                    <FormattedMessage id="common.button.create" />
                  </Button>
                }
                typeValueEnum={departmentTypeValueEnum}
                onSuccess={reloadTable}
              />
            )}
            <Button
              key="expand"
              icon={
                expandedRowKeys.length > 0 ? <UpOutlined /> : <DownOutlined />
              }
              onClick={() =>
                setExpandedRowKeys(
                  expandedRowKeys.length > 0 ? [] : [...allRowKeys],
                )
              }
            >
              {expandedRowKeys.length > 0 ? (
                <FormattedMessage id="common.button.collapseAll" />
              ) : (
                <FormattedMessage id="common.button.expandAll" />
              )}
            </Button>
          </Space>
        }
      />
    </PageContainer>
  );
};

export default DepartmentPage;
