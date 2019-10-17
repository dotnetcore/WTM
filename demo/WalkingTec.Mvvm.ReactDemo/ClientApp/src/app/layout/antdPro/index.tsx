import { BasicLayout } from '@ant-design/pro-layout';
import SiderMenu from '@ant-design/pro-layout/lib/SiderMenu/SiderMenu';
// import SiderMenu from '@ant-design/pro-layout/es/SiderMenu/SiderMenu';
import LayoutSpin from "components/other/LayoutSpin";
import GlobalConfig from 'global.config';
import * as React from 'react';
import { renderRoutes } from 'react-router-config';
import { Link } from 'react-router-dom';
import Store from 'store/index';
import lodash from 'lodash';
import RightContent from './GlobalHeader/RightContent';
import SettingDrawer from './SettingDrawer';
import { toJS } from 'mobx';
console.dir(SiderMenu)
// SiderMenu.prototype.handleOpenChange = function (openKeys) {
//     console.log("TCL: SiderMenu.prototype.handleOpenChange -> openKeys", openKeys)

// }
export default class App extends React.Component<any> {
    renderRoutes = renderRoutes(this.props.route.routes);
    public render() {
        return (
            <>
                <BasicLayout
                    logo={GlobalConfig.default.logo}
                    title={GlobalConfig.default.title}
                    fixedHeader
                    fixSiderbar
                    rightContentRender={rightProps => (
                        <RightContent {...rightProps} />
                    )}
                    menuDataRender={() => toJS(Store.Meun.subMenu)}
                    menuItemRender={(menuItemProps, defaultDom) => {
                        if (menuItemProps.isUrl && lodash.startsWith(menuItemProps.path, '/external')) {
                            defaultDom = <Link to={menuItemProps.path}>{lodash.get(defaultDom, 'props.children')}</Link>
                        }
                        return menuItemProps.isUrl ? defaultDom : <Link to={menuItemProps.path}>{defaultDom}</Link>;
                    }}
                    menuProps={{
                        // openKeys: undefined,
                        // onOpenChange: event => {
                        //     console.log("TCL: App -> render -> event", event)
                        // }
                    }}
                    footerRender={() => null}
                >
                    <React.Suspense fallback={<LayoutSpin />}>
                        {this.renderRoutes}
                    </React.Suspense>
                </BasicLayout>
                <SettingDrawer settings={{
                    navTheme: 'dark',
                    layout: 'sidemenu',
                    contentWidth: 'Fluid',
                    fixedHeader: false,
                    autoHideHeader: false,
                    fixSiderbar: false,
                    menu: {
                        locale: true,
                    },
                    title: 'Ant Design Pro',
                    iconfontUrl: '',
                }} />
            </>
        );
    }
}
