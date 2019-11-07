/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2018-07-24 05:02:33
 * @modify date 2018-07-24 05:02:33
 * @desc [description]
 */
import { ConfigProvider } from 'antd';
import GlobalConfig from 'global.config';
import { getConfigProvider, getLocales } from 'locale';
import { observer } from 'mobx-react';
import * as React from 'react';
import { IntlProvider } from 'react-intl';
import { BrowserRouter } from 'react-router-dom';
import routes from './router';
@observer
export default class App extends React.Component<any> {
    public render() {
        const { language } = GlobalConfig;
        return (
            <ConfigProvider locale={getConfigProvider(language)}>
                <IntlProvider locale={language} messages={getLocales(language)}>
                    <BrowserRouter >
                        {routes}
                    </BrowserRouter>
                </IntlProvider>
            </ConfigProvider>
        );
    }
}
