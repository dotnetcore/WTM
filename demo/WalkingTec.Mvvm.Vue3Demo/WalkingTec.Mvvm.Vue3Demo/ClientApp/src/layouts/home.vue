<template>
    <div>我是首页</div>
</template>
<script lang="ts">
import { Inject, Options, Vue } from "vue-property-decorator";
@Options({ components: {} })
export default class extends Vue {
    created() {
        console.log("LENG ~ extends ~ created ~ this", this)
    }
}
</script>
<style lang="less" scoped>
</style>
