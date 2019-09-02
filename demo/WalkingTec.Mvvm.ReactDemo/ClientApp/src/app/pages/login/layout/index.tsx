
import { Col, Layout, Row, Typography } from 'antd';
import * as React from 'react';
import lodash from 'lodash';
import logo from '../../../../assets/img/logo.png';
import code from './images/code.png';
import './style.less';
const { Header, Content, Sider, Footer } = Layout;
export class LayoutLogin extends React.Component<{
  links?: React.ReactNodeArray;
  code?: string;
}, any>{
  render() {
    return (
      <Layout className="lenovo-layout-login lenovo-fade-enter">
        <Layout>
          <Header>
            <img className='lenovo-login-logo' src={logo} alt="" height="48" />
          </Header>
          <Content>
            <div className='lenovo-login-form'>
              {this.props.children}
            </div>
          </Content>
        </Layout>
        <Footer>
          <Row type="flex" align="top" className='lenovo-login-links'>
            <Col lg={18} md={24}>
              <Typography>
                <Typography.Title>Quick Links</Typography.Title>
                <Typography.Paragraph>
                  <Row type="flex" align="top">
                    {lodash.get(this.props as any, 'links', ['IT Support', 'IDMC', 'BT/IT T2.0', 'User Case Testing', 'Assitant', ' Support', 'Web VPN', 'IT Service']).map((value, index) => (
                      <Col key={index} lg={6} md={8} xs={8} sm={12}>
                        {value}
                      </Col>
                    ))}
                  </Row>
                </Typography.Paragraph>
              </Typography>
            </Col>
            <Col lg={6} md={24} className='lenovo-login-codeimg'>
              <img src={lodash.get(this.props, 'code', code)} alt="" width="88" height="88" />
              <p>IT Service everywhere</p>
              <p>Scan to download</p>
            </Col>
          </Row>
          <div className="lenovo-login-record">
            Help    Contact us    @2019 WTM rights reserved
          </div>
        </Footer>
      </Layout>
    );
  }
}