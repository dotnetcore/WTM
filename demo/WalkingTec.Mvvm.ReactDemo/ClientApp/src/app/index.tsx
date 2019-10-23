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
import { IntlProvider } from 'react-intl';
import { BrowserRouter } from 'react-router-dom';
import routes from './router';
@observer
export default class App extends React.Component<any> {
    public render() {
        return (
            <ConfigProvider locale={zhCN}>
                <IntlProvider locale="en">
                    <BrowserRouter >
                        {routes}
                    </BrowserRouter>
                </IntlProvider>
            </ConfigProvider>
        );
    }
}
