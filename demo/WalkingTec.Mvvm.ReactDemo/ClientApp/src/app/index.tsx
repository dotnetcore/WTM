/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-07-24 05:02:33
 * @modify date 2018-07-24 05:02:33
 * @desc [description]
 */
import { ConfigProvider } from 'antd';
import zhCN from 'antd/lib/locale-provider/zh_CN';
import { observer } from 'mobx-react';
import * as React from 'react';
import { BrowserRouter } from 'react-router-dom';
import routes from './router';
@observer
export default class RootRoutes extends React.Component<any, any> {
    render() {
        return (
            <ConfigProvider locale={zhCN}>
                <BrowserRouter >
                    {routes}
                </BrowserRouter>
            </ConfigProvider>
        );
    }

}