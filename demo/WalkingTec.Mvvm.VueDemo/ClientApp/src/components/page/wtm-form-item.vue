<template>
  <el-form-item ref="elItem" v-bind="$attrs">
    <slot />
  </el-form-item>
</template>

<script lang='ts'>
import { Component, Vue } from "vue-property-decorator";

@Component
export default class WtmFormItem extends Vue {
    created() {}
    showError(msg) {
        this.$refs["elItem"].clearValidate();
        if (msg) {
            this.$refs["elItem"].validateMessage = msg;
            this.$refs["elItem"].validateState = "error";
        }
    }
}
</script>
