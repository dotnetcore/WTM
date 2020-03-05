import Vue from 'vue';
import Dnd from './dnd';
import Viewer from './viewer';
import IframeResizer from './iframeResizer';
export default {
    install(vue: typeof Vue) {
        Dnd(vue);
        Viewer(vue);
        IframeResizer(vue);
        // vue.directive('display', {
        //     // 当被绑定的元素插入到 DOM 中时……
        //     bind(el, binding, vnode, oldVnode) {
        //         vnode.componentOptions.propsData.action='www.baidu.com'
        //         vnode.componentInstance.action='www.baidu.com'
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