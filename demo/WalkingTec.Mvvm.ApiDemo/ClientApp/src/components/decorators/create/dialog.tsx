/**
 * @author 冷 (https://github.com/LengYXin)
 * @email lengyingxin8966@gmail.com
 * @create date 2019-02-24 17:07:00
 * @modify date 2019-02-24 17:07:00
 * @desc [description]
 */
import * as React from 'react';
import { FormItem, InfoShell, InfoShellFooter } from '../../dataView';
import { DrawerProps } from 'antd/lib/drawer';
import { ModalProps } from 'antd/lib/modal';
import { DesError } from './error'
import { DesForm } from './form'
import { Form } from 'antd';
import ReactDOM from 'react-dom';
import { observable, computed } from 'mobx';
import { observer } from 'mobx-react';

interface IAppProps extends ModalProps {
    onSubmit?: (err, values) => void
}
export function DesModel(props?: IAppProps) {
    return (Component: React.ComponentClass<any, any>) => {
        return DesForm(class AppModel extends Component {
            onSubmit(e) {
                e.stopPropagation();
                e.preventDefault();
                this.props.form.validateFields(props.onSubmit);
            }
            onVisible(visible = !this.state.visible) {
                this.setState({ visible })
            }
            state = {
                ...this.state,
                visible: props.visible || true,
                loadingEdit: false,
                submit: true
            }
            render() {
                return <InfoShell
                    {...props}
                    {...this.state}
                    onCancel={this.onVisible.bind(this, false)}
                >
                    <Form onSubmit={this.onSubmit.bind(this)}>
                        {super.render()}
                    </Form>
                </InfoShell>
            }
        })
    }
}
// const DialogModelBody = document.createElement("div");
// document.body.append(DialogModelBody);
export function DialogModel(props?: IAppProps, Component?: React.ComponentClass) {
    // const Comp = DesForm(class AppModel extends React.Component<any, any> {
    //     onSubmit(e) {
    //         e.stopPropagation();
    //         e.preventDefault();
    //         this.props.form.validateFields(props.onSubmit);
    //     }
    //     onVisible(visible = !this.state.visible) {
    //         this.setState({ visible })
    //     }
    //     state = {
    //         ...this.state,
    //         visible: props.visible || true,
    //         loadingEdit: false,
    //         submit: true
    //     }
    //     render() {
    //         return <InfoShell
    //             {...props}
    //             {...this.state}
    //             onCancel={this.onVisible.bind(this, false)}
    //         >
    //             <Form onSubmit={this.onSubmit.bind(this)}>
    //                 {/* {React.cloneElement(, {})} */}
    //             </Form>
    //         </InfoShell>
    //     }
    // })
    class Store {
        @observable private __visible = false;
        @computed
        public get visible() {
            return this.__visible;
        }
        public set visible(value) {
            this.__visible = value;
        }
    }
    const ModelStore = new Store();
    @observer
    class AppModel extends React.Component<any, any> {
        onSubmit(e) {
            e.stopPropagation();
            e.preventDefault();
            this.props.form.validateFields(props.onSubmit);
        }
        render() {
            return <InfoShell
                {...props}
                visible={ModelStore.visible}
                onCancel={() => { ModelStore.visible = false }}
            >
                <Component />
            </InfoShell>
        }
    }
    // ReactDOM.render(<AppModel />, DialogModelBody,()=>{
    //     console.log("aaaaa")
    // })
    return {
        onVisible: (visible = !ModelStore.visible) => {
            ModelStore.visible = visible
        }
    }
}
