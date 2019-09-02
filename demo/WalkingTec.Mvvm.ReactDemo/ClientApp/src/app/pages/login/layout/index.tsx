
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
    const isIe = "ActiveXObject" in window;
    return (
      <Layout className="app-layout-login app-fade-enter">
        <Layout style={isIe ? { minHeight: window.innerHeight - 197 } : {}}>
          <Header>
            <img className='app-login-logo' src={logo} alt="" height="48" />
          </Header>
          <Content>
            <div className='app-login-form'>
              {this.props.children}
            </div>
          </Content>
        </Layout>
        <Footer>
          <Row type="flex" align="top" className='app-login-links'>
            <Col lg={18} md={24}>
              <Typography>
                <Typography.Title>Quick Links</Typography.Title>
                <Typography.Paragraph>
                  <Row type="flex" align="top">
                    {lodash.get(this.props as any, 'links', [
                      { name: 'GitHub', url: 'https://github.com/dotnetcore/WTM' },
                      { name: "React", url: 'https://reactjs.org/' },
                      { name: "Ant Design", url: 'https://ant.design/' },
                      { name: "Rxjs", url: 'https://rxjs.dev/' },
                      { name: "Mobx", url: 'https://mobx.js.org/' },
                      { name: "Lodash", url: ' https://lodash.com/' },
                    ]).map((value, index) => (
                      <Col key={index} lg={6} md={8} xs={8} sm={12}>
                        <a href={value.url} target="_blank" >    {value.name}</a>
                      </Col>
                    ))}
                  </Row>
                </Typography.Paragraph>
              </Typography>
            </Col>
            <Col lg={6} md={24} className='app-login-codeimg'>
              <img src={lodash.get(this.props, 'code', code)} alt="" width="88" height="88" />
              <p>QR code</p>
            </Col>
          </Row>
          <div className="app-login-record">
            Help      @2019 WTM rights reserved
          </div>
        </Footer>
      </Layout>
    );
  }
}