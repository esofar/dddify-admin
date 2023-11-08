import React, { useRef } from 'react';
import { message, Alert, Space } from 'antd';
import { FormattedMessage, useIntl } from '@umijs/max';
import { ProForm, DrawerForm, ProFormText, ProFormSelect, ProFormDatePicker, ProFormTreeSelect, ProFormSwitch } from '@ant-design/pro-components';
import type { ProFormInstance, ProSchemaValueEnumObj } from '@ant-design/pro-components';
import { createUser, updateUser } from '@/services/user';
import { getOrganizations } from '@/services/organization';
import moment from 'moment';

export type EditDrawerFormProps = {
    option: OptionType;
    values?: User;
    visible: boolean;
    genderEnum?: ProSchemaValueEnumObj;
    statusEnum?: ProSchemaValueEnumObj;
    onVisibleChange: (visible: boolean) => void;
    onSuccess: (data?: User) => void;
};

const EditDrawerForm: React.FC<EditDrawerFormProps> = (props) => {
    const intl = useIntl();
    const formRef = useRef<ProFormInstance>();

    const handleSubmit = async (values: User) => {
        values.id = props.values?.id || '';
        values.concurrencyStamp = props.values?.concurrencyStamp;

        const { success, data, error } = props.option === 'create'
            ? await createUser(values)
            : await updateUser(values);

        if (success) {
            message.success(intl.formatMessage({ id: `common.message.success.${props.option}` }));
            props.onVisibleChange(false);
            props.onSuccess(data);
        } else {
            const fieldsValue = formRef.current?.getFieldsValue();
            const fieldsss = [];
            for (const key in fieldsValue) {
                fieldsss.push({ name: key, errors: [] });
            }
            formRef.current?.setFields(fieldsss);
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

    return (<>
        <DrawerForm
            formRef={formRef}
            title={intl.formatMessage({ id: `user.form.title.${props.option}` })}
            layout={'horizontal'}
            grid={true}
            width={600}
            labelCol={{ span: 24 }}
            wrapperCol={{ span: 24 }}
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
                name="fullName"
                required={true}
                label={<FormattedMessage id="user.label.fullName" />}
                colProps={{ span: 12 }}
            />
            <ProFormText
                name="nickName"
                label={<FormattedMessage id="user.label.nickName" />}
                colProps={{ span: 12 }}
            />
            <ProFormSelect
                name='gender'
                required={true}
                label={<FormattedMessage id='user.label.gender' />}
                colProps={{ span: 12 }}
                valueEnum={props.genderEnum}
                convertValue={(value) => value != undefined ? `${value}` : value}
                transform={(value, namePath) => {
                    return {
                        [namePath]: JSON.parse(value)
                    }
                }}
            />
            <ProFormDatePicker
                name='birthDate'
                required={true}
                label={<FormattedMessage id='user.label.birthDate' />}
                colProps={{ span: 12 }}
                width={'md'}
                transform={(value, namePath) => {
                    const valueFormat = moment(value).format('YYYY-MM-DD');
                    return {
                        [namePath]: valueFormat
                    }
                }}
            />
            <ProFormText
                name="email"
                required={true}
                label={<FormattedMessage id="user.label.email" />}
                colProps={{ span: 12 }}
            />
            <ProFormText
                name="phoneNumber"
                required={true}
                label={<FormattedMessage id="user.label.phoneNumber" />}
                colProps={{ span: 12 }}
            />
            <ProFormTreeSelect
                name='organizationId'
                required={true}
                placeholder={intl.formatMessage({ id: `common.form.treeselect.placeholder` })}
                label={<FormattedMessage id='user.label.organization' />}
                colProps={{ span: 12 }}
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
            {/* <ProFormSwitch
                name="isEnabled"
                required={true}
                label={<FormattedMessage id='user.label.status' />}
                checkedChildren={<FormattedMessage id='user.enum.status.enabled' />}
                unCheckedChildren={<FormattedMessage id='user.enum.status.disabled' />}
                colProps={{ span: 12 }}
            /> */}
            <ProForm.Group
                colProps={{ span: 24 }}
            >
                {props.option == 'create' ? (<Space direction="vertical" style={{ width: '100%' }}>
                    <Alert message={<FormattedMessage id="user.alert.password" />} type="info" showIcon closable />
                </Space>) : ''}
            </ProForm.Group>
        </DrawerForm>
    </>);
}

export default EditDrawerForm;