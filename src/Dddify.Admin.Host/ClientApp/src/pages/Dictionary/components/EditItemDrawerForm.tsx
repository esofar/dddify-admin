import React, { useRef } from 'react';
import { message } from 'antd';
import { FormattedMessage, useIntl } from '@umijs/max';
import type { ProFormInstance } from '@ant-design/pro-components';
import { DrawerForm, ProFormText, } from '@ant-design/pro-components';
import { createDictionaryItem, updateDictionaryItem } from '@/services/dictionary';

export type Option = 'create' | 'update';

export type EditItemModalFormProps = {
    option: OptionType;
    dictionaryId: string;
    values?: DictionaryItem | undefined;
    visible: boolean;
    onVisibleChange: (visible: boolean) => void;
    onSuccess: (data?: DictionaryItem) => void;
};

const EditItemModalForm: React.FC<EditItemModalFormProps> = (props) => {
    const { dictionaryId } = props;

    const intl = useIntl();
    const formRef = useRef<ProFormInstance>();

    const handleSubmit = async (values: DictionaryItem) => {
        values.id = props.values?.id || '';
        values.concurrencyStamp = props.values?.concurrencyStamp;
        const { success, error } = props.option === 'create'
            ? await createDictionaryItem(dictionaryId, values)
            : await updateDictionaryItem(dictionaryId, values);
        if (success) {
            message.success(intl.formatMessage({ id: `common.message.success.${props.option}` }));
            props.onVisibleChange(false);
            props.onSuccess(values);
        } else {
            if (error) {
                const fields = [];
                for (const key in error) {
                    fields.push({ name: key, errors: error[key] });
                }
                formRef.current?.setFields(fields);
            }
        }
    };

    return (<DrawerForm
        formRef={formRef}
        title={intl.formatMessage({ id: `dictionaryItem.form.title.${props.option}` })}
        width={350}
        drawerProps={{
            closable: false,
            destroyOnClose: true,
        }}
        open={props.visible}
        onOpenChange={props.onVisibleChange}
        initialValues={props.option === 'update' ? props.values : undefined}
        onFinish={handleSubmit}
    >
        <ProFormText
            name="code"
            disabled={props.option === 'update'}
            label={<FormattedMessage id="dictionaryItem.label.code" />}
            tooltip={<FormattedMessage id='dictionaryItem.tooltip.code' />}
            colProps={{ span: 12 }}
            fieldProps={{
                showCount: true,
                maxLength: 50
            }}
            rules={[
                {
                    required: true,
                    message: <FormattedMessage id="dictionaryItem.form.rules.code.required" />,
                },
            ]}
        />
        <ProFormText
            name="content"
            label={<FormattedMessage id="dictionaryItem.label.name" />}
            colProps={{ span: 12 }}
            fieldProps={{
                showCount: true,
                maxLength: 50
            }}
            rules={[
                {
                    required: true,
                    message: <FormattedMessage id="dictionaryItem.form.rules.name.required" />,
                },
            ]}
        />
    </DrawerForm>);
}

export default EditItemModalForm;