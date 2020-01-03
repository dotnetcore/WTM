import Vue, { CreateElement } from 'vue';
import lodash from 'lodash';
export default {
    'avatar': Vue.component('wtm-avatar', {
        template: '<a-avatar shape="square" :src="src" />',
        computed: {
            // 计算属性的 getter
            src() {
                return `/api/_file/downloadFile/${lodash.get(this, 'params.value')}`
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
                return lodash.includes(['true', true],lodash.get(this,'params.value'))
            },
        },
    }),
}