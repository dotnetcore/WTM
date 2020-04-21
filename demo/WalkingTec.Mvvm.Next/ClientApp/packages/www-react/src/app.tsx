import { EntitiesTimeStore, EntitiesUserStore, EntitiesPageStore } from '@leng/public/src';
import { Layout, Menu, Spin } from 'antd';
import { BasicLayout, SettingDrawer } from '@ant-design/pro-layout';
import lodash from 'lodash';
import { Provider, observer } from 'mobx-react';
import * as React from 'react';
import { BrowserRouter, Link } from 'react-router-dom';
import RenderRoutes from './router';
import { toJS } from 'mobx';
const { Header, Content, Footer, Sider } = Layout;

const RootStore = {
    UserStore: new EntitiesUserStore(),
    TimeStore: new EntitiesTimeStore(),
    PageStore: new EntitiesPageStore({
        target: '/api',
        Search: { url: '/_FrameworkUserBase/Search', },
        Details: { url: '/_FrameworkUserBase/{id}' },
        Insert: { url: '/_FrameworkGroup/Add' },
        Update: { url: '/_FrameworkGroup/Edit' },
        Delete: { url: '/_FrameworkGroup/BatchDelete' },
        Export: { url: '/_FrameworkGroup/ExportExcel', body: {} },
    }),
}
RootStore.UserStore.onCheckLogin()
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
@observer
class AppLayout extends React.Component<any> {
    public render() {
        const { MenuTrees } = RootStore.UserStore;
        return (
            <>
                <BasicLayout menuDataRender={() => toJS(MenuTrees)}>
                    <React.Suspense fallback={<div style={{
                        width: '100%',
                        height: '100vh',
                        display: 'flex',
                        justifyContent: 'center',
                        alignItems: 'center',
                    }}><Spin size="large" tip="loading..." /></div>}>
                        {RenderRoutes}
                    </React.Suspense>
                </BasicLayout>
                <SettingDrawer settings={{}} />
            </>
        )
    }
}
