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
    onConfirm(title: string = "确认删除, 是否继续?") {
        return this["$confirm"](title, "提示", {
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type: "warning"
        });
    }
}
