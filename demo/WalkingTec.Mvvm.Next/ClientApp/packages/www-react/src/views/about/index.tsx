import { EntitiesTimeStore, EntitiesUserStore } from '@leng/public/src';
import lodash from 'lodash';
import { inject, observer } from 'mobx-react';
import * as React from 'react';
import { message } from 'antd';
import { BindAll } from 'lodash-decorators';
export interface IAppProps {
    TimeStore?: EntitiesTimeStore,
}
@inject('TimeStore')
@observer
@BindAll()
export default class App extends React.Component<IAppProps> {
    componentDidMount() {
        console.log("TCL: App -> componentDidMount -> this.props", this.props)
        this.onToggleTime()
    }
    componentWillUnmount(){
        this.onToggleTime()
    }
    onToggleTime() {
        if (this.props.TimeStore.onToggleTime()) {
            message.success('计时开始')
        }else{
            message.error('计时结束')
        }
    }
    public render() {
        return (
            <div>
                <h1>{this.props.TimeStore.currentTime}</h1>
                <button onClick={this.onToggleTime}>切换计时</button>
            </div>
        );
    }
}
