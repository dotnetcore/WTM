import { ImportModal } from 'components/dataView';
import * as React from 'react';
import Store from '../store';

/**
 * 其他弹框类组件
 */
export default class extends React.Component<any, any>{
    shouldComponentUpdate() {
        return false
    }
    render() {
        return <React.Fragment key="page-other">
            <ImportModal Store={Store} />
        </React.Fragment>
    }
}