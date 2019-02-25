/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Select } from 'antd';
import { SelectProps } from 'antd/lib/select';
import { DesError } from 'components/decorators'; //错误
import lodash from 'lodash';
import React from 'react';
import { Observable } from 'rxjs';

interface IAppProps {
    dataSource: Observable<any[]> | any[] | Promise<any[]>;
    dataKey?: string;
    value?: any;
    /** 多选 */
    multiple?: boolean;
    disabled?: boolean;
    placeholder?: React.ReactNode;
    SelectProps?: SelectProps;
    display?: boolean;
    [key: string]: any;
}
@DesError
export default class extends React.Component<IAppProps, any> {
    static wtmType = "Select";
    state = {
        loading: true,
        mockData: [],
        targetKeys: [],
    }
    // shouldComponentUpdate(nextProps, nextState, nextContext) {
    //     if (lodash.isEqual(this.state, nextState)) {
    //         return true
    //     }
    //     return !lodash.isEqual(this.props.value, nextProps.value)
    // }
    async  componentDidMount() {
        const { dataSource } = this.props;
        let mockData = [],
            targetKeys = [],
            res = [];
        try {
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
                    key: lodash.toString(item.Value),
                    title: item.Text,
                    description: item.Text,
                }
            })
        } catch (error) {
            console.error("Select 获取数据出错",error)
        }
        // 回填 已选择数据
        // if (lodash.isArray(this.props.value) && lodash.isString(this.props.dataKey)) {
        //     targetKeys = this.props.value.map(x => (lodash.get(x, this.props.dataKey)))
        // }
        this.setState({
            mockData,
            targetKeys,
            loading: false
        })
    }
    // filterOption = (inputValue, option) => option.description.indexOf(inputValue) > -1
    handleChange = (targetKeys) => {
        // 多选 返回 值 为数组 的情况下 有 dataKey 重组 数据
        if (this.props.dataKey && lodash.isArray(targetKeys)) {
            return this.props.onChange(
                targetKeys.map(x => (
                    { [this.props.dataKey]: x }
                ))
            );
        }
        this.props.onChange(targetKeys);
    }
    getDefaultValue(config) {
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
            onChange: this.handleChange
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
            <Select
                {...config}
            >
                {this.renderOption()}
            </Select>
        );
    }
    renderOption() {
        return this.state.mockData.map(x => {
            return <Select.Option key={x.key} value={x.key}>{x.title}</Select.Option>
        })
    }
}

