import { Select, Spin } from 'antd';
import React from 'react';
import { Observable } from 'rxjs';
import lodash from 'lodash';
import { SelectProps } from 'antd/lib/select';
interface IAppProps {
    dataSource: Observable<any[]> | any[] | Promise<any[]>;
    dataKey?: string;
    value?: any;
    disabled?: boolean;
    placeholder?: React.ReactNode;
    SelectProps?: SelectProps;
    [key: string]: any;
}
export default class extends React.Component<IAppProps, any> {
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
        // if (lodash.isArray(this.props.value) && lodash.isString(this.props.dataKey)) {
        //     targetKeys = this.props.value.map(x => (lodash.get(x, this.props.dataKey)))
        // }
        this.setState({
            mockData,
            targetKeys,
            loading: false
        })
    }
    filterOption = (inputValue, option) => option.description.indexOf(inputValue) > -1
    handleChange = (targetKeys) => {
        console.log(targetKeys)
        // this.setState({ targetKeys });
        if (this.props.dataKey && lodash.isArray(targetKeys)) {
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
            // <Spin spinning={this.state.loading}>
            <Select
                loading={this.state.loading}
                placeholder={this.props.placeholder}
                disabled={this.props.disabled}
                onChange={this.handleChange}
            >
                {this.renderOption()}
            </Select>
            // </Spin>
        );
    }
    renderOption() {
        return this.state.mockData.map(x => {
            return <Select.Option key={x.key} value={x.key}>={x.title}</Select.Option>
        })
    }
}

