import { Transfer, Spin } from 'antd';
import React from 'react';
import { Observable } from 'rxjs';
import lodash from 'lodash';

export default class extends React.Component<{
    dataSource: Observable<any[]> | any[];
    dataKey?: string;
    value?: any;
    [key: string]: any;
}, any> {
    state = {
        loading: true,
        mockData: [],
        targetKeys: [],
    }
    async  componentDidMount() {
        // this.getMock();
        if (this.props.dataSource instanceof Observable) {
            const res = await this.props.dataSource.toPromise();
            let targetKeys = [];
            if (lodash.isArray(this.props.value) && lodash.isString(this.props.dataKey)) {
                targetKeys = this.props.value.map(x => (lodash.get(x, this.props.dataKey)))
            }
            this.setState({
                mockData: res.map(item => {
                    return {
                        ...item,
                        key: item.Value,
                        title: item.Text,
                        description: item.Text,
                    }
                }),
                targetKeys: targetKeys,
                loading: false
            })
        }
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
                />
            </Spin>
        );
    }
}

