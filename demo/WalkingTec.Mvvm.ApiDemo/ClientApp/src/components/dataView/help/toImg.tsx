import { Icon, Modal } from 'antd';
import * as React from 'react';
import RequestFiles from 'utils/RequestFiles';
import './toImg.less';
interface IAppProps {
    fileID: String;
    style?: React.CSSProperties;
}
/**
 * 控制组件 展示
 */
export class ToImg extends React.Component<IAppProps, any> {
    state = {
        visible: false
    }
    renderImg() {
        // if (this.props.url) {
        const { visible } = this.state;
        const src = RequestFiles.onFileUrl(this.props.fileID)
        return <>
            <img className="app-to-image" src={src} />
            <Modal key={src} width={"1000px"} visible={visible} footer={null} onCancel={() => this.setState({ visible: false })}>
                <img alt="example" style={{ maxWidth: "100%", maxHeight: "80vh", margin: "auto",display:"block" }} src={src} />
            </Modal>
        </>
        // }
    }
    renderBtn() {
        let btns = [];
        // if (this.props.url) {
        btns.push(<a key='url' target="_blank" onClick={e => this.setState({ visible: true })} ><Icon type="eye" /></a>)
        // }
        // if (this.props.download) {
        btns.push(<a key='download' href={RequestFiles.onFileDownload(this.props.fileID)}><Icon type="cloud-download" /></a>)
        // }
        return btns
    }
    render() {
        if (this.props.fileID) {
            return <div className="app-to-img" style={this.props.style} >
                {this.renderImg()}
                <div className="app-to-img-hove">
                    {this.renderBtn()}
                </div>
            </div>
        }
        return <div className="app-to-img" style={this.props.style} key="app-to-img"></div>
    }
}
