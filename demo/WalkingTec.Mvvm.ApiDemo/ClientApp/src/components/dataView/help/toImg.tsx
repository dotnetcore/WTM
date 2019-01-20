import { Icon, Modal } from 'antd';
import * as React from 'react';
import './toImg.less';
export interface IAppProps {
    url?: string;
    download?: string;
    style?: React.CSSProperties;
}
/**
 * 控制组件 展示
 */
export default class IApp extends React.Component<IAppProps, any> {
    state = {
        visible: false
    }
    renderImg() {
        if (this.props.url) {
            const { visible } = this.state;
            return <>
                <img className="app-to-image" src={this.props.url} />
                <Modal key={this.props.url} visible={visible} footer={null} onCancel={() => this.setState({ visible: false })}>
                    <img alt="example" style={{ width: '100%' }} src={this.props.url} />
                </Modal>
            </>
        }
    }
    renderBtn() {
        let btns = [];
        if (this.props.url) {
            btns.push(<a key='url' target="_blank" onClick={e => this.setState({ visible: true })} ><Icon type="eye" /></a>)
        }
        if (this.props.download) {
            btns.push(<a key='download' href={this.props.download}><Icon type="cloud-download" /></a>)
        }
        return btns
    }
    render() {
        if (this.props.url || this.props.download) {
            return <div className="app-to-img" key={this.props.url || this.props.download} style={this.props.style} >
                {this.renderImg()}
                <div className="app-to-img-hove">
                    {this.renderBtn()}
                </div>
            </div>
        }
        return <div className="app-to-img" style={this.props.style} key="app-to-img"></div>
    }
}
