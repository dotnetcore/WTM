import { BasicLayout, GridContent } from '@ant-design/pro-layout';
import { ConfigProvider, Icon, Tabs } from 'antd';
import zhCN from 'antd/lib/locale-provider/zh_CN';
import LayoutSpin from "components/other/LayoutSpin";
import GlobalConfig from 'global.config';
import lodash from 'lodash';
import { action, toJS, observable, runInAction } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import { MenuProvider, Menu, Item } from 'react-contexify';
import { matchRoutes, renderRoutes } from 'react-router-config';
import { Link } from 'react-router-dom';
import { fromEvent } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import Store from 'store/index';
import RightContent from './GlobalHeader/RightContent';
import { BrowserRouter } from 'react-router-dom';
import SettingDrawer from './SettingDrawer';
import './style.less';
@observer
export default class App extends React.Component<any> {
    @action.bound
    onSettingChange(settings) {
        GlobalConfig.settings = settings;
    }
    renderRoutes = renderRoutes(this.props.route.routes);
    defaultOpenKeys = [];
    selectedKeys = [];
    getDefaultOpenKeys(Menus, Menu, OpenKeys = []) {
        const ParentId = lodash.get(Menu, 'ParentId');
        if (ParentId) {
            OpenKeys.push(ParentId);
            const Parent = lodash.find(Menus, ["Id", ParentId]);
            if (Parent.ParentId) {
                this.getDefaultOpenKeys(Menus, Parent, OpenKeys);
            }
        }
        return OpenKeys
    }
    getMenu(pathname) {
        const menu = lodash.find(Store.Meun.ParallelMenu, ["path", pathname]);
        this.selectedKeys = [lodash.get(menu, 'Id', '/')]
        return menu;
    }
    UNSAFE_componentWillMount() {
        this.defaultOpenKeys = this.getDefaultOpenKeys(Store.Meun.ParallelMenu, this.getMenu(this.props.location.pathname));
    }
    UNSAFE_componentWillUpdate(nextProps) {
        if (this.props.location.pathname !== nextProps.location.pathname) {
            this.defaultOpenKeys = this.getDefaultOpenKeys(Store.Meun.ParallelMenu, this.getMenu(nextProps.location.pathname));
        }
    }
    public render() {
        const settings = toJS(GlobalConfig.settings);
        // window['g_locale']='en-US'
        return (
            <ConfigProvider locale={zhCN}>
                <BasicLayout
                    logo={GlobalConfig.default.logo}
                    {...settings}
                    rightContentRender={rightProps => (
                        <RightContent {...rightProps} changeLang={event => {
                            // window['g_locale'] = event.key
                        }} />

                    )}
                    menuHeaderRender={(logo, title) => <Link to="/">{logo}{title}</Link>}
                    menuDataRender={() => toJS(Store.Meun.subMenu)}
                    menuItemRender={(menuItemProps, defaultDom) => {
                        if (menuItemProps.isUrl && lodash.startsWith(menuItemProps.path, '/external')) {
                            defaultDom = <Link to={menuItemProps.path}>{lodash.get(defaultDom, 'props.children')}</Link>
                        }
                        return menuItemProps.isUrl ? defaultDom : <Link to={menuItemProps.path}>{defaultDom}</Link>;
                    }}
                    menuProps={{
                        selectedKeys: this.selectedKeys,
                        openKeys: this.defaultOpenKeys,
                        onOpenChange: event => {
                            this.defaultOpenKeys = [...event];
                            this.forceUpdate();
                        }
                    }}
                    footerRender={() => null}
                >
                    <GridContent {...settings}>

                        <React.Suspense fallback={<LayoutSpin />}>
                            {this.renderRoutes}
                        </React.Suspense>
                        {/* <TabsPages {...this.props} /> */}
                    </GridContent>
                </BasicLayout>
                <SettingDrawer settings={settings} onSettingChange={this.onSettingChange} />
            </ConfigProvider>
        );
    }
}


/**
 * tabs 页面布局
 */
@observer
class TabsPages extends React.Component<any, any> {
    TabsPagesStore = new TabsPagesStore(this.props.route.routes);
    componentDidMount() {
        this.TabsPagesStore.pushTabPane(this.props.location.pathname);
    }
    componentWillUnmount() {
        this.TabsPagesStore.componentWillUnmount();
    }
    componentDidUpdate() {
        this.TabsPagesStore.pushTabPane(this.props.location.pathname);
        // console.log("TCL: TabsPages -> componentDidUpdate -> this.props", this.props)
    }
    onChange(event) {
        if (!lodash.eq(this.props.location.pathname, event)) {
            this.props.history.replace(event)
        }
    }
    onEdit(event) {
        const path = this.TabsPagesStore.onClosable(event);
        if (lodash.eq(this.props.location.pathname, event)) {
            this.onChange(path)
        }
    }
    /**
     * 关闭
     * @param type 其他，当前，全部 
     * @param event 
     */
    onClose(type: "Other" | "Current" | "All", event) {
        this.onChange(this.TabsPagesStore.onClose(type, event))
    }
    getDisabled(event) {
        return event.props.pathname === '/'
    }
    render() {
        const tabPane = this.TabsPagesStore.tabPane;
        const height = this.TabsPagesStore.height;
        return (
            <>
                <Tabs
                    activeKey={this.props.location.pathname}
                    // size="small"
                    className="app-layout-tabs"
                    tabPosition={lodash.get(GlobalConfig, "tabPosition", "top") as any}
                    onChange={this.onChange.bind(this)}
                    animated={GlobalConfig.tabsAnimated}
                    type="editable-card"
                    onEdit={this.onEdit.bind(this)}
                >
                    {tabPane.map(item => {
                        const router = item.router;
                        const props = { ...this.props, match: router.match };
                        return <Tabs.TabPane
                            tab={<MenuProvider id="TabPane" component="span" data={item}>{renderIconTitle({ Icon: item.icon, Text: item.title })}</MenuProvider>}
                            key={item.pathname}
                            closable={item.closable}
                            style={{ height: height }}>
                            <React.Suspense fallback={<LayoutSpin />}>
                                {React.createElement(router.component, props)}
                            </React.Suspense>
                        </Tabs.TabPane>
                    })}
                </Tabs>
                <Menu id='TabPane' animation="pop" style={{ minWidth: 100 }}>
                    <Item disabled={this.getDisabled} onClick={this.onClose.bind(this, 'Current')}><span><Icon type="close" /></span> 关闭当前</Item>
                    <Item onClick={this.onClose.bind(this, 'Other')}><span><Icon type="close" /></span> 关闭其他</Item>
                    <Item onClick={this.onClose.bind(this, 'All')}><span><Icon type="close" /></span> 关闭全部</Item>
                </Menu>
            </>
        );
    }
}
class TabsPagesStore {
    constructor(private routes) {
    }
    componentWillUnmount() {
        this.resize.unsubscribe();
    }
    @observable height = this.getHeight();
    @observable tabPane = [{
        title: '首页',
        pathname: "/",
        closable: false,
        icon: "home",
        router: this.getRoutes("/")
    }];
    @action
    pushTabPane(pathname) {
        const router = this.getRoutes(pathname);
        if (lodash.some(this.tabPane, item => lodash.eq(item.pathname, pathname))) return;
        const menu = lodash.find(Store.Meun.ParallelMenu, ['Url', pathname]);
        const title = lodash.get(menu, 'Text', "Null")
        const icon = lodash.get(menu, 'Icon', "appstore")
        this.tabPane.push({
            title: title,//renderIconTitle(menu || { Text: "NULL", Icon: "appstore", Id: Help.GUID() }),
            pathname,
            closable: true,
            icon,
            router
        });
    }
    @action
    onClosable(pathname) {
        const index = lodash.findIndex(this.tabPane, ['pathname', pathname]);
        const path = lodash.get(this.tabPane, `[${index - 1}].pathname`, "/");
        lodash.remove(this.tabPane, ['pathname', pathname]);
        return path
    }
    /**
    * 关闭
    * @param type 其他，当前，全部 
    * @param event 
    */
    @action
    onClose(type: "Other" | "Current" | "All", event) {
        let path = "/";
        switch (type) {
            case 'Other':
                this.tabPane = [{
                    title: '首页',
                    pathname: "/",
                    closable: false,
                    icon: "home",
                    router: this.getRoutes("/")
                }];
                this.pushTabPane(event.props.pathname);
                path = event.props.pathname;
                break;
            case 'Current':
                path = this.onClosable(event.props.pathname);
                break;
            case 'All':
                this.tabPane = [{
                    title: '首页',
                    pathname: "/",
                    closable: false,
                    icon: "home",
                    router: this.getRoutes("/")
                }];
                break;
        }
        return path;
    }
    getHeight() {
        return window.innerHeight - (lodash.some(["top", "bottom"], data => lodash.eq(data, GlobalConfig.tabPosition)) ? 90 : 50);
    }
    getRoutes(pathname) {
        const router = matchRoutes(this.routes, pathname);
        return {
            component: router[0].route.component,
            match: router[0].match
        }
    }
    resize = fromEvent(window, "resize").pipe(debounceTime(200)).subscribe(e => {
        const height = this.getHeight()
        if (this.height != height) {
            runInAction(() => this.height = height)
        }
    });
}
function renderIconTitle(menu) {
    let icon = null;
    icon = <Icon type={menu.Icon || 'appstore'} />
    return <>{icon}<span>{menu.Text}</span> </>
}
