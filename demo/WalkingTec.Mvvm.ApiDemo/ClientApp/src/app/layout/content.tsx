
import { Layout } from 'antd';
import * as React from 'react';
import ReactDOM from 'react-dom';
import { renderRoutes } from 'react-router-config';
const { Header, Content, Sider } = Layout;

export default class App extends React.Component<any, any> {
  // shouldComponentUpdate() {
  //   return false
  // }
  body: HTMLDivElement;
  setHeight() {
    if (this.body) {
      const content: HTMLDivElement = this.body.querySelector(".app-animate-content");
      if (content) {
        content.style.minHeight = (this.body.offsetHeight - 40) + "px";
      }
    }
  }
  componentDidMount() {
    this.setHeight();
  }
  componentDidUpdate() {
    this.setHeight();
  }
  render() {
      return (
        <Layout className="app-layout-body" ref={e => this.body = ReactDOM.findDOMNode(e) as any}>
          <Content className="app-layout-content">
            {renderRoutes(this.props.route.routes)}
          </Content>
        </Layout>
      );
  
  }
}

