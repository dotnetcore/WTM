import iFrameResize from 'iframe-resizer/js/iframeResizer'
import lodash from 'lodash';
import Vue from 'vue';
export default (vue: typeof Vue) => {
    vue.directive('resize', {
        bind: function (el, { value = {} }) {
            //   el.addEventListener('load', () => iFrameResize(value, el))
            el.iFrameResize = iFrameResize(value, el)
        },
        unbind(el: any) {
            el.iFrameResize && el.iFrameResize.close && el.iFrameResize.close()
        }
    })
}