import Vue, { CreateElement } from 'vue';
import lodash from 'lodash';
export default {
    'avatar': Vue.component('wtm-grid-avatar', {
        template: '<a-avatar icon="user" :src="src" v-w-viewer />',
        computed: {
            fileService(){
                return this.$GlobalConfig.onCreateFileService();
            },
            // 计算属性的 getter
            src() {
                const src = lodash.get(this, 'params.value');
                if (src) {
                    return `${this.fileService.fileGet.src}/${src}`
                }
                return 'https://zos.alipayobjects.com/rmsportal/ODTLcjxAfvqbxHnVXCYX.png'
                // return ''
            },
        },
    }),
    'switch': Vue.component('wtm-grid-switch', {
        template: `
        <a-switch :checked="checked" disabled>
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