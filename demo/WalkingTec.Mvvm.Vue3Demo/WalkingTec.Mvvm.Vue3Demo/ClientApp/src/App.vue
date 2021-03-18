<template>
  <a-config-provider :locale="locale">
    <Layout />
  </a-config-provider>
</template>
<script lang="ts">
import { Vue, Options } from "vue-property-decorator";
import en from "ant-design-vue/es/locale/en_US";
import zh from "ant-design-vue/es/locale/zh_CN";
import Layout from "./layouts/layout.vue";
@Options({ components: { Layout } })
export default class extends Vue {
  get locale() {
    return { en, zh }[this.$i18n.locale];
  }
  created() {
    // console.log("LENG ~ extends ~ created ~ this", this)
  }
}
</script>
<style lang="less">
.w-app {
  height: 100%;
}
</style>
