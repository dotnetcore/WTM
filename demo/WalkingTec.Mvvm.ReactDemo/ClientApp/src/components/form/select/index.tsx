/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Select } from 'antd';
import { SelectProps } from 'antd/lib/select';
import { DesLoadingData, ILoadingDataProps } from 'components/decorators'; //错误
import React from 'react';
@DesLoadingData()
export class WtmSelect extends React.Component<ILoadingDataProps & SelectProps<any>, any> {
    static wtmType = "Select";
    state = {
        spinning: false,
        dataSource: [],
    }
    render() {
        const { dataSource, ...props } = this.props;
        console.log("TCL: WtmSelect -> render -> props", props)
        return (
            <Select
                placeholder="Please choose"
                allowClear
                showSearch
                filterOption={(input, option: any) => {
                    return option.props.children.toLowerCase().indexOf(input.toLowerCase()) >= 0
                }}
                {...props}
            >
                {this.renderItem(this.state.dataSource)}
            </Select>
        );
    }
    renderItem(dataSource) {
        if (this.props.renderItem) {
            return this.props.renderItem(dataSource)
        }
        return dataSource.map(x => {
            return <Select.Option key={x.key} value={x.key}>{x.title}</Select.Option>
        })
    }
}
export default WtmSelect