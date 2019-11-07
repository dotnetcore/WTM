/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:06:31
 * @modify date 2019-02-24 17:06:31
 * @desc [description]
 */
import { Button, Col, Divider, Drawer, Modal, Row, Spin } from 'antd';
import { DrawerProps } from 'antd/lib/drawer';
import { ModalProps } from 'antd/lib/modal';
import { DesError } from 'components/decorators';
import GlobalConfig from 'global.config';
import Hammer from 'hammerjs';
import lodash from 'lodash';
import { observer } from 'mobx-react';
import * as React from 'react';
import './style.less';
import { FormattedMessage } from 'react-intl';


class DndTitle extends React.Component<{ styleState: { left: number, top: number }, onUpdate?: (style: React.CSSProperties) => void }, any> {
    deltaX = 0;
    deltaY = 0;
    panstart = false;
    div = React.createRef<any>();
    onUpdate(x, y) {
        this.props.onUpdate({
            // transform: `translate(${x}px,${y}px)`
            left: x,
            top: y
        })
    }
    hammer;
    componentDidMount() {
        const hammer = this.hammer = new Hammer(this.div.current)
        hammer.on('panstart', (event) => {
            if (this.props.styleState.left && this.props.styleState.top) {
                this.deltaX = this.props.styleState.left;
                this.deltaY = this.props.styleState.top;
            }
            this.panstart = true;
        });
        hammer.on('panmove', (event) => {
            this.onUpdate(event.deltaX + this.deltaX, event.deltaY + this.deltaY)
        });
        hammer.on('panend', (event) => {
            this.panstart = false
        });
    }
    componentWillUnmount() {
        this.hammer && this.hammer.destroy()
    }
    shouldComponentUpdate() {
        return !this.panstart
    }
    render() {
        return (
            <div className="modal-dnd-title" ref={this.div}>
                {this.props.children}
            </div>
        );
    }
}

/**
 *  详情 窗口 
 *  根据 类型 显示不同的 窗口
 */
@DesError
export class InfoShell extends React.Component<DrawerProps | ModalProps, any> {
    state = {
        style: {}
    }
    render() {
        if (GlobalConfig.settings.infoType === "Modal") {
            const onClose = (this.props as DrawerProps).onClose
            const onCancel = (e) => { onClose && onClose(e) }
            return <Modal
                width={GlobalConfig.infoTypeWidth}
                maskClosable={false}
                destroyOnClose
                onCancel={onCancel}
                {...this.props as any}
                style={this.state.style}
                title={
                    <DndTitle styleState={this.state.style as any} onUpdate={style => this.setState({ style })}>
                        {this.props.title}
                    </DndTitle>
                }
                footer={<div className="data-view-modal-footer"></div>}
                className={` ${this.props.className}`}
                wrapClassName="data-view-modal"
            >
                {this.props.children}
            </Modal>
        }
        const onCancel = (this.props as ModalProps).onCancel
        const onClose = (e) => { onCancel && onCancel(e) }
        return <Drawer
            width={GlobalConfig.infoTypeWidth}
            onClose={onClose}
            destroyOnClose
            {...this.props as any}
            className={`data-view-drawer ${this.props.className}`}>
            {this.props.children}
        </Drawer>
    }
}
/**
 * Items 外壳 布局
 */
@DesError
@observer
export class InfoShellLayout extends React.Component<{ loading?: boolean }, any> {
    render() {
        const childrens = React.Children.toArray(this.props.children).map((node: any) => {
            try {
                // console.log(lodash.has(node.props, "label"), node.props)
                // 没有嵌套 col 的自动添加 嵌套的 解除
                // if (["fieId", "models", "labelCol", "wrapperCol", "label", "hasFeedback"].some(x => lodash.has(node.props, x))) {
                if (lodash.isEqual(node.type.wtmType, 'FormItem') || ["labelCol", "wrapperCol", "label", "hasFeedback"].some(x => lodash.has(node.props, x))) {
                    const layout = lodash.get(node, "props.layout");
                    const hidden = lodash.get(node, "props.hidden");
                    if (hidden) {
                        return node
                    }
                    return <InfoShellCol
                        key={node.key}
                        layout={layout}
                    >
                        {node}
                    </InfoShellCol>
                }
                return node
            } catch (error) {
                return node
            }
        });
        const spinning = lodash.get(this.props, 'loading', false);
        return <div className="data-view-form-item">
            <Spin tip="Loading..." spinning={lodash.get(this.props, 'loading', false)} key={lodash.toString(spinning)}>
                    {childrens}
            </Spin>
        </div>
    }
}
/**
 * Items 外壳 底部按钮
 */
@DesError
@observer
export class InfoShellFooter extends React.Component<{ btns?: React.ReactNode, loadingEdit?: boolean, onCancel?: () => void, submit?: boolean }, any> {
    render() {
        return <>
            <InfoShellLayout loading={this.props.loadingEdit}>
                {this.props.children}
            </InfoShellLayout>
            {this.props.btns ? this.props.btns : <div className="data-view-form-btns" >
                <Button onClick={() => this.props.onCancel && this.props.onCancel()} ><FormattedMessage id="action.close" /></Button>
                {this.props.submit && <>
                    <Divider type="vertical" />
                    <Button loading={this.props.loadingEdit} type="primary" htmlType="submit"  ><FormattedMessage id="action.submit" /></Button>
                </>}
            </div>}

        </>
    }
}
/**
 * Items 外壳
 */
export class InfoShellCol extends React.Component<{ layout?: string }, any> {
    columnCount = GlobalConfig.infoColumnCount || 1;
    render() {
        let colSpan = 24;
        if (this.props.layout != "row") {
            colSpan = 24 / this.columnCount;//每列 值
        }
        return <Col lg={colSpan} md={12} sm={24} xs={24} {...this.props}>
            {this.props.children}
        </Col>
    }
}

/**
* 转换 value 值
* @param value value 值
* @param showType 转换类型 span 情况下 非基础类型。需要格式为字符串。不然 react 会报错
*/
export function toValues(value: any, showType: "value" | "span" = "value") {
    // 检查 value 是否是 null 或者 undefined。
    if (lodash.isNil(value)) {
        return
    }
    // 检查数字类型 
    // if (lodash.isNumber(value)) {
    //     return lodash.toString(value)
    // }
    // 检查其他
    if (lodash.isArray(value) || lodash.isObject(value)) {
        return lodash.toString(value)
    }
    return value
}