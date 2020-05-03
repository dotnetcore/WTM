import { FormItem, InfoShellLayout } from 'components/dataView';
import { DesForm } from 'components/decorators';
import { WtmEditor, WtmDatePicker, WtmRangePicker, WtmSelect, WtmCheckbox, WtmRadio, WtmTransfer } from 'components/form';
import Request from 'utils/Request';
import * as React from 'react';
import lodash from 'lodash';
import { Button, Divider } from 'antd';
export default class extends React.Component<any, any> {
    render() {

        return (
            <div>
                <Divider>级联选择</Divider>
                <Select />
                <Divider>Checkbox</Divider>
                <Checkbox />
                <Divider>Radio</Divider>
                <Radio />
                <Divider>Transfer</Divider>
                <Transfer />
            </div>
        );
    }
}

@DesForm
class Select extends React.Component<any, any> {
    models: WTM.FormItem = {
        "province": {
            label: "province",
            rules: [],
            formItem: <WtmSelect
                dataSource={Request.cache({ url: "/mock/select" })}
                // 重置 子数据 使用 setFieldsValue 或者 resetFields
                onChange={event => {
                    const { resetFields } = this.props.form;
                    resetFields(['city', 'county'])
                }} />
        },
        "city": {
            label: "city",
            rules: [],
            formItem: <WtmSelect
                // 级联模型 配合 dataSource 函数 返回使用
                linkage={['province']}
                dataSource={(linkage) => {
                    const province = lodash.get(linkage, 'province');
                    if (province) {
                        return Request.cache({ url: "/mock/select?one=" + province })
                    }
                }}
                // 重置 子数据 使用 setFieldsValue 或者 resetFields
                onChange={event => {
                    const { resetFields } = this.props.form;
                    resetFields(['county'])
                }} />
        },
        "county": {
            label: "county",
            rules: [],
            formItem: <WtmSelect
                mode="multiple"
                // 级联模型  配合 dataSource 函数 返回使用
                linkage={['province', 'city']}
                dataSource={(linkage) => {
                    const province = lodash.get(linkage, 'province');
                    const city = lodash.get(linkage, 'city');
                    if (province && city) {
                        return Request.cache({ url: `/mock/select?one=${province}&two=${city}` })
                    }
                }} />
        },
    }
    onSubmit() {
        this.props.form.validateFields((err, values) => {
            console.log("TCL: App -> onSubmit -> values", values)
        });
    }
    render() {
        const props = {
            ...this.props,
            models: this.models,
        }
        return (
            <div>
                <div>
                    <Button onClick={this.onSubmit.bind(this)}>打印数据（控制台）</Button>
                </div>
                <InfoShellLayout>
                    <FormItem fieId="province" {...props} />
                    <FormItem fieId="city" {...props} />
                    <FormItem fieId="county" {...props} />
                </InfoShellLayout>
            </div>
        );
    }
}


@DesForm
class Radio extends React.Component<any, any> {
    models: WTM.FormItem = {
        "province": {
            label: "province",
            rules: [],
            formItem: <WtmRadio
                dataSource={Request.cache({ url: "/mock/select" })}
                // 重置 子数据 使用 setFieldsValue 或者 resetFields
                onChange={event => {
                    const { resetFields } = this.props.form;
                    resetFields(['city', 'county'])
                }} />
        },
        "city": {
            label: "city",
            rules: [],
            formItem: <WtmRadio
                // 级联模型 配合 dataSource 函数 返回使用
                linkage={['province']}
                dataSource={(linkage) => {
                    const province = lodash.get(linkage, 'province');
                    if (province) {
                        return Request.cache({ url: "/mock/select?one=" + province })
                    }
                }}
                // 重置 子数据 使用 setFieldsValue 或者 resetFields
                onChange={event => {
                    const { resetFields } = this.props.form;
                    resetFields(['county'])
                }} />
        },
        "county": {
            label: "county",
            rules: [],
            formItem: <WtmRadio
                // 级联模型  配合 dataSource 函数 返回使用
                linkage={['province', 'city']}
                dataSource={(linkage) => {
                    const province = lodash.get(linkage, 'province');
                    const city = lodash.get(linkage, 'city');
                    if (province && city) {
                        return Request.cache({ url: `/mock/select?one=${province}&two=${city}` })
                    }
                }} />
        },
    }
    onSubmit() {
        this.props.form.validateFields((err, values) => {
            console.log("TCL: App -> onSubmit -> values", values)
        });
    }
    render() {
        const props = {
            ...this.props,
            models: this.models,
        }
        return (
            <div>
                <div>
                    <Button onClick={this.onSubmit.bind(this)}>打印数据（控制台）</Button>
                </div>
                <InfoShellLayout>
                    <FormItem fieId="province" {...props} layout="row" />
                    <FormItem fieId="city" {...props} layout="row" />
                    <FormItem fieId="county" {...props} layout="row" />
                </InfoShellLayout>
            </div>
        );
    }
}


@DesForm
class Checkbox extends React.Component<any, any> {
    models: WTM.FormItem = {
        "province": {
            label: "province",
            rules: [],
            formItem: <WtmCheckbox
                dataSource={Request.cache({ url: "/mock/select" })}
                // 重置 子数据 使用 setFieldsValue 或者 resetFields
                onChange={event => {
                    const { resetFields } = this.props.form;
                    resetFields(['city', 'county'])
                }} />
        },
        "city": {
            label: "city",
            rules: [],
            formItem: <WtmCheckbox
                // 级联模型 配合 dataSource 函数 返回使用
                linkage={['province']}
                dataSource={(linkage) => {
                    const province = lodash.get(linkage, 'province');
                    if (province) {
                        return Request.cache({ url: "/mock/select?one=" + province })
                    }
                }}
                // 重置 子数据 使用 setFieldsValue 或者 resetFields
                onChange={event => {
                    const { resetFields } = this.props.form;
                    resetFields(['county'])
                }} />
        },
        "county": {
            label: "county",
            rules: [],
            formItem: <WtmCheckbox
                // 级联模型  配合 dataSource 函数 返回使用
                linkage={['province', 'city']}
                dataSource={(linkage) => {
                    const province = lodash.get(linkage, 'province');
                    const city = lodash.get(linkage, 'city');
                    if (province && city) {
                        return Request.cache({ url: `/mock/select?one=${province}&two=${city}` })
                    }
                }} />
        },
    }
    onSubmit() {
        this.props.form.validateFields((err, values) => {
            console.log("TCL: App -> onSubmit -> values", values)
        });
    }
    render() {
        const props = {
            ...this.props,
            models: this.models,
        }
        return (
            <div>
                <div>
                    <Button onClick={this.onSubmit.bind(this)}>打印数据（控制台）</Button>
                </div>
                <InfoShellLayout>
                    <FormItem fieId="province" {...props} layout="row" />
                    <FormItem fieId="city" {...props} layout="row" />
                    <FormItem fieId="county" {...props} layout="row" />
                </InfoShellLayout>
            </div>
        );
    }
}

@DesForm
class Transfer extends React.Component<any, any> {
    models: WTM.FormItem = {
        "province": {
            label: "province",
            rules: [],
            formItem: <WtmTransfer
                listStyle={undefined}
                dataSource={Request.cache({ url: "/mock/select" })}
                // 重置 子数据 使用 setFieldsValue 或者 resetFields
                onChange={event => {
                    const { resetFields } = this.props.form;
                    resetFields(['city', 'county'])
                }} />
        },
        "city": {
            label: "city",
            rules: [],
            formItem: <WtmTransfer
                listStyle={undefined}
                // 级联模型 配合 dataSource 函数 返回使用
                linkage={['province']}
                dataSource={(linkage) => {
                    const province = lodash.get(linkage, 'province');
                    if (province) {
                        return Request.cache({ url: "/mock/select?one=" + province })
                    }
                }}
                // 重置 子数据 使用 setFieldsValue 或者 resetFields
                onChange={event => {
                    const { resetFields } = this.props.form;
                    resetFields(['county'])
                }} />
        },
        "county": {
            label: "county",
            rules: [],
            formItem: <WtmTransfer
                listStyle={undefined}
                // 级联模型  配合 dataSource 函数 返回使用
                linkage={['province', 'city']}
                dataSource={(linkage) => {
                    const province = lodash.get(linkage, 'province');
                    const city = lodash.get(linkage, 'city');
                    if (province && city) {
                        return Request.cache({ url: `/mock/select?one=${province}&two=${city}` })
                    }
                }} />
        },
    }
    onSubmit() {
        this.props.form.validateFields((err, values) => {
            console.log("TCL: App -> onSubmit -> values", values)
        });
    }
    render() {
        const props = {
            ...this.props,
            models: this.models,
        }
        return (
            <div>
                <div>
                    <Button onClick={this.onSubmit.bind(this)}>打印数据（控制台）</Button>
                </div>
                <InfoShellLayout>
                    <FormItem fieId="province" {...props} layout="row" />
                    <FormItem fieId="city" {...props} layout="row" />
                    <FormItem fieId="county" {...props} layout="row" />
                </InfoShellLayout>
            </div>
        );
    }
}
