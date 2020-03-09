<template>
    <div id="screenfull">
        <!-- <svg-icon
      :name="isFullscreen? 'exit-fullscreen': 'fullscreen'"
      @click="onClick"
    /> -->
        <el-tooltip :content="$t('navbar.full')" effect="dark" placement="bottom">
            <i class="el-icon-full-screen" @click.stop="onClick"></i>
        </el-tooltip>
    </div>
</template>

<script lang="ts">
import screenfull from "screenfull";
import { Component, Vue } from "vue-property-decorator";

const sf = screenfull;

@Component({
    name: "Screenfull"
})
export default class extends Vue {
    private isFullscreen = false;

    mounted() {
        if (sf && sf.enabled) {
            sf.on("change", this.change);
        }
    }

    beforeDestory() {
        if (sf && sf.enabled) {
            sf.off("change", this.change);
        }
    }

    private change() {
        if (sf && sf.enabled) {
            this.isFullscreen = sf.isFullscreen;
        }
    }

    private onClick() {
        if (sf) {
            if (!sf.enabled) {
                this.$message({
                    message: "you browser can not work",
                    type: "warning"
                });
                return false;
            }
            sf.toggle();
        }
    }
}
</script>
