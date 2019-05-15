import * as React from 'react';
import code from './code';
import './style.less'
export default class IApp extends React.Component<{
    onSuccess: () => void;
}, any> {
    element = React.createRef<HTMLDivElement>()
    componentDidMount() {
        new code({
            el: this.element.current,
            onSuccess: this.props.onSuccess,
            onFail: () => { },
            onRefresh: () => { },
        }).init()
    }
    render() {
        return (
            <div ref={this.element}>

            </div>
        );
    }
}
