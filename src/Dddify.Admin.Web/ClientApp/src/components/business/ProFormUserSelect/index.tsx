import { TeamOutlined, UserOutlined } from '@ant-design/icons';
import type {
  ActionType,
  ProColumns,
  ProFormSelectProps,
} from '@ant-design/pro-components';
import { ProFormSelect, ProTable } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import type { GetProp, SelectProps } from 'antd';
import { Avatar, Form, Modal, Space, Tag } from 'antd';
import type { NamePath } from 'antd/es/form/interface';
import type { Key, ReactNode } from 'react';
import {
  forwardRef,
  useCallback,
  useEffect,
  useImperativeHandle,
  useMemo,
  useRef,
  useState,
} from 'react';
import { userGenderValueEnum } from '@/pages/system/user/data';
import { searchUsers } from '@/services/v1/user';

type SelectValue = {
  value: string;
  label: ReactNode;
};

type UserSelectFieldProps = Omit<
  NonNullable<ProFormSelectProps['fieldProps']>,
  | 'fieldNames'
  | 'labelInValue'
  | 'maxCount'
  | 'mode'
  | 'onOpenChange'
  | 'open'
  | 'options'
  | 'suffixIcon'
  | 'value'
>;

export interface UserSelectRef {
  /** 打开用户选择弹窗。 */
  openModal: () => void;
  /** 关闭用户选择弹窗。 */
  closeModal: () => void;
  /** 清空当前选择。 */
  clearSelection: () => void;
}

export interface ProFormUserSelectProps
  extends Omit<
    ProFormSelectProps,
    'fieldProps' | 'labelInValue' | 'mode' | 'options' | 'request'
  > {
  /** 是否支持多选。 */
  multiple?: boolean;
  /** 多选时最多可选择的用户数。 */
  maxCount?: number;
  /** 是否在用户列表中显示头像。 */
  showAvatar?: boolean;
  /** 用户选择弹窗宽度。 */
  modalWidth?: number;
  /** 用户选择弹窗标题。 */
  modalTitle?: ReactNode;
  /** 透传给底层 Select 的属性。 */
  fieldProps?: UserSelectFieldProps;
}

const getUserLabel = (user: Pick<API.UserListDto, 'email' | 'id' | 'name'>) =>
  user.name || user.email || user.id;

const normalizeValue = (value: unknown): SelectValue[] => {
  const values = Array.isArray(value) ? value : value ? [value] : [];

  return values
    .map<SelectValue | null>((item) => {
      if (typeof item === 'string' || typeof item === 'number') {
        return {
          value: String(item),
          label: String(item),
        };
      }

      if (!item || typeof item !== 'object') {
        return null;
      }

      const option = item as Partial<SelectValue>;

      if (!option.value) {
        return null;
      }

      return {
        value: String(option.value),
        label: option.label ?? String(option.value),
      };
    })
    .filter((item): item is SelectValue => Boolean(item));
};

const getNamePath = (name: ProFormSelectProps['name']) => name as NamePath;

const ProFormUserSelect = forwardRef<UserSelectRef, ProFormUserSelectProps>(
  (
    {
      multiple = false,
      maxCount,
      showAvatar = true,
      modalWidth = 960,
      modalTitle,
      fieldProps,
      ...rest
    },
    ref,
  ) => {
    const actionRef = useRef<ActionType | null>(null);
    const form = Form.useFormInstance();
    const fieldName = getNamePath(rest.name);
    const watchedValue = Form.useWatch(fieldName, form);

    const [open, setOpen] = useState(false);
    const [selectedMap, setSelectedMap] = useState<
      Map<string, API.UserListDto>
    >(new Map());
    const [pendingKeys, setPendingKeys] = useState<Key[]>([]);
    const [pendingMap, setPendingMap] = useState<Map<string, API.UserListDto>>(
      new Map(),
    );

    const setFieldValue = useCallback(
      (value: SelectValue | SelectValue[] | null) => {
        form.setFieldValue(fieldName, value);
      },
      [fieldName, form],
    );

    const clearSelection = useCallback(() => {
      setPendingKeys([]);
      setPendingMap(new Map());
      setSelectedMap(new Map());
      setFieldValue(multiple ? [] : null);
    }, [multiple, setFieldValue]);

    const openModal = useCallback(() => {
      setPendingMap(new Map(selectedMap));
      setPendingKeys(Array.from(selectedMap.keys()));
      setOpen(true);
    }, [selectedMap]);

    useImperativeHandle(
      ref,
      () => ({
        openModal,
        closeModal: () => setOpen(false),
        clearSelection,
      }),
      [clearSelection, openModal],
    );

    useEffect(() => {
      const values = normalizeValue(watchedValue);

      if (!values.length) {
        setSelectedMap(new Map());
        return;
      }

      setSelectedMap((previous) => {
        const next = new Map<string, API.UserListDto>();

        values.forEach((item) => {
          const cached = previous.get(item.value);

          next.set(
            item.value,
            cached ??
              ({
                id: item.value,
                name: String(item.label),
              } as API.UserListDto),
          );
        });

        return next;
      });
    }, [watchedValue]);

    const handleConfirm = useCallback(() => {
      const users = Array.from(pendingMap.values());
      const values = users.map<SelectValue>((user) => ({
        value: user.id,
        label: getUserLabel(user),
      }));

      setFieldValue(multiple ? values : (values[0] ?? null));
      setSelectedMap(new Map(pendingMap));
      setOpen(false);
    }, [multiple, pendingMap, setFieldValue]);

    const handleSelectionChange = useCallback(
      (keys: Key[], rows: API.UserListDto[] = []) => {
        const nextKeys = keys.map((key) => String(key));

        if (!multiple) {
          const key = nextKeys[0];
          const user =
            rows[0] ??
            (key ? (pendingMap.get(key) ?? selectedMap.get(key)) : undefined);

          setPendingKeys(user ? [user.id] : []);
          setPendingMap(user ? new Map([[user.id, user]]) : new Map());
          return;
        }

        const nextMap = new Map<string, API.UserListDto>();

        nextKeys.forEach((key) => {
          const cached = pendingMap.get(key) ?? selectedMap.get(key);

          if (cached) {
            nextMap.set(key, cached);
          }
        });

        rows.forEach((user) => {
          nextMap.set(user.id, user);
        });

        if (maxCount && nextMap.size > maxCount) {
          return;
        }

        setPendingKeys(nextKeys);
        setPendingMap(nextMap);
      },
      [maxCount, multiple, pendingMap, selectedMap],
    );

    const columns = useMemo<ProColumns<API.UserListDto>[]>(
      () => [
        {
          title: <FormattedMessage id="user.label.name" />,
          dataIndex: 'name',
          width: 160,
          ellipsis: true,
          render: (_, user) => (
            <Space>
              {showAvatar && (
                <Avatar
                  size="small"
                  src={user.avatar}
                  icon={!user.avatar ? <UserOutlined /> : undefined}
                />
              )}
              <span>{user.name}</span>
            </Space>
          ),
        },
        {
          title: <FormattedMessage id="user.label.gender" />,
          dataIndex: 'gender',
          valueType: 'select',
          valueEnum: userGenderValueEnum,
          width: 120,
          render: (_, user) => {
            const option = userGenderValueEnum[user.gender];

            if (!option) {
              return '-';
            }

            return (
              <Tag icon={option.icon} color={option.color} variant="filled">
                {option.text}
              </Tag>
            );
          },
        },
        {
          title: <FormattedMessage id="user.label.email" />,
          dataIndex: 'email',
          width: 200,
          ellipsis: true,
        },
        {
          title: <FormattedMessage id="user.label.phoneNumber" />,
          dataIndex: 'phoneNumber',
          width: 180,
          ellipsis: true,
        },
        {
          title: <FormattedMessage id="user.label.departmentId" />,
          dataIndex: 'departmentId',
          search: false,
          ellipsis: true,
          render: (_, user) => user.department?.name ?? '-',
        },
      ],
      [showAvatar],
    );

    const selectFieldProps = useMemo<ProFormSelectProps['fieldProps']>(
      () => ({
        ...fieldProps,
        allowClear: true,
        labelInValue: true,
        maxCount,
        mode: multiple ? 'multiple' : undefined,
        open: false,
        suffixIcon: multiple ? <TeamOutlined /> : <UserOutlined />,
        onClear: clearSelection,
        onOpenChange: (nextOpen) => {
          fieldProps?.onOpenChange?.(nextOpen);

          if (nextOpen) {
            openModal();
          }
        },
        tagRender: (
          tagProps: GetProp<SelectProps, 'tagRender'> extends (
            props: infer P,
          ) => ReactNode
            ? P
            : never,
        ) => (
          <Tag
            closable={tagProps.closable}
            variant="filled"
            style={{ marginInlineEnd: 4 }}
            onClose={tagProps.onClose}
          >
            {tagProps.label}
          </Tag>
        ),
      }),
      [clearSelection, fieldProps, maxCount, multiple, openModal],
    );

    return (
      <>
        <ProFormSelect
          {...rest}
          fieldProps={selectFieldProps}
          transform={
            rest.transform ??
            ((value: unknown) => {
              if (typeof rest.name !== 'string') {
                return {};
              }

              const values = normalizeValue(value).map((item) => item.value);

              return {
                [rest.name]: multiple ? values : (values[0] ?? null),
              };
            })
          }
        />

        <Modal
          title={
            modalTitle ?? (
              <FormattedMessage id="component.userSelect.modal.title" />
            )
          }
          open={open}
          width={modalWidth}
          onOk={handleConfirm}
          onCancel={() => setOpen(false)}
          destroyOnHidden
          maskClosable={false}
          okButtonProps={{
            disabled: !multiple && pendingKeys.length === 0,
          }}
          styles={{
            body: {
              paddingTop: 8,
            },
          }}
        >
          <ProTable<API.UserListDto, API.SearchUsersParams>
            actionRef={actionRef}
            columns={columns}
            rowKey="id"
            search={{
              labelWidth: 'auto',
              style: { paddingBottom: 6 },
            }}
            request={async (params) => {
              const { success, data } = await searchUsers(params);

              return {
                data: data?.items ?? [],
                total: data?.total ?? 0,
                success: success ?? false,
              };
            }}
            rowSelection={{
              type: multiple ? 'checkbox' : 'radio',
              selectedRowKeys: pendingKeys,
              onChange: handleSelectionChange,
              preserveSelectedRowKeys: true,
              getCheckboxProps: (user) => ({
                disabled:
                  multiple &&
                  Boolean(maxCount) &&
                  pendingKeys.length >= Number(maxCount) &&
                  !pendingKeys.includes(user.id),
              }),
            }}
            pagination={{
              defaultPageSize: 20,
              showSizeChanger: true,
            }}
            scroll={{ y: 380, x: 'max-content' }}
            toolBarRender={false}
            options={false}
          />
        </Modal>
      </>
    );
  },
);

ProFormUserSelect.displayName = 'ProFormUserSelect';

export default ProFormUserSelect;
