import { BasicLayout, Settings, PageHeaderWrapper, GridContent } from '@ant-design/pro-layout';
import LayoutSpin from "components/other/LayoutSpin";
import GlobalConfig from 'global.config';
import lodash from 'lodash';
import { action, observable, toJS } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import { renderRoutes } from 'react-router-config';
import { Link } from 'react-router-dom';
import Store from 'store/index';
import RightContent from './GlobalHeader/RightContent';
import SettingDrawer from './SettingDrawer';
import './style.less';
@observer
export default class App extends React.Component<any> {
    @observable
    settings: Settings = {
        navTheme: 'dark',
        layout: 'sidemenu',
        contentWidth: 'Fluid',
        fixedHeader: true,
        autoHideHeader: false,
        fixSiderbar: true,
        menu: {
            locale: true,
        },
        title: GlobalConfig.default.title,
        iconfontUrl: '',
    }
    @action.bound
    onSettingChange(settings) {
        this.settings = settings;
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
        console.log("TCL: App -> render -> ", this.props)
        // console.log("TCL: App -> render -> this.defaultOpenKeys", this.defaultOpenKeys)
        const settings = toJS(this.settings);
        // window['g_locale']='en-US'
        return (
            <>
                <BasicLayout
                    logo={GlobalConfig.default.logo}
                    {...settings}
                    rightContentRender={rightProps => (
                        <RightContent {...rightProps} changeLang={event => {
                            // window['g_locale'] = event.key
                        }} />

                    )}
                    // layout="topmenu"
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
                        // openKeys: undefined,
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
                    </GridContent>
                </BasicLayout>
                <SettingDrawer settings={settings} onSettingChange={this.onSettingChange} />
            </>
        );
    }
}
