import React, { useRef } from 'react';
import { message } from 'antd';
import { FormattedMessage, useIntl } from '@umijs/max';
import type { ProFormInstance } from '@ant-design/pro-components';
import { DrawerForm, ProFormText, ProFormTreeSelect, ProFormSelect, ProFormDigit, ProFormSwitch } from '@ant-design/pro-components';
import { getOrganizations, createOrganization, updateOrganization } from '@/services/organization';

export type EditDrawerFormProps = {
    option: OptionType;
    values?: Organization;
    visible: boolean;
    typeOptions?: { value: string, label: string }[];
    onVisibleChange: (visible: boolean) => void;
    onSuccess: (data?: Organization) => void;
};

const EditDrawerForm: React.FC<EditDrawerFormProps> = (props) => {
    const intl = useIntl();
    const formRef = useRef<ProFormInstance>();

    const handleSubmit = async (values: Organization) => {
        values.id = props.values?.id;
        values.concurrencyStamp = props.values?.concurrencyStamp;
        const { success, data, error } = props.option === 'create'
            ? await createOrganization(values)
            : await updateOrganization(values);
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

    const organizations = async (params: { keyWords?: string }) => {
        const { success, data } = await getOrganizations({ name: params.keyWords });
        return success ? (data as Organization[]).map(org => {
            return { id: org.id, pId: org.parentId, name: org.name, disabled: !org.isEnabled };
        }) : [];
    };

    return (<DrawerForm
        formRef={formRef}
        title={intl.formatMessage({ id: `organization.form.title.${props.option}` })}
        layout={'horizontal'}
        grid={true}
        width={400}
        labelCol={{ span: 8 }}
        wrapperCol={{ span: 16 }}
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
            name='name'
            required={true}
            label={<FormattedMessage id='organization.label.name' />}
            colProps={{ span: 24 }}
        />
        <ProFormTreeSelect
            name='parentId'
            placeholder={intl.formatMessage({ id: `common.form.treeselect.placeholder` })}
            label={<FormattedMessage id='organization.label.parentId' />}
            tooltip={<FormattedMessage id='organization.tooltip.parentId' />}
            colProps={{ span: 24 }}
            request={organizations}
            fieldProps={{
                allowClear: true,
                treeDataSimpleMode: true,
                treeDefaultExpandAll: true,
                showSearch: false,
                treeLine: {
                    showLeafIcon: false
                },
                fieldNames: {
                    label: 'name',
                    value: 'id',
                },
            }}
        />
        <ProFormSelect
            name='type'
            label={<FormattedMessage id='organization.label.type' />}
            colProps={{ span: 24 }}
            fieldProps={{ options: props.typeOptions}}
        />
        <ProFormDigit
            name='order'
            label={<FormattedMessage id='organization.label.order' />}
            colProps={{ span: 24 }}
            min={1}
            fieldProps={{ precision: 0 }}
        />
        <ProFormSwitch
            name="isEnabled"
            label={<FormattedMessage id='organization.label.status' />}
            checkedChildren={<FormattedMessage id='organization.label.status.enabled' />}
            unCheckedChildren={<FormattedMessage id='organization.label.status.disabled' />}
            colProps={{ span: 24 }}
            fieldProps={{
                defaultChecked: false
            }}
            transform={(value, namePath) => {
                return {
                    [namePath]: JSON.parse(value)
                }
            }}
        />
    </DrawerForm>);
}

export default EditDrawerForm;