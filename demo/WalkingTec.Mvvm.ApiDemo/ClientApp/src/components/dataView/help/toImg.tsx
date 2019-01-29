import { Icon, Modal } from 'antd';
import * as React from 'react';
import RequestFiles from 'utils/RequestFiles';
import Viewer from 'viewerjs';
import './toImg.less';
interface IAppProps {
    fileID: String;
    hideDownload?: boolean;
    style?: React.CSSProperties;
}
/**
 * 控制组件 展示
 */
export class ToImg extends React.Component<IAppProps, any> {
    componentDidMount() {
        if (this.img.current) {
            this.viewer = new Viewer(this.img.current)
        }
    }
    componentWillUnmount() {
        this.viewer && this.viewer.destroy();
    }
    img = React.createRef<any>();
    viewer: Viewer;
    render() {
        if (this.props.fileID) {
            const src = RequestFiles.onFileUrl(this.props.fileID)
            return <div className="app-to-img" style={this.props.style} >
                <img className="app-to-image" src={src} ref={this.img} />
                <div className="app-to-img-hove">
                    <a key='url' target="_blank" onClick={e => { this.viewer && this.viewer.show() }} ><Icon type="eye" /></a>
                    {this.props.hideDownload ? null : <a key='download' href={RequestFiles.onFileDownload(this.props.fileID)}><Icon type="cloud-download" /></a>}
                </div>
            </div>
        }
        return <div className="app-to-img" style={this.props.style} key="app-to-img"></div>
    }
}
