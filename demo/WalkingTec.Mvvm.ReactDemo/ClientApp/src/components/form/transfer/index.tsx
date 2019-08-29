/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:47
 * @modify date 2019-02-24 17:06:47
 * @desc [description]
 */
import { Transfer } from 'antd';
import { TransferProps } from 'antd/lib/transfer';
import { DesLoadingData, ILoadingDataProps } from 'components/decorators'; //错误
import React from 'react';
@DesLoadingData()
export class WtmTransfer extends React.Component<ILoadingDataProps & TransferProps & { value?: any }, any> {
    static wtmType = "Transfer";
    state = {
        spinning: false,
        dataSource: [],
    }
    render() {
        const { dataSource, ...props } = this.props;
        return (
            <Transfer
                render={item => item.title}
                dataSource={this.state.dataSource}
                targetKeys={props.value}
                // showSearch
                // filterOption={(inputValue, option) => option.description.indexOf(inputValue) > -1}
                {...props}
            />
        );
    }
}
export default WtmTransfer