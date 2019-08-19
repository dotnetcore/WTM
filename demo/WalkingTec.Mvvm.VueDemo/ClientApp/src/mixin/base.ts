import { Component, Vue } from "vue-property-decorator";
import { dialogType } from "@/config/enum";

@Component
export default class BaseMixins extends Vue {
    dialogType = dialogType;
    // 跳转
    onPath(path) {
        this.$router.push({
            path: "/" + path
        });
    }
    onHref(path) {
        location.href = path;
    }
}
