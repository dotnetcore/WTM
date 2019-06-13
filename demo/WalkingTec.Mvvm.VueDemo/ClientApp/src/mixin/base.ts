/**
 * base js
 */
import { mapGetters } from "vuex";
// const vuexMixin = {
//     computed: {
//         ...mapState({
//             isLogin: 'isLogin'
//         })
//     }
// };
const mixin = {
    data() {
        return {};
    },
    mounted() {},
    methods: {
        // 跳转
        onPath(path) {
            this.$router.push({
                path: "/" + path
            });
        },
        onHref(path) {
            location.href = path;
        }
    },
    computed: {
        ...mapGetters(["isLogin"])
    }
};
export default mixin;
