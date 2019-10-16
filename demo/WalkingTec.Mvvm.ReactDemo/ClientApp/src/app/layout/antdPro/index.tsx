import { BasicLayout, SettingDrawer } from '@ant-design/pro-layout';
import { Skeleton } from 'antd';
import GlobalConfig from 'global.config';
import * as React from 'react';
import { renderRoutes } from 'react-router-config';
import RightContent from './GlobalHeader/RightContent';
import LayoutSpin from "components/other/LayoutSpin";

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
                    footerRender={() => null}
                >
                    <React.Suspense fallback={<LayoutSpin/>}>
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
