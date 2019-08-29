/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Radio, Spin, Tag, Row, Col } from 'antd';
import { DesError, DesLoadingData, ILoadingDataProps } from 'components/decorators'; //错误
import lodash from 'lodash';
import React from 'react';
import { Observable } from 'rxjs';
import { RadioGroupProps } from 'antd/lib/radio';
@DesLoadingData()
export class WtmRadio extends React.Component<ILoadingDataProps & RadioGroupProps, any> {
    static wtmType = "Radio";
    state = {
        spinning: false,
        dataSource: [],
    }
    render() {
        const { dataSource, ...props } = this.props;
        return (
            <Radio.Group {...props} >
                {this.renderItem(this.state.dataSource)}
            </Radio.Group>
        );
    }
    renderItem(dataSource) {
        if (this.props.renderItem) {
            return this.props.renderItem(dataSource)
        }
        return <Row type="flex" align="middle">
            {dataSource.map(data => {
                return <Col key={data.value}>
                    <Radio value={data.value}>{data.title}</Radio>
                </Col>
            })}
        </Row>
    }
}
export default WtmRadio