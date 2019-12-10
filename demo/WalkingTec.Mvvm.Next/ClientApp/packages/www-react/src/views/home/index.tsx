import { EntitiesUserStore } from '@leng/public/src';
import { inject, observer } from 'mobx-react';
import * as React from 'react';
import Login from './login';
import UserInfo from './user';
export interface IAppProps {
    UserStore?: EntitiesUserStore,
}
@inject('UserStore')
@observer
export default class App extends React.Component<IAppProps> {
    componentDidMount() {
        console.log("TCL: App -> componentDidMount", this.props)
    }
    UNSAFE_componentWillUpdate(){
        console.log('TCL: App -> componentWillUpdate')
    }
    public render() {
        if (this.props.UserStore.OnlineState) {
            return <UserInfo />;
        }
        return <Login />
    }
}