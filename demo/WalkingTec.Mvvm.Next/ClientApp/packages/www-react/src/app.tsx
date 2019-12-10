import { EntitiesTimeStore, EntitiesUserStore } from '@leng/public/src';
import { Icon, Layout, Menu, Spin } from 'antd';
import lodash from 'lodash';
import { Provider } from 'mobx-react';
import * as React from 'react';
import { BrowserRouter, Link } from 'react-router-dom';
import RenderRoutes from './router';
const { Header, Content, Footer, Sider } = Layout;

const RootStore = {
    UserStore: new EntitiesUserStore(),
    TimeStore: new EntitiesTimeStore()
}
export default class App extends React.Component<any> {
    componentDidMount() {
    }
    public render() {
        return (
            <Provider {...RootStore}>
                <BrowserRouter>
                    <AppLayout />
                </BrowserRouter>
            </Provider>
        );
    }
}
class AppLayout extends React.Component<any> {
    public render() {
        console.log("TCL: App -> componentDidMount -> this.props", this.props)
        return (
            <Layout style={{ minHeight: '100vh' }}>
                <Sider
                    breakpoint="lg"
                    collapsedWidth="0"
                    onBreakpoint={broken => {
                        console.log(broken);
                    }}
                    onCollapse={(collapsed, type) => {
                        console.log(collapsed, type);
                    }}
                >
                    <div className="logo" />
                    <Menu theme="dark" mode="inline" defaultSelectedKeys={[window.location.pathname]}>
                        <Menu.Item key="/">
                            <Link to="/">
                                <Icon type="user" />
                                <span className="nav-text">Home</span>
                            </Link>
                        </Menu.Item>
                        <Menu.Item key="/about">
                            <Link to="/about">
                                <Icon type="user" />
                                <span className="nav-text">About</span>
                            </Link>
                        </Menu.Item>
                    </Menu>
                </Sider>
                <Layout>
                    <Header style={{ background: '#fff', padding: 0 }} />
                    <Content style={{ margin: '24px 16px 0' }}>
                        <React.Suspense fallback={<div style={{
                            width: '100%',
                            height: '100vh',
                            display: 'flex',
                            justifyContent: 'center',
                            alignItems: 'center',
                        }}><Spin size="large" tip="loading..." indicator={<Icon type="loading" spin />} /></div>}>
                            {RenderRoutes}
                        </React.Suspense>
                    </Content>
                    <Footer style={{ textAlign: 'center' }}>Ant Design ©2018 Created by Ant UED</Footer>
                </Layout >
            </Layout >
        );
    }
}
