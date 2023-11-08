import React, { useRef } from 'react';
import { message } from 'antd';
import { FormattedMessage, useIntl } from '@umijs/max';
import type { ProFormInstance } from '@ant-design/pro-components';
import { DrawerForm, ProFormText, ProFormTextArea } from '@ant-design/pro-components';
import { createRole, updateRole } from '@/services/role';

export type EditDrawerFormProps = {
    option: OptionType;
    values?: Role;
    visible: boolean;
    onVisibleChange: (visible: boolean) => void;
    onSuccess: (data?: Role) => void;
};

const EditDrawerForm: React.FC<EditDrawerFormProps> = (props) => {
    const intl = useIntl();
    const formRef = useRef<ProFormInstance>();

    const handleSubmit = async (values: Role) => {
        values.id = props.values?.id;
        values.concurrencyStamp = props.values?.concurrencyStamp;
        const { success, data, error } = props.option === 'create'
            ? await createRole(values)
            : await updateRole(values);
        if (success) {
            message.success(intl.formatMessage({ id: `common.message.success.${props.option}` }));
            props.onVisibleChange(false);
            props.onSuccess(data);
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
        title={intl.formatMessage({ id: `role.form.title.${props.option}` })}
        // layout={'horizontal'}
        width={400}
        // grid={true}
        // labelCol={{ span: 8 }}
        // wrapperCol={{ span: 16 }}
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
            required={true}
            disabled={props.option === 'update'}
            label={<FormattedMessage id="role.label.code" />}
            tooltip={<FormattedMessage id='role.tooltip.code' />}
            colProps={{ span: 24 }}
        />
        <ProFormText
            name="name"
            required={true}
            label={<FormattedMessage id="role.label.name" />}
            colProps={{ span: 24 }}
        />
        <ProFormTextArea
            name="description"
            label={<FormattedMessage id="role.label.description" />}
            colProps={{ span: 24 }}
            fieldProps={{
                showCount: true,
                maxLength: 100
            }}
        />
    </DrawerForm>);
}

export default EditDrawerForm;