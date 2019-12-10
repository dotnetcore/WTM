# mobile 小程序端
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

## mobile 子类实体/继承父类
> packages/mobile/src/store/user.ts

``` jsx
import { EntitiesUserStore } from '@leng/entities';
class UserStore extends EntitiesUserStore {
    constructor() {
        super()
    }
    type = "WeChat";
}
export default new UserStore();
```

## mobile 使用
> packages/mobile/src/pages/index/index.tsx

``` jsx
import { Button, Image, View } from '@tarojs/components';
import { observer } from '@tarojs/mobx';
import Taro, { Component, Config } from '@tarojs/taro';
import User from '../../store/user';
import Time from '../../store/time';
import { ComponentType } from 'react';
import img from '../../img';
import './index.less';
@observer
class Index extends Component {

  /**
   * 指定config的类型声明为: Taro.Config
   *
   * 由于 typescript 对于 object 类型推导只能推出 Key 的基本类型
   * 对于像 navigationBarTextStyle: 'black' 这样的推导出的类型是 string
   * 提示和声明 navigationBarTextStyle: 'black' | 'white' 类型冲突, 需要显示声明类型
   */
  config: Config = {
    navigationBarTitleText: '首页2'
  }

  componentWillMount() { }

  componentWillReact() {
    console.log('componentWillReact')
  }

  componentDidMount() { }

  componentWillUnmount() { }

  componentDidShow() { }

  componentDidHide() { }

  goTo = () => {
    Taro.navigateTo({ url: "/pages/test/index" })
  }
  onUpdate() {
    User.onUpdate(`名字${Math.random()}`)
  }
  render() {
    const { type, name } = User;
    const { currentTime } = Time;
    return (
      <View className='index'>
        <View>{type}名字：{name}</View>
        <Button onClick={this.onUpdate}>更改</Button>
        <View>{currentTime}</View>
        <Button onClick={this.goTo}>跳转</Button>
        <Image src={img.wx} />
      </View>
    )
  }
}

export default Index as ComponentType
```