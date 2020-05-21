// import 'echarts';
import * as React from 'react';
import { injectIntl, FormattedDate } from 'react-intl';
import En from './en';
import Zh from './zh';
class IApp extends React.Component<any, any> {
    public render() {
        switch (this.props.intl.locale) {
            case "en-US":
                return <En />
            default:
                return <Zh />
        }
    }
}
export default injectIntl(IApp)
