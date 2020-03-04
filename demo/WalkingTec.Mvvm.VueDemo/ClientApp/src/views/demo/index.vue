<template>
  <div>

    测试tsx:
    <el-button type="primary" @click="onTest">onTest</el-button>
    <form-maker :status="status" />

  </div>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import { AppModule } from "@/store/modules/app";
import FormMaker from "@/components/page/FormMaker/index";

@Component({
    name: "demo",
    components: {
        FormMaker
    }
})
export default class extends Vue {
    form = {};
    status = "add";
    created() {}
    onTest() {
        this.status = this.status === "add" ? "aaa" : "add";
    }
}
</script>

<style lang="less" scoped>
</style>
