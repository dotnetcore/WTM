import { Skeleton } from 'antd';
import globalConfig from 'global.config';
import { action, observable } from 'mobx';
import { observer } from 'mobx-react';
import * as React from 'react';
import './style.less';
class MessageStore {
    constructor() {
        /**
         * 接收 子窗口 发送的 请求加载通知
         */
        window.addEventListener("message", (event) => {
            // if (event.data) {
            //     switch (event.data.type) {
            //         // 加载活动
            //         case "NProgress":
            //             if (event.data.data == "start") {
            //                 NProgress.start();
            //             } else {
            //                 NProgress.done();
            //             }
            //             break;
            //         // 消息通知
            //         case "notification":
            //             const { type, args } = event.data.data;
            //             try {
            //                 // 处理 发送的二进制 文件
            //                 if (args.blob) {
            //                     const filedow = Request.onCreateBlob(args.blob);
            //                     return notification[type]({
            //                         ...args,
            //                         btn: <Button type="primary" size="small" onClick={() => {
            //                             notification.close("import");
            //                             filedow.click();
            //                         }}>
            //                             下载文件
            //                 </Button>
            //                     });
            //                 }
            //                 notification[type](args);
            //             } catch (error) {
            //                 console.error("notification", error);
            //             }
            //             break;
            //         // 子页面弹框 显示
            //         case 'visible':
            //             if (typeof event.data.data === "boolean") {
            //                 runInAction(() => {
            //                     if (this.visible != event.data.data) {
            //                         this.visible = event.data.data
            //                     }
            //                 })
            //             }
            //         // 子页高度更改
            //         // case 'updateHeight':
            //         //     if (typeof event.data.data === "number") {
            //         //         runInAction(() => {
            //         //             this.onUpdateHeight(event.data.data)
            //         //         })
            //         //     }
            //         //     break;
            //         // default:
            //         //     break;
            //     }
            // }
        })
    }
    @observable visible = false;
    @observable iframeHeight = window.innerHeight;
    @action.bound
    onUpdateHeight(height) {
        if (this.iframeHeight != height) {
            this.iframeHeight = height
        }
    }
}
const MsgeStore = new MessageStore();
@observer
export default class IApp extends React.Component<any, any> {
    ref = React.createRef<any>()
    state = {
        loding: true
    }
    /**
     * 发送消息
     */
    sendPostMessage() {
        return {
            type: "Portal_Token",
            token: globalConfig.token.get(),
        }
    }
    componentDidMount() {

    }
    componentWillMount() {
    }
    onLoad(e) {
        // 发送消息
        // e.target.contentWindow.postMessage(this.sendPostMessage(), decodeURIComponent(this.props.match.params.url));
        console.log(decodeURIComponent(this.props.match.params.url), this.ref.current.contentWindow)
        this.setState({ loding: false })
    }
    render() {
        const src = decodeURIComponent(this.props.match.params.url)

        return (
            <>
                <iframe
                    ref={this.ref}
                    key={src}
                    src={src}
                    className={"app-external-iframe " + (MsgeStore.visible && "app-external-visible")}
                    onLoad={this.onLoad.bind(this)}
                >
                </iframe>
                {this.state.loding ? <div className="app-external-iframe-Skeleton">
                    <Skeleton paragraph={{ rows: 10 }} />
                </div> : null}
            </>
        );
    }
}
