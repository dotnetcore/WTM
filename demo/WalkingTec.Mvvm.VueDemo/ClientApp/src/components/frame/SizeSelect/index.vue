<template>
    <el-dropdown id="size-select" trigger="click" @command="handleSetSize">
        <!-- <svg-icon class="size-icon" name="size" /> -->
        <el-tooltip :content="$t('navbar.size')" effect="dark" placement="bottom">
            <i class="el-icon-s-operation"></i>
        </el-tooltip>
        <el-dropdown-menu slot="dropdown">
            <el-dropdown-item v-for="item of sizeOptions" :key="item.value" :disabled="size===item.value" :command="item.value">
                {{item.label }}
            </el-dropdown-item>
        </el-dropdown-menu>
    </el-dropdown>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import { AppModule } from "@/store/modules/app";
import { TagsViewModule } from "@/store/modules/tags-view";

@Component({
    name: "SizeSelect"
})
export default class extends Vue {
    private sizeOptions = [
        { label: "Default", value: "default" },
        { label: "Medium", value: "medium" },
        { label: "Small", value: "small" },
        { label: "Mini", value: "mini" }
    ];

    get size() {
        return AppModule.size;
    }

    private handleSetSize(size: string) {
        (this as any).$ELEMENT.size = size;
        AppModule.SetSize(size);
        this.refreshView();
        this.$message({
            message: "Switch Size Success",
            type: "success"
        });
    }

    private refreshView() {
        // In order to make the cached page re-rendered
        TagsViewModule.delAllCachedViews();
        this.$nextTick(() => {
            this.$router.replace({ path: "/" });
        });
    }
}
</script>
<style lang="less" scoped>
i {
    font-size: 16px;
}
</style>
