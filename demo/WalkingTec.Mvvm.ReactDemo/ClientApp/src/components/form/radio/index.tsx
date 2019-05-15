/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Radio, Spin, Tag } from 'antd';
import { DesError } from 'components/decorators'; //错误
import lodash from 'lodash';
import React from 'react';
import { Observable } from 'rxjs';
import { RadioGroupProps } from 'antd/lib/radio';
const RadioGroup = Radio.Group;
interface IAppProps {
    dataSource: Observable<any[]> | any[] | Promise<any[]>;
    dataKey?: string;
    value?: any;
    /** 多选 */
    // multiple?: boolean;
    disabled?: boolean;
    placeholder?: React.ReactNode;
    RadioGroupProps?: RadioGroupProps;
    display?: boolean;
    [key: string]: any;
}
@DesError
export class WtmRadio extends React.Component<IAppProps, any> {
    static wtmType = "Radio";
    key = "Value";
    title = "Text";
    description = "Text";
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
    Unmount = false
    componentWillUnmount() {
        this.Unmount = true;
    }
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
                    key: lodash.toString(lodash.get(item, this.key)),
                    title: lodash.get(item, this.title),
                    description: lodash.get(item, this.description),
                }
            })
        } catch (error) {
            console.error("Select 获取数据出错", error)
        }
        // 回填 已选择数据
        // if (lodash.isArray(this.props.value) && lodash.isString(this.props.dataKey)) {
        //     targetKeys = this.props.value.map(x => (lodash.get(x, this.props.dataKey)))
        // }
        if (this.Unmount) return
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
    render() {
        // console.log(this.props)
        let config: RadioGroupProps = {
            disabled: this.props.disabled,
            onChange: this.handleChange,
            options: this.state.mockData.map(x => {
                return {
                    ...x,
                    label: x.title,
                    value: x.key
                }
            }),
            value: lodash.toString(this.props.value),
            // defaultValue: lodash.toString(this.props.value)
        }
        if (this.props.display) {
            if (!this.state.loading) {
                // 多选的
                if (lodash.isArray(config.value)) {
                    return lodash.intersectionBy(this.state.mockData, (config.value as string[]).map(x => ({ key: x })), "key").map(x => {
                        return <Tag color="geekblue" key={x.key}>{x.title}</Tag>
                    })
                }
                return <span>{lodash.get(lodash.find(this.state.mockData, ["key", lodash.toString(config.value)]), "title")}</span>
            }
            return <span></span>
        }
        return (
            <Spin spinning={this.state.loading}>
                <RadioGroup
                    {...config}
                />
            </Spin>

        );
    }
}

export default WtmRadio