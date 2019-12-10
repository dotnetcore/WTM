import { ComponentType } from 'react'
import Taro, { Component, Config } from '@tarojs/taro'
import { View, Button, Text, Image } from '@tarojs/components'
import { observer, inject } from '@tarojs/mobx'

import './index.less'
import { EntitiesTimeStore, EntitiesUserStore } from '@leng/public/src'
type PageStateProps = {
  TimeStore: EntitiesTimeStore,
  UserStore: EntitiesUserStore,
}

interface Index {
  props: PageStateProps;
}

@inject('TimeStore', 'UserStore')
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
    navigationBarTitleText: '首页'
  }

  componentWillMount() { }

  componentWillReact() {
  }

  componentDidMount() {
    console.log('componentWillReact', this.props)
    // this.props.TimeStore.onToggleTime()
  }

  componentWillUnmount() { }

  componentDidShow() { }

  componentDidHide() { }
  onToggleTime = () => {
    const { TimeStore, UserStore } = this.props;
    let res: Taro.showToast.Param = {
      title: '开始计时',
      icon: 'none',
      duration: 2000
    }
    if (TimeStore.onToggleTime()) {

    } else {
      res.title = "结束计时"
    }
    Taro.showToast(res)
  }

  render() {
    const { TimeStore, UserStore } = this.props;
    const { currentTime } = TimeStore;
    const { OnlineState, Loading } = UserStore;
    if (OnlineState) {
      return <View className='index'>
        <View>
          <Text>Name：{UserStore.Name}</Text>
        </View>
        <View>
          <Text>Birthday：{UserStore.Birthday.toLocaleString()}</Text>
        </View>
        <View>
          <Text>Age：{UserStore.Age}</Text>
        </View>
        <View>
          <Image src={UserStore.Avatar} />
        </View>
        <View>
          <Text>Address：{UserStore.Address}</Text>
        </View>
        <Button onClick={() => { UserStore.onOutLogin() }}>退出登陆</Button>
      </View>
    }
    return (
      <View className='index'>
        <View>
          <Text>当前时间：{currentTime}</Text>
          <Button onClick={() => { this.onToggleTime() }}>切换计时</Button>
        </View>
        <Button loading={Loading} onClick={() => { UserStore.onLogin('aaaa', '000000') }}>登陆</Button>
      </View>
    )
  }
}

export default Index as ComponentType
