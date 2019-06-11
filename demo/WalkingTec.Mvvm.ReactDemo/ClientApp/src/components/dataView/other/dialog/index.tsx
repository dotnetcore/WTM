/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-03-08 02:36:43
 * @modify date 2019-03-08 02:36:43
 * @desc [description]
 */
import { Button, Divider, message, notification, Skeleton, Spin, Icon } from 'antd';
import Form, { WrappedFormUtils } from 'antd/lib/form/Form';
import globalConfig from 'global.config';
import lodash from 'lodash';
import { Debounce } from 'lodash-decorators';
import * as React from 'react';
import { Help } from 'utils/Help';
import { DesError } from 'components/decorators';

import { InfoShell } from '../infoShell';
declare type Props = {
    form?: WrappedFormUtils;
    // InfoShell?: DrawerProps | ModalProps;
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
    /** 宽度*/
    width?: string | number;
    /** 自定义按钮 */
    button?: (props: {
        /** form 提交 事件 */
        onSubmit: () => void;
        /** 关闭窗口 事件 */
        onCancel: () => void;
    }) => React.ReactNode;
    /**ß
     * 按钮类型ß
     */
    type?: 'button' | 'a';
    /**
     * 表单提交 方法     */
    onFormSubmit?: (values, onClose?) => Promise<boolean> | void | boolean;
    [key: string]: any
};
@DesError
export class DialogForm extends React.Component<Props, any> {
    shouldComponentUpdate(nextProps, nextState, nextContext) {
        if (!lodash.isEqual(this.props.disabled, nextProps.disabled)) {
            return true;
        }
        if (nextState.load) {
            return !lodash.eq(this.state, nextState);
        }
        return false
    }
    key = Date.now()
    state = {
        //显示
        visible: false,
        //初始化装载  按钮 第一次 点击 显示 为 已装载 优化性能
        load: false,
    }
    @Debounce(100)
    onVisible(visible = !this.state.visible) {
        this.setState(state => {
            state.visible = visible;
            if (!state.load) {
                state.load = true;
                this.Optimization = Form.create()(Optimization);
            }
            return { ...state }
        })
    }
    Optimization;//= Form.create()(Optimization);
    render() {
        const option = {
            title: lodash.get(this.props, 'title', '未设置标题'),
            disabled: lodash.get(this.props, "disabled", false),
            showSubmit: lodash.get(this.props, 'showSubmit', true),
            closeText: lodash.get(this.props, 'closeText', '关闭'),
            submitText: lodash.get(this.props, 'submitText', '提交'),
            icon: lodash.get(this.props, 'icon'),
            type: lodash.get(this.props, 'type', 'button'),
            button: lodash.get(this.props, 'button'),
            width: lodash.get(this.props, 'width', globalConfig.infoTypeWidth),
        }
        const button = option.type === "button" ? <Button
            icon={option.icon}
            disabled={option.disabled}
            onClick={this.onVisible.bind(this, true)}>{option.title}</Button> :
            <a onClick={this.onVisible.bind(this, true)} >{option.icon && <Icon type={option.icon} />} {option.title}</a>;
        return (
            <React.Fragment key={this.key}>
                {button}
                {this.state.load && <this.Optimization
                    option={option}
                    visible={this.state.visible}
                    onVisible={this.onVisible.bind(this)}
                >
                    {this.props.children}
                </this.Optimization>}
            </React.Fragment>
        );
    }
};
@DesError
class Optimization extends React.Component<{
    option: any;
    visible: any;
    onVisible: any;
    onFormSubmit?: any;
    form?: WrappedFormUtils;
}, any> {
    constructor(props) {
        super(props)
        // if (globalConfig.development && this.props.visible) {
        console.log('装载成功', this)
        // }
    }
    // shouldComponentUpdate(nextProps, nextState, nextContext) {

    //     return true
    // }
    state = { loading: false }
    onVisible(visible) {
        this.props.onVisible(visible)
    }

    onSubmit(e?) {
        if (e) {
            e.stopPropagation();
            e.preventDefault();
        }
        const onFormSubmit = this.props.onFormSubmit || lodash.get(this.props.children, 'type.onFormSubmit');
        if (lodash.isFunction(onFormSubmit)) {
            // debugger
            // 加载中 返回。
            if (this.state.loading) return;
            this.props.form.validateFields((err, values) => {
                console.info('表单数据', values)
                if (err) {
                    // notification.warn({ key: 'validateFields_err', message: "数据未填写完整" })
                    this.state.loading && this.setState({ loading: false })
                    return
                }
                this.setState({ loading: true }, async () => {
                    try {
                        const callbackValue = onFormSubmit(values, this.onVisible.bind(this, false));
                        // console.log(callbackValue)
                        let res = false;
                        if (lodash.isBoolean(callbackValue)) {
                            res = callbackValue;
                        } else if (callbackValue instanceof Promise) {
                            res = await callbackValue
                        } else if (lodash.isFunction(callbackValue)) {
                            res = callbackValue()
                        } else {
                            globalConfig.development && notification.warn({
                                key: 'validateFields_err',
                                message: "onFormSubmit 无返回值  ",
                                description: "返回 true | Promise<boolean> 关闭窗口"
                            })
                        }
                        console.log(res)
                        if (res) {
                            this.onVisible(false)
                        }

                    } catch (error) {
                        this.onSetErrorMsg(error)
                        // onErrorMessage("操作失败", lodash.map(error, (value, key) => ({ value, key })))
                        console.error(error)
                    } finally {
                        this.setState({ loading: false })
                    }
                });
            });
        } else {
            globalConfig.development && message.warning("未 配置 onFormSubmit 函数")
            // console.log("没有")
        }
    }
    onSetErrorMsg(errors) {
        const { setFields, getFieldValue } = this.props.form;
        setFields(lodash.mapValues(lodash.get(errors, 'Form', {}), (error, key) => {
            return {
                value: getFieldValue(key),
                errors: [new Error(error)]
            }
        }))
    }
    renderFrom() {
        const { option, visible } = this.props;
        const children = this.props.children && React.cloneElement(this.props.children as any, { form: this.props.form });
        if (option.button) {
            return <div className='app-shell-body'>
                <Spin tip="Loading..." spinning={this.state.loading} >
                    {children}
                </Spin>
                <div className="data-view-form-btns" >
                    {option.button({
                        onSubmit: (event?) => { this.onSubmit(event) },
                        onCancel: (event?) => { this.onVisible(false) }
                    })}
                </div>
            </div>
        }
        if (option.showSubmit) {
            return <Form onSubmit={this.onSubmit.bind(this)} className='app-shell-body'>
                <Spin tip="Loading..." spinning={this.state.loading} >
                    {children}
                </Spin>
                <div className="data-view-form-btns" >
                    <Button onClick={this.onVisible.bind(this, false)} > {option.closeText} </Button>
                    <Divider type="vertical" />
                    <Button loading={this.state.loading} type="primary" htmlType="submit" >{option.submitText} </Button>
                </div>
            </Form>
        }
        return <div className='app-shell-body'>
            <Spin tip="Loading..." spinning={this.state.loading} >
                {children}
            </Spin>
            <div className="data-view-form-btns" >
                <Button onClick={this.onVisible.bind(this, false)} > {option.closeText} </Button>
            </div>
        </div>

    }
    render() {
        const { option, visible } = this.props;
        // console.log("Optimization", this)
        return (
            <InfoShell
                title={option.title}
                visible={visible}
                onCancel={this.onVisible.bind(this, false)}
                width={option.width}
                {...this.props}
            >
                {this.renderFrom()}
            </InfoShell>
        );
    }
};
export function DialogFormDes(params: {
    /**
     * 表单提交
     */
    onFormSubmit?: (values) => Promise<boolean> | Function | void,
    /**
     * 表单 默认 数据加载
     */
    onLoadData?: (values, props) => Promise<boolean> | Object,
    /**
     * 加载提示文字 默认 Loading。。。
     */
    tip?: string;
}) {
    return (Component: React.ComponentClass<any, any>) => {
        const DFC: any = class extends React.PureComponent<{ loadData: Object | Function }, any> {
            constructor(props) {
                super(props)
            }
            isOnLoadData = lodash.isFunction(params.onLoadData)
            isLoadData = this.props.loadData && this.isOnLoadData || false;
            state = {
                ...this.state,
                __spinning: true,
                __details: {},
                __key: Help.GUID()
            }
            async componentDidMount() {
                // if (this.isOnLoadData && this.props.loadData) {
                let res = {}
                const time = Date.now();
                if (this.isOnLoadData) {
                    // 检查 loadData 类型 函数 执行 后获取返回值
                    let loadData = this.props.loadData ? lodash.isFunction(this.props.loadData) ? this.props.loadData() : this.props.loadData : {};
                    const onLoadData = params.onLoadData.bind(this)(loadData, this.props);
                    if (onLoadData instanceof Promise) {
                        res = await onLoadData;
                    } else if (lodash.isFunction(onLoadData)) {
                        res = onLoadData();
                    } else {
                        res = onLoadData;
                    }
                }
                // 强制 执行加载最少 400 毫秒
                await new Promise((res, rej) => {
                    lodash.delay(res, 400 - (Date.now() - time))
                });
                this.setState({ __details: res, __spinning: false, __key: Help.GUID() })
                super.componentDidMount && super.componentDidMount()
            }
            render() {
                const { __spinning, __details, __key } = this.state;
                // const notLoadData = this.isOnLoadData && !this.props.loadData;
                return <Spin tip={params.tip || 'Loading...'} spinning={__spinning}>
                    {__spinning ? <Skeleton paragraph={{ rows: 10 }} /> : <Component {...this.props} defaultValues={__details} />}
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