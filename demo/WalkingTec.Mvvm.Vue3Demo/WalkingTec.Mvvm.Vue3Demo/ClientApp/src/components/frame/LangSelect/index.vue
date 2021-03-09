<template>
    <el-dropdown trigger="click" class="international" @command="handleSetLanguage">
        <el-tooltip :content="$t('navbar.language')" effect="dark" placement="bottom">
            <wtm-icon icon="icon-language" icon-font-class="iconfont" />
        </el-tooltip>
        <el-dropdown-menu slot="dropdown">
            <el-dropdown-item :disabled="language==='zh'" command="zh">
                中文
            </el-dropdown-item>
            <el-dropdown-item :disabled="language==='en'" command="en">
                English
            </el-dropdown-item>
            <!-- <el-dropdown-item :disabled="language==='es'" command="es">
                Español
            </el-dropdown-item>
            <el-dropdown-item :disabled="language==='ja'" command="ja">
                日本語
            </el-dropdown-item> -->
        </el-dropdown-menu>
    </el-dropdown>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import { AppModule } from "@/store/modules/app";

@Component({
    name: "Login"
})
export default class extends Vue {
    get language() {
        return AppModule.language;
    }

    private handleSetLanguage(lang: string) {
        this.$i18n.locale = lang;
        AppModule.SetLanguage(lang);
        this.$message({
            message: "Switch Language Success",
            type: "success"
        });
        location.reload();
    }
}
</script>
<style lang="less" scoped>
i {
    font-size: 16px;
}
</style>
