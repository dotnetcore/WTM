import * as React from 'react';

 interface IAppProps {
    visible: boolean
}
/**
 * 控制组件 展示
 */
export  class Visible extends React.Component<IAppProps, any> {
    render() {
        if (this.props.visible) {
            return this.props.children;
        }
        return null;
    }
}
