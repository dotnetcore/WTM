
import Viewer from 'viewerjs';
import Vue from 'vue';
export default (vue: typeof Vue) => {
    vue.directive('w-viewer', {
        // 当被绑定的元素插入到 DOM 中时……
        bind(el) {
            el.style.position = 'relative';
            const span = document.createElement('span');
            el.appendChild(span);
            const viewer = new Viewer(el);
            el.Viewer = viewer;
            new Vue({
                template: `
                <div class='viewer-icon' v-on:click="onShow" >
                   <a-icon type="eye" />
                </div>
                `,
                el: span,
                methods: {
                    onShow() {
                        viewer.show()
                    }
                }
            })
        },
        unbind(el: any) {
            el.Viewer && el.Viewer.destroy && el.Viewer.destroy()
        }
    });
}