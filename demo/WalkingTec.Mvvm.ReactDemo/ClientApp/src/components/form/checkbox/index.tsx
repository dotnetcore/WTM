/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:42
 * @modify date 2019-02-24 17:06:42
 * @desc [description]
 */
import { Checkbox, Col, Row } from 'antd';
import { CheckboxGroupProps } from 'antd/lib/checkbox';
import { DesLoadingData, ILoadingDataProps } from 'components/decorators'; //错误
import React from 'react';
@DesLoadingData()
export class WtmCheckbox extends React.Component<ILoadingDataProps & CheckboxGroupProps, any> {
    static wtmType = "Checkbox";
    state = {
        spinning: false,
        dataSource: [],
    }
    render() {
        const { dataSource, ...props } = this.props;
        return (
            <Checkbox.Group style={{ width: '100%' }} {...props}>
                {this.renderItem(this.state.dataSource)}
            </Checkbox.Group>
        );
    }
    renderItem(dataSource) {
        if (this.props.renderItem) {
            return this.props.renderItem(dataSource)
        }
        return <Row type="flex" align="middle">
            {dataSource.map(data => {
                return <Col key={data.value}>
                    <Checkbox value={data.value}>{data.title}</Checkbox>
                </Col>
            })}
        </Row>
    }
}
export default WtmCheckbox