<template>
  <div class="app-login-form">
    <!-- <h1>WalkingTec MVVM</h1> -->
    <el-input v-model="username" class="form-item" placeholder="请输入账号" prefix-icon="el-icon-user" size="mini" />
    <el-input v-model="password" class="form-item" placeholder="请输入密码" prefix-icon="el-icon-lock" size="mini" show-password />
    <el-button class="form-item" size="mini" type="primary" :loading="isloading" @click="onSubmit">
      Log in
    </el-button>
  </div>
</template>
<script lang="ts">
import { mapState, mapMutations, mapActions } from "vuex";
import baseMixin from "@/mixin/base.ts";
import { Component, Vue } from "vue-property-decorator";

const mixin = {
    computed: {
        ...mapState({})
    },
    methods: {
        ...mapMutations({}),
        ...mapActions({
            postLogin: "postLogin"
        })
    }
};
@Component({
    mixins: [mixin, baseMixin]
})
export default class Index extends Vue {
    username = "";
    password = "";
    isloading = false;
    mounted() {}
    onSubmit() {
        this.isloading = true;
        const params = {
            userid: this.username,
            password: this.password
        };
        this["postLogin"](params)
            .then(() => {
                this.isloading = false;
                this["onHref"]("/index.html");
            })
            .catch(() => {
                this.isloading = false;
            });
    }
}
</script>
<style lang="less">
.app-login-form {
    width: 400px;
    box-sizing: border-box;
    color: rgba(0, 0, 0, 0.65);
    font-size: 14px;
    line-height: 1.5;
    font-feature-settings: "tnum";
    margin: 0px;
    padding: 0px;
    font-variant: tabular-nums;
    list-style: none;
    & > h1 {
        font-size: 50px;
        color: #40a9ff;
        text-align: center;
        margin-bottom: 20px;
    }
    .form-item {
        width: 100%;
        margin: 0px 0px 24px;
    }
}
</style>
