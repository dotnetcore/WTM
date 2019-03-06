import { Button, Divider, message, Spin } from 'antd';
import { DrawerProps } from 'antd/lib/drawer';
import Form, { WrappedFormUtils } from 'antd/lib/form/Form';
import { ModalProps } from 'antd/lib/modal';
import globalConfig from 'global.config';
import lodash from 'lodash';
import * as React from 'react';
import { DesForm } from '../../../decorators'; //错误
import { InfoShell } from '../infoShell';
import { Help } from 'utils/Help';
declare type Props = {
    form?: WrappedFormUtils;
    InfoShell?: DrawerProps | ModalProps;
    /** 标题 */
    title?: React.ReactNode;
    /** 按钮可用状态 */
    disabled?: boolean;
    /** 显示 提交按钮 */
    showSubmit?: boolean;
    /** 关闭按钮 文案 */
    closeText?: string;
    /** 提交按钮 文案 */
    submitText?: string;
    /** 图标 */
    icon?: string;
    /** 加载 */
    loading?: boolean;
    /**
     * 按钮类型
     */
    type?: 'button' | 'a';
    /**
     * 表单提交 方法     */
    onFormSubmit?: (values, onClose?) => Promise<boolean> | void | boolean;
    [key: string]: any
};
export class DialogForm extends React.Component<Props, any> {
    shouldComponentUpdate(nextProps, nextState, nextContext) {
        return !lodash.eq(this.state, nextState) || !lodash.eq(this.props.disabled, nextProps.disabled)
    }
    key = Date.now()
    state = {
        //显示
        visible: false,
        //初始化装载  按钮 第一次 点击 显示 为 已装载 优化性能
        load: false,
    }
    onVisible(visible = !this.state.visible) {
        this.setState(state => {
            state.visible = visible;
            if (!state.load) {
                state.load = true;
            }
            return state
        })
    }
    render() {
        const option = {
            title: lodash.get(this.props, 'title', '未设置标题'),
            disabled: lodash.get(this.props, "disabled", false),
            showSubmit: lodash.get(this.props, 'showSubmit', true),
            closeText: lodash.get(this.props, 'closeText', '关闭'),
            submitText: lodash.get(this.props, 'submitText', '提交'),
            icon: lodash.get(this.props, 'icon'),
            type: lodash.get(this.props, 'type', 'button'),
        }
        const button = option.type === "button" ? <Button
            icon={option.icon}
            disabled={option.disabled}
            onClick={this.onVisible.bind(this, true)}>{option.title}</Button> :
            <a onClick={this.onVisible.bind(this, true)} >{option.title}</a>;
        return (
            <React.Fragment key={this.key}>
                {button}
                {this.state.load && <Optimization option={option} visible={this.state.visible} onVisible={this.onVisible.bind(this)}>
                    {this.props.children}
                </Optimization>}
            </React.Fragment>
        );
    }
}
@DesForm
class Optimization extends React.Component<{
    option: any;
    visible: any;
    onVisible: any;
    onFormSubmit?: any;
    form?: WrappedFormUtils;
}, any> {
    constructor(props) {
        super(props)
        if (globalConfig.development && this.props.visible) {
            console.log('装载成功', this.props.children)
        }
    }
    // shouldComponentUpdate(nextProps, nextState, nextContext) {

    //     return true
    // }
    state = { loading: false }
    onVisible(visible) {
        this.props.onVisible(visible)
    }
    onSubmit(e) {
        e.stopPropagation();
        e.preventDefault();
        const onFormSubmit = this.props.onFormSubmit || lodash.get(this.props.children, 'type.onFormSubmit');
        if (lodash.isFunction(onFormSubmit)) {
            // debugger
            // 加载中 返回。
            if (this.state.loading) return;
            try {
                this.props.form.validateFields((err, values) => {
                    if (!err) {
                        this.setState({ loading: true }, async () => {
                            try {
                                const callbackValue = onFormSubmit(values, this.onVisible.bind(this, false));
                                let res = false;
                                if (lodash.isBoolean(callbackValue)) {
                                    res = callbackValue;
                                } else if (callbackValue) {
                                    res = await callbackValue
                                }
                                if (res) {
                                    this.onVisible(false)
                                }
                            } catch (error) {
                                this.onSetErrorMsg(error || {})
                                // onErrorMessage("操作失败", lodash.map(error, (value, key) => ({ value, key })))
                                console.error(error)
                            } finally {
                                this.setState({ loading: false })
                            }
                        });
                    } else {
                        console.error(err)
                        this.setState({ loading: false })
                    }
                });
            } catch (error) {
                console.error(error)
            }
        } else {
            globalConfig.development && message.warning("为配置 onFormSubmit 函数")
            // console.log("没有")
        }
    }
    onSetErrorMsg(errors = { Entity: { "Name": "姓名是必填项" } }) {
        const { setFields } = this.props.form;
        setFields(lodash.mapValues(errors.Entity, data => {
            return {
                errors: [new Error(data)]
            }
        }))
        // [fieldName]: { value: any, errors: [Error] }
    }
    render() {
        const { option, visible } = this.props;
        const children = React.cloneElement(this.props.children as any, { form: this.props.form }, null);

        return (
            <InfoShell
                title={option.title}
                visible={visible}
                onCancel={this.onVisible.bind(this, false)}
                {...this.props}
            >
                <Form onSubmit={this.onSubmit.bind(this)}>
                    <Spin tip="Loading..." spinning={this.state.loading} >
                        {children}
                    </Spin>
                    <div className="data-view-form-btns" >
                        <Button onClick={this.onVisible.bind(this, false)} > {option.closeText} </Button>
                        {option.showSubmit && <>
                            <Divider type="vertical" />
                            <Button loading={this.state.loading} type="primary" htmlType="submit"  >{option.submitText} </Button>
                        </>}
                    </div>
                </Form>
            </InfoShell>
        );
    }
}
export function DialogFormDes(params: {
    onFormSubmit: (values) => Promise<boolean>,
    onLoadData?: (values, props) => Promise<boolean>,
}) {
    return (Component: React.ComponentClass<any, any>) => {
        const DFC: any = class extends React.PureComponent<any, any> {
            constructor(props) {
                super(props)
            }
            state = {
                ...this.state,
                __spinning: this.props.LoadData && lodash.isFunction(params.onLoadData) || false,
                __details: {},
                __key: Help.GUID()
            }
            async componentDidMount() {
                if (lodash.isFunction(params.onLoadData)) {
                    let loadData = lodash.isFunction(this.props.loadData) ? this.props.loadData() : this.props.loadData;
                    const res = await params.onLoadData.bind(this)(loadData, this.props);
                    this.setState({ __details: res, __spinning: false, __key: Help.GUID() })
                }
                super.componentDidMount && super.componentDidMount()
            }
            render() {
                const { __spinning, __details, __key } = this.state;
                return <Spin tip="Loading..." spinning={__spinning} key={__key}>
                    <Component {...this.props} defaultValues={__details} />
                </Spin>
            }
        }
        lodash.set(Component, 'onFormSubmit', params.onFormSubmit)
        lodash.set(Component, 'onLoadData', params.onLoadData)
        lodash.set(DFC, 'onFormSubmit', params.onFormSubmit)
        lodash.set(DFC, 'onLoadData', params.onLoadData)
        return DFC
    }
}
export function DialogFormSubmit(params?: any) {
    return function (target: any, propertyKey: string, descriptor) {
        // const newDes = {
        //     configurable: true,
        //     enumerable: false,
        //     get() {
        //         return ((params) => {
        //             descriptor.value.bind(this)(params)(this);
        //         }) as any;
        //     },
        // }
        // console.log(newDes)
        /**
         * 设置原型中的 方法
         */
        lodash.set(target.constructor, 'onFormSubmit', descriptor.value.bind(target))
        return descriptor
    }
}
export function DialogLoadData(params?: any) {
    return function (target: any, propertyKey: string, descriptor) {
        /**
         * 设置原型中的 方法
         */
        lodash.set(target.constructor, 'onLoadData', descriptor.value.bind(target))
        return descriptor
    }
}