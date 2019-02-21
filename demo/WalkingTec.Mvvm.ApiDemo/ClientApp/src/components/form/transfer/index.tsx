import { Transfer, Spin } from 'antd';
import React from 'react';
import { Observable } from 'rxjs';
import lodash from 'lodash';

export default class extends React.Component<{
    dataSource: Observable<any[]> | any[] | Promise<any[]>;
    dataKey?: string;
    value?: any;
    disabled?: boolean;
    [key: string]: any;
}, any> {
    state = {
        loading: true,
        mockData: [],
        targetKeys: [],
    }
    async  componentDidMount() {
        const { dataSource } = this.props;
        let mockData = [],
            targetKeys = [],
            res = [];
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
                key: item.Value,
                title: item.Text,
                description: item.Text,
            }
        })
        // 回填 已选择数据
        if (lodash.isArray(this.props.value) && lodash.isString(this.props.dataKey)) {
            targetKeys = this.props.value.map(x => (lodash.get(x, this.props.dataKey)))
        }
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

