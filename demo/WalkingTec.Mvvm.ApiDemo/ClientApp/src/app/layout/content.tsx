
import { Layout, Tabs } from 'antd';
import * as React from 'react';
import ReactDOM from 'react-dom';
import { renderRoutes } from 'react-router-config';
import { Subscription } from 'rxjs';
const { Header, Content, Sider } = Layout;
const TabPane = Tabs.TabPane;
export default class App extends React.Component<any, any> {
  minHeight = 0;
  body: HTMLDivElement;
  setHeight() {
    if (this.body) {
      let content: HTMLDivElement = this.body.querySelector(".app-animate-content");
      if (!content) {
        content = this.body.querySelector(".antd-pro-exception-exception");
      }
      if (content) {
        content.style.minHeight = (this.body.offsetHeight - 20) + "px";
      }
    }
  }
  resize: Subscription;
  componentDidMount() {
    this.setHeight();
  }
  componentWillUnmount() {
  }
  componentDidUpdate() {
    this.setHeight();
  }
  renderRoutes = renderRoutes(this.props.route.routes);
  render() {
    return (
      <Layout tagName="main" className="app-layout-body" ref={e => this.body = ReactDOM.findDOMNode(e) as any}>
        <Content tagName="main" className="app-layout-content" >
          {this.renderRoutes}
        </Content>
      </Layout>
    );

  }
}