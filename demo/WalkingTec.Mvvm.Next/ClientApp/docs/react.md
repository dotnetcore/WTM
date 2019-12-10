# www react 程序
> 使用mobx 状态管理 管理组件状态，继承 entities 状态管理使用


## 父类实体
> packages/entities/user/index.ts

``` jsx
import { observable, action } from 'mobx';
/**
 * 用户实体
 */
export class EntitiesUserStore {
    constructor() {

    }
    @observable
    info = {
        name: "",
        age: 0,
        sex: true,
    }
    @observable
    name = "名字123";
    @action
    onUpdate(name) {
        this.name = name;
        console.log("TCL: UserStore -> onUpdate -> name", name)
    }
}
```

## react 子类实体/继承父类
> packages/www/src/store/user.ts

``` jsx
import { EntitiesUserStore } from '@leng/entities';
class Store extends EntitiesUserStore {
    // constructor(public a) {
    //     super()
    // }
    type = "React";
}
export default new Store();
```

## react 使用
> packages/www/src/pages/test/index.tsx

``` jsx
import { observer } from 'mobx-react';
import React from 'react';
import Logo from 'components/logo';
import { Link } from 'react-router-dom';

import './style.scss';
import User from 'store/user';
import Time from 'store/time';
// @observer
export default class App extends React.Component<any, any> {

  render() {
    console.log('render App')
    return (
      <div className="App">
        <header className="App-header">
          <Logo />
          <AppTime />
          <AppName />
          <button onClick={() => {
            User.onUpdate(`名字-${Math.random()}`)
          }}>更改</button>
          <Link to="/page" >跳转</Link>
          <Link to="/page/1" >跳转（参数）</Link>
        </header>
      </div>
    );
  }
}
@observer
class AppName extends React.Component<any, any> {
  componentWillUpdate() {
    console.log('componentWillUpdate')
  }
  UNSAFE_componentWillReceiveProps() {
    console.log('UNSAFE_componentWillReceiveProps')
  }
  UNSAFE_componentWillMount() {
    console.log('UNSAFE_componentWillMount')
  }
  UNSAFE_componentWillUpdate() {
    console.log('UNSAFE_componentWillUpdate')
  }
  componentDidMount() {
    console.log('componentDidMount')
  }
  componentDidUpdate() {
    console.log('componentDidUpdate')
  }
  componentWillMount() {
    console.log('componentWillMount')
  }
  componentWillReceiveProps() {
    console.log('componentWillReceiveProps')
  }
  componentWillUnmount() {
    console.log('componentWillUnmount')
  }
  componentDidCatch() {
    console.log('componentDidCatch')
  }
  render() {
    console.log('render AppName')
    return (
      <h1>{User.type}名字：{User.name}</h1>
    );
  }
}

@observer
export class AppTime extends React.Component<any, any> {
  render() {
    console.log('render AppTime')
    return (
      <h1>时间：{Time.currentTime}</h1>
    );
  }
}


```