<template>
  <div class="n-input" :class="{'error': !!errorMsg}">
    <div class="datawrap">
      <div class="form-tle" :class="{'form-tle-active': strCss}">
        {{ title }}
      </div>
      <el-input v-bind="$attrs" clearable v-on="$listeners" @focus="onFocus" @blur="onBlur" @clear="onClear" />
      <div class="form-line" />
      <div class="form-error">
        {{ errorMsg }}
      </div>
    </div>
    <slot />
  </div>
</template>
<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class nInput extends Vue {
    @Prop({ default: "" })
    title: string;
    @Prop({ default: "" })
    errorMsg: string;

    strCss: boolean = false;
    onFocus(e) {
        console.log("onFocus");
        this.strCss = true;
    }
    onBlur(e) {
        if (e.target.value) {
            this.strCss = true;
        } else {
            this.strCss = false;
        }
        this.$emit("onIptBlur", e.target.value);
    }
    onClear() {
        this.strCss = false;
    }
}
</script>
<style lang="less">
@import "~@/assets/css/variable.less";
.n-input {
    width: 390vw * @vwUnit;
    margin-top: 30vw * @vwUnit;
    font-size: 16px;
    text-align: left;
    color: #c0c4cc;
    .datawrap {
        flex: 1;
        position: relative;
        .form-tle {
            font-size: 16px;
            transform: translateY(30vw * @vwUnit);
            transition: all 0.3s ease-in-out;
            &.form-tle-active {
                transform: translateY(0);
            }
        }
        .form-line {
            background: #fff;
            height: 1px;
        }
        .form-error {
            color: #ff5a53;
            visibility: hidden;
            position: absolute;
            bottom: -30vw * @vwUnit;
            transition: all 1s ease-in-out;
            // height: 22vw * @vwUnit;
            // margin-top: 8vw * @vwUnit;
        }
        .el-input__inner {
            background-color: rgba(255, 255, 255, 0);
            border: 0;
            padding: 0;
            color: #ffffff;
        }
        .el-input {
            font-size: 16px;
        }
    }
    &.error {
        .datawrap {
            .form-line {
                background: #ff5a53;
            }
            .form-error {
                visibility: inherit;
            }
        }
    }
}
</style>
