import { ComponentOptions, defineComponent } from "vue"
import lodash from "lodash"
import $WtmConfig, { WtmConfig } from './config';
const options: ComponentOptions = {
    data: function () {
        return {}
    },
    methods: {
        /**
         * 跳转详情
         * 合并当前页面的 query 追加 detailsVisible 触发显示
         * @param {*} [query='0']
         */
        __wtmToDetails(query: any = '') {
            if (!lodash.isObject(query)) {
                query = { [$WtmConfig.detailsVisible]: query }
            }
            query = lodash.assign({}, this.$route.query, query)
            this.$router.replace({ query })
        },
        /**
         * 详情返回
         * 去除当前页面的 query 中 detailsVisible 触发隐藏
         */
        __wtmBackDetails() {
            const query = lodash.omit(lodash.assign({}, this.$route.query), [$WtmConfig.detailsVisible])
            this.$router.replace({ query })
        },
        /**
         * 将组件转换为 row 组件
         * @param action  row 组件
         * @param PageController 页面控制器
         * @returns 
         */
        __wtmToRowAction(action, PageController) {
            return defineComponent({
                extends: lodash.get(action, "__c", action),
                data() {
                    return { PageController }
                }
            })
        }
    },
}
declare module '@vue/runtime-core' {
    interface ComponentCustomProperties {
        /**
         * 跳转详情
         * 合并当前页面的 query 追加 detailsVisible 触发显示
         * @param {*} [query]
         */
         __wtmToDetails: (query: any) => void
        /**
         * 详情返回
         * 去除当前页面的 query 中 detailsVisible 触发隐藏
         */
         __wtmBackDetails: () => void
        /**
         * 将组件转换为 row 组件
         * @param action  row 组件
         * @param PageController 页面控制器
         * @returns 
         */
         __wtmToRowAction: (action, PageController) => any
    }
}

export default options