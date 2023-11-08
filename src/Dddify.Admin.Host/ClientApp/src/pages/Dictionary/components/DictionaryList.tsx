import React, { useRef, useState } from 'react';
import { message, Button, Popconfirm, Tooltip } from 'antd';
import { FormattedMessage, useIntl } from '@umijs/max';
import { PlusOutlined, EditOutlined, DeleteOutlined, QuestionCircleOutlined } from '@ant-design/icons';
import type { ActionType, ProColumns } from '@ant-design/pro-components';
import { ProTable } from '@ant-design/pro-components';
import { getDictionaries, deleteDictionary } from '@/services/dictionary';
import EditDrawerForm from "./EditDrawerForm";

export type DictionaryListProps = {
    onChange: (dictionary?: Dictionary) => void;
};

const DictionaryList: React.FC<DictionaryListProps> = (props) => {
    const { onChange } = props;

    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [currentRow, setCurrentRow] = useState<Dictionary>();
    const [currentOption, setCurrentOption] = useState<OptionType>('create');

    const intl = useIntl();
    const actionRef = useRef<ActionType>();

    const handleDelete = async (id?: string) => {
        const { success } = await deleteDictionary(id);
        if (success) {
            actionRef.current?.reload();
            message.success(intl.formatMessage({ id: `common.message.success.delete` }));
        }
    };

    const columns: ProColumns<Dictionary>[] = [
        {
            title: <FormattedMessage id='dictionary.label.index' />,
            dataIndex: 'index',
            width: 54,
            valueType: 'index',
        },
        {
            title: <FormattedMessage id='dictionary.label.name' />,
            dataIndex: 'name',
            width: '50%',
            ellipsis: true,
            render: (_, row) => [
                <>
                    <Tooltip placement="right" title={row.description}>
                        <Button
                            type="link"
                            size='small'
                            onClick={() => onChange(row)}>
                            {row.name}
                        </Button>
                    </Tooltip>
                </>
            ]
        },
        {
            title: <FormattedMessage id='dictionary.label.code' />,
            dataIndex: 'code',
            width: '50%',
            copyable: true,
            ellipsis: true,
        },
        {
            title: <FormattedMessage id="dictionary.label.option" />,
            dataIndex: 'option',
            valueType: 'option',
            width: 140,
            render: (_, row) => [
                <a
                    key='update'
                    onClick={() => {
                        setCurrentRow(row);
                        setCurrentOption('update');
                        setEditModalVisible(true);
                    }}
                >
                    <EditOutlined /> <FormattedMessage id='common.button.update' />
                </a>,
                <Popconfirm
                    key={'delete'}
                    title={<FormattedMessage id='common.confirm.title.delete' />}
                    onConfirm={async () => {
                        await handleDelete(row.id);
                    }}
                    icon={<QuestionCircleOutlined />}>
                    <a>
                        <DeleteOutlined /> <FormattedMessage id='common.button.delete' />
                    </a>
                </Popconfirm>,
            ],
        },
    ];

    return (<>
        <ProTable<Dictionary>
            actionRef={actionRef}
            columns={columns}
            rowKey='id'
            search={{
                filterType: 'light',
            }}
            request={async (params) => {
                const { success, data } = await getDictionaries(params);
                return {
                    data: data.items,
                    total: data.total,
                    success,
                };
            }}
            headerTitle={
                <Button
                    type='primary'
                    key='primary'
                    onClick={() => {
                        setCurrentOption('create');
                        setEditModalVisible(true);
                    }}
                >
                    <PlusOutlined /> <FormattedMessage id='common.button.create' />
                </Button>
            }
        />
        <EditDrawerForm
            option={currentOption}
            values={currentRow}
            visible={editModalVisible}
            onVisibleChange={setEditModalVisible}
            onSuccess={() => {
                actionRef.current?.reload();
            }}
        />
    </>);
}

export default DictionaryList;