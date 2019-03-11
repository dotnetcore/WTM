/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:20
 * @modify date 2019-02-24 17:06:20
 * @desc [description]
 */
import { Input, Switch, Icon } from 'antd';
import Form, { GetFieldDecoratorOptions, WrappedFormUtils } from 'antd/lib/form/Form';
import { FormItemProps } from 'antd/lib/form/FormItem';
import { ToImg } from 'components/dataView';
import GlobalConfig from 'global.config'; //全局配置
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
const formItemLayout = lodash.cloneDeep(GlobalConfig.formItemLayout);//布局
/**
 * 表单item
 */
interface IFormItemProps {
    fieId: string;
    /** 模型 */
    models?: {
        [key: string]: {
            label: string,
            rules: any[],
            formItem: React.ReactNode
        };
    };
    /** 禁用 控件输入更改 */
    disabled?: boolean;
    /** 禁用 组件 显示 span */
    display?: boolean;
    /** 默认值 */
    defaultValues?: Object;
    /** Form.Item 的 props */
    formItemProps?: FormItemProps;
    /** 装饰器参数  */
    decoratorOptions?: GetFieldDecoratorOptions;
    /** 布局类型 row 整行 span 24 */
    layout?: "row";
    /** 覆盖默认渲染 */
    render?: (props) => React.ReactNode;
    [key: string]: any;
}
@observer
export class FormItem extends React.Component<IFormItemProps, any> {
    static wtmType = "FormItem";
    render() {
        const { form = {}, fieId, models, decoratorOptions, formItemProps, defaultValues, disabled, display, render, layout } = this.props;
        // 获取模型 item
        const model = lodash.get(models, fieId) || { rules: [], label: `未获取到模型(${fieId})`, formItem: <Input placeholder={`未获取到模型(${fieId})`} /> };
        let options: GetFieldDecoratorOptions = {
            rules: model.rules,
            ...decoratorOptions
        };
        // 获取默认值 默认值，禁用，显示 span 
        if (typeof defaultValues === "object") {
            options.initialValue = lodash.get(defaultValues, fieId);
        }
        let renderItem = null;
        // 重写渲染
        if (typeof render === "function") {
            renderItem = render(this.props);
        } else {
            renderItem = itemRender(this.props, { options, model })
        }
        // 布局
        // let itemlayout = layout == "row" ? formItemLayoutRow : formItemLayout;//整行
        let itemlayout = formItemLayout;
        if (layout == "row") {
            let labelSpan = itemlayout.labelCol.span / GlobalConfig.infoColumnCount;
            let warppSpan = 24 - labelSpan - 2;
            itemlayout = {
                labelCol: {
                    span: labelSpan
                },
                wrapperCol: {
                    span: warppSpan
                },
            }
            // 富文本 分 2行
            if (lodash.get(model.formItem, "type.wtmType") === "Editor") {
                itemlayout = {
                    labelCol: {
                        span: labelSpan
                    },
                    wrapperCol: {
                        span: 24
                    },
                }
            }
        }
        // console.log(models, renderItem)
        return <Form.Item label={model.label} {...itemlayout}  {...formItemProps}>
            {renderItem}
        </Form.Item >
    }
}
/**
 * 重写 渲染 组件
 * @param props 
 * @param config 
 */
function itemRender(props, config) {
    const { form = {}, disabled, display, fieId } = props;
    const { options, model } = config;
    let { formItem } = model;
    const { getFieldDecorator }: WrappedFormUtils = form;
    let renderItem, propsNew: any = { form };
    if (lodash.isFunction(formItem)) {
        formItem = formItem()
    }
    // 禁用显示 span
    if (lodash.isEqual(display, true)) {
        propsNew.display = "true";
        propsNew.value = options.initialValue;
        renderItem = itemToDisplay(props, config)
    } else {
        //  判断 组件 是否 已经 是 getFieldDecorator组件
        if (lodash.get(formItem, "props.data-__field")) {
            renderItem = formItem;
        } else {
            if (getFieldDecorator) {
                // console.log(fieId, options.initialValue)
                renderItem = getFieldDecorator(fieId as never, options)(formItem);
            } else {
                renderItem = formItem;
                console.warn("没有解析到 getFieldDecorator")
            }
        }
    }
    // 禁用 输入控件
    if (lodash.isEqual(disabled, true)) {
        propsNew.disabled = true
    }
    // 布尔类型 Swatch
    if (lodash.isEqual(options.initialValue, true)) {
        propsNew.defaultChecked = true;
    }
    // console.log(propsNew)
    return React.cloneElement(renderItem, propsNew);
}
/**
 * 重写渲染 禁用 组件
 * @param props 
 * @param config 
 */
function itemToDisplay(props, config) {
    const { disabled, display, fieId } = props;
    const { options, model } = config;
    let { formItem } = model;
    let value = options.initialValue;
    let render = null;

    switch (lodash.get(formItem, "type.wtmType")) {
        case "UploadImg":
            render = <ToImg fileID={value} />
            break;
        case "Select":
        case "Transfer":
            render = formItem;// React.cloneElement(model.formItem, {});
            break;
        default:
            // 数据 是 obj 类型转换 为 json 字符串，防止 react 报错
            if (lodash.isObject(value)) {
                value = value && JSON.stringify(value);
            } else if (lodash.isBoolean(value) || value === "true" || value === "false") {
                value = (value || value === "true") ? <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} disabled defaultChecked /> : <Switch checkedChildren={<Icon type="check" />} unCheckedChildren={<Icon type="close" />} disabled />;
            }
            render = <span>{value}</span>
            break;
    }
    return render
}
