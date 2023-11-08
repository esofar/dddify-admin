import React, { useEffect, useState } from 'react';
import { message, Button, Popconfirm, Space } from 'antd';
import { FormattedMessage, useIntl } from '@umijs/max';
import { PlusOutlined, EditOutlined, DeleteOutlined, QuestionCircleOutlined, MenuOutlined } from '@ant-design/icons';
import type { ProColumns } from '@ant-design/pro-components';
import { DragSortTable } from '@ant-design/pro-components';
import { getDictionaryItems, deleteDictionaryItem, sortDictionaryItems } from '@/services/dictionary';
import EditItemDrawerForm from "./EditItemDrawerForm";

export type DictionaryItemListProps = {
    dictionary?: Dictionary;
};

const DictionaryItemList: React.FC<DictionaryItemListProps> = (props) => {
    const { dictionary } = props;

    const [dictionaryItems, setDictionaryItems] = useState<DictionaryItem[]>([]);
    const [editModalVisible, setEditModalVisible] = useState<boolean>(false);
    const [itemListLoading, setItemListLoading] = useState<boolean>(false);
    const [currentRow, setCurrentRow] = useState<DictionaryItem>();
    const [currentOption, setCurrentOption] = useState<OptionType>('create');

    const intl = useIntl();

    const reloadItems = (dictionaryId: string) => {
        setItemListLoading(true);
        getDictionaryItems(dictionaryId).then(({ success, data }) => {
            if (success) {
                setDictionaryItems(data as DictionaryItem[]);
                setItemListLoading(false);
            }
        });
    }

    const handleDelete = async (id: string) => {
        const { success } = await deleteDictionaryItem(id, dictionary?.id);
        if (success) {
            if (dictionary?.id) {
                reloadItems(dictionary.id);
            }
            message.success(intl.formatMessage({ id: `common.message.success.delete` }));
        }
    };

    const handleDragSortEnd = async (sortedDictionaryItems: DictionaryItem[]) => {
        const orderedIds = sortedDictionaryItems.map(item => item.id);
        const { success } = await sortDictionaryItems(dictionary?.id, orderedIds);
        if (success) {
            setDictionaryItems(sortedDictionaryItems);
            message.success(intl.formatMessage({ id: `common.message.success.sort` }));
        }
    };

    useEffect(() => {
        if (dictionary?.id) {
            reloadItems(dictionary.id);
        }
    }, [dictionary?.id]);

    const columns: ProColumns<DictionaryItem>[] = [
        {
            title: '排序',
            dataIndex: 'sort',
            width: 60,
        },
        {
            title: <FormattedMessage id='dictionaryItem.label.name' />,
            dataIndex: 'name',
            width: '50%',
            ellipsis: true,
        },
        {
            title: <FormattedMessage id='dictionaryItem.label.code' />,
            dataIndex: 'code',
            width: '50%',
            ellipsis: true,
        },
        {
            title: <FormattedMessage id="dictionaryItem.label.option" />,
            dataIndex: 'option',
            valueType: 'option',
            width: 140,
            render: (_, row) => [
                <a
                    key="update"
                    onClick={() => {
                        setCurrentRow(row);
                        setCurrentOption('update');
                        setEditModalVisible(true);
                    }}
                >
                    <Space>
                        <EditOutlined />
                        <FormattedMessage id='common.button.update' />
                    </Space>
                </a>,
                <Popconfirm
                    key={'delete'}
                    title={<FormattedMessage id='common.confirm.title.delete' />}
                    onConfirm={async () => {
                        await handleDelete(row.id);
                    }}
                    icon={<QuestionCircleOutlined />}>
                    <a>
                    <Space>
                        <DeleteOutlined />
                        <FormattedMessage id='common.button.delete' />
                    </Space>
                    </a>
                </Popconfirm>,
            ],
        },
    ];

    return (<>
        <DragSortTable
            headerTitle={dictionary?.name}
            columns={columns}
            rowKey='id'
            dragSortKey='sort'
            search={false}
            pagination={false}
            loading={itemListLoading}
            dataSource={dictionaryItems}
            dragSortHandlerRender={() => <MenuOutlined style={{ cursor: 'grab' }} />}
            onDragSortEnd={handleDragSortEnd}
            toolBarRender={() => [
                <Button
                    disabled={dictionary === undefined}
                    type='dashed'
                    key='create'
                    onClick={() => {
                        setCurrentOption('create');
                        setEditModalVisible(true);
                    }}
                >
                    <PlusOutlined /> <FormattedMessage id='common.button.create' />
                </Button>
            ]}
        />
        <EditItemDrawerForm
            option={currentOption}
            dictionaryId={dictionary?.id || ''}
            values={currentRow}
            visible={editModalVisible}
            onVisibleChange={setEditModalVisible}
            onSuccess={() => {
                if (dictionary) {
                    reloadItems(dictionary.id);
                }
            }}
        />
    </>);
}

export default DictionaryItemList;