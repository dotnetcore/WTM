import { Card, Col, Row } from 'antd';
import * as React from 'react';
import Github from 'components/other/githubStar';
import Antv1 from './echarts/link';
import Antv2 from './echarts/lunar';
import Antv3 from './echarts/pie';
import Colendar from './colendar';

import globalConfig from 'global.config';

export default class IApp extends React.Component<any, any> {
    public render() {
        console.log("Home");
        return (
            <div>
                {globalConfig.development && <Row type="flex" gutter={16}>
                    <Col span={24} >
                        <div style={{
                            textAlign: "center",
                            fontSize: 18
                        }}>
                            <a href="http://wtmdoc.walkingtec.cn" target="_block" style={{ fontSize: 24 }}>WTM文档地址</a><br />
                            WTM是纯开源框架，如果喜欢，来GitHub赏个星~~
                              <Github />
                        </div>
                    </Col>
                </Row>}
                <Row type="flex" gutter={16}>
                    <Col span={24} >
                        <Card bordered={false}>
                            <Colendar />
                        </Card>
                    </Col>
                </Row>
                <Row type="flex" gutter={16}>
                    <Col span={12} >
                        <Card bordered={false}>  <Antv1 /></Card>
                    </Col>
                    <Col span={12} >
                        <Card bordered={false}>  <Antv3 /></Card>

                    </Col>
                </Row>
                <Row type="flex" gutter={16}>
                    <Col span={24} >
                        <Card bordered={false}>    <Antv2 /></Card>
                    </Col>
                </Row>

            </div>
        );
    }
}
