import Vue, { CreateElement } from 'vue';
import lodash from 'lodash';
export default {
    'avatar': Vue.component('wtm-avatar', {
        template: '<a-avatar icon="user" :src="src" v-viewer />',
        computed: {
            // 计算属性的 getter
            src() {
                const src = lodash.get(this, 'params.value');
                if (src) {
                    return `/api/_file/downloadFile/${src}`
                }
                return 'https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png'
                // return ''
            },
        },
    }),
    'switch': Vue.component('wtm-switch', {
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