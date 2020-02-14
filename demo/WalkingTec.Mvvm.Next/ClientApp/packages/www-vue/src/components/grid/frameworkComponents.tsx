import globalConfig from "../../global.config";
import Vue, { CreateElement } from 'vue';
import lodash from 'lodash';
const fileService = globalConfig.onCreateFileService();
export default {
    'avatar': Vue.component('wtm-grid-avatar', {
        template: '<a-avatar icon="user" :src="src" v-viewer />',
        computed: {
            // 计算属性的 getter
            src() {
                const src = lodash.get(this, 'params.value');
                if (src) {
                    return `${fileService.fileGet.src}/${src}`
                }
                return 'https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png'
                // return ''
            },
        },
    }),
    'switch': Vue.component('wtm-grid-switch', {
        template: `
        <a-switch :checked="checked">
            <a-icon type="check" slot="checkedChildren" />
            <a-icon type="close" slot="unCheckedChildren" />
        </a-switch>
        `,
        computed: {
            // 计算属性的 getter
            checked() {
                return lodash.includes(['true', true], lodash.get(this, 'params.value'))
            },
        },
    }),
}