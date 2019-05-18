/**
 * base js
 */
import utilPath from '@/util/path.js';
import { mapGetters } from 'vuex';
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
        onPath(path, isBefore = true) {
            utilPath.onPath.bind(this)(path, isBefore);
        }
    },
    computed: {
        ...mapGetters(['isLogin'])
    }
};
export default mixin;
