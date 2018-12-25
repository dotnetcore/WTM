import * as React from 'react';

export interface IAppProps {
    visible: boolean
}
/**
 * 控制组件 展示
 */
export default class IApp extends React.Component<IAppProps, any> {
    render() {
        if (this.props.visible) {
            return this.props.children;
        }
        return null;
    }
}
