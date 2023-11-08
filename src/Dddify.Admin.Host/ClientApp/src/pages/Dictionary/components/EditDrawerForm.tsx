import React, { useRef } from 'react';
import { message } from 'antd';
import { FormattedMessage, useIntl } from '@umijs/max';
import type { ProFormInstance } from '@ant-design/pro-components';
import { DrawerForm, ProFormText, ProFormTextArea } from '@ant-design/pro-components';
import { createDictionary, updateDictionary } from '@/services/dictionary';

export type Option = 'create' | 'update';

export type EditDrawerFormProps = {
    option: OptionType;
    values?: Dictionary;
    visible: boolean;
    onVisibleChange: (visible: boolean) => void;
    onSuccess: (data?: Dictionary) => void;
};

const EditDrawerForm: React.FC<EditDrawerFormProps> = (props) => {
    const intl = useIntl();
    const formRef = useRef<ProFormInstance>();

    const handleSubmit = async (values: Dictionary) => {
        values.id = props.values?.id || '';
        values.concurrencyStamp = props.values?.concurrencyStamp;
        const { success, error } = props.option === 'create'
            ? await createDictionary(values)
            : await updateDictionary(values);
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
        title={intl.formatMessage({ id: `dictionary.form.title.${props.option}` })}
        width={600}
        grid={true}
        rowProps={{
            gutter: [16, 0],
        }}
        drawerProps={{
            closable: false,
            destroyOnClose: true,
        }}
        visible={props.visible}
        onVisibleChange={props.onVisibleChange}
        initialValues={props.option === 'update' ? props.values : undefined}
        onFinish={handleSubmit}
    >
        <ProFormText
            name="code"
            disabled={props.option === 'update'}
            label={<FormattedMessage id="dictionary.label.code" />}
            tooltip={<FormattedMessage id='dictionary.tooltip.code' />}
            colProps={{ span: 12 }}
            fieldProps={{
                showCount: true,
                maxLength: 50
            }}
            rules={[
                {
                    required: true,
                    message: <FormattedMessage id="dictionary.form.rules.code.required" />,
                },
            ]}
        />
        <ProFormText
            name="name"
            label={<FormattedMessage id="dictionary.label.name" />}
            colProps={{ span: 12 }}
            fieldProps={{
                showCount: true,
                maxLength: 50
            }}
            rules={[
                {
                    required: true,
                    message: <FormattedMessage id="dictionary.form.rules.name.required" />,
                },
            ]}
        />
        <ProFormTextArea
            name="description"
            label={<FormattedMessage id="dictionary.label.description" />}
            colProps={{ span: 24 }}
            fieldProps={{
                showCount: true,
                maxLength: 200
            }}
        />
    </DrawerForm>);
}

export default EditDrawerForm;