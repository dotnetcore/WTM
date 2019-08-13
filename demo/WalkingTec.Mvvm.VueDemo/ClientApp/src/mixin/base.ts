import { Component, Vue } from "vue-property-decorator";

@Component
export default class BaseMixins extends Vue {
    formData = {
        testname: "aaa"
    };
    test() {
        console.log("fldls");
    }
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
