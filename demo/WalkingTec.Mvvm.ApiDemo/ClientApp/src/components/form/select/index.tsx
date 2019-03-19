/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Select, notification, Spin } from 'antd';
import { SelectProps } from 'antd/lib/select';
import { DesError } from 'components/decorators'; //错误
import lodash from 'lodash';
import React from 'react';
import { Observable } from 'rxjs';
import { WrappedFormUtils } from 'antd/lib/form/Form';
// 联动模型
declare type linkageModels = (linkageModels?: any) => Observable<any[]> | any[] | Promise<any[]>;
interface IAppProps {
    dataSource: Observable<any[]> | any[] | Promise<any[]> | linkageModels;
    dataKey?: string;
    value?: any;
    /** 多选 */
    multiple?: boolean;
    disabled?: boolean;
    placeholder?: React.ReactNode;
    SelectProps?: SelectProps;
    display?: boolean;
    /** 联动模型 key 值 */
    linkageModels?: string;
    [key: string]: any;
}
@DesError
export class WtmSelect extends React.Component<IAppProps, any> {
    static wtmType = "Select";
    key = "Value";
    title = "Text";
    description = "Text";
    // 联动模型的 value
    linkageModelsValue;
    state = {
        loading: false,
        mockData: [],
    }
    /**
     * 优化渲染
     * @param nextProps 
     * @param nextState 
     * @param nextContext 
     */
    shouldComponentUpdate(nextProps, nextState, nextContext) {
        return true
        if (!lodash.eq(this.state.loading, nextState.loading)) {
            return true
        }
        if (!lodash.eq(this.state.mockData, nextState.mockData)) {
            return true
        }
        if (!lodash.eq(this.props.value, nextProps.value)) {
            return true
        }
        // 父子联动
        if (this.props.linkageModels) {
            const { getFieldValue }: WrappedFormUtils = this.props.form;
            return !lodash.eq(getFieldValue(this.props.linkageModels), this.linkageModelsValue)
        }
        return false
    }
    Unmount = false
    componentWillUnmount() {
        this.Unmount = true;
    }
    componentDidUpdate() {
        this.onlinkageModelsUpdate()
    }
    async  componentDidMount() {
        const { dataSource } = this.props;
        // 联动模型 不加载 数据
        if (this.props.linkageModels) {
            if (!lodash.isFunction(this.props.dataSource)) {
                const description = "linkageModels 模式 dataSource 为 函数，返回一个 Observable<any[]> | any[] | Promise<any[]>  对象"
                notification.warn({ message: "配置错误", description })
            }
            return this.setState({ loading: false })
        }
        this.onLoadingData(dataSource)
    }
    /**
     * 加载数据
     * @param dataSource 
     */
    async  onLoadingData(dataSource) {
        let mockData = [],
            res = [];
        try {
            if (this.state.loading) return
            this.setState({
                loading: true
            })
            // 值为 数组 
            if (lodash.isArray(dataSource)) {
                res = dataSource;
            }
            // 值为 Promise
            else if (dataSource instanceof Promise) {
                res = await dataSource;
            }
            // 值为 Observable 
            else if (dataSource instanceof Observable) {
                res = await dataSource.toPromise();
            }
            // 转换 数据 为 渲染 格式
            mockData = res.map(item => {
                return {
                    ...item,
                    key: lodash.toString(lodash.get(item, this.key)),
                    title: lodash.get(item, this.title),
                    description: lodash.get(item, this.description),
                }
            })
        } catch (error) {
            console.error("Select 获取数据出错", error)
        }
        if (this.Unmount) return
        this.setState({
            mockData,
            loading: false
        })
        return mockData
    }
    /**
     * 模型数据改变
     */
    async onlinkageModelsUpdate() {
        if (this.props.linkageModels && lodash.isFunction(this.props.dataSource)) {
            try {
                const { getFieldValue, resetFields, setFields }: WrappedFormUtils = this.props.form;
                const linkageModelsValue = getFieldValue(this.props.linkageModels);
                if (!lodash.eq(this.linkageModelsValue, linkageModelsValue)) {
                    // console.log("onlinkageModelsUpdate", this.props)
                    this.linkageModelsValue = linkageModelsValue;
                    // 重置选择的值
                    // resetFields([this.props.id])
                    // 加载数据 联动
                    if (!lodash.isNil(linkageModelsValue)) {
                        const data = await this.onLoadingData(this.props.dataSource(linkageModelsValue));
                        if (!lodash.some(data, [this.key, this.props.value])) {
                            setFields({ [this.props.id]: { value: undefined } })
                        }
                    } else {
                        setFields({ [this.props.id]: { value: undefined } })
                        this.setState({ mockData: [] })
                    }
                }
            } catch (error) {
                console.error(error)
            }
        }
    }
    // filterOption = (inputValue, option) => option.description.indexOf(inputValue) > -1
    handleChange = (targetKeys) => {
        // 多选 返回 值 为数组 的情况下 有 dataKey 重组 数据
        if (this.props.dataKey && lodash.isArray(targetKeys)) {
            targetKeys = targetKeys.map(x => (
                { [this.props.dataKey]: x }
            ))
        }
        this.props.onChange && this.props.onChange(targetKeys);
    }
    getDefaultValue(config: SelectProps) {
        const { value, dataKey } = this.props;
        // 默认值
        if (!lodash.isNil(value)) {
            let newValue = null;
            // 默认值 多选
            if (config.mode == "multiple" && lodash.isArray(value)) {
                newValue = value.map(x => {
                    if (lodash.isString(x)) {
                        return x;
                    }
                    if (dataKey) {
                        return lodash.get(x, dataKey)
                    }
                    // 没有找到 dataKey
                    return lodash.toString(x);
                })
            }
            // 单选
            else {
                newValue = lodash.toString(value);
            }
            config.defaultValue = newValue
        }
        return config;
    }
    render() {
        let config: SelectProps = {
            allowClear: true,
            showArrow: true,
            loading: this.state.loading,
            // mode: "multiple",
            placeholder: this.props.placeholder,
            disabled: this.props.disabled,
            onChange: this.handleChange,
            value: this.props.value,
        }
        // 多选
        if (this.props.multiple) {
            config.mode = "multiple"
        }
        config = this.getDefaultValue(config);
        if (this.props.display) {
            if (!this.state.loading) {
                // 多选的
                if (lodash.isArray(config.defaultValue)) {
                    return lodash.intersectionBy(this.state.mockData, (config.defaultValue as string[]).map(x => ({ key: x })), "key").map(x => x.title).join(",")
                }
                return <span>{lodash.get(lodash.find(this.state.mockData, ["key", lodash.toString(config.defaultValue)]), "title")}</span>
            }
            return <span></span>
        }
        // throw "aaaaa"
        return (
            <Spin spinning={this.state.loading}>
                <Select
                    {...config}
                >
                    {this.renderOption(config)}
                </Select>
            </Spin>
        );
    }
    renderOption(config?: SelectProps) {
        return this.state.mockData.map(x => {
            return <Select.Option key={x.key} value={x.key}>{x.title}</Select.Option>
        })
    }
}

export default WtmSelect