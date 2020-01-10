import Vue from 'vue';
import Viewer from 'viewerjs';
export default {
    install(vue: typeof Vue) {
        vue.directive('viewer', {
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
        // vue.directive('display', {
        //     // 当被绑定的元素插入到 DOM 中时……
        //     bind(el, binding, vnode, oldVnode) {
        //         console.log("TCL: bind -> el", binding, vnode, oldVnode)

        //     },
        //     update(el, binding, vnode, oldVnode) {
        //         el.innerHTML='aaaa'
        //     },
        //     unbind(el: any) {
        //     }
        // });
    }
}