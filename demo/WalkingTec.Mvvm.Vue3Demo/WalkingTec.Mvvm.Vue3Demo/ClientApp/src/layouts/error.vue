<template>
    <a-result status="404" title="404" sub-title="Sorry, the page you visited does not exist.">
        <template #extra>
            <a-button type="primary">Back Home</a-button>
        </template>
    </a-result>
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
