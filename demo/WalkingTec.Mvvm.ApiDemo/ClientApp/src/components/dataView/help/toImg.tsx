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
const img = new Image();
const viewer = new Viewer(img)
/**
 * 控制组件 展示
 */
export class ToImg extends React.Component<IAppProps, any> {
    componentDidMount() {

    }
    componentWillUnmount() {

    }
    render() {
        if (this.props.fileID) {
            const src = RequestFiles.onFileUrl(this.props.fileID)
            return <div className="ant-upload-list ant-upload-list-picture-card app-to-img" >
                <div className="ant-upload-list-item ant-upload-list-item-done">
                    <div className="ant-upload-list-item-thumbnail">
                        <img className="app-to-image" src={src} />
                    </div>
                    <div className="app-img-hove">
                        <a key='url' target="_blank" onClick={e => {
                            img.src = src
                            viewer.show()
                        }} ><Icon type="eye" /></a>
                        {this.props.hideDownload ? null : <a key='download' href={RequestFiles.onFileDownload(this.props.fileID)}><Icon type="cloud-download" /></a>}
                    </div>
                </div>
            </div>
        }
        return <div className="ant-upload-list ant-upload-list-picture-card app-to-img"  key="app-to-img"></div>
    }
}
