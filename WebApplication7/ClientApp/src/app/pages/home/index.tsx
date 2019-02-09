import * as React from 'react';
import { Link } from 'react-router-dom';
import { Form, Icon, Input, Button, Spin, Row, Col, Card } from 'antd';
import { inject, observer } from 'mobx-react';
import Antv1 from './antv1';
import Antv2 from './antv2';
import Antv3 from './antv3';
export default class IApp extends React.Component<any, any> {
    public render() {
        console.log("Home");
        return (
            <div>
                <Row type="flex" gutter={16}>
                    <Col span={24} >
                        <Card bordered={false}>
                            <div style={{
                                textAlign:"center",
                                fontSize:60
                            }}>
                                <a href="https://wtm-front.github.io/wtm-cli/" target="_block">WTM文档地址</a>
                            </div>
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
