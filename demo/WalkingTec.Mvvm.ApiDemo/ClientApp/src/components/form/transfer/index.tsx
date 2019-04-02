/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:47
 * @modify date 2019-02-24 17:06:47
 * @desc [description]
 */
import { Spin, Transfer } from 'antd';
import { DesError } from 'components/decorators'; //错误
import lodash from 'lodash';
import React from 'react';
import { Observable } from 'rxjs';

interface IAppProps {
    dataSource: Observable<any[]> | any[] | Promise<any[]>;
    dataKey?: string;
    value?: any;
    disabled?: boolean;
    display?: boolean;
    [key: string]: any;
}
@DesError
export class WtmTransfer extends React.Component<IAppProps, any> {
    static wtmType = "Transfer";
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
            console.error("Transfer 获取数据出错", error)
        }
        // 回填 已选择数据
        if (lodash.isArray(this.props.value) && lodash.isString(this.props.dataKey)) {
            targetKeys = this.props.value.map(x => (lodash.toString(lodash.get(x, this.props.dataKey))))
        }
        if (this.Unmount) return
        this.setState({
            mockData,
            targetKeys,
            loading: false
        })
    }
    filterOption = (inputValue, option) => option.description.indexOf(inputValue) > -1
    handleChange = (targetKeys) => {
        this.setState({ targetKeys });
        if (this.props.dataKey) {
            return this.props.onChange(
                targetKeys.map(x => (
                    { [this.props.dataKey]: x }
                ))
            );
        }
        this.props.onChange(targetKeys);
    }
    handleSearch = (dir, value) => {
        console.log('search:', dir, value);
    };
    render() {
        if (this.props.display) {
            return lodash.intersectionBy(this.state.mockData, this.state.targetKeys.map(x => ({ key: x })), "key").map(x => x.title).join(",")
        }
        return (
            <Spin spinning={this.state.loading}>
                <Transfer
                    dataSource={this.state.mockData}
                    showSearch
                    filterOption={this.filterOption}
                    targetKeys={this.state.targetKeys}
                    onChange={this.handleChange}
                    onSearch={this.handleSearch}
                    render={item => item.title}
                    disabled={this.props.disabled}
                />
            </Spin>
        );
    }
}

export default WtmTransfer