# web vue 程序
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

## vue 子类实体/继承父类
> packages/web/src/store/user.ts

``` jsx
import { EntitiesUserStore } from '@leng/entities';
class Store extends EntitiesUserStore {
    constructor() {
        super()
    }
    type = "Vue";
}
export default new Store();
```

## vue 使用
> packages/web/src/pages/home/index.vue

``` jsx
<template>
  <div class="home">
    <img alt="Vue logo" src="@/assets/logo.png">
    <HelloWorld msg="Welcome to Your Vue.js + TypeScript App"/>
    <h1>时间：{{Time.currentTime}}</h1>
    <h1>{{User.type}}{{User.name}}</h1>
    <button @click="onUpdate">更改</button>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { Observer } from "mobx-vue";
import User from "../../store/user";
import Time from "../../store/time";
import HelloWorld from "@/components/HelloWorld.vue"; // @ is an alias to /src
@Observer
@Component({
  components: {
    HelloWorld
  }
})
export default class Home extends Vue {
  User = User;
  Time = Time;
  mounted() {
    console.log("TCL: Home -> mounted", this);
  }
  onUpdate() {
    this.User.onUpdate(`测试${Math.random()}`);
  }
}
</script>

```